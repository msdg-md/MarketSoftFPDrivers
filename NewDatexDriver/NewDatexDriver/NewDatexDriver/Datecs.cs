using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Globals;
using Phoenix.Globals.Units;
using Phoenix.Devices.Printers;
using Phoenix.PrintService.Templates.Printer;
using System.Drawing;
using WindowsBase = System.Windows;
using Phoenix.PrintService.Templates;

namespace Phoenix.Devices.Printers.Datecs
{
    public class Datecs : IFiscalPrinter
    {

        private DatecsProtocol printer = new DatecsProtocol();
        private string cashierName = "";
        protected bool checkOpened = false;
        protected bool printOneCount = false;
        protected bool printArtCode = false;
        protected bool printDiscountEachPosition = false;
        protected int closeReceiptStep = 0;
        protected PrinterReceiptType checkType = PrinterReceiptType.Sales;
        protected PrinterFeatures features = new PrinterFeatures();
        protected DatecsSums receiptSums = new DatecsSums();
        protected DatecsSums discountSums = new DatecsSums();
        protected string recNumberForNoFiscalReceipt;
        protected string recNumberForNoFiscalRefundReceipt;
        protected bool addPaymentSums = true;

        protected virtual DatecsProtocol Printer
        {
            get
            {
                return printer;
            }
        }

