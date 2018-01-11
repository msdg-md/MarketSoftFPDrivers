namespace Phoenix.Devices.Printers.DatecsMD
{
    using Phoenix.Devices;
    using Phoenix.Devices.Printers;
    using Phoenix.Globals;
    using Phoenix.Globals.Units;
    using Phoenix.PrintService.Templates;
    using Phoenix.PrintService.Templates.Printer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Windows;


    internal class CultureStrings
    {
        // Fields
        private static CultureInfo resourceCulture;
        private static ResourceManager resourceMan;

        // Methods
        internal CultureStrings()
        {
        }
        // Properties
        internal static CultureInfo Culture { get; set; }
        internal static string PrintBottomCommentsOnTop { get; }
        internal static string PrintZReportFull { get; }
        internal static ResourceManager ResourceManager { get; }
    }


    public class Datecs : IFiscalPrinter, IDevice
    {
        protected bool addPaymentSums = true;
        protected uint baudrate = 0x4b00;
        private string cashierName = "";
        protected bool checkOpened;
        protected PrinterReceiptType checkType;
        protected int closeReceiptStep;
        protected DatecsSums discountSums = new DatecsSums();
        protected PrinterFeatures features = new PrinterFeatures();
        protected bool printArtCode;
        protected bool printDiscountEachPosition;
        private DatecsProtocol printer = new DatecsProtocol();
        protected bool printOneCount;
        protected DatecsSums receiptSums = new DatecsSums();
        protected string recNumberForNoFiscalReceipt;
        protected string recNumberForNoFiscalRefundReceipt;


        protected bool printBottomCommentsOnTop;
        protected bool printZReportFull;
        private int sleepAfterPositionPrint;

        protected virtual void AddSums(TaxGrp taxGrp, Money cost, Money discount, Money recDiscount)
        {
            this.discountSums.AddSum(taxGrp, discount + recDiscount);
            this.receiptSums.AddSum(taxGrp, cost);
        }

        public virtual void BeginReceipt(PrinterReceiptType type, int cashNum, int systemRecNum, string comment, string recNum)
        {
            //if (!this.IsOpened)
            //{
            //    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            //}
            //if (this.checkOpened)
            //{
            //    throw new FiscalPrinterException(DatecsStrings.GetString(0x1d));
            //}
            Log.Write($"Открытие чека: комментарий: {comment} тип: {this.checkType} номер кассы: {cashNum}, сист.номер чека: {systemRecNum} фиск.номер: {recNum}", Log.MessageType.Info, this);
            this.closeReceiptStep = 0;
            string text = DatecsStrings.GetString(30) + cashNum.ToString();
            switch (type)
            {
                case PrinterReceiptType.Sales:
                    this.Printer.OpenFiscalReceipt(cashNum);
                    this.Printer.PrintFiscalText(DatecsStrings.GetString(0x1f) + this.NextRecNum);
                    this.Printer.PrintFiscalText(text);
                    this.Printer.PrintFiscalText(comment);
                    break;

                case PrinterReceiptType.Refund:
                    this.OpenRefundReceipt(cashNum);
                    this.PrintRefundText(DatecsStrings.GetString(0x1f) + this.NextRefRecNum);
                    this.PrintRefundText(text);
                    this.PrintRefundText(DatecsStrings.GetString(0x20) + recNum);
                    this.PrintRefundText(comment);
                    break;

                case PrinterReceiptType.Copy:
                    this.Printer.OpenNonfiscalReceipt();
                    this.Printer.PrintNonfiscalText(this.CashierString);
                    this.Printer.PrintNonfiscalText(DatecsStrings.GetString(0x21) + recNum);
                    this.Printer.PrintNonfiscalText(text);
                    this.Printer.PrintNonfiscalText(comment);
                    break;

                case PrinterReceiptType.Text:
                    this.Printer.OpenNonfiscalReceipt();
                    break;

                case PrinterReceiptType.NoFiscalSales:
                    this.Printer.OpenNonfiscalReceipt();
                    this.Printer.PrintNonfiscalText(this.CashierString);
                    this.Printer.PrintNonfiscalText(this.recNumberForNoFiscalReceipt + " " + systemRecNum);
                    this.Printer.PrintNonfiscalText(text);
                    this.Printer.PrintNonfiscalText(comment);
                    break;

                case PrinterReceiptType.NoFiscalRefund:
                    this.Printer.OpenNonfiscalReceipt();
                    this.Printer.PrintNonfiscalText(this.CashierString);
                    this.Printer.PrintNonfiscalText(this.recNumberForNoFiscalRefundReceipt + " " + systemRecNum);
                    this.Printer.PrintNonfiscalText(text);
                    this.Printer.PrintNonfiscalText(DatecsStrings.GetString(0x20) + recNum);
                    this.Printer.PrintNonfiscalText(comment);
                    break;
            }
            this.discountSums.Clear();
            this.receiptSums.Clear();
            this.checkOpened = true;
            this.checkType = type;
        }

        public void BeginTextDoc()
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.OpenNonfiscalReceipt();
            this.checkOpened = true;
            this.checkType = PrinterReceiptType.Text;
        }

        public virtual void CancelReceipt()
        {
            return;
            this.checkOpened = false;
            this.closeReceiptStep = 0;


            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            Status sts = this.Printer.GetStatus();
            if (sts.NonFiscalCheckOpen)
            {
                this.Printer.CloseNonfiscalReceipt();
            }
            this.Printer.CancelFiscalReceipt();
        }

        public void CashIn(Money sum)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.CashIn(sum);
        }

        public void CashOut(Money sum)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.CashOut(sum);
        }

        public void ClearDisplay()
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.ClearDisplay();
        }

        public void Close()
        {
            this.Printer.Disconnect();
        }

        private void CloseCopyReceipt(PrinterPayment payment)
        {
            Money money;
            Percent rate;
            this.Printer.PrintNonfiscalText(new string('-', 0x22));
            Money money2 = this.receiptSums.Total - this.discountSums.Total;
            this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x27), money2.ToString("F") + "  ", 0x22));
            Hashtable currentTaxRates = this.Printer.GetCurrentTaxRates();
            if (this.receiptSums.TaxA != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxA))
                {
                    PrinterTaxGrp grp = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxA];
                    rate = grp.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxA - this.discountSums.TaxA;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxA)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            if (this.receiptSums.TaxB != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxB))
                {
                    PrinterTaxGrp grp2 = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxB];
                    rate = grp2.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxB - this.discountSums.TaxB;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxB)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            if (this.receiptSums.TaxC != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxC))
                {
                    PrinterTaxGrp grp3 = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxC];
                    rate = grp3.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxC - this.discountSums.TaxC;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxC)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            if (this.receiptSums.TaxD != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxD))
                {
                    PrinterTaxGrp grp4 = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxD];
                    rate = grp4.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxD - this.discountSums.TaxD;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxD)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            if (this.receiptSums.TaxFree != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxFree))
                {
                    PrinterTaxGrp grp5 = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxFree];
                    rate = grp5.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxFree - this.discountSums.TaxFree;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxFree)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            if (this.receiptSums.TaxM != new Money(0))
            {
                if (currentTaxRates.Contains(TaxGrp.TaxE))
                {
                    PrinterTaxGrp grp6 = (PrinterTaxGrp) currentTaxRates[TaxGrp.TaxE];
                    rate = grp6.Rate;
                }
                else
                {
                    rate = new Percent(0);
                }
                money = this.receiptSums.TaxM - this.discountSums.TaxM;
                this.Printer.PrintNonfiscalText(this.ConcatStrings($"{DatecsStrings.GetString(0x2a)} {this.Printer.GetDatecsTaxName(TaxGrp.TaxE)}={rate.ToString()}%", money.GetInTax(rate).ToString("F") + "  ", 0x22));
            }
            this.Printer.PrintNonfiscalText(new string('-', 0x22));
            this.Printer.CloseNonfiscalReceipt();
        }

        protected virtual void CloseRefundReceipt(PrinterPayment payment)
        {
            if (this.discountSums.TaxA != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxA.ToString("F")} {'А'}", 0x22));
            }
            if (this.discountSums.TaxB != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxB.ToString("F")} {'Б'}", 0x22));
            }
            if (this.discountSums.TaxC != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxC.ToString("F")} {'В'}", 0x22));
            }
            if (this.discountSums.TaxD != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxD.ToString("F")} {'Г'}", 0x22));
            }
            if (this.discountSums.TaxFree != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxFree.ToString("F")} {'*'}", 0x22));
            }
            if (this.discountSums.TaxM != new Money(0))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x26), $"-{this.discountSums.TaxM.ToString("F")} {'М'}", 0x22));
            }
            this.Printer.PrintNonfiscalText("");
            Money money = this.receiptSums.Total - this.discountSums.Total;
            this.Printer.PrintNonfiscalText(this.ConcatStrings(DatecsStrings.GetString(0x27), money.ToString("F") + "  ", 0x22));
            this.Printer.PrintNonfiscalText("");
            this.Printer.CloseNonfiscalReceipt();
            if (this.receiptSums.TaxA != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxA - this.discountSums.TaxA, TaxGrp.TaxA, DatecsStrings.GetString(40));
            }
            if (this.receiptSums.TaxB != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxB - this.discountSums.TaxB, TaxGrp.TaxB, DatecsStrings.GetString(40));
            }
            if (this.receiptSums.TaxC != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxC - this.discountSums.TaxC, TaxGrp.TaxC, DatecsStrings.GetString(40));
            }
            if (this.receiptSums.TaxD != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxD - this.discountSums.TaxD, TaxGrp.TaxD, DatecsStrings.GetString(40));
            }
            if (this.receiptSums.TaxFree != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxFree - this.discountSums.TaxFree, TaxGrp.TaxFree, DatecsStrings.GetString(40));
            }
            if (this.receiptSums.TaxM != new Money(0))
            {
                this.Printer.VoidSell(this.receiptSums.TaxM - this.discountSums.TaxM, TaxGrp.TaxE, DatecsStrings.GetString(40));
            }
        }

        protected virtual void CloseSalesReceipt(PrinterPayment payment)
        {
            string comment = "";
            try
            {
                if (this.closeReceiptStep == 0)
                {
                 //   this.SetReceiptDiscount();
                    this.closeReceiptStep++;
                }
                if (this.closeReceiptStep == 1)
                {
                    if (payment.GetSum(PaymentType.Credit) != new Money(0))
                    {
                        this.Printer.Total(payment.GetSum(PaymentType.Credit), PaymentType.Credit, comment);
                    }
                    this.closeReceiptStep++;
                }
                if (this.closeReceiptStep == 2)
                {
                    if (payment.GetSum(PaymentType.Check) != new Money(0))
                    {
                        this.Printer.Total(payment.GetSum(PaymentType.Check), PaymentType.Check, comment);
                    }
                    this.closeReceiptStep++;
                }
                if (this.closeReceiptStep == 3)
                {
                    if (payment.GetSum(PaymentType.Card) != new Money(0))
                    {
                        this.Printer.Total(payment.GetSum(PaymentType.Card), PaymentType.Card, comment);
                    }
                    this.closeReceiptStep++;
                }
                if (this.closeReceiptStep == 4)
                {
                    if (payment.GetSum(PaymentType.Cash) != new Money(0))
                    {
                        this.Printer.Total(payment.GetSum(PaymentType.Cash), PaymentType.Cash, comment);
                    }
                    this.closeReceiptStep++;
                }
                if (this.closeReceiptStep == 5)
                {
                    this.Printer.CloseFiscalReceipt();
                    this.closeReceiptStep++;
                }
                this.closeReceiptStep = 0;
            }
            catch (LostConnectException exception)
            {
                Log.Write(exception, this);
                throw;
            }
            catch (DeviceException exception2)
            {
                Log.Write(exception2, this);
                bool flag = false;
                try
                {
                    flag = this.Printer.status.PaperOut || this.Printer.ReceiptPaymentAndNoClose;
                }
                catch (DeviceException exception3)
                {
                    Log.Write(exception2, this);
                    throw new LostConnectException(exception3.Message, exception3);
                }
                if (flag)
                {
                    throw new CriticalPaymentException(exception2.Message, exception2);
                }
                throw;
            }
        }

        protected string ConcatStrings(string str1, string str2, int maxLen)
        {
            int length = (maxLen - str2.Length) - 1;
            if (str1.Length > length)
            {
                str1 = str1.Substring(0, length);
            }
            return (str1 + new string(' ', (length - str1.Length) + 1) + str2);
        }

        public void EndReceipt()
        {
            this.EndReceipt(true);
        }

        public virtual void EndReceipt(bool printToControlType)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            Log.Write("Закрытие чека", Log.MessageType.Info, this);
            this.Printer.CloseNonfiscalReceipt();
            this.checkOpened = false;
        }

        public virtual void EndReceipt(Money receiptSum, PrinterPayment payment)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            Log.Write("Закрытие чека", Log.MessageType.Info, this);
            switch (this.checkType)
            {
                case PrinterReceiptType.Sales:
                    this.CloseSalesReceipt(payment);
                    break;

                case PrinterReceiptType.Refund:
                    this.CloseRefundReceipt(payment);
                    break;

                case PrinterReceiptType.Copy:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                    this.CloseCopyReceipt(payment);
                    break;

                case PrinterReceiptType.Text:
                    this.Printer.CloseNonfiscalReceipt();
                    break;
            }
            this.checkOpened = false;
        }

        public void Feed()
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.AdvancePaper();
        }

        public Parameter[] GetDefaultParameters(PortType portType) => 
            this.GetParameters(portType);

        public virtual string GetModemError() => 
            null;

        public Parameter[] GetParameters(PortType portType) => 
            this.Parameters;

        public void Init(string[] header, string[] footer, string[] firstHeader, string[] endFooter, string recNumberForNoFiscalReceipt, string recNumberForNoFiscalRefundReceipt, PrinterOptions options)
        {
            if (!this.Printer.IsOpen)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.SetHeaderFooter(header, footer);
            this.printOneCount = (options & PrinterOptions.PrintOneCount) != PrinterOptions.Empty;
            this.printArtCode = (options & PrinterOptions.PrintArtCode) != PrinterOptions.Empty;
            this.printDiscountEachPosition = (options & PrinterOptions.PrintDiscountEachPosition) != PrinterOptions.Empty;
            this.recNumberForNoFiscalReceipt = recNumberForNoFiscalReceipt;
            this.recNumberForNoFiscalRefundReceipt = recNumberForNoFiscalRefundReceipt;
        }

        protected virtual void InitFeatures()
        {
            this.features.OnlineMode = false;
            this.features.MaxRecItemCount = 250;
            this.features.TextDocLineLength = 0x22;
            this.features.PrintNonFiscalDoc = true;
            this.features.RefundCashless = true;
            this.features.PrintCommentsInCheck = true;
            this.features.PrintCheckCopy = true;
            this.features.CashBoxControl = true;
            this.features.ArtRegistration = false;
            this.features.SeparateRecCounters = false;
            this.features.PaymentSumControl = false;
            this.features.SetTaxes = true;
            this.features.DisplayLen = 20;
        }

        public bool IsPortSupported(PortType portType)
        {
            foreach (PortType type in this.PortTypes)
            {
                if (type == portType)
                {
                    return true;
                }
            }
            return false;
        }

        private System.Windows.Size MeasureChar(FontSizeType fontSizeType) => 
            new System.Windows.Size(15.0, 15.0);

        public virtual void Open(Port port)
        {
            this.Printer.Connect(port, this.baudrate);
            this.Printer.GetStatus();
            this.InitFeatures();
        }

        public void OpenDrawer()
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.OpenDrawer();
        }

        protected virtual void OpenRefundReceipt(int cashNum)
        {
            this.Printer.OpenNonfiscalReceipt();
        }

        void IFiscalPrinter.Open(Port port, string deviceVersion)
        {
            this.Open(port);
        }

        public virtual void PrintBarcode(string barcode, BarcodeType barcodeType = 0)
        {
        }

        public virtual void PrintBottomComment(string[] comments)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            foreach (string str in this.TrimComment(comments, 0x22))
            {
                switch (this.checkType)
                {
                    case PrinterReceiptType.Sales:
                        this.Printer.PrintFiscalText(str);
                        break;

                    case PrinterReceiptType.Refund:
                    case PrinterReceiptType.Copy:
                    case PrinterReceiptType.Text:
                    case PrinterReceiptType.NoFiscalSales:
                        this.Printer.PrintNonfiscalText(str);
                        break;
                    case PrinterReceiptType.NoFiscalRefund:
                        this.Printer.PrintNonfiscalText(str);
                        break;
                }
            }
        }

        public void PrintClientInfo(string clientName)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            switch (this.checkType)
            {
                case PrinterReceiptType.Sales:
                    this.Printer.PrintFiscalText(DatecsStrings.GetString(0x25) + clientName);
                    return;

                case PrinterReceiptType.Refund:
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.Text:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                    this.Printer.PrintNonfiscalText(DatecsStrings.GetString(0x25) + clientName);
                    return;
            }
        }

        public void PrintExtReport(int reportId, object parameters)
        {
        }

        protected virtual void PrintNonfiscalPosition(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName, Money discount, Money recDiscount, TaxGrp taxGrp, int artNum, bool cancelled)
        {
            if ((this.printOneCount || (count.Type != Quantity.QuantityTypes.NoWeight)) || (count.Amount != 1))
            {
                this.Printer.PrintNonfiscalText(this.ConcatStrings(count.ToString("F") + " X", unitPrice.ToString("F") + "  ", 0x22));
            }
            if (artName.Length > 0x19)
            {
                artName = artName.Substring(0, 0x19);
            }
            this.Printer.PrintNonfiscalText(this.ConcatStrings(artName, $"{cost.ToString("F")} {this.Printer.GetDatecsTaxName(taxGrp)}", 0x22));
        }

        public virtual void PrintNullRec()
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.OpenFiscalReceipt(1);
            this.Printer.PrintFiscalText(DatecsStrings.GetString(0x1c));
            this.Printer.Total(new Money(0), PaymentType.Cash, "");
            this.Printer.CloseFiscalReceipt();
        }

        public virtual void PrintRecCopy()
        {
            this.Printer.MakeReceiptCopy();
   //         this.Printer.Cut();
        }

        public void PrintRecDiscount(Money sum, string discountName)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
        }

        public virtual void PrintReceiptItem(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName, Money discount, Money recDiscount, TaxGrp taxGrp, int artNum, bool cancelled, string artKey = null)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            Log.Write($"Печать позиции чека: код товара: {artCode} имя: {artName} цена: {unitPrice} количество: {count} ед.измерения: {unitName} стоимость: {cost} спец.предложение: {offerName} скидка:	{discount} скидка на чек: {recDiscount} налог: {taxGrp} номер артикула: {artNum} отменено: {cancelled}", Log.MessageType.Info, this);
            if (!cancelled)
            {
                if (this.printArtCode)
                {
                    artName = $"{artCode}-{artName}";
                }
                if (this.printDiscountEachPosition)
                {
                    discount += recDiscount;
                    recDiscount = new Money(0);
                }
                switch (this.checkType)
                {
                    case PrinterReceiptType.Sales:
                        this.Printer.RegisterItem(artName, unitPrice, count, taxGrp, this.printOneCount, artNum, discount);
                        break;

                    case PrinterReceiptType.Refund:
                        this.RegisterRefundItem(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
                        break;

                    case PrinterReceiptType.Copy:
                    case PrinterReceiptType.Text:
                    case PrinterReceiptType.NoFiscalSales:
                    case PrinterReceiptType.NoFiscalRefund:
                        this.PrintNonfiscalPosition(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
                        break;
                }
                this.AddSums(taxGrp, cost, discount, recDiscount);
            }
        }

        protected virtual void PrintRefundText(string text)
        {
            this.Printer.PrintNonfiscalText(text);
        }

        public virtual void PrintReport(ReportType reportType)
        {
            if (!this.Printer.IsOpen)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            switch (reportType)
            {
                case ReportType.XReport:
                    this.Printer.PrintXReport();
                    return;

                case ReportType.ZReport:
                    this.Printer.PrintZReport();
                    return;

                case ReportType.ArtReport:
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x1b));
            }
        }

        public virtual void PrintReport(ReportType reportType, DateTime beginDate, DateTime endDate)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (reportType == ReportType.ZReport)
            {
                this.Printer.PrintZReport(beginDate, endDate);
            }
        }

        public virtual void PrintReport(ReportType reportType, int beginNum, int endNum)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (reportType == ReportType.ZReport)
            {
                this.Printer.PrintZReport(beginNum, endNum);
            }
        }

        public void PrintTemplate(IPrintTemplateFactory templateFactory)
        {
            try
            {
                ITextTemplatePrintInfo info = templateFactory.CreateTextPrinter();
                info.PageSizeInChars = new System.Windows.Size((double) this.Features.TextDocLineLength, 0.0);
                info.MeasureCharHandler = new MeasureCharHandler(this.MeasureChar);
                this.BeginTextDoc();
                info.CreatePages(true, 96.0, 96.0);
                foreach (ITemplatePrintInfoPage page in info.Pages)
                {
                    foreach (ITemplatePrintInfoPageItem item in page.PageItems)
                    {
                        if (item.PageItemType == PageItemType.String)
                        {
                            IStringTemplatePrintInfoPageItem item2 = (IStringTemplatePrintInfoPageItem) item;
                            this.PrintTextLine(item2.Text);
                        }
                    }
                }
                this.EndReceipt();
            }
            catch (TemplateException exception)
            {
                Log.Write(exception, this);
                throw new DeviceException(exception.Message, exception);
            }
        }

        public void PrintTextLine(string line)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (!this.checkOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x22));
            }
            switch (this.checkType)
            {
                case PrinterReceiptType.Sales:
                    this.Printer.PrintFiscalText(line);
                    return;

                case PrinterReceiptType.Refund:
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.Text:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                    this.Printer.PrintNonfiscalText(line);
                    return;
            }
        }

        protected virtual void RegisterRefundItem(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName, Money discount, Money recDiscount, TaxGrp taxGrp, int artNum, bool cancelled)
        {
            this.PrintNonfiscalPosition(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
        }

        public void Reset(bool reconnect)
        {
             Status sts = this.printer.GetStatus();
            if (sts.NonFiscalCheckOpen)
            {
                this.printer.CloseNonfiscalReceipt();
            }
                else
                if (sts.FiscalCheckOpen)
            {
                this.printer.CancelFiscalReceipt();
            }
            

        }

        public virtual void SetLogo(Bitmap bmp)
        {
        }

        public void SetPaymentTypes(Dictionary<PaymentType, string> paymentTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void SetReceiptDiscount()
        {
            this.Printer.RefundSums(this.discountSums, DatecsStrings.GetString(0x26));
        }

        public void ShowDisplayLine(string line, DisplayLineType displayLineType)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            if (line.Length > 20)
            {
                line = line.Substring(0, 20);
            }
            switch (displayLineType)
            {
                case DisplayLineType.Top:
                    this.Printer.DisplayTextUL(line);
                    return;

                case DisplayLineType.Bootom:
                    this.Printer.DisplayTextLL(line);
                    return;
            }
        }

        public void ShowItemInfo(string itemName, Quantity itemCount, Money itemCost, Money recSum)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.DisplayTextUL(this.ConcatStrings(itemName.ToString(), itemCount.ToString(), 20));
            this.Printer.DisplayTextLL(this.ConcatStrings(itemCost.ToString("F"), recSum.ToString("F"), 20));
        }

        public void ShowPaymentInfo(Money recSum, string recSumHint, Money changeSum, string changeSumHint)
        {
            if (!this.IsOpened)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
            }
            this.Printer.DisplayTextUL(this.ConcatStrings(recSumHint, recSum.ToString("F"), 20));
            this.Printer.DisplayTextLL(this.ConcatStrings(changeSumHint, changeSum.ToString("F"), 20));
        }

        protected string[] TrimComment(string[] comments, int commentLength)
        {
            List<string> list = new List<string>();
            foreach (string str in comments)
            {
                int num = (str.Length / commentLength) + (((str.Length % commentLength) > 0) ? 1 : 0);
                for (int i = 0; i < num; i++)
                {
                    int startIndex = commentLength * i;
                    int length = commentLength;
                    if ((startIndex + length) > str.Length)
                    {
                        length = str.Length - startIndex;
                    }
                    list.Add(str.Substring(startIndex, length));
                }
            }
            return list.ToArray();
        }

        public string CashierName
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.cashierName;
            }
            set
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                this.cashierName = value;
                this.Printer.SetOperatorName(value);
            }
        }

        protected string CashierString =>
            $"{DatecsStrings.GetString(0x2b)}  {this.cashierName}";

        public Money CashSum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetDrawerSum();
            }
        }

        public Port DefaultPort =>
            new Port(PortType.COM, 1);

        public bool DemoMode =>
            false;

        public virtual string DeviceFriendlyName =>
            DatecsStrings.GetString(0x2c);

        public virtual string DeviceName =>
            "A38EDF44-B9FA-4b37-917D-379439674010";

        public PrinterFeatures Features =>
            this.features;

        public bool FiscalMode
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetStatus().FiscalRamFiscalized;
            }
        }

        public bool IsOpened =>
            this.Printer.IsOpen;

        public virtual string LastRecNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetLastCheckNumber().ToString();
            }
        }

        public virtual string LastRefRecNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetLastCheckNumber().ToString();
            }
        }

        public virtual string LastZRepNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetLastFiscalClosure().ToString();
            }
        }

        public virtual string NextRecNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                int num = this.Printer.GetLastCheckNumber() + 1;
                return num.ToString();
            }
        }

        public virtual string NextRefRecNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                int num = this.Printer.GetLastCheckNumber() + 1;
                return num.ToString();
            }
        }

        public virtual string NextZRepNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                int num = this.Printer.GetLastFiscalClosure() + 1;
                return num.ToString();
            }
        }


        public virtual Parameter[] Parameters
        {
            get
            {
                string str = DatecsStrings.GetString(50);
                string str2 = DatecsStrings.GetString(0x33);
                List<Parameter> list = new List<Parameter> {
                    new Parameter(3, (int) baudrate, new int[] {
                        0x4b0,
                        0x960,
                        0x12c0,
                        0x2580,
                        0x3840,
                        0x4b00,
                        0x9600,
                        0xe100,
                        0x1c200
                    }, DatecsStrings.GetString(0x36)),
                    new Parameter(1, this.sleepAfterPositionPrint, 0, 500, DatecsStrings.GetString(0x2f)),
                    new Parameter(2, addPaymentSums ? str : str2, new string[] {
                        str,
                        str2
                    }, DatecsStrings.GetString(0x31)),
                    new Parameter(6, this.printBottomCommentsOnTop ? str : str2, new string[] {
                        str,
                        str2
                    },  CultureStrings.PrintBottomCommentsOnTop),
                    new Parameter(7, this.printZReportFull ? str : str2, new string[] {
                        str,
                        str2
                    }, CultureStrings.PrintZReportFull)
                };
                return list.ToArray();
            }
            set
            {
                foreach (Parameter parameter in value)
                {
                    switch (parameter.Key)
                    {
                        case 1:
                            this.sleepAfterPositionPrint = parameter.IntValue;
                            break;

                        case 2:
                            addPaymentSums = parameter.StringValue == DatecsStrings.GetString(50);
                            break;

                        case 3:
                            baudrate = (uint)parameter.IntValue;
                            break;

                        case 6:
                            this.printBottomCommentsOnTop = parameter.StringValue == DatecsStrings.GetString(50);
                            break;

                        case 7:
                            this.printZReportFull = parameter.StringValue == DatecsStrings.GetString(50);
                            break;
                    }
                }
            }
        }


        public PortType[] PortTypes =>
            new PortType[1];

        protected virtual DatecsProtocol Printer =>
            this.printer;

        public DateTime PrinterTime
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetDateTime();
            }
            set
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                this.Printer.SetDateTime(value);
            }
        }

        public Money RefundSum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetRefundSum();
            }
        }

        public string RefundSumByTax
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                if (!this.addPaymentSums)
                {
                    return this.Printer.GetRefundSumByTax();
                }
                return (this.Printer.GetRefundSumByTax() + ";" + this.Printer.GetRefundSumByPayment());
            }
        }

        public Money SalesSum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetSalesSum();
            }
        }

        public string SalesSumByTax
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                if (!this.addPaymentSums)
                {
                    return this.Printer.GetSalesSumByTax();
                }
                //return (this.Printer.GetSalesSumByTax() + ";" + this.Printer.GetSalesSumByPayment());
                return this.Printer.GetSalesSumByPayment();
            }
        }

        public string SerialNum
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetSerialNumber();
            }
        }

        public bool SessionOpened
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.SessionOpened;
            }
        }

        public Hashtable Taxes
        {
            get
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                return this.Printer.GetCurrentTaxRates();
            }
            set
            {
                if (!this.IsOpened)
                {
                    throw new FiscalPrinterException(DatecsStrings.GetString(0x11));
                }
                this.Printer.SetCurrentTaxRates(value);
            }
        }

        protected enum SettingsParameter
        {
            AddPaymentSums = 2,
            Baudrate = 3,
            CompositeTax = 5,
            PrintBottomCommentsOnTop = 6,
            PrintWidth = 4,
            PrintZReportFull = 7,
            SleepAfterPositionPrint = 1
        }
    }
}

