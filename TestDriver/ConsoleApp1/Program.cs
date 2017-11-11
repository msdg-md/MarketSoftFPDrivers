using DATECS550TLib;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DatecsFP550TConfig config = new DatecsFP550TConfig();
            config.casaNmb = "1";
            config.operatorNmb = "1";
            config.operatorPass= "0000";
            config.port = 1;
            config.rate = 19200;

            DatecsFP550T prn = new DatecsFP550T();
            string errorText = "";
            prn.InitPrinter(config, out errorText);
            Console.WriteLine("Main - " + errorText);

            prn.OpenPort();

            prn.OpenFiscalBon();

            prn.PrintSellItem("placinte", 1, "10.5", "2", "");

            double sum = prn.CalcSum();

            prn.PerformPay("10.5", 1);

            prn.CloseFiscal();

            prn.ClosePort();
        }
    }
}