        public string NextRecNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return (Printer.GetLastCheckNumber() + 1).ToString();
            }
        }

        public virtual string NextRefRecNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return (Printer.GetLastCheckNumber() + 1).ToString();
            }
        }

        public string NextZRepNum
        {
            get
            {
                Log.Write("(Printer.GetLastFiscalClosure(): " + "1234567", Log.MessageType.Message, this);
                //if(!IsOpened)
                //	throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                //return (Printer.GetLastFiscalClosure() + 1).ToString();

                return " 1234567";
            }
        }

        public virtual string LastRefRecNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetLastCheckNumber().ToString();
            }
        }

        public virtual string LastRecNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetLastCheckNumber().ToString();
            }
        }
        public virtual string LastZRepNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetLastFiscalClosure().ToString();
            }
        }
        public bool SessionOpened
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.SessionOpened;
            }
        }
        public bool FiscalMode
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetStatus().FiscalRamFiscalized;
            }
        }
        public bool DemoMode
        {
            get
            {
                return false;
            }
        }
        public string CashierName
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return cashierName;
            }
            set
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                cashierName = value;
                Printer.SetOperatorName(value);
            }
        }
        public DateTime PrinterTime
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetDateTime();
            }
            set
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                Printer.SetDateTime(value);
            }
        }
        public string SerialNum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetSerialNumber();
            }
        }

        public Hashtable Taxes
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetCurrentTaxRates();
            }
            set
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                Printer.SetCurrentTaxRates(value);
            }
        }

        public PrinterFeatures Features
        {
            get
            {
                return features;
            }
        }

        public Money CashSum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetDrawerSum();
            }
        }

        public Money SalesSum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetSalesSum();
            }
        }

        public Money RefundSum
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                return Printer.GetRefundSum();
            }
        }

        public string RefundSumByTax
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                // return addPaymentSums ? Printer.GetRefundSumByTax() + ";" + Printer.GetRefundSumByPayment() : Printer.GetRefundSumByTax();
                return Printer.GetRefundSumByTax();
            }
        }

        public string SalesSumByTax
        {
            get
            {
                if (!IsOpened)
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

                //;Наличные,карточка,кредит,чек
                // return addPaymentSums ? Printer.GetSalesSumByTax() + ";" + Printer.GetSalesSumByPayment() : Printer.GetSalesSumByTax();

                return string.Empty;
            }
        }

        public virtual string DeviceName
        {
            get
            {
                return "A38EDF44-B9FA-4b37-917D-37943967401E";
            }
        }

        public virtual string DeviceFriendlyName
        {
            get
            {
                return DatecsStrings.GetString((int)Message.DeviceFriendlyName);
            }
        }

        public Port DefaultPort
        {
            get
            {
                return new Port(PortType.COM, 1);
            }
        }

        public PortType[] PortTypes
        {
            get
            {
                return new PortType[] { PortType.COM };
            }
        }

        public virtual Parameter[] Parameters
        {
            get
            {
                return new Parameter[0];
            }
            set
            {
            }
        }

        public bool IsOpened
        {
            get
            {
                return Printer.IsOpen;
            }
        }

        //public void BeginReceipt(PrinterReceiptType type, int cashNum, int systemRecNum, string comment, string recNum)
        //{
        //    throw new NotImplementedException();
        //}
        virtual public void BeginReceipt(PrinterReceiptType type, int cashNum, int systemRecNum, string comment, string recNum)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptAlreadyOpen));

            Log.Write(String.Format("Открытие чека: комментарий: {0} тип: {1} номер кассы: {2}, сист.номер чека: {3} фиск.номер: {4}", comment, checkType, cashNum, systemRecNum, recNum), Log.MessageType.Message, this);

            closeReceiptStep = 0;
            string cash = DatecsStrings.GetString((int)Message.CashNum) + cashNum.ToString();

            int printerRecNum = 0;

            switch (type)
            {
                case PrinterReceiptType.Sales:
                    Printer.OpenFiscalReceipt(cashNum);

                    Printer.PrintFiscalText(DatecsStrings.GetString((int)Message.Check) + NextRecNum);
                    Printer.PrintFiscalText(cash);
                    Printer.PrintFiscalText(comment);
                    break;
                case PrinterReceiptType.Refund:
                    OpenRefundReceipt(cashNum);

                    PrintRefundText(DatecsStrings.GetString((int)Message.Check) + NextRefRecNum);
                    PrintRefundText(cash);
                    PrintRefundText(DatecsStrings.GetString((int)Message.RefundByCheck) + recNum);
                    PrintRefundText(comment);
                    break;
                case PrinterReceiptType.Copy:
                    Printer.OpenNonfiscalReceipt();

                    Printer.PrintNonfiscalText(CashierString);
                    Printer.PrintNonfiscalText(DatecsStrings.GetString((int)Message.CopyByCheck) + recNum);
                    Printer.PrintNonfiscalText(cash);
                    Printer.PrintNonfiscalText(comment);
                    break;
                case PrinterReceiptType.NoFiscalSales:
                    Printer.OpenNonfiscalReceipt();

                    Printer.PrintNonfiscalText(CashierString);
                    Printer.PrintNonfiscalText(recNumberForNoFiscalReceipt + " " + systemRecNum);
                    Printer.PrintNonfiscalText(cash);
                    Printer.PrintNonfiscalText(comment);
                    break;
                case PrinterReceiptType.NoFiscalRefund:
                    Printer.OpenNonfiscalReceipt();

                    Printer.PrintNonfiscalText(CashierString);
                    Printer.PrintNonfiscalText(recNumberForNoFiscalRefundReceipt + " " + systemRecNum);
                    Printer.PrintNonfiscalText(cash);
                    Printer.PrintNonfiscalText(DatecsStrings.GetString((int)Message.RefundByCheck) + recNum);
                    Printer.PrintNonfiscalText(comment);
                    break;
                case PrinterReceiptType.Text:
                    Printer.OpenNonfiscalReceipt();
                    break;
            }

            discountSums.Clear();
            receiptSums.Clear();
            checkOpened = true;
            checkType = type;
        }

        protected string CashierString
        {
            get
            {
                return string.Format("{0}  {1}", DatecsStrings.GetString((int)Message.Cashier), cashierName);
            }
        }

        public void BeginTextDoc()
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.OpenNonfiscalReceipt();

            checkOpened = true;
            checkType = PrinterReceiptType.Text;
        }

        virtual public void CancelReceipt()
        {
            Log.Write("CancelReceipt()", Log.MessageType.Message, this);
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            checkOpened = false;
            closeReceiptStep = 0;

            if (Printer.GetStatus().NonFiscalCheckOpen)
            {
                Printer.CloseNonfiscalReceipt();
                Log.Write("Printer.CloseNonfiscalReceipt();", Log.MessageType.Message, this);
            }

            Printer.CancelFiscalReceipt();
            Log.Write("Printer.CancelFiscalReceipt();", Log.MessageType.Message, this);
        }

        public void CashOut(Money sum)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.CashOut(sum);
        }

        public void CashIn(Money sum)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.CashIn(sum);
        }

        public void ClearDisplay()
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.ClearDisplay();
        }

        public void Close()
        {
            Printer.Disconnect();
        }

        virtual public void EndReceipt(Money receiptSum, PrinterPayment payment)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            Log.Write("Закрытие чека", Log.MessageType.Message, this);

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                    CloseSalesReceipt(payment);
                    break;
                case PrinterReceiptType.Refund:
                    CloseRefundReceipt(payment);
                    break;
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                    CloseCopyReceipt(payment);
                    break;
                case PrinterReceiptType.Text:
                    Printer.CloseNonfiscalReceipt();
                    break;
            }

            checkOpened = false;
        }

        protected virtual void SetReceiptDiscount()
        {
            Printer.RefundSums(discountSums, DatecsStrings.GetString((int)Message.Discount));
        }

        protected virtual void CloseSalesReceipt(PrinterPayment payment)
        {
            string comment = "";
            try
            {
                if (closeReceiptStep == 0)
                {
                    SetReceiptDiscount();
                    closeReceiptStep++;
                }

                if (closeReceiptStep == 1)
                {
                    if (payment.GetSum(PaymentType.Credit) != new Money(0))
                        Printer.Total(payment.GetSum(PaymentType.Credit), PaymentType.Credit, comment);
                    closeReceiptStep++;
                }

                if (closeReceiptStep == 2)
                {
                    if (payment.GetSum(PaymentType.Check) != new Money(0))
                        Printer.Total(payment.GetSum(PaymentType.Check), PaymentType.Check, comment);
                    closeReceiptStep++;
                }

                if (closeReceiptStep == 3)
                {
                    if (payment.GetSum(PaymentType.Card) != new Money(0))
                        Printer.Total(payment.GetSum(PaymentType.Card), PaymentType.Card, comment);
                    closeReceiptStep++;
                }

                if (closeReceiptStep == 4)
                {
                    if (payment.GetSum(PaymentType.Cash) != new Money(0))
                        Printer.Total(payment.GetSum(PaymentType.Cash), PaymentType.Cash, comment);
                    closeReceiptStep++;
                }

                if (closeReceiptStep == 5)
                {
                    Printer.CloseFiscalReceipt();
                    closeReceiptStep++;
                }

                closeReceiptStep = 0;
            }
            catch (LostConnectException err)
            {
                Log.Write(err, this);

                throw;
            }
            catch (DeviceException err)
            {
                Log.Write(err, this);
                bool needCritical = false;

                try
                {
                    needCritical = Printer.status.PaperOut || Printer.ReceiptPaymentAndNoClose;
                }
                catch (DeviceException e)
                {
                    Log.Write(err, this);
                    throw new LostConnectException(e.Message, e);
                }

                if (needCritical)
                    throw new CriticalPaymentException(err.Message, err);
                else
                    throw;
            }
        }

        protected virtual void CloseRefundReceipt(PrinterPayment payment)
        {
            if (discountSums.TaxA != new Money(0))
                Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Discount),
                    string.Format("-{0} {1}", discountSums.TaxA.ToString("F"), DatecsTaxNames.TaxA), Const.MaxNonFiscalString));
            if (discountSums.TaxB != new Money(0))
                Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Discount),
                    string.Format("-{0} {1}", discountSums.TaxB.ToString("F"), DatecsTaxNames.TaxB), Const.MaxNonFiscalString));
            if (discountSums.TaxC != new Money(0))
                Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Discount),
                    string.Format("-{0} {1}", discountSums.TaxC.ToString("F"), DatecsTaxNames.TaxC), Const.MaxNonFiscalString));
            if (discountSums.TaxD != new Money(0))
                Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Discount),
                    string.Format("-{0} {1}", discountSums.TaxD.ToString("F"), DatecsTaxNames.TaxD), Const.MaxNonFiscalString));
            if (discountSums.TaxFree != new Money(0))
                Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Discount),
                    string.Format("-{0} {1}", discountSums.TaxFree.ToString("F"), DatecsTaxNames.TaxFree), Const.MaxNonFiscalString));

            Printer.PrintNonfiscalText("");

            Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Total), (receiptSums.Total - discountSums.Total).ToString("F") + "  ", Const.MaxNonFiscalString));
            Printer.PrintNonfiscalText("");
            Printer.CloseNonfiscalReceipt();

            if (receiptSums.TaxA != new Money(0))
                Printer.VoidSell((receiptSums.TaxA - discountSums.TaxA), TaxGrp.TaxA, DatecsStrings.GetString((int)Message.Refund));
            if (receiptSums.TaxB != new Money(0))
                Printer.VoidSell((receiptSums.TaxB - discountSums.TaxB), TaxGrp.TaxB, DatecsStrings.GetString((int)Message.Refund));
            if (receiptSums.TaxC != new Money(0))
                Printer.VoidSell((receiptSums.TaxC - discountSums.TaxC), TaxGrp.TaxC, DatecsStrings.GetString((int)Message.Refund));
            if (receiptSums.TaxD != new Money(0))
                Printer.VoidSell((receiptSums.TaxD - discountSums.TaxD), TaxGrp.TaxD, DatecsStrings.GetString((int)Message.Refund));
            if (receiptSums.TaxFree != new Money(0))
                Printer.VoidSell((receiptSums.TaxFree - discountSums.TaxFree), TaxGrp.TaxFree, DatecsStrings.GetString((int)Message.Refund));
        }

        private void CloseCopyReceipt(PrinterPayment payment)
        {
            Printer.PrintNonfiscalText(new string('-', Const.MaxNonFiscalString));

            Printer.PrintNonfiscalText(ConcatStrings(DatecsStrings.GetString((int)Message.Total), (receiptSums.Total - discountSums.Total).ToString("F") + "  ", Const.MaxNonFiscalString));

            Hashtable taxes = Printer.GetCurrentTaxRates();
            Money taxSum;
            Percent taxRate;

            if (receiptSums.TaxA != new Money(0))
            {
                if (taxes.Contains(TaxGrp.TaxA))
                    taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxA]).Rate;
                else
                    taxRate = new Percent(0);

                taxSum = receiptSums.TaxA - discountSums.TaxA;
                Printer.PrintNonfiscalText(ConcatStrings(string.Format("{0} {1}={2}%", DatecsStrings.GetString((int)Message.NDS), Printer.GetDatecsTaxName(TaxGrp.TaxA), taxRate.ToString()),
                    taxSum.GetInTax(taxRate).ToString("F") + "  ", Const.MaxNonFiscalString));
            }

            if (receiptSums.TaxB != new Money(0))
            {
                if (taxes.Contains(TaxGrp.TaxB))
                    taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxB]).Rate;
                else
                    taxRate = new Percent(0);

                taxSum = receiptSums.TaxB - discountSums.TaxB;
                Printer.PrintNonfiscalText(ConcatStrings(string.Format("{0} {1}={2}%", DatecsStrings.GetString((int)Message.NDS), Printer.GetDatecsTaxName(TaxGrp.TaxB), taxRate.ToString()),
                    taxSum.GetInTax(taxRate).ToString("F") + "  ", Const.MaxNonFiscalString));
            }

            if (receiptSums.TaxC != new Money(0))
            {
                if (taxes.Contains(TaxGrp.TaxC))
                    taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxC]).Rate;
                else
                    taxRate = new Percent(0);

                taxSum = receiptSums.TaxC - discountSums.TaxC;
                Printer.PrintNonfiscalText(ConcatStrings(string.Format("{0} {1}={2}%", DatecsStrings.GetString((int)Message.NDS), Printer.GetDatecsTaxName(TaxGrp.TaxC), taxRate.ToString()),
                    taxSum.GetInTax(taxRate).ToString("F") + "  ", Const.MaxNonFiscalString));
            }

            if (receiptSums.TaxD != new Money(0))
            {
                if (taxes.Contains(TaxGrp.TaxD))
                    taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxD]).Rate;
                else
                    taxRate = new Percent(0);

                taxSum = receiptSums.TaxD - discountSums.TaxD;
                Printer.PrintNonfiscalText(ConcatStrings(string.Format("{0} {1}={2}%", DatecsStrings.GetString((int)Message.NDS), Printer.GetDatecsTaxName(TaxGrp.TaxD), taxRate.ToString()),
                    taxSum.GetInTax(taxRate).ToString("F") + "  ", Const.MaxNonFiscalString));
            }

            if (receiptSums.TaxFree != new Money(0))
            {
                if (taxes.Contains(TaxGrp.TaxFree))
                    taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxFree]).Rate;
                else
                    taxRate = new Percent(0);

                taxSum = receiptSums.TaxFree - discountSums.TaxFree;
                Printer.PrintNonfiscalText(ConcatStrings(string.Format("{0} {1}={2}%", DatecsStrings.GetString((int)Message.NDS), Printer.GetDatecsTaxName(TaxGrp.TaxFree), taxRate.ToString()),
                    taxSum.GetInTax(taxRate).ToString("F") + "  ", Const.MaxNonFiscalString));
            }

            Printer.PrintNonfiscalText(new string('-', Const.MaxNonFiscalString));
            Printer.CloseNonfiscalReceipt();
        }

        public void EndReceipt()
        {
            EndReceipt(true);
        }

        public void EndReceipt(bool printToControlType)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            Log.Write("Закрытие чека", Log.MessageType.Message, this);

            Printer.CloseNonfiscalReceipt();

            checkOpened = false;
        }

        public void Feed()
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.AdvancePaper();
        }

        public Parameter[] GetDefaultParameters(PortType portType)
        {
            return GetParameters(portType);
        }

        public string GetModemError()
        {
            return null;
        }

        public Parameter[] GetParameters(PortType portType)
        {
            return Parameters;
        }

        public void Init(string[] header, string[] footer, string[] firstHeader, string[] endFooter, string recNumberForNoFiscalReceipt, string recNumberForNoFiscalRefundReceipt, Printers.PrinterOptions options)
        {
            Log.Write("Init start", Log.MessageType.Message, this);

            if (!Printer.IsOpen)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (Printer.GetStatus().FiscalCheckOpen)
            {
                Log.Write("Init CloseFiscalReceipt", Log.MessageType.Message, this);
                Printer.CloseFiscalReceipt();
            }

            if (Printer.GetStatus().NonFiscalCheckOpen)
            {
                Log.Write("Init CloseNonfiscalReceipt", Log.MessageType.Message, this);
                Printer.CloseNonfiscalReceipt();
            }
            Log.Write("Success", Log.MessageType.Message, this);

            Printer.SetHeaderFooter(header, footer);
            printOneCount = (options & PrinterOptions.PrintOneCount) != 0;
            printArtCode = (options & PrinterOptions.PrintArtCode) != 0;
            printDiscountEachPosition = (options & PrinterOptions.PrintDiscountEachPosition) != 0;
            this.recNumberForNoFiscalReceipt = recNumberForNoFiscalReceipt;
            this.recNumberForNoFiscalRefundReceipt = recNumberForNoFiscalRefundReceipt;

            Log.Write("Init end", Log.MessageType.Message, this);
        }

        public bool IsPortSupported(PortType portType)
        {
            foreach (PortType supPort in PortTypes)
            {
                if (supPort == portType)
                    return true;
            }

            return false;
        }

        void IFiscalPrinter.Open(Port port, string deviceVersion)
        {
            Open(port);
        }

        public virtual void Open(Port port)
        {
            Log.Write("Open(Port port) Start", Log.MessageType.Message, this);
            Printer.Connect(port);
            Printer.GetStatus();
            InitFeatures();
            Log.Write("Open(Port port) End", Log.MessageType.Message, this);
        }

        protected virtual void InitFeatures()
        {
            features.OnlineMode = false;
            features.MaxRecItemCount = Const.MaxRecItemCount;
            features.TextDocLineLength = Const.MaxNonFiscalString;
            features.PrintNonFiscalDoc = true;
            features.RefundCashless = true;
            features.PrintCommentsInCheck = true;
            features.PrintCheckCopy = true;
            features.CashBoxControl = true;
            features.ArtRegistration = false;
            features.SeparateRecCounters = false;
            features.PaymentSumControl = false;
            features.SetTaxes = true;
            features.DisplayLen = Const.DisplayLen;
        }

        public void OpenDrawer()
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.OpenDrawer();
        }

        public void PrintBarcode(string barcode, BarcodeType barcodeType = BarcodeType.Code128)
        {
        }

        virtual public void PrintBottomComment(string[] comments)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                    foreach (string comment in TrimComment(comments, Const.MaxFiscalString))
                        Printer.PrintFiscalText(comment);
                    break;
                case PrinterReceiptType.Refund:
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Text:
                    foreach (string comment in TrimComment(comments, Const.MaxNonFiscalString))
                        Printer.PrintNonfiscalText(comment);
                    break;
            }
        }

        protected string[] TrimComment(string[] comments, int commentLength)
        {
            List<string> trimComments = new List<string>();

            foreach (string comment in comments)
            {
                int lineCount = comment.Length / commentLength + (comment.Length % commentLength > 0 ? 1 : 0);

                for (int pos = 0; pos < lineCount; pos++)
                {
                    int begin = commentLength * pos;
                    int count = commentLength;

                    if (begin + count > comment.Length)
                        count = comment.Length - begin;

                    trimComments.Add(comment.Substring(begin, count));
                }
            }

            return trimComments.ToArray();
        }

        public void PrintClientInfo(string clientName)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                    Printer.PrintFiscalText(DatecsStrings.GetString((int)Message.User) + clientName);
                    break;
                case PrinterReceiptType.Refund:
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Text:
                    Printer.PrintNonfiscalText(DatecsStrings.GetString((int)Message.User) + clientName);
                    break;
            }
        }

        public void PrintExtReport(int reportId, object parameters)
        {
        }

        public void PrintNullRec()
        {
            Log.Write("PrintNullRec Start",  Log.MessageType.Message, null);
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));
            else
                Log.Write("PrintNullRec IsOpened",  Log.MessageType.Message, this);

            Log.Write("PrintNullRec BeginReceipt Start",  Log.MessageType.Message, this);
            BeginTextDoc();
            Log.Write("PrintNullRec BeginReceipt End",  Log.MessageType.Message, this);
            Log.Write("PrintNullRec BeginReceipt PrintNonfiscalText Start",  Log.MessageType.Message, this);
            Printer.PrintNonfiscalText(DatecsStrings.GetString((int)Message.NullReceipt));
            Log.Write("PrintNullRec BeginReceipt PrintNonfiscalText End",  Log.MessageType.Message, this);
            //Printer.Total(new Money(0), PaymentType.Cash, "");

            EndReceipt();
        }

        virtual public void PrintRecCopy()
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.MakeReceiptCopy();
            Printer.Cut();
        }

        public void PrintRecDiscount(Money sum, string discountName)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));
        }

        public void PrintReceiptItem(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName, Money discount, Money recDiscount, 
            TaxGrp taxGrp, int artNum, bool cancelled, string artKey = null)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            Log.Write(String.Format("Печать позиции чека: код товара: {0} имя: {1} цена: {2} количество: {3} ед.измерения: {4} стоимость: {5} спец.предложение: {6} скидка:	{7} скидка на чек: {8} налог: {9} номер артикула: {10} отменено: {11}"
                , artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled), Log.MessageType.Message, this);

            if (cancelled)
                return;

            if (printArtCode)
                artName = string.Format("{0}-{1}", artCode, artName);

            if (printDiscountEachPosition)
            {
                discount += recDiscount;
                recDiscount = new Money(0);
            }

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                    Printer.RegisterItem(artName, unitPrice, count, taxGrp, printOneCount, artNum, discount);
                    break;
                case PrinterReceiptType.Refund:
                    RegisterRefundItem(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
                    break;
                case PrinterReceiptType.Text:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Copy:
                    PrintNonfiscalPosition(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
                    break;
            }

            AddSums(taxGrp, cost, discount, recDiscount);
        }

        protected virtual void RegisterRefundItem(int artCode, string artName, Money unitPrice, Quantity count, string unitName, 
            Money cost, string offerName, Money discount, Money recDiscount, TaxGrp taxGrp, int artNum, bool cancelled)
        {
            PrintNonfiscalPosition(artCode, artName, unitPrice, count, unitName, cost, offerName, discount, recDiscount, taxGrp, artNum, cancelled);
        }

        protected virtual void PrintNonfiscalPosition(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName,
            Money discount, Money recDiscount, Printers.TaxGrp taxGrp, int artNum, bool cancelled)
        {
            if (printOneCount || (count.Type != Quantity.QuantityTypes.NoWeight || count.Amount != 1))
                Printer.PrintNonfiscalText(ConcatStrings(count.ToString("F") + " X", unitPrice.ToString("F") + "  ", Const.MaxNonFiscalString));

            if (artName.Length > Const.MaxArticleString)
                artName = artName.Substring(0, Const.MaxArticleString);

            Printer.PrintNonfiscalText(ConcatStrings(artName, string.Format("{0} {1}", cost.ToString("F"), Printer.GetDatecsTaxName(taxGrp)), Const.MaxNonFiscalString));
        }

        virtual protected void AddSums(Printers.TaxGrp taxGrp, Money cost, Money discount, Money recDiscount)
        {
            discountSums.AddSum(taxGrp, discount + recDiscount);
            receiptSums.AddSum(taxGrp, cost);
        }

        public virtual void PrintReport(ReportType reportType)
        {
            if (!Printer.IsOpen)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            switch (reportType)
            {
                case ReportType.ArtReport:
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.NotSupported));
                case ReportType.XReport:
                    Printer.PrintXReport();
                    break;
                case ReportType.ZReport:
                    Printer.PrintZReport();
                    break;
            }
        }

        public void PrintReport(ReportType reportType, int beginNum, int endNum)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (reportType == ReportType.ZReport)
                Printer.PrintZReport(beginNum, endNum);
        }

        public void PrintReport(Printers.ReportType reportType, DateTime beginDate, DateTime endDate)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (reportType == ReportType.ZReport)
                Printer.PrintZReport(beginDate, endDate);
        }

        private WindowsBase.Size MeasureChar(FontSizeType fontSizeType)
        {
            return new WindowsBase.Size(15, 15);
        }

        public void PrintTemplate(PrintService.Templates.Printer.IPrintTemplateFactory templateFactory)
        {
            try
            {
                ITextTemplatePrintInfo templatePrinter = templateFactory.CreateTextPrinter();
                templatePrinter.PageSizeInChars = new WindowsBase.Size(Features.TextDocLineLength, 0);
                templatePrinter.MeasureCharHandler = MeasureChar;

                BeginTextDoc();

                templatePrinter.CreatePages(true, 96, 96);
                foreach (ITemplatePrintInfoPage page in templatePrinter.Pages)
                {
                    foreach (ITemplatePrintInfoPageItem item in page.PageItems)
                    {
                        if (item.PageItemType == PageItemType.String)
                        {
                            IStringTemplatePrintInfoPageItem stringPageItem = (IStringTemplatePrintInfoPageItem)item;
                            PrintTextLine(stringPageItem.Text);
                        }
                    }
                }

                EndReceipt();
            }
            catch (TemplateException err)
            {
                Log.Write(err, this);
                throw new DeviceException(err.Message, err);
            }
        }

        public void PrintTextLine(string line)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (!checkOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.ReceiptNotOpen));

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                    Printer.PrintFiscalText(line);
                    break;
                case PrinterReceiptType.Refund:
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Copy:
                case PrinterReceiptType.Text:
                    Printer.PrintNonfiscalText(line);
                    break;
            }
        }

        public void Reset(bool reconnect)
        {
            Log.Write("Start Print Z reset", Log.MessageType.Message, this);
            CancelReceipt();
            Log.Write("End Print Z reset", Log.MessageType.Message, this);
        }

        public void SetLogo(Bitmap bmp)
        {
            throw new NotImplementedException();
        }

        public void SetPaymentTypes(Dictionary<PaymentType, string> paymentTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowDisplayLine(string line, DisplayLineType displayLineType)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            if (line.Length > Const.DisplayLen)
                line = line.Substring(0, Const.DisplayLen);

            switch (displayLineType)
            {
                case DisplayLineType.Top:
                    Printer.DisplayTextUL(line);
                    break;
                case DisplayLineType.Bootom:
                    Printer.DisplayTextLL(line);
                    break;
            }
        }

        public void ShowItemInfo(string itemName, Quantity itemCount, Money itemCost, Money recSum)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.DisplayTextUL(ConcatStrings(itemName.ToString(), itemCount.ToString(), Const.DisplayLen));
            Printer.DisplayTextLL(ConcatStrings(itemCost.ToString("F"), recSum.ToString("F"), Const.DisplayLen));
        }

        public void ShowPaymentInfo(Money recSum, string recSumHint, Money changeSum, string changeSumHint)
        {
            if (!IsOpened)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.DeveceNotOpen));

            Printer.DisplayTextUL(ConcatStrings(recSumHint, recSum.ToString("F"), Const.DisplayLen));
            Printer.DisplayTextLL(ConcatStrings(changeSumHint, changeSum.ToString("F"), Const.DisplayLen));
        }

        protected string ConcatStrings(string str1, string str2, int maxLen)
        {
            int maxLenStr1 = maxLen - str2.Length - 1;
            if (str1.Length > maxLenStr1)
                str1 = str1.Substring(0, maxLenStr1);

            return str1 + new string(' ', maxLenStr1 - str1.Length + 1) + str2;
        }

        protected virtual void OpenRefundReceipt(int cashNum)
        {
            Printer.OpenNonfiscalReceipt();
        }

        virtual protected void PrintRefundText(string text)
        {
            Printer.PrintNonfiscalText(text);
        }

    }
}
