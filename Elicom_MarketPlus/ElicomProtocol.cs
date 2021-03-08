using SoftMarket.Devices.IO;
using SoftMarket.Devices.IO.COMPort;
using SoftMarket.Globals;
using SoftMarket.Globals.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace SoftMarket.Devices.Printers.Elicom
{
    public class ElicomProtocol
    {
        protected IComPort comPort = (IComPort)new ComPort();
        private Port port;
        private uint baudrate;
        private byte databits;
        private FlowControl flowControl;
        private Parity parity;
        private StopBits stopbits;
        private bool isOpen = false;
        protected int timeoutAnswer = 5;
        private int trySendCount = 1;
        private byte lastCommandNumber = 0;

        public ElicomProtocol()
        {

        }

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
        }

        public void Connect()
        {
            Connect(port, baudrate, databits, flowControl, parity, stopbits, timeoutAnswer);
        }

        public void Connect(Port port, uint baudrate, byte databits, FlowControl flowControl, Parity parity, StopBits stopbits, int printerAnswerTimeout)
        {
            if (port.Type != PortType.COM)
                throw new DeviceException(CultureStrings.PortTypeCorrupt);

            this.port = port;
            this.baudrate = baudrate;
            this.databits = databits;
            this.flowControl = flowControl;
            this.parity = parity;
            this.stopbits = stopbits;
            this.timeoutAnswer = printerAnswerTimeout;

            ComPortSettings settings = new ComPortSettings();

            settings.baudrate = baudrate;
            settings.databits = databits;
            settings.flowControl = flowControl;
            settings.parity = parity;
            settings.stopbits = stopbits;
            settings.timeouts = new SerialTimeouts(0, 0, 150, 0, 0);

            comPort.Init(settings);
            Log.Write(String.Format("Соединение с фискальным принтером: СОМ{0}", port.Number), Log.MessageType.Info, this);
            Log.Write( string.Format("flowControl:{0} parity:{1} stopbits:{2}", flowControl.ToString(), parity.ToString(),
                stopbits.ToString()), Log.MessageType.Info,  null);

            comPort.Open(port.Number);

            isOpen = true;
        }

        public void Disconnect()
        {
            Log.Write("Закрытие соединения с фискальным принтером", Log.MessageType.Info, this);
            comPort.Close();
            isOpen = false;
        }

        #region Protocol level

        private byte GetCommadNumber()
        {
            if (lastCommandNumber < 0x20 || lastCommandNumber >= 0x9F)
                lastCommandNumber = 0x20;
            else
                lastCommandNumber++;

            return lastCommandNumber;
        }

        protected string[] SendPacket(Packet packet)
        {
            return SendPacketToPrinter(packet).StringsData;
        }

        //private void SendPacketForPrint(Packet packet)
        //{
        //    SendPacketToPrinter(packet);
        //}
    
        private byte[] SendPacketAndGetBytes(Packet packet)
        {
            return SendPacketToPrinter(packet).BytesData;
        }

        #region Printing line to fiscal printer

        private void SendPacket_ForPrinting(Packet packet)
        {
            SendPacketToPrinterNoResponse(packet);
        }

        private void SendPacketToPrinterNoResponse(Packet packet)
        {
            if (!IsOpen)
                throw new FiscalPrinterException(CultureStrings.PrinterNotOpen);

            if (!comPort.IsOpen)
                Connect();

            try
            {
                //packet.CommandNumber = GetCommadNumber();
                int tryPos = 0;
                Packet answer = null;

                while (true)
                {
                    comPort.Reset();
                    comPort.Write(packet.PacketData);

                    try
                    {
                        if (!ReadAnswerForPrint(out answer))
                        {
                            Log.Write("No response at all from printer",Log.MessageType.Error, this);
                            return;
                        }

                        if (answer != null)
                        {
                            break;
                        }

                        //if (answer == null)
                        //    Thread.Sleep(2000);
                        //else
                        //    break;
                    }
                    catch (Exception err)
                    {
                        Log.Write(err, null);
                        if (++tryPos > trySendCount)
                            throw;
                        // new FiscalPrinterException(CultureStrings.RecivedNAK);
                    }
                }

                if (answer.IsSession24HoursException)
                    throw new Session24HoursException(answer.GetErrorDescription());

                if (answer.IsError)
                    throw new FiscalPrinterException(answer.GetErrorDescription());

                if (!answer.IsACKPacket && answer.Command != packet.Command)
                    throw new FiscalPrinterException(CultureStrings.ProtocolError);

                if (answer.CommandNumber != (packet.CommandNumber + 0x20))
                    throw new FiscalPrinterException(CultureStrings.ProtocolError);
            }
            catch (COMPortException err)
            {
                Log.Write(err, null);
                comPort.Close();

                throw;
            }
        }

        private bool ReadAnswerForPrint(out Packet packet)
        {
            DateTime endTime = DateTime.Now.AddSeconds(timeoutAnswer);
            List<byte> answer = new List<byte>();
            packet = null;

            while (endTime > DateTime.Now)
            {
                byte response;
                if (comPort.Read(out response))
                {
                    if (answer.Count == 0 && response == Symbols.NAK)
                        throw new FiscalPrinterException(CultureStrings.RecivedNAK);
                    if (answer.Count == 0 && response == Symbols.RETRY)
                        return true;
                    //else
                    answer.Add(response);
                }
                else if (answer.Count > 0)
                {
                    packet = new Packet();

                    if (!packet.ParseData(answer.ToArray()))
                        throw new FiscalPrinterException(CultureStrings.RecivedCurruptPacket);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            Log.Write(string.Format("Before timeout - Environment.StackTrace: {0}", Environment.StackTrace), Log.MessageType.Message, this);
            throw new LostConnectException(CultureStrings.ReciveTimeout);
        }

        #endregion

        private Packet SendPacketToPrinter(Packet packet)
        {
            if (!IsOpen)
                throw new FiscalPrinterException(CultureStrings.PrinterNotOpen);

            if (!comPort.IsOpen)
                Connect();

            try
            {
                //packet.CommandNumber = GetCommadNumber();
                int tryPos = 0;
                Packet answer = null;

                while (true)
                {
                    comPort.Reset();
                    comPort.Write(packet.PacketData);

                    try
                    {
                        answer = ReadAnswer();

                        if (answer != null)
                        {
                            break;
                        }
                        
                        //if (answer == null)
                        //    Thread.Sleep(2000);
                        //else
                        //    break;
                    }
                    catch (Exception err)
                    {
                        Log.Write(string.Format("Command={0} tryPos={1}", packet.Command, tryPos), Log.MessageType.Message, null);
                        Log.Write(err, null);

                        if (++tryPos > trySendCount)
                            throw;
                        // new FiscalPrinterException(CultureStrings.RecivedNAK);
                    }
                }

                if (answer.IsSession24HoursException)
                    throw new Session24HoursException(answer.GetErrorDescription());

                if (answer.IsError)
                    throw new FiscalPrinterException(answer.GetErrorDescription());

                if (!answer.IsACKPacket && answer.Command != packet.Command)
                    throw new FiscalPrinterException(CultureStrings.ProtocolError);

                if (answer.CommandNumber != (packet.CommandNumber + 0x20))
                    throw new FiscalPrinterException(CultureStrings.ProtocolError);

                return answer;
            }
            catch (COMPortException err)
            {
                Log.Write(err, null);
                comPort.Close();

                throw;
            }
        }

        private Packet ReadAnswer()
        {
            //Log.Write(string.Format("timeout = {0}", timeoutAnswer), Log.MessageType.Message, null);

            DateTime endTime = DateTime.Now.AddSeconds(timeoutAnswer);
            List<byte> answer = new List<byte>();

            while (endTime > DateTime.Now)
            {
                byte response;
                if (comPort.Read(out response))
                {
                    if (answer.Count == 0 && response == Symbols.NAK)
                        throw new FiscalPrinterException(CultureStrings.RecivedNAK);
                    if (answer.Count == 0 && response == Symbols.RETRY)
                        return null;
                    //else
                    answer.Add(response);
                }
                else if (answer.Count > 0)
                {
                    Packet packet = new Packet();

                    if (!packet.ParseData(answer.ToArray()))
                        throw new FiscalPrinterException(CultureStrings.RecivedCurruptPacket);

                    return packet;
                }
                else
                {
                    Log.Write(string.Format("comPort.Read() = false && answer.Count == 0 error ({0})", response), Log.MessageType.Info, this);
                }
                
            }
            Log.Write(string.Format("Before timeout - Environment.StackTrace: {0}",  Environment.StackTrace), Log.MessageType.Message, this);
            throw new LostConnectException(CultureStrings.ReciveTimeout);
        }

        private string NormalizeString(string str, int len)
        {
            if (str == null)
                str = string.Empty;

            if (str.Length > len)
                str = str.Substring(0, len);

            if (str.Length < len)
                str += new string(' ', len - str.Length);

            return str;
        }

        private Money StringToMoney(string sum)
        {
            sum = sum.Replace(".", "");
            return new Money(int.Parse(sum));
        }

        #endregion

        public byte[] GetStatus()
        {
            return SendPacketAndGetBytes(new Packet(Commands.GetStatus));
        }

        public void OpenDrawer()
        {
            SendPacket(new Packet(Commands.OpenDrawer));
        }

        public void ClearDisplay()
        {
            SendPacket(new Packet(Commands.ClearDisplay));
        }

        public void ShowDisplayLine(string line, DisplayLineType displayLineType)
        {
            line = NormalizeString(line, Consts.DisplayTextLength);

            Packet packet;

            if (displayLineType == DisplayLineType.Top)
                packet = new Packet(Commands.DisplayTextLine1);
            else if (displayLineType == DisplayLineType.Bootom)
                packet = new Packet(Commands.DisplayTextLine2);
            else
                return;

            packet.AddMessage(line);

            SendPacket(packet);

        }

        public bool GetFisclaMode()
        {
            return (GetStatus()[3] & (1 << 5)) != 0 ;
        }

        public void Feed()
        {
            SendPacket(new Packet(Commands.Feed));
        }

        public string GetSerialNum()
        {
           return SendPacket(new Packet(Commands.GetSerialNum))[0];
        }

        public DateTime GetPrinterTime()
        {
            return DateTime.ParseExact( SendPacket(new Packet(Commands.GetPrinterTime))[0], "dd-MM-yyyy HH:mm", null);
        }

        public void SetPrinterTime(DateTime dateTime)
        {
            SendPacket(new Packet(Commands.SetPrinterTime, dateTime.ToString("dd-MM-yy HH:mm:ss")));
        }

        public void SetCashierName(string cashierName)
        {
            //cashierName = NormalizeString(cashierName, 20);

            //Packet packet = new Packet(Commands.SetCashierName);
            //packet.AddMessage("1");
            //packet.AddMessage(cashierName);
            //packet.AddMessage(Consts.CashierPassword);
            //SendPacket(packet);
        }

        public string GetCashierName()
        {
           return SendPacket(new Packet(Commands.GetCashierName, "1"))[1];
        }

        public void PrintRecCopy()
        {
            SendPacket(new Packet(Commands.PrintRecCopy));
        }

        public int GetLastReceiptNum()
        {
            int receiptNum;

            if (int.TryParse(SendPacket(new Packet(Commands.GetLastReceiptNum))[0], out receiptNum))
                return receiptNum;
            else
                return 0;
        }

        public int GetLastZRepNum()
        {
            int receiptNum;

            if (int.TryParse(SendPacket(new Packet(Commands.GetLastZRepNum))[1], out receiptNum))
            {
                return receiptNum;
            }
            else
                return 0;
        }

        public void PrintXReport()
        {
            SendPacket(new Packet(Commands.DailyFiscalReport, "X"));
        }

        public void PrintZReport()
        {
            //Thread.Sleep(1500);
            SendPacket(new Packet(Commands.DailyFiscalReport, "Z"));
            //Thread.Sleep(1500);
        }

        public void CashIn(Money sum)
        {
            Packet packet = new Packet(Commands.CashIO);
            packet.AddMessage("1");
            packet.AddMessage(Consts.CashierPassword);
            packet.AddMessage("0");
            packet.AddMessage(sum.ToString("F"));
            SendPacket(packet);
        }

        public void CashOut(Money sum)
        {
            Packet packet = new Packet(Commands.CashIO);
            packet.AddMessage("1");
            packet.AddMessage(Consts.CashierPassword);
            packet.AddMessage("0");
            packet.AddMessage("-" + sum.ToString("F"));
            SendPacket(packet);
        }

        public Money GetCashSum()
        {
            return StringToMoney(SendPacket(new Packet(Commands.GetSumsByPaymetType, "0"))[1]);
        }

        public void SetHeaderFooter(int lineNum, string line)
        {
            //Packet packet = new Packet(Commands.SetHeaderFooter);
            //packet.AddMessage(lineNum.ToString());
            //packet.AddMessage(NormalizeString(line, 38));

            //SendPacket(packet);
        }

        public void CancelReceipt()
        {
            SendPacket(new Packet(Commands.CancelReceipt));
        }

        public void CloseNoFislalReceipt()
        {
            SendPacket(new Packet(Commands.CloseNoFislalReceipt));
        }

        public void OpenNoFislalReceipt()
        {
            Packet packet = new Packet(Commands.OpenNoFislalReceipt);
            packet.AddMessage("1");
            packet.AddMessage(Consts.CashierPassword);
            SendPacket(packet);
        }

        public void PrintLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                line = "  ";

            if (line.Length > Consts.TextDocLineLength)
                line = line.Substring(0, Consts.TextDocLineLength);

            SendPacket_ForPrinting(new Packet(Commands.PrintTextLine, line));
        }

        public bool IsReceiptOpen
        {
            get
            {
                return (GetStatus()[2] & (1 << 1)) != 0;
            }
        }

        public bool IsNoFiscalReceiptOpen
        {
            get
            {
                return (GetStatus()[2] & (1 <<0)) != 0;
            }
        }

        public virtual void OpenReceipt()
        {
            Packet packet = new Packet(Commands.OpenReceipt);
            packet.AddMessage("1");
            packet.AddMessage(Consts.CashierPassword);
            packet.AddMessage("0");
            packet.AddMessage("0");
            //packet.AddMessage("0");
            SendPacket(packet);
        }

        public void CloseReceipt()
        {
            SendPacket(new Packet(Commands.CloseReceipt));
        }

        public void AutomaticCloseReceipt()
        {
            SendPacket(new Packet(Commands.AutomaticCloseReceipt));
        }

        public void Payment(Money money, PaymentType paymentType)
        {
            Packet packet = new Packet(Commands.Payment);
            packet.AddMessage(GetPaymentType( paymentType));
            packet.AddMessage("1");
            packet.AddMessage(money.ToString("F"));
            SendPacket(packet);
        }

        private string GetPaymentType(PaymentType paymentType)
        {
            switch (paymentType)
            {
                case PaymentType.Cash:
                    return "0";
                case PaymentType.Card:
                    return "1";
                case PaymentType.Check:
                    return "2";
                case PaymentType.Credit:
                    return "3";
                case PaymentType.Ext1:
                    return "5";
                case PaymentType.Ext2:
                    return "6";
                default:
                    throw new FiscalPrinterException($"{CultureStrings.UnknownPaymentType} - {paymentType}");
            }

        }

        private string GetTaxGrp(TaxGrp taxGrp)
        {
            switch (taxGrp)
            {
                case TaxGrp.TaxA:
                    return "Б";
                case TaxGrp.TaxB:
                    return "В";
                case TaxGrp.TaxC:
                    return "Г";
                case TaxGrp.TaxD:
                    return "Д";
                case TaxGrp.TaxE:
                    return "Е";
                case TaxGrp.TaxFree:
                    return "А";
                default:
                    throw new FiscalPrinterException(CultureStrings.UnknownTaxGrp);
            }
        }

        public void AddReceiptItem(string atrName, Money unitPrice, Quantity count, TaxGrp taxGrp, Money disc)
        {
            Packet packet = new Packet(Commands.Sale);
            packet.AddMessage(NormalizeString(atrName, 36));
            packet.AddMessage(GetTaxGrp(taxGrp));

            string priceString = unitPrice.ToString("F") + "*" + count.ToString("F");

            if (disc > new Money(0))
                priceString += ":-" + disc.ToString("F");

            packet.AddMessage(priceString);

            SendPacket(packet);
        }

        public void PrintBarcode(string barcode)
        {
            SendPacket(new Packet(Commands.Barcode, string.Format("P;H;{0};{1}", barcode.Length, barcode))); //CODE_93 barcode
        }

        public void PrintArtReport()
        {
            SendPacket(new Packet(Commands.ArtReport, "X"));
        }

        public void PrintReportByNumber(int beginNum, int endNum)
        {
            Packet packet = new Packet(Commands.DailyFiscalReportByNmbOfBlocks);
            packet.AddMessage(beginNum.ToString("0000"));
            packet.AddMessage(endNum.ToString("0000"));

            SendPacket(packet);
        }

        public void PrintReportByDate(DateTime beginDate, DateTime endDate)
        {
            Packet packet = new Packet(Commands.DailyFiscalReportByDate);
            packet.AddMessage(beginDate.ToString("ddMMyy"));
            packet.AddMessage(endDate.ToString("ddMMyy"));

            SendPacket(packet);
        }

        public void SetLogo(Bitmap bmp)
        {
            /*
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Bmp);

            Packet packet = new Packet(Commands.SetLogo, stream.GetBuffer());

            SendPacket(packet);
             * */
        }

        public virtual int TextDocLineLength
        {
            get
            {
                return Consts.TextDocLineLength;
            }
        }

        protected virtual void CheckPrinterStatus()
        {
        }

        public string GetSumByTax(bool refund)
        {
            Log.Write("Получение сумм " + (refund ? "возвратов" : "продаж") + " по группам налогообложения.", Log.MessageType.Warning, this);

            string[] sums = SendPacket(new Packet(Commands.ReadAmountsByVATGroups));

            if (sums.Length < 1)
                throw new FiscalPrinterException(CultureStrings.UnknownError);

            return $"{formatInt(sums[5])},{formatInt(sums[0])},{formatInt(sums[1])},{formatInt(sums[2])},{formatInt(sums[3])}";

        }
        public string GetSalesSumByPayment()
        {
            Log.Write($"Получение сумм продаж по типам оплат.", Log.MessageType.Warning, this);

            string[] data = SendPacket(new Packet(Commands.GetSumsByPaymetType, "0"));



            if (data.Length < 1)

                throw new FiscalPrinterException(CultureStrings.UnknownError);

            return $"{0},{formatInt(data[1])},{formatInt(data[2])},{0},{0},{0}";

        }

        public string GetRefundSumByPayment() => "";

        private int formatInt(string s) => int.Parse(s.Trim().Replace(".",""));
    }
}
