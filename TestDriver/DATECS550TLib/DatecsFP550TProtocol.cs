using WTDEDITLib;

namespace DATECS550TLib
{
    public class DatecsFP550TProtocol
    {
        FpAtl printer;

        public DatecsFP550TProtocol()
        {
        }

        public void InitPrinter()
        {
            printer = new FpAtlClass();
            printer.TypeModel = FpTypeModel.FpTypeModel_FP550;
            printer.TypeProtocol = FpTypeProtocol.FpTypeProtocol_OLD;
        }

        public void OpenPort(byte port = 1, short rate = 9600)
        {
            printer.OpenPort(911304495, port, port);
        }

        public bool SendCommand(byte command, string parameters, out string response)
        {
            response = "";
            return printer.Send(488839203, command, parameters, out response);
        }

        public void ClosePort()
        {
            printer.ClosePort();
        }
    }

    public static class DatecsFP550Commands
    {
        public readonly static byte OpenNonFiscalBon = 38;
        public readonly static byte CloseNonFiscalBon = 39;
        public readonly static byte PrintNonFiscalText = 42;

        public readonly static byte OpenFiscalBon = 48;
        public readonly static byte CloseFiscalBon = 56;
        public readonly static byte PrintSellItem = 49;
        public readonly static byte CalcTotal = 51;
        public readonly static byte PerformPay = 53;

        public readonly static byte SetDateTime = 61;
        public readonly static byte GetDateTime = 62;
    }
}
