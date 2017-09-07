using System;
using System.Collections;
using System.Linq;
using System.Text;
using Phoenix.Devices.IO;
using Phoenix.Devices.IO.COMPort;
using Phoenix.Devices.Printers;
using Phoenix.Globals.Units;
using Phoenix.Devices;
using Phoenix.Globals;

namespace Phoenix.Devices.Printers.Datecs
{
    /// <summary>
    /// Реализация протокола для фискального принтера Datecs.
    /// </summary>
    public class DatecsProtocol
    {
        private IComPort comPort = (IComPort)new ComPort();
        private byte lastCommandNumber = 0;
        private Command lastCommand;
        private int lastRepeat = 0;
        public Status status = null;

        public DatecsProtocol()
        {
        }
        public virtual char GetDatecsTaxName(TaxGrp taxGrp)
        {
            char tax;

            switch (taxGrp)
            {
                case TaxGrp.TaxA:
                    tax = DatecsTaxNames.TaxA;
                    break;
                case TaxGrp.TaxB:
                    tax = DatecsTaxNames.TaxB;
                    break;
                case TaxGrp.TaxC:
                    tax = DatecsTaxNames.TaxC;
                    break;
                case TaxGrp.TaxD:
                    tax = DatecsTaxNames.TaxD;
                    break;
                case TaxGrp.TaxFree:
                    tax = DatecsTaxNames.TaxFree;
                    break;
                default:
                    throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.TaxCorrupt));
            }

