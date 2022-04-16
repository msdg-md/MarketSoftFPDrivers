using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Devices.Printers.Elicom
{
    public class Packet
    {
        private byte command;
        private byte commandNumber;
        private byte commandErrorByte = 0x30;
        private byte fprErrorByte = 0x30;
        private static Dictionary<byte, string> commandErrorDescription;
        private static Dictionary<byte, string> fprErrorDescription;
        private string data = string.Empty;
        private byte[] bytesData = null;
        private bool isACKPacket;
        private byte[] answerData = new byte[0];

        public byte[] BytesData
        {
            get
            {
                return answerData;
            }
        }

        public string[] StringsData
        {
            get
            {
                return Encoding.GetEncoding(1251).GetString(answerData).Split(';');
            }
        }

        static Packet()
        {
            commandErrorDescription = new Dictionary<byte, string>();
            fprErrorDescription = new Dictionary<byte, string>();

            fprErrorDescription.Add(0x30, CultureStrings.FPRError30);
            fprErrorDescription.Add(0x31, CultureStrings.FPRError31);
            fprErrorDescription.Add(0x32, CultureStrings.FPRError32);
            fprErrorDescription.Add(0x33, CultureStrings.FPRError33);
            fprErrorDescription.Add(0x34, CultureStrings.FPRError34);
            fprErrorDescription.Add(0x35, CultureStrings.FPRError35);
            fprErrorDescription.Add(0x36, CultureStrings.FPRError36);
            fprErrorDescription.Add(0x37, CultureStrings.FPRError37);
            fprErrorDescription.Add(0x38, CultureStrings.FPRError38);
            fprErrorDescription.Add(0x39, CultureStrings.FPRError39);
            fprErrorDescription.Add(0x41, CultureStrings.FPRError41);
            fprErrorDescription.Add(0x3A, CultureStrings.FPRError3A);
            fprErrorDescription.Add(0x42, CultureStrings.FPRError42);
            fprErrorDescription.Add(0x3B, CultureStrings.FPRError3B);
            fprErrorDescription.Add(0x43, CultureStrings.FPRError43);
            fprErrorDescription.Add(0x3C, CultureStrings.FPRError3C);
            fprErrorDescription.Add(0x44, CultureStrings.FPRError44);
            fprErrorDescription.Add(0x3D, CultureStrings.FPRError3D);
            fprErrorDescription.Add(0x45, CultureStrings.FPRError45);
            fprErrorDescription.Add(0x3E, CultureStrings.FPRError3E);
            fprErrorDescription.Add(0x46, CultureStrings.FPRError46);
            fprErrorDescription.Add(0x3F, CultureStrings.FPRError3F);

            commandErrorDescription.Add(0x30, CultureStrings.CommandError30);
            commandErrorDescription.Add(0x31, CultureStrings.CommandError31);
            commandErrorDescription.Add(0x32, CultureStrings.CommandError32);
            commandErrorDescription.Add(0x33, CultureStrings.CommandError33);
            commandErrorDescription.Add(0x34, CultureStrings.CommandError34);
            commandErrorDescription.Add(0x35, CultureStrings.CommandError35);
            commandErrorDescription.Add(0x36, CultureStrings.CommandError36);
            commandErrorDescription.Add(0x37, CultureStrings.CommandError37);
            commandErrorDescription.Add(0x38, CultureStrings.CommandError38);
            commandErrorDescription.Add(0x39, CultureStrings.CommandError39);
        }

        public Packet()
        {
        }

        public Packet(byte command)
            : this(command, string.Empty)
        {
        }

        public Packet(byte command, string packetData)
        {
            this.command = command;
            this.data = packetData == null ? string.Empty : packetData;
        }

        public Packet(byte command, byte[] packetData)
        {
            this.command = command;
            this.bytesData = packetData;
        }

        public byte CommandNumber
        {
            get
            {
                return commandNumber;
            }
            set
            {
                commandNumber = value;
            }
        }

        public bool IsError
        {
            get
            {
                return commandErrorByte != 0x30 || fprErrorByte != 0x30;
            }
        }

        public bool IsSession24HoursException
        {
            get
            {
                return commandErrorByte == 0x32 && fprErrorByte == 0x42;
            }
        }

        public string GetErrorDescription()
        {
            return GetCommandErrorDescription(commandErrorByte) + Environment.NewLine + GetFprErrorByteDescription(fprErrorByte);
        }

        public byte[] PacketData
        {
            get
            {
                return CreatePacketData();
            }
        }

        public byte Command
        {
            get
            {
                return command;
            }
            private set
            {
                command = value;
            }
        }

        public bool IsACKPacket
        {
            get
            {
                return isACKPacket;
            }
            private set
            {
                isACKPacket = value;
            }
        }

        public bool ParseData(byte[] data)
        {
            if (data.Length <= 0)
                return false;

            if (data[0] == Symbols.ACK)
                return ParseACKPacket(data);
            if (data[0] == Symbols.STX)
               return ParseSTXPacket(data);
            
            return false;

        }

        private bool ParseACKPacket(byte[] data)
        {
            IsACKPacket = true;

            if (data == null || data.Length < 7)
                return false;

            if (data[0] != Symbols.ACK)
                return false;

            if (data[6] != Symbols.ETX)
                return false;

            byte[] crc = CalcCRC(data, 1, 3);

            if (data[4] != crc[0] || data[5] != crc[1])
                return false;

            /*
                      if (data[data.Length - 1] != Symbols.ETX)
                          return false;

                      byte[] crc = CalcCRC(data, 1, data.Length - 4);

                      if (data[data.Length - 3] != crc[0] || data[data.Length - 2] != crc[1])
                          return false;
                      */
            CommandNumber = data[1];
            fprErrorByte = data[2];
            commandErrorByte = data[3];

            return true;
        }

        private bool ParseSTXPacket(byte[] data)
        {
            IsACKPacket = false;

            if (data == null || data.Length < 7)
                return false;

            if (data[0] != Symbols.STX)
                return false;

            if (data[data.Length - 1] != Symbols.ETX)
                return false;

            byte[] crc = CalcCRC(data, 1, data.Length - 4);

            if (data[data.Length - 3] != crc[0] || data[data.Length - 2] != crc[1])
                return false;

            int len = data[1] - 0x20;

            if (len != data.Length - 4)
                return false;

            CommandNumber = data[2];
            Command = data[3];

            answerData = new byte[data.Length - 7];
            Array.Copy(data, 4, answerData, 0, data.Length - 7);

            return true;
        }

        private byte[] CreatePacketData()
        {
            List<byte> packetData = new List<byte>();

            packetData.Add(Symbols.STX);
            packetData.Add((byte)(data.Length + 3 + 0x20));
            packetData.Add(CommandNumber);
            packetData.Add(Command);

            if (bytesData == null)
                packetData.AddRange(Encoding.GetEncoding(1251).GetBytes(data));
            else
                packetData.AddRange(bytesData);

            packetData.AddRange(CalcCRC(packetData.ToArray(), 1, packetData.Count - 1));
            packetData.Add(Symbols.ETX);

            return packetData.ToArray();
        }

        private byte[] CalcCRC(byte[] data, int start, int count)
        {
            byte crc = 0;

            for (int pos = 0; pos < count; pos++)
                crc ^= data[start + pos];

            byte[] crcResult = new byte[2];

            crcResult[0] = (byte)(0x30 + ((crc >> 4) & 0x0F));
            crcResult[1] = (byte)(0x30 + ((crc >> 0) & 0x0F));

            return crcResult;
        }

        private string GetCommandErrorDescription(byte err)
        {
            if (commandErrorDescription.ContainsKey(err))
                return commandErrorDescription[err];
            else
                return CultureStrings.UnknownError + err.ToString();
        }

        private string GetFprErrorByteDescription(byte err)
        {
            if (fprErrorDescription.ContainsKey(err))
                return fprErrorDescription[err];
            else
                return CultureStrings.UnknownError + err.ToString();
        }

        public void AddMessage(string msg)
        {
            if (string.IsNullOrEmpty(data))
                data = msg;
            else
                data += ";" + msg;
        }
    }
}
