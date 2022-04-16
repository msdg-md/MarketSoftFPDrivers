using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Devices.Printers.Elicom
{
    public class TremolSProtocol : ElicomProtocol
    {
        public TremolSProtocol() : base()
        {
            IsStandartPrinting = false;
        }

        public bool IsStandartPrinting
        {
            get;
            set;
        }

        public override void OpenReceipt()
        {
            Packet packet = new Packet(Commands.OpenReceipt);
            packet.AddMessage("1");
            packet.AddMessage(CashierPassword);
            packet.AddMessage("0"); //  ‘0’ for brief format and ‘1’ for detailed format
            packet.AddMessage("0"); //  ‘0’ for standard receipt or ‘1’ for refund receipt 
            if (IsStandartPrinting) // value ‘0’ or ‘2’ for standard or postponed 
                packet.AddMessage("0");
            else
                packet.AddMessage("2"); 

            SendPacket(packet);
        }

        public void OpenRefundReceipt()
        {
            Packet packet = new Packet(Commands.OpenReceipt);
            packet.AddMessage("1");
            packet.AddMessage(CashierPassword);
            packet.AddMessage("0"); //  ‘0’ for brief format and ‘1’ for detailed format
            packet.AddMessage("1"); //  ‘0’ for standard receipt or ‘1’ for refund receipt
            if (IsStandartPrinting) // value ‘0’ or ‘2’ for standard or postponed 
                packet.AddMessage("0");
            else
                packet.AddMessage("2"); 

            SendPacket(packet);
        }

        public override int TextDocLineLength
        {
            get
            {
                return 30;
            }
        }

        protected override void CheckPrinterStatus()
        {
            comPort.Reset();
            comPort.Write(new byte[] { 0x05 });

            DateTime endTime = DateTime.Now.AddSeconds(timeoutAnswer);

            while (endTime > DateTime.Now)
            {
                byte response;

                if (comPort.Read(out response) && response == 0x05)
                    return;

                comPort.Reset();
                comPort.Write(new byte[] { 0x05 });
            }

            throw new FiscalPrinterException(CultureStrings.ReciveTimeout);
        }
    }
}