            return tax;
        }


        #region Protocol level function
        /// <summary>
        /// Открывает СОМ порт фискального принтера.
        /// </summary>
        /// <param name="port">Номер СОМ порта(1 .. 9)</param>
        public void Connect(Port port)
        {
            if (port.Type != PortType.COM)
                throw new DeviceException(DatecsStrings.GetString((int)Message.PortTypeCorrupt));

            ComPortSettings settings = new ComPortSettings();

            settings.baudrate = 19200;
            settings.databits = 8;
            settings.flowControl = FlowControl.None;
            settings.parity = Parity.None;
            settings.stopbits = StopBits.One;
            settings.timeouts = new SerialTimeouts(0, 0, 60, 0, 0);

            comPort.Init(settings);
            Log.Write(String.Format("Соединение с фискальным принтером: СОМ{0}", port.Number), Log.MessageType.Message, this);

            comPort.Open(port.Number);
        }
        /// <summary>
        /// Закрывает СОМ порт фискального принтера.
        /// </summary>
        public void Disconnect()
        {
            Log.Write("Закрытие соединения с фискальным принтером", Log.MessageType.Message, this);
            comPort.Close();
        }
        /// <summary>
        /// Посылает комманду фискальному принтеру и возвращает результат выполнения комманды. 
        /// </summary>
        /// <param name="command">Комманда</param>
        /// <returns>Результат выполнения комманды</returns>
        protected byte[] SendCommand(Command command)
        {
            return SendCommand(command, new byte[0]);
        }
        /// <summary>
        /// Посылает комманду фискальному принтеру и возвращает результат выполнения комманды. 
        /// </summary>
        /// <param name="command">Комманда</param>
        /// <param name="extendedData">Данные для комманды</param>
        /// <returns>Результат выполнения комманды</returns>
        protected byte[] SendCommand(Command command, string extendedData)
        {
            return SendCommand(command, Encoding.GetEncoding(1251).GetBytes(extendedData));
        }
        /// <summary>
        /// Посылает комманду фискальному принтеру и возвращает результат выполнения комманды. 
        /// </summary>
        /// <param name="command">Комманда</param>
        /// <param name="extendedData">Данные для комманды</param>
        /// <returns>Результат выполнения комманды</returns>
        protected byte[] SendCommand(Command command, byte[] extendedData)
        {
            if (command != Command.GetStatus && GetStatusEachCommand)
            {
                try
                {
                    GetStatus();
                }
                catch (DeviceException err)
                {
                    Log.Write(err, this);
                }
            }

            lastCommand = command;
            lastRepeat = 1;

            while (true)
            {
                try
                {
                    if (++lastCommandNumber >= Const.MaxCommandNumber)
                        lastCommandNumber = 1;

                    byte[] packet = PreparePacketToSend(command, extendedData);

                    Log.Write(String.Format("Отправка команды на фискальный принтер: {0} пакет({1})", command, BitConverter.ToString(packet)), Log.MessageType.Message, this);

                    comPort.Reset();
                    comPort.Write(packet);
                    byte[] answer = ReadAnswer();
                    /*
                                        try
                                        {
                                            if(command != Command.GetStatus)
                                            {
                                                if(++lastCommandNumber >= Const.MaxCommandNumber)
                                                    lastCommandNumber = 1;

                                                comPort.Write(PreparePacketToSend(Command.GetStatus, new byte[0]));
                                            }
                                        }
                                        catch(Exception err)
                                        {
                                            Log.Write(err, this);
                                        }
                    */
                    return answer;
                }
                catch (CorruptPacketException err)
                {
                    Log.Write(err, this);

                    if (lastRepeat < Const.RepeatCount)
                        lastRepeat++;
                    else
                        throw new FiscalPrinterException(err.Message, err);
                }
                catch (LostConnectException err)
                {
                    Log.Write(err, this);

                    if (lastRepeat < Const.RepeatCount)
                        lastRepeat++;
                    else
                        throw;
                }
            }
        }
        /// <summary>
        /// Получает результат выполнения комманды от фискального принтера.
        /// </summary>
        /// <returns>Результат выполнения комманды</returns>
        private byte[] ReadAnswer()
        {
            Log.Write("Получение ответа от фискального принтера.", Log.MessageType.Message, this);
            return PrepareReceivedPacket(ReceivePacket());
        }
        /// <summary>
        /// Получает пакет с ответом на комманду от фискального принтера.
        /// </summary>
        /// <returns>Полученый пакет</returns>
        private byte[] ReceivePacket()
        {
            byte data;
            ArrayList packet = new ArrayList();
            DateTime startTime = DateTime.Now;

            while (true)
            {
                if (comPort.Read(out data))
                {
                    packet.Add(data);
                    startTime = DateTime.Now;
                }
                else if (packet.Count > 0)
                {
                    Log.Write(String.Format("Получен пакет: ({0})", BitConverter.ToString((byte[])packet.ToArray(typeof(byte)))), Log.MessageType.Message, this);

                    if ((byte)packet[0] == (byte)Flag.NAK)
                        throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedNAK));

                    if ((byte)packet[0] == (byte)Flag.SYN)
                    {
                        while (packet.Count > 0 && (byte)packet[0] == (byte)Flag.SYN)
                            packet.Remove(packet[0]);
                        startTime = DateTime.Now;
                    }

                    if (packet.Count > 0 && (byte)packet[packet.Count - 1] == (byte)Flag.End)
                        return (byte[])packet.ToArray(typeof(byte));
                }

                if (Math.Abs((DateTime.Now - startTime).TotalMilliseconds) > Const.ReadTimeOut)
                {
                    if (packet.Count > 0)
                        Log.Write(String.Format("Получен пакет: ({0})", BitConverter.ToString((byte[])packet.ToArray(typeof(byte)))), Log.MessageType.Message, this);

                    throw new LostConnectException(DatecsStrings.GetString((int)Message.ReciveTimeout));
                }
            }
        }
        /// <summary>
        /// Подготавливает данные для отправки фискальному принтеру.
        /// </summary>
        /// <param name="command">Комманда</param>
        /// <param name="extendedData">Данные для комманды</param>
        /// <returns>Пакет для отправки фискальному принтеру</returns>
        private byte[] PreparePacketToSend(Command command, byte[] extendedData)
        {
            ArrayList packet = new ArrayList();


            byte conSum = (byte)((int)lastCommandNumber + (int)command);

            packet.Add((byte)Flag.Begin);
            packet.Add((byte)(4 + extendedData.Length + 0x20));
            packet.Add((byte)(lastCommandNumber + 0x20));
            packet.Add((byte)command);
            packet.AddRange(DataToPinter(extendedData));
            packet.Add((byte)Flag.EndData);
            packet.AddRange(CalculateCheckSum((byte[])packet.ToArray(typeof(byte))));
            packet.Add((byte)Flag.End);

            return (byte[])packet.ToArray(typeof(byte));
        }
        /// <summary>
        /// Расчитывает контрольную сумму.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private byte[] CalculateCheckSum(byte[] packet)
        {
            UInt16 totalSum = 0;
            for (int pos = 1; pos < packet.Length; pos++)
            {
                totalSum += packet[pos];
                if (packet[pos] == (byte)Flag.EndData)
                    break;
            }

            byte[] sum = new byte[4];

            sum[0] = (byte)(((totalSum >> 12) & 0x0F) + 0x30);
            sum[1] = (byte)(((totalSum >> 8) & 0x0F) + 0x30);
            sum[2] = (byte)(((totalSum >> 4) & 0x0F) + 0x30);
            sum[3] = (byte)(((totalSum >> 0) & 0x0F) + 0x30);
            return sum;
        }
        /// <summary>
        /// Делает разбор пакета, полученого от фискального принтера.
        /// </summary>
        /// <param name="receivedPacket">Полученый пакет</param>
        /// <returns>Результат выполнения комманды</returns>
        private byte[] PrepareReceivedPacket(byte[] receivedPacket)
        {
            ArrayList data = new ArrayList();


            if (receivedPacket.Length < 4)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket[0] != (byte)Flag.Begin)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket[receivedPacket.Length - 1] != (byte)Flag.End)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            int len = receivedPacket[1] - 0x20;

            if ((receivedPacket[2] - 0x20) != lastCommandNumber)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket[3] != (byte)lastCommand)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket.Length < len + 1 || receivedPacket[len] != (byte)Flag.EndData)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket.Length < len + 6)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            byte[] checkSum = CalculateCheckSum(receivedPacket);

            for (int pos = 0; pos < checkSum.Length; pos++)
                if (checkSum[pos] != receivedPacket[receivedPacket.Length - (5 - pos)])
                    throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            if (receivedPacket[len - 7] != (byte)Flag.Separator)
                throw new CorruptPacketException(DatecsStrings.GetString((int)Message.RecivedBadPacket));

            byte[] statusBytes = new byte[] {receivedPacket[len - 6], receivedPacket[len - 5], receivedPacket[len - 4],
                                                receivedPacket[len - 3], receivedPacket[len - 2], receivedPacket[len - 1]};

            status = new Status(statusBytes);

            if (status.IsOk == false)
                throw new FiscalPrinterException(status.ToString());

            int posData = 4;
            while (receivedPacket[posData] != (byte)Flag.Separator)
                data.Add(receivedPacket[posData++]);

            return PinterToData((byte[])data.ToArray(typeof(byte)));
        }

        /// <summary>
        /// Преобразует данные для передачи на принтер.
        /// </summary>
        /// <param name="data">Данные.</param>
        /// <returns>Преобразованные данные.</returns>
        virtual protected byte[] DataToPinter(byte[] data)
        {
            byte[] returnData = new byte[data.Length];
            for (int pos = 0; pos < data.Length; pos++)
                returnData[pos] = Const.ToPrn[data[pos]];

            return returnData;
        }
        /// <summary>
        /// Преобразует данные, полученные то принтера.
        /// </summary>
        /// <param name="data">Данные.</param>
        /// <returns>Преобразованные данные.</returns>
        private byte[] PinterToData(byte[] data)
        {
            byte[] returnData = new byte[data.Length];
            for (int pos = 0; pos < data.Length; pos++)
                returnData[pos] = Const.ToANSI[data[pos]];

            return returnData;
        }

        protected string GetParameter(byte[] data, int paramNumber)
        {
            string strData = Encoding.GetEncoding(1251).GetString(data);
            string[] parameters = strData.Split(Const.DataSeparator);

            if (parameters.Length > paramNumber)
                return parameters[paramNumber];
            else
                return "";
        }
        protected void CheckResponse(byte response)
        {
            string message = DatecsStrings.GetString((int)Message.RecivedUninspected);
            switch (response)
            {
                case (byte)Flag.ResponseF:
                    message = DatecsStrings.GetString((int)Message.ResponseF);
                    break;
                case (byte)Flag.Response1:
                    message = DatecsStrings.GetString((int)Message.Response1);
                    break;
                case (byte)Flag.Response2:
                    message = DatecsStrings.GetString((int)Message.Response2);
                    break;
                case (byte)Flag.Response3:
                    message = DatecsStrings.GetString((int)Message.Response3);
                    break;
                case (byte)Flag.ResponseE:
                    message = DatecsStrings.GetString((int)Message.ResponseE);
                    break;
                case (byte)Flag.ResponseD:
                    message = DatecsStrings.GetString((int)Message.ResponseD);
                    break;
                case (byte)Flag.ResponseR:
                    message = DatecsStrings.GetString((int)Message.ResponseR);
                    break;
                case (byte)Flag.ResponseI:
                    message = DatecsStrings.GetString((int)Message.ResponseI);
                    break;
                default:
                    return;
            }

            throw new FiscalPrinterException(message);
        }

        protected virtual bool GetStatusEachCommand
        {
            get
            {
                return true;
            }
        }

        #endregion

        public bool IsOpen
        {
            get
            {
                return comPort.IsOpen;
            }
        }
        /// <summary>
        /// Возвращает состояние принтера.
        /// </summary>
        /// <returns>Состояние принтера.</returns>
        public Status GetStatus()
        {
            SendCommand(Command.GetStatus, "W");
            return status;
        }
        /// <summary>
        /// Возвращает серийный номер принтера.
        /// </summary>
        /// <returns>Серийный номер принтера.</returns>
        public virtual string GetSerialNumber()
        {
            return GetParameter(SendCommand(Command.GetDiagnosticInfo, "0"), 4);
        }
        /// <summary>
        /// Открывает денежный ящик.
        /// </summary>
        public void OpenDrawer()
        {
            SendCommand(Command.OpenDrawer);
        }
        /// <summary>
        /// Служебное внесение.
        /// </summary>
        /// <param name="sum">Сумма.</param>
        public void CashIn(Money sum)
        {
            byte[] data = SendCommand(Command.ServiceInputOutput, "+" + sum.ToString("F"));

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            CheckResponse(data[0]);
        }
        /// <summary>
        /// Служебное вынесение.
        /// </summary>
        /// <param name="sum">Сумма.</param>
        public void CashOut(Money sum)
        {
            byte[] data = SendCommand(Command.ServiceInputOutput, "-" + sum.ToString("F"));

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            CheckResponse(data[0]);
        }
        /// <summary>
        /// Возвращает текущие налоговые ставки.
        /// </summary>
        /// <returns>Налоговые ставки.</returns>
        public virtual Hashtable GetCurrentTaxRates()
        {
            Hashtable taxes = new Hashtable();
            byte[] data = SendCommand(Command.GetCurrentTaxRates);

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            if (data[0] == (byte)Flag.ResponseF)
                CheckResponse(data[0]);

            taxes.Add(TaxGrp.TaxA, new PrinterTaxGrp(PrinterTaxGrp.TaxType.InTaxType, new Percent(uint.Parse(GetParameter(data, 0) + "00")), new Percent[0]));
            taxes.Add(TaxGrp.TaxB, new PrinterTaxGrp(PrinterTaxGrp.TaxType.InTaxType, new Percent(uint.Parse(GetParameter(data, 1) + "00")), new Percent[0]));
            taxes.Add(TaxGrp.TaxC, new PrinterTaxGrp(PrinterTaxGrp.TaxType.InTaxType, new Percent(uint.Parse(GetParameter(data, 2) + "00")), new Percent[0]));
            taxes.Add(TaxGrp.TaxD, new PrinterTaxGrp(PrinterTaxGrp.TaxType.InTaxType, new Percent(uint.Parse(GetParameter(data, 3) + "00")), new Percent[0]));

            return taxes;
        }
        /// <summary>
        /// Продвигает бумагу на одну строчку.
        /// </summary>
        public void AdvancePaper()
        {
            SendCommand(Command.AdvancePaper);
        }
        /// <summary>
        /// Возвращает сумму в денежном ящике.
        /// </summary>
        /// <returns>Сумма в денежном ящике.</returns>
        virtual public Money GetDrawerSum()
        {
            byte[] data = SendCommand(Command.ServiceInputOutput, "0");

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            CheckResponse(data[0]);

            return new Money(int.Parse(GetParameter(data, 1)));
        }
        /// <summary>
        /// Возвращает сумму продаж.
        /// </summary>
        /// <returns>Сумма продаж.</returns>
        virtual public Money GetSalesSum()
        {
            byte[] data = SendCommand(Command.DayInfo);

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            return new Money(int.Parse(GetParameter(data, 0))) + new Money(int.Parse(GetParameter(data, 1))) + new Money(int.Parse(GetParameter(data, 2))) +
                new Money(int.Parse(GetParameter(data, 3)));
        }
        virtual public Money GetRefundSum()
        {
            return new Money(0);
        }

        virtual public string GetSalesSumByTax()
        {
            Log.Write("26666: GetCurrentTaxes:  Start", Log.MessageType.Message, this);
            byte[] data = SendCommand(Command.GetCurrentTaxRates);
            Log.Write("26666: GetCurrentTaxes:  End", Log.MessageType.Message, this);
            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            Money a = new Money(int.Parse(GetParameter(data, 1)));
            Money b = new Money(int.Parse(GetParameter(data, 2)));
            Money c = new Money(int.Parse(GetParameter(data, 3)));
            Money d = new Money(int.Parse(GetParameter(data, 4)));
            Money free = GetSalesSum() - a - b - c - d;

            string str = string.Format("{0},{1},{2},{3},{4}", free.ToString("S"), a.ToString("S"), b.ToString("S"), c.ToString("S"), d.ToString("S"));

            Log.Write("26666: GetCurrentTaxes:  STR" + str, Log.MessageType.Message, this);

            return str;
        }

        public string GetSumByTax(bool refund)
        {
            Log.Write("26666: GetCurrentTaxes:  Start", Log.MessageType.Message, this);
            byte[] array = SendCommand(Command.GetCurrentTaxRates, refund ? "1" : "0");
            Log.Write("26666: GetCurrentTaxes:  End", Log.MessageType.Message, this);
            if (array.Length < 1)
            {
                throw new FiscalPrinterException(DatecsStrings.GetString(26));
            }
            Money money = new Money(int.Parse(GetParameter(array, 0)));
            Money money2 = new Money(int.Parse(GetParameter(array, 1)));
            Money money3 = new Money(int.Parse(GetParameter(array, 2)));
            Money money4 = new Money(int.Parse(GetParameter(array, 3)));
            Money money5 = new Money(int.Parse(GetParameter(array, 4)));
            string str = string.Format("{0},{1},{2},{3},{4}", new object[]
    {
        money5.ToString("S"),
        money.ToString("S"),
        money2.ToString("S"),
        money3.ToString("S"),
        money4.ToString("S")
    });

            Log.Write("26666999999999999: GetCurrentTaxes:  Start: " + str, Log.MessageType.Message, this);
            return str;
        }

        virtual public string GetRefundSumByTax()
        {
            return "";
        }

        virtual public string GetSalesSumByPayment()
        {
            byte[] data = SendCommand(Command.DayInfo);

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            return string.Format("{0},{1},{2},{3}",
                new Money(int.Parse(GetParameter(data, 0))).ToString("S"), new Money(int.Parse(GetParameter(data, 2))).ToString("S"),
                new Money(int.Parse(GetParameter(data, 1))).ToString("S"), new Money(int.Parse(GetParameter(data, 3))).ToString("S"));
        }
        virtual public string GetRefundSumByPayment()
        {
            return "";
        }

        /// <summary>
		/// Возвращает номер последнего чека.
		/// </summary>
		/// <returns>Номер последнего чека.</returns>
		public int GetLastCheckNumber()
        {
            byte[] data = SendCommand(Command.GetCurrentSums);

            return int.Parse(GetParameter(data, 3));
        }
        /// <summary>
        /// Возвращает номер последнего Z - отчета.
        /// </summary>
        /// <returns>Номер последнего Z - отчета.</returns>
        public virtual int GetLastFiscalClosure()
        {
            byte[] data = SendCommand(Command.DayInfo);

            return int.Parse(GetParameter(data, 4)) - 1;
        }
        public DateTime GetDateTime()
        {
            byte[] data = SendCommand(Command.GetDateTime);

            return DateTime.ParseExact(GetParameter(data, 0), Const.DateTimeFormat, null);
        }
        public void SetDateTime(DateTime dateTime)
        {
            SendCommand(Command.SetDateTime, dateTime.ToString(Const.DateTimeFormat));
        }
        public void ClearDisplay()
        {
            SendCommand(Command.ClearDisplay);
        }
        public void DisplayTextLL(string msg)
        {
            SendCommand(Command.DisplayTextLL, msg);
        }
        public void DisplayTextUL(string msg)
        {
            SendCommand(Command.DisplayTextUL, msg);
        }
        public void SetOperatorName(string name)
        {
            if (name.Length > 23)
                name = name.Substring(0, 23);

            SendCommand(Command.SetOperatorName, string.Format("1,{0},{1}", Const.StdUserPassword, name));
        }

        public bool SessionOpened
        {
            get
            {
                byte[] data = SendCommand(Command.DayInfo);

                return int.Parse(GetParameter(data, 5)) != 0;
            }
        }
        public virtual void PrintXReport()
        {
            SendCommand(Command.FiscalClosure, "3");
        }
        public virtual void PrintZReport()
        {
            Log.Write("PrintXReport Command.FiscalClosure = 0", Log.MessageType.Message, this);
            SendCommand(Command.FiscalClosure, "0");
        }
        public virtual void PrintZReport(DateTime beginDate, DateTime endDate)
        {
            Log.Write("PrintZReport(DateTime beginDate, DateTime endDate) : " + beginDate.ToString("ddMMyy") + "; " + endDate.ToString("ddMMyy"), Log.MessageType.Message, this);
            SendCommand(Command.PrintReportByDate, string.Format("{0},{1}", beginDate.ToString("ddMMyy"), endDate.ToString("ddMMyy")));
        }
        public virtual void PrintZReport(int beginNum, int endNum)
        {
            Log.Write("PrintZReport(int beginNum, int endNum) : " + beginNum + "; " + endNum.ToString("ddMMyy"), Log.MessageType.Message, this);

            byte[] data = SendCommand(Command.PrintReportByNum, string.Format("{0},{1}", beginNum, endNum));

            if (data.Length > 0)
                CheckResponse(data[0]);
        }
        protected void SetHeaderFooter(int num, string msg)
        {
            if (msg.Length > Const.MaxHeaderLenght)
                msg = msg.Substring(0, Const.MaxHeaderLenght);

            ArrayList data = new ArrayList();
            data.Add((byte)(num + 0x30));
            data.AddRange(Encoding.GetEncoding(1251).GetBytes(msg));

            SendCommand(Command.SetHeaderFooter, (byte[])data.ToArray(typeof(byte)));
        }
        public virtual void SetHeaderFooter(string[] header, string[] footer)
        {
            if (header.Length > 0)
            {
                SetHeaderFooter(0, header.Length > 0 ? header[0] : "");
                SetHeaderFooter(1, header.Length > 1 ? header[1] : "");
                SetHeaderFooter(2, header.Length > 2 ? header[2] : "");
                SetHeaderFooter(3, header.Length > 3 ? header[3] : "");
                SetHeaderFooter(4, header.Length > 4 ? header[4] : "");
                SetHeaderFooter(5, header.Length > 5 ? header[5] : "");
            }

            if (footer.Length > 0)
            {
                SetHeaderFooter(6, footer.Length > 0 ? footer[0] : "");
                SetHeaderFooter(7, footer.Length > 1 ? footer[1] : "");
            }
        }
        public virtual void OpenFiscalReceipt(int cashNum)
        {
            SendCommand(Command.OpenFiscalReceipt, string.Format("1,{0},{1}", Const.StdUserPassword, cashNum));
        }
        public void PrintFiscalText(string text)
        {
            if (text.Length > Const.MaxFiscalString)
                text = text.Substring(0, Const.MaxFiscalString);

            SendCommand(Command.PrintFiscalText, text);
        }
        public virtual void Total(Money summa, PaymentType type, string comment)
        {
            if (comment.Length > Const.MaxPaymentString)
                comment = comment.Substring(0, Const.MaxPaymentString);

            char paymentType = DatecsPaymentTypes.Cash;
            switch (type)
            {
                case PaymentType.Card:
                    paymentType = DatecsPaymentTypes.Card;
                    break;
                case PaymentType.Cash:
                    paymentType = DatecsPaymentTypes.Cash;
                    break;
                case PaymentType.Check:
                    paymentType = DatecsPaymentTypes.Check;
                    break;
                case PaymentType.Credit:
                    paymentType = DatecsPaymentTypes.Credit;
                    break;
            }

            string message = string.Format("{0}\t{1}{2}", comment, paymentType, summa.ToString("F"));
            byte[] data = SendCommand(Command.Total, message);

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            if (data[0] == (byte)Flag.ResponseF || data[0] == (byte)Flag.ResponseE)
                CheckResponse(data[0]);
        }
        public virtual void CloseFiscalReceipt()
        {
            SendCommand(Command.CloseFiscalReceipt);
        }
        public void MakeReceiptCopy()
        {
            SendCommand(Command.MakeReceiptCopy, "1");
        }
        public void Cut()
        {
            SendCommand(Command.Cut);
        }
        public void OpenNonfiscalReceipt()
        {
            byte[] data = SendCommand(Command.OpenNonfiscalReceipt);

            if (data.Length > 0)
                CheckResponse(data[0]);
        }
        public void PrintNonfiscalText(string text)
        {
            if (text.Length > Const.MaxNonFiscalString)
                text = text.Substring(0, Const.MaxNonFiscalString);

            SendCommand(Command.PrintNonfiscalText, text);
        }
        public virtual void RegisterItem(string artName, Money unitPrice, Quantity count, TaxGrp taxGrp, bool printOneCount, int artNum, Money discount)
        {
            if (artName.Length > Const.MaxArticleString)
                artName = artName.Substring(0, Const.MaxArticleString);

            string message = string.Format("{0}\t{1}+{2}", artName, GetDatecsTaxName(taxGrp), unitPrice.ToString("F"));

            if (printOneCount || (count.Type != Quantity.QuantityTypes.NoWeight || count.Amount != 1))
                message += "*" + count.ToString("F");
            Log.Write("Pozitia dlea peceati: " + message, Log.MessageType.Message, this);
            SendCommand(Command.RegisterItem, message);
        }
        public void CloseNonfiscalReceipt()
        {
            SendCommand(Command.CloseNonfiscalReceipt);
        }
        public void RefundSums(DatecsSums sums, string text)
        {
            Money retMoney = new Money(0);

            if (sums.TaxA != new Money(0))
                SendCommand(Command.CancelPayment, string.Format("{0}\t{1}-{2}", text, DatecsTaxNames.TaxA, sums.TaxA.ToString("F")));

            if (sums.TaxB != new Money(0))
                SendCommand(Command.CancelPayment, string.Format("{0}\t{1}-{2}", text, DatecsTaxNames.TaxB, sums.TaxB.ToString("F")));

            if (sums.TaxC != new Money(0))
                SendCommand(Command.CancelPayment, string.Format("{0}\t{1}-{2}", text, DatecsTaxNames.TaxC, sums.TaxC.ToString("F")));

            if (sums.TaxD != new Money(0))
                SendCommand(Command.CancelPayment, string.Format("{0}\t{1}-{2}", text, DatecsTaxNames.TaxD, sums.TaxD.ToString("F")));

            if (sums.TaxFree != new Money(0))
                SendCommand(Command.CancelPayment, string.Format("{0}\t{1}-{2}", text, DatecsTaxNames.TaxFree, sums.TaxFree.ToString("F")));
        }
        virtual public void CancelFiscalReceipt()
        {
            Log.Write("CancelFiscalReceipt()", Log.MessageType.Message, this);
            if (!GetStatus().FiscalCheckOpen)
                return;

            byte[] data = SendCommand(Command.GetReceiptInfo);

            DatecsSums sums = new DatecsSums();

            sums.TaxA = new Money(int.Parse(GetParameter(data, 1)));
            sums.TaxB = new Money(int.Parse(GetParameter(data, 2)));
            sums.TaxC = new Money(int.Parse(GetParameter(data, 3)));
            sums.TaxD = new Money(int.Parse(GetParameter(data, 4)));
            sums.TaxFree = new Money(int.Parse(GetParameter(data, 5)));

            RefundSums(sums, DatecsStrings.GetString((int)Message.Cancel));
            Total(new Money(0), PaymentType.Cash, "");
            CloseFiscalReceipt();
        }
        public bool ReceiptPaymentAndNoClose
        {
            get
            {
                if (!GetStatus().FiscalCheckOpen)
                    return false;

                byte[] data = SendCommand(Command.GetReceiptInfo);

                return int.Parse(GetParameter(data, 0)) == 0;
            }
        }
        public void VoidSell(Money summa, TaxGrp taxGrp, string comment)
        {
            if (comment.Length > Const.MaxArticleString)
                comment = comment.Substring(0, Const.MaxArticleString);

            string message = string.Format("{0},{1}\t{2}", GetDatecsTaxName(taxGrp), comment, summa.ToString("F"));
            SendCommand(Command.VoidSell, message);
        }

        virtual public void SetCurrentTaxRates(Hashtable taxes)
        {
            string message = "";
            Percent taxRate;

            if (taxes.Contains(TaxGrp.TaxA))
                taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxA]).Rate;
            else
                taxRate = new Percent(0);
            message += taxRate.ToString("D") + ",";

            if (taxes.Contains(TaxGrp.TaxB))
                taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxB]).Rate;
            else
                taxRate = new Percent(0);
            message += taxRate.ToString("D") + ",";

            if (taxes.Contains(TaxGrp.TaxC))
                taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxC]).Rate;
            else
                taxRate = new Percent(0);
            message += taxRate.ToString("D") + ",";

            if (taxes.Contains(TaxGrp.TaxD))
                taxRate = ((PrinterTaxGrp)taxes[TaxGrp.TaxD]).Rate;
            else
                taxRate = new Percent(0);
            message += taxRate.ToString("D");


            byte[] data = SendCommand(Command.SetTaxRates, message);

            if (data.Length < 1)
                throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.RecivedUninspected));

            CheckResponse(data[0]);
        }
    }
}
