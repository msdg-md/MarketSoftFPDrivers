using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Devices.IO;
using System.Drawing;
using Phoenix.Globals.Units;
using Phoenix.Globals;
using System.Collections;
//using Phoenix.Globals.TemplatePrintInfo;
using WindowsBase = System.Windows;
using Phoenix.PrintService.Templates.Printer;
using Phoenix.PrintService.Templates;

namespace Phoenix.Devices.Printers.Elicom
{
    public class Elicom : IFiscalPrinter
    {
        #region Members

        protected uint baudrate;
        protected FlowControl flowControl;
        protected Parity parity;
        protected byte databits;
        protected StopBits stopbits;

        private ElicomProtocol printer = new ElicomProtocol();
        private PrinterFeatures features = new PrinterFeatures();


        private string recNumberForNoFiscalReceipt;
        private string recNumberForNoFiscalRefundReceipt;
        private PrinterOptions options;
        protected PrinterReceiptType checkType = PrinterReceiptType.Sales;
        protected bool printArtCode = false;
        private string cashName;
        private string comment;
        private bool checkOpened = false;
        List<string> bottomComments = new List<string>();
        private string barcode;
        protected List<ItemInfo> items = new List<ItemInfo>();
        private bool printDiscountEachPosition;
        private Bitmap logo = null;

        private Money discountSum;

        #endregion

        #region Constructors

        public Elicom()
        {
            baudrate = 115200;
            databits = 8;
            flowControl = FlowControl.None;
            parity = Parity.None;
            stopbits = StopBits.One;
        }

        #endregion



        public void OpenDrawer()
        {
            Printer.OpenDrawer();
        }

        public void ClearDisplay()
        {
            Printer.ClearDisplay();
        }

        public void ShowDisplayLine(string line, DisplayLineType displayLineType)
        {
            Printer.ShowDisplayLine(line, displayLineType);
        }

        public bool FiscalMode
        {
            get
            {
                return Printer.GetFisclaMode();
            }
        }

        public bool DemoMode
        {
            get
            {
                return false;
            }
        }

        public void Feed()
        {
            Printer.Feed();
        }

        public string SerialNum
        {
            get
            {
                return Printer.GetSerialNum();
            }
        }

        public DateTime PrinterTime
        {
            get
            {
                return Printer.GetPrinterTime();
            }
            set
            {
                Printer.SetPrinterTime(value);
            }
        }

        public string CashierName
        {
            get
            {
                return Printer.GetCashierName();
            }
            set
            {
                try
                {
                    Printer.SetCashierName(value);
                }
                catch (Exception err)
                {
                    Log.Write(err, null);
                }
            }
        }

        public void PrintRecCopy()
        {
            Printer.PrintRecCopy();
        }

        public string NextRecNum
        {
            get
            {
                return (Printer.GetLastReceiptNum() + 1).ToString();
            }
        }

        public string NextRefRecNum
        {
            get
            {
                return (Printer.GetLastReceiptNum() + 1).ToString();
            }
        }

        public string LastRefRecNum
        {
            get
            {
                return Printer.GetLastReceiptNum().ToString();
            }
        }

        public string LastRecNum
        {
            get
            {
                return Printer.GetLastReceiptNum().ToString();
            }
        }

        public string NextZRepNum
        {
            get
            {
                return (Printer.GetLastZRepNum() + 1).ToString();
            }
        }

        public string LastZRepNum
        {
            get
            {
                return Printer.GetLastZRepNum().ToString();
            }
        }

        public PrinterFeatures Features
        {
            get
            {
                return features;
            }
        }

        public void PrintExtReport(int reportId, object parameters)
        {
        }

        public void CashIn(Money sum)
        {
            Printer.CashIn(sum);
        }

        public void CashOut(Money sum)
        {
            Printer.CashOut(sum);
        }

        public Money CashSum
        {
            get
            {
                return Printer.GetCashSum();
            }
        }

        public void BeginTextDoc()
        {
            BeginReceipt(PrinterReceiptType.Text, 0, 0, "", "");
        }

