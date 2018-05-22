using SoftMarket.Devices.IO;
using SoftMarket.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftMarket.Devices.Printers.Elicom
{
    public class TremolS : Elicom
    {
        private TremolSProtocol tremolSPrinter = new TremolSProtocol();

        public TremolS()
        {
            baudrate = 9600;
        }

        protected override ElicomProtocol Printer
        {
            get
            {
                return tremolSPrinter;
            }
        }

        protected virtual TremolSProtocol TremolSPrinter
        {
            get
            {
                return tremolSPrinter;
            }
        }

        public override string DeviceName
        {
            get
            {
                return "3F8DDC26-80BC-4E55-8A89-F296258A9987";
            }
        }

        public override string DeviceFriendlyName
        {
            get
            {
                return CultureStrings.TremolSFriendlyName;
            }
        }

        public override SoftMarket.Globals.Parameter[] Parameters
        {
            get
            {
                List<SoftMarket.Globals.Parameter> parameters = new List<Parameter>(base.Parameters);

                parameters.Add(new Parameter((int)SettingsParameter.IsStandartPrinting,
                    tremolSPrinter.IsStandartPrinting ? CultureStrings.StandartPrinting : CultureStrings.PrintingAfterClose, 
                    new string[] { CultureStrings.StandartPrinting, CultureStrings.PrintingAfterClose }, 
                    CultureStrings.PrintingType));

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
                        case (int)SettingsParameter.IsStandartPrinting:
                            tremolSPrinter.IsStandartPrinting = parameter.StringValue == CultureStrings.StandartPrinting;
                            break;

                    }
                }
            }
        }

        //protected override void OpenRefundReceipt()
        //{
        //    TremolSPrinter.OpenRefundReceipt();
        //}

        //public override void BeginReceipt(PrinterReceiptType type, int cashNum, int systemRecNum, string comment, string recNum)
        //{
        //    base.BeginReceipt(type, cashNum, systemRecNum, comment, recNum);
        //    checkType = type;
        //}
    }
}