        public void Init(string[] header, string[] footer, string[] firstHeader, string[] endFooter, string recNumberForNoFiscalReceipt, string recNumberForNoFiscalRefundReceipt, PrinterOptions options)
        {
            this.options = options;
            this.recNumberForNoFiscalReceipt = recNumberForNoFiscalReceipt;
            this.recNumberForNoFiscalRefundReceipt = recNumberForNoFiscalRefundReceipt;

            printArtCode = (options & PrinterOptions.PrintArtCode) != 0;
            printDiscountEachPosition = true;//(options & PrinterOptions.PrintDiscountEachPosition) != 0;

            try
            {
                for (int pos = 0; pos < 1; pos++)
                {
                    string val = footer.Length < pos + 1 ? "" : footer[pos];
                    Printer.SetHeaderFooter(pos + 8, val);
                }


                for (int pos = 0; pos < 7; pos++)
                {
                    string val = header.Length < pos + 1 ? "" : header[pos];
                    Printer.SetHeaderFooter(pos + 1, val);
                }
            }
            catch (Exception)
            {
            }
        }

        //public void PrintTemplate(ITemplatePrintInfo template)
        //{
        //    try
        //    {
        //        template.IsTape = true;
        //        template.PrinterType = PrinterType.String;
        //        template.PageSize = new Size(Features.TextDocLineLength, 0);

        //        FontRatio fontRatio = new FontRatio();
        //        fontRatio.MediumFontRatio.RatioH = 1;
        //        fontRatio.MediumFontRatio.RatioW = 1;

        //        fontRatio.LargeFontRatio.RatioH = 1;
        //        fontRatio.LargeFontRatio.RatioW = 1;

        //        template.FontRatio = fontRatio;

        //        BeginTextDoc();

        //        template.CreatePages();
        //        foreach (ITemplatePrintInfoPage page in template.Pages)
        //        {
        //            foreach (ITemplatePrintInfoPageItem item in page.PageItems)
        //            {
        //                if (item.PageItemType == PageItemType.String)
        //                {
        //                    PrintTextLine(item.StringItem);
        //                }
        //                else if (item.PageItemType == PageItemType.Barcode)
        //                {
        //                    barcode = item.StringItem;
        //                }
        //            }
        //        }

        //        EndReceipt();
        //    }
        //    catch (TemplateException err)
        //    {
        //        Log.Write(err, this);
        //        throw new DeviceException(err.Message, err);
        //    }
        //}

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

        private WindowsBase.Size MeasureChar(FontSizeType fontSizeType)
        {
            return new WindowsBase.Size(15, 15);
        }

        public void CancelReceipt()
        {
            if (Printer.IsReceiptOpen)
                Printer.CancelReceipt();

            if (Printer.IsNoFiscalReceiptOpen)
                Printer.CloseNoFislalReceipt();

            checkOpened = false;
            barcode = null;
        }

        public void Reset(bool reconnect)
        {
            CancelReceipt();
        }

        public void PrintTextLine(string line)
        {
            if (!checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentNotOpen);

            Printer.PrintLine(line);
        }

        public void PrintNullRec()
        {
            Printer.OpenNoFislalReceipt();
            Printer.PrintLine(CultureStrings.NullReceipt);
            // Printer.AutomaticCloseReceipt();
            Printer.CloseNoFislalReceipt();
        }

        public void ShowPaymentInfo(Money recSum, string recSumHint, Money changeSum, string changeSumHint)
        {
            Printer.ShowDisplayLine(ConcatStrings(recSumHint, recSum.ToString("F"), Consts.DisplayTextLength), DisplayLineType.Top);
            Printer.ShowDisplayLine(ConcatStrings(changeSumHint, changeSum.ToString("F"), Consts.DisplayTextLength), DisplayLineType.Bootom);
        }

        public void ShowItemInfo(string itemName, Quantity itemCount, Money itemCost, Money recSum)
        {
            Printer.ShowDisplayLine(ConcatStrings(itemName.ToString(), itemCount.ToString(), Consts.DisplayTextLength), DisplayLineType.Top);
            Printer.ShowDisplayLine(ConcatStrings(itemCost.ToString("F"), recSum.ToString("F"), Consts.DisplayTextLength), DisplayLineType.Bootom);
        }

        public void PrintBarcode(string barcode, BarcodeType barcodeType = BarcodeType.Code128)
        {
            if (!checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentNotOpen);

            this.barcode = barcode;
        }

        public void PrintBottomComment(string[] comments)
        {
            //if (comments.Length > 0)
            //    Printer.PrintLine("");

            foreach (string comment in TrimComment(comments, features.TextDocLineLength))
                Printer.PrintLine(comment);
        }

        public void PrintClientInfo(string clientName)
        {
            Printer.PrintLine(CultureStrings.Customer + clientName);
        }

        public void PrintRecDiscount(Money sum, string discountName)
        {
            if (!checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentNotOpen);

            if (printDiscountEachPosition || sum == new Money(0))
                return;

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                case PrinterReceiptType.Refund:
                    //Printer.AddDiscount(sum, true);
                    break;
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Copy:
                    discountSum = sum;
                    break;
                case PrinterReceiptType.Text:
                    break;
            }
        }

        public void EndReceipt()
        {
            EndReceipt(true);
        }

        public void EndReceipt(bool printToControlType)
        {
            PrintBarcode();
            checkOpened = false;
            Printer.CloseNoFislalReceipt();

            // Printer.Feed(5);
            // Printer.Cut();
        }

        public virtual void BeginReceipt(PrinterReceiptType type, int cashNum, int systemRecNum, string comment, string recNum)
        {
            if (checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentAlreadyOpen);

            string cash = null;
            if (cashNum != 0)
                cash = CultureStrings.CashNo + cashNum.ToString();

            switch (type)
            {
                case PrinterReceiptType.Sales:
                    Printer.OpenReceipt();
                    //Printer.PrintLine(CashierString);
                    Printer.PrintLine(cash);
                    Printer.PrintLine(comment);
                    //Printer.PrintLine("");
                    break;
                case PrinterReceiptType.Refund:
                    OpenRefundReceipt();
                    //headerDocText = CultureStrings.RefundByReceipt + recNum;
                    //Printer.PrintHeader(CultureStrings.RefundByReceipt + recNum);
                    //Printer.PrintLine(CashierString);
                    Printer.PrintLine(cash);
                    Printer.PrintLine(CultureStrings.RefundByReceipt + recNum);
                    Printer.PrintLine(comment);
                    //Printer.PrintLine("");

                    /*Printer.OpenReceipt();
                                           //Printer.PrintLine(CashierString);
                                           Printer.PrintLine(cash);
                                           Printer.PrintLine(CultureStrings.RefundByReceipt + recNum);
                                           Printer.PrintLine(comment);
                                           Printer.PrintLine("");*/
                    break;
                case PrinterReceiptType.Copy:
                    Printer.OpenNoFislalReceipt();
                    //headerDocText = CultureStrings.CopyNo + recNum;
                    //Printer.PrintHeader(CultureStrings.CopyNo + recNum);
                    //Printer.PrintLine(CashierString);
                    Printer.PrintLine(cash);
                    Printer.PrintLine(CultureStrings.CopyNo + recNum);
                    Printer.PrintLine(comment);
                    //Printer.PrintLine("");
                    break;
                case PrinterReceiptType.NoFiscalSales:
                    Printer.OpenNoFislalReceipt();
                    //headerDocText = recNumberForNoFiscalReceipt + " " + systemRecNum;
                    //Printer.PrintHeader(recNumberForNoFiscalReceipt + " " + systemRecNum);
                    //Printer.PrintLine(CashierString);
                    Printer.PrintLine(cash);
                    Printer.PrintLine(recNumberForNoFiscalReceipt + " " + systemRecNum);
                    Printer.PrintLine(comment);
                    //Printer.PrintLine("");
                    break;
                case PrinterReceiptType.NoFiscalRefund:
                    Printer.OpenNoFislalReceipt();
                    //headerDocText = CultureStrings.RefundByReceipt + recNum;
                    //Printer.PrintHeader(CultureStrings.RefundByReceipt + recNum);
                    //Printer.PrintLine(CashierString);
                    Printer.PrintLine(cash);
                    Printer.PrintLine(CultureStrings.RefundByReceipt + recNum);
                    Printer.PrintLine(comment);
                    //Printer.PrintLine("");
                    break;
                case PrinterReceiptType.Text:
                    Printer.OpenNoFislalReceipt();
                    //Printer.PrintHeader("");
                    //PrintLogo();
                    break;
            }

            checkOpened = true;
            discountSum = new Money(0);
            checkType = type;
            barcode = null;

            if (checkType == PrinterReceiptType.Refund)
                checkType = PrinterReceiptType.NoFiscalRefund;
        }

        protected virtual void OpenRefundReceipt()
        {
            Printer.OpenNoFislalReceipt();
        }

        public void EndReceipt(Money receiptSum, PrinterPayment payment)
        {
            if (!checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentNotOpen);

            switch (checkType)
            {
                case PrinterReceiptType.Sales:
                case PrinterReceiptType.Refund:
                    PrintBarcode();
                    PaymentCheck(payment);
                    Printer.CloseReceipt();
                    break;
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Copy:
                    PrintBarcode();
                    PrintNoFiscalPayment(receiptSum, payment);
                    Printer.CloseNoFislalReceipt();
                    break;
                case PrinterReceiptType.Text:
                    EndReceipt();
                    break;
            }

            if (Printer.IsReceiptOpen)
                throw new FiscalPrinterException(CultureStrings.NotCloseRecNotMoney);

            checkOpened = false;
        }

        public void PrintReceiptItem(int artCode, string artName, Money unitPrice, Quantity count, string unitName, Money cost, string offerName, Money discount, Money recDiscount, TaxGrp taxGrp, int artNum, bool cancelled, string artKey = null)
        {
            if (!checkOpened)
                throw new FiscalPrinterException(CultureStrings.DocumentNotOpen);

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
                    Printer.AddReceiptItem(artName, unitPrice, count, taxGrp, discount);
                    break;
                case PrinterReceiptType.Refund:
                    Printer.AddRefundReceiptItem(artName, unitPrice, count, taxGrp, discount);
                    break;
                case PrinterReceiptType.NoFiscalSales:
                case PrinterReceiptType.NoFiscalRefund:
                case PrinterReceiptType.Copy:
                    Printer.PrintLine(artName);
                    Printer.PrintLine(StringToRight(string.Format("{0} X {1} ={2}", count.ToString("F"), unitPrice.ToString("F"), cost.ToString("F")), Printer.TextDocLineLength));
                    if (discount != new Money(0))
                        Printer.PrintLine(ConcatStrings(CultureStrings.Discount, "=" + discount.ToString("F"), Printer.TextDocLineLength));
                    break;
                case PrinterReceiptType.Text:
                    break;
            }
        }

        #region IFiscalPrinter Members


        public void SetLogo(Bitmap bmp)
        {
            Printer.SetLogo(bmp);
        }

        public Hashtable Taxes
        {
            get
            {
                return new Hashtable();
            }
            set
            {

            }
        }

        public bool SessionOpened
        {
            get
            {
                return false;
            }
        }

        public string RefundSumByTax
        {
            get
            {
                return "";
            }
        }

        public string SalesSumByTax
        {
            get
            {
                return "";
            }
        }

        public Money SalesSum
        {
            get
            {
                return new Money(0);
            }
        }

        public Money RefundSum
        {
            get
            {
                return new Money(0);
            }
        }



        public void PrintReport(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.XReport:
                    Printer.PrintXReport();
                    break;
                case ReportType.ZReport:
                    Printer.PrintZReport();
                    break;
                case ReportType.ArtReport:
                    Printer.PrintArtReport();
                    break;
                case ReportType.SavedSales:
                default:
                    throw new FiscalPrinterException(CultureStrings.ReportTypeNotSupport);
            }
        }

        public void PrintReport(ReportType reportType, int beginNum, int endNum)
        {
            switch (reportType)
            {
                case ReportType.ZReport:
                   // Printer.PrintReportByNumber(beginNum, endNum);
                    break;
                case ReportType.XReport:
                case ReportType.ArtReport:
                case ReportType.SavedSales:
                default:
                    throw new FiscalPrinterException(CultureStrings.ReportTypeNotSupport);
            }
        }

        public void PrintReport(ReportType reportType, DateTime beginDate, DateTime endDate)
        {
            switch (reportType)
            {
                case ReportType.ZReport:
                    //Printer.PrintReportByDate(beginDate, endDate);
                    break;
                case ReportType.XReport:
                case ReportType.ArtReport:
                case ReportType.SavedSales:
                default:
                    throw new FiscalPrinterException(CultureStrings.ReportTypeNotSupport);
            }
        }

        public string GetModemError()
        {
            return null;
        }

        #endregion

        #region IDevice Members

        public virtual string DeviceName
        {
            get
            {
                return "06C01E8A-11A6-4d22-A79B-644EBA64EEFE";
            }
        }

        public virtual string DeviceFriendlyName
        {
            get
            {
                return CultureStrings.ElicomFriendlyName;
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
                List<Parameter> parameters = new List<Parameter>();

                parameters.Add(new Parameter((int)SettingsParameter.Baudrate, (int)baudrate, new int[] { 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200 }, CultureStrings.BaudrateDescription));
                parameters.Add(new Parameter((int)SettingsParameter.Databits, (int)databits, 5, 8, CultureStrings.DatabitsDescription));
                parameters.Add(new Parameter((int)SettingsParameter.FlowControl, flowControl.ToString(), new string[] { FlowControl.None.ToString(), FlowControl.Hardware.ToString(), FlowControl.XOnXOff.ToString() }, CultureStrings.FlowControlDescription));
                parameters.Add(new Parameter((int)SettingsParameter.StopBits, stopbits.ToString(), new string[] { StopBits.One.ToString(), StopBits.OneAndHalf.ToString(), StopBits.Two.ToString() }, CultureStrings.StopBitsDescription));
                parameters.Add(new Parameter((int)SettingsParameter.Parity, parity.ToString(), new string[] { Parity.Even.ToString(), Parity.Mark.ToString(), Parity.None.ToString(), Parity.Odd.ToString(), Parity.Space.ToString() }, CultureStrings.ParityDescription));

                return parameters.ToArray();
            }
            set
            {

                foreach (Parameter parameter in value)
                {
                    switch (parameter.Key)
                    {
                        case (int)SettingsParameter.Baudrate:
                            baudrate = (uint)parameter.IntValue;
                            break;
                        case (int)SettingsParameter.Databits:
                            databits = (byte)parameter.IntValue;
                            break;
                        case (int)SettingsParameter.FlowControl:
                            flowControl = (FlowControl)Enum.Parse(typeof(FlowControl), parameter.StringValue, true);
                            break;
                        case (int)SettingsParameter.Parity:
                            parity = (Parity)Enum.Parse(typeof(Parity), parameter.StringValue, true);
                            break;
                        case (int)SettingsParameter.StopBits:
                            stopbits = (StopBits)Enum.Parse(typeof(StopBits), parameter.StringValue, true);
                            break;
                    }
                }
            }
        }

        public void Open(Port port, string deviceVersion)
        {
            Open(port);
        }

        public void Open(Port port)
        {
            Printer.Connect(port, baudrate, databits, flowControl, parity, stopbits);
            Printer.GetStatus();

            features.OnlineMode = false;
            features.MaxRecItemCount = 1000;
            features.TextDocLineLength = Printer.TextDocLineLength;
            features.FiscalCommentLineLength = Printer.TextDocLineLength;
            features.PrintNonFiscalDoc = true;
            features.RefundCashless = true;
            features.PrintSavedSalesReport = false;
            features.PrintCommentsInCheck = true;
            features.PrintCheckCopy = true;
            features.CashBoxControl = true;
            features.ArtRegistration = false;
            features.SeparateRecCounters = false;
            features.PaymentSumControl = true;
            features.SetTaxes = false;
            features.DisplayLen = Consts.DisplayTextLength;
        }

        public void Close()
        {
            Printer.Disconnect();
        }

        public bool IsOpened
        {
            get
            {
                return Printer.IsOpen;
            }
        }

        public Parameter[] GetParameters(PortType portType)
        {
            return Parameters;
        }

        public Parameter[] GetDefaultParameters(PortType portType)
        {
            return GetParameters(portType);
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

        #endregion

        #region Private

        protected virtual ElicomProtocol Printer
        {
            get
            {
                return printer;
            }
        }

        private void PrintBarcode()
        {
            Printer.PrintBarcode(barcode);
        }

        private string ConcatStrings(string str1, string str2, int maxLen)
        {
            int maxLenStr1 = maxLen - str2.Length - 1;
            if (str1.Length > maxLenStr1)
                str1 = str1.Substring(0, maxLenStr1);

            return str1 + new string(' ', maxLenStr1 - str1.Length + 1) + str2;
        }

        private string StringToRight(string str, int maxLen)
        {
            if (str.Length > maxLen)
                return str.Substring(0, maxLen);
            else
                return new string(' ', maxLen - str.Length) + str;
        }

        private string[] TrimComment(string[] comments, int commentLength)
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

        private void PrintNoFiscalPayment(Money receiptSum, PrinterPayment payment)
        {
            if (discountSum != new Money(0))
            {
                Printer.PrintLine(ConcatStrings(CultureStrings.Total2, "=" + (discountSum + receiptSum).ToString("F"), Printer.TextDocLineLength));
                Printer.PrintLine(ConcatStrings(CultureStrings.Discount, "=" + discountSum.ToString("F"), Printer.TextDocLineLength));
            }

            Printer.PrintLine(ConcatStrings(CultureStrings.Total, "=" + receiptSum.ToString("F"), Printer.TextDocLineLength));

            if (payment.GetSum(PaymentType.Credit) != new Money(0))
                Printer.PrintLine(ConcatStrings(CultureStrings.Credit, "=" + payment.GetSum(PaymentType.Credit).ToString("F"), Printer.TextDocLineLength));

            if (payment.GetSum(PaymentType.Check) != new Money(0))
                Printer.PrintLine(ConcatStrings(CultureStrings.Check, "=" + payment.GetSum(PaymentType.Check).ToString("F"), Printer.TextDocLineLength));

            if (payment.GetSum(PaymentType.Card) != new Money(0))
                Printer.PrintLine(ConcatStrings(CultureStrings.Card, "=" + payment.GetSum(PaymentType.Card).ToString("F"), Printer.TextDocLineLength));

            if (payment.GetSum(PaymentType.Cash) != new Money(0))
                Printer.PrintLine(ConcatStrings(CultureStrings.Cash, "=" + payment.GetSum(PaymentType.Cash).ToString("F"), Printer.TextDocLineLength));

            Printer.PrintLine(ConcatStrings(CultureStrings.GetSum, "=" + payment.TotalSum.ToString("F"), Printer.TextDocLineLength));
            Printer.PrintLine(ConcatStrings(CultureStrings.RestSum, "=" + (payment.TotalSum - receiptSum).ToString("F"), Printer.TextDocLineLength));
            Printer.PrintLine("");
            //Printer.PrintLine(StringToRight(Printer.GetDateTime().ToString("dd-MM-yy HH:mm"), Printer.TextDocLineLength));
            // Printer.PrintLine(ConcatStrings(" ???", Printer.GetSerialNum(), Printer.TextDocLineLength));
        }

        private void PaymentCheck(PrinterPayment payment)
        {
            if (payment.GetSum(PaymentType.Ext2) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Ext2), PaymentType.Ext2);

            if (payment.GetSum(PaymentType.Ext3) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Ext3), PaymentType.Ext3);

            if (payment.GetSum(PaymentType.Credit) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Credit), PaymentType.Credit);

            if (payment.GetSum(PaymentType.Check) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Check), PaymentType.Check);

            if (payment.GetSum(PaymentType.Card) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Card), PaymentType.Card);

            if (payment.GetSum(PaymentType.Cash) != new Money(0))
                Printer.Payment(payment.GetSum(PaymentType.Cash), PaymentType.Cash);
        }

        #endregion

        #region Nested classes

        protected enum SettingsParameter
        {
            Baudrate = 1,
            Databits = 2,
            FlowControl = 3,
            Parity = 4,
            StopBits = 5,
            IsStandartPrinting =6,
        }

        protected class ItemInfo
        {
            string artName;
            Money unitPrice;
            Quantity count;
            TaxGrp taxGrp;
            int artNum;
            Percent discount;

            public ItemInfo(string artName, Money unitPrice, Quantity count, TaxGrp taxGrp, int artNum, Percent discount)
            {
                this.artName = artName;
                this.unitPrice = unitPrice;
                this.count = count;
                this.taxGrp = taxGrp;
                this.artNum = artNum;
                this.discount = discount;
            }

            public string ArtName
            {
                get
                {
                    return artName;
                }
            }

            public Money UnitPrice
            {
                get
                {
                    return unitPrice;
                }
            }

            public Quantity Count
            {
                get
                {
                    return count;
                }
            }

            public TaxGrp TaxGrp
            {
                get
                {
                    return taxGrp;
                }
            }

            public int ArtNum
            {
                get
                {
                    return artNum;
                }
            }

            public Percent Discount
            {
                get
                {
                    return discount;
                }
            }

        }

        #endregion

        #region IFiscalPrinter Members


        public void SetPaymentTypes(Dictionary<PaymentType, string> paymentTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PrintReceiptItem(PrintReceiptItem receiptItem)
        {
            this.PrintReceiptItem(receiptItem.ArtCode, receiptItem.ArtName, receiptItem.UnitPrice, receiptItem.Count,
                receiptItem.UnitName, receiptItem.Cost, receiptItem.OfferName, receiptItem.Discount, receiptItem.RecDiscount, receiptItem.TaxGrp,
                receiptItem.ArtNum, receiptItem.Cancelled, receiptItem.ArtKey);
        }

        public void PrintImage(Bitmap bmp)
        {
          //  throw new NotImplementedException();
        }

        public void PrintReceiptItemComments(string[] comments)
        {
            //throw new NotImplementedException();
        }

        public void SetIPAddress(string ipAddres, string networkMask, string defaultGateway, bool useDHCP)
        {
            //throw new NotImplementedException();
        }

        public void ExecuteExtCommand(int commandId, object parameters)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
