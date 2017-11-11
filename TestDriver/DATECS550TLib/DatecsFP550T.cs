using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATECS550TLib
{
    public class DatecsFP550T : Printer.ServicePrinter<DatecsFP550TProtocol, DatecsFP550TConfig>
    {
        DatecsFP550TProtocol printer;
        DatecsFP550TConfig config;

        public void setPrinter(DatecsFP550TProtocol printer)
        {
            this.printer = printer;
        }

        public DatecsFP550TProtocol getPrinter()
        {
            return printer;
        }

        public void InitPrinter(DatecsFP550TConfig config, out string error)
        {
            error = "";
            try
            {
                printer = new DatecsFP550TProtocol();
                this.config = config;

                printer.InitPrinter();
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
        }

        public void OpenFiscalBon()
        {
            string response = "";
            string pars = String.Format("{0},{1},{2}", config.operatorNmb, config.operatorPass, config.casaNmb);
            bool executed = printer.SendCommand(DatecsFP550Commands.OpenFiscalBon, pars, out response);

            Console.WriteLine("OpenFiscalBon - " + response);
        }

        public void CloseFiscal()
        {
            string response = "";
            bool executed = printer.SendCommand(DatecsFP550Commands.CloseFiscalBon, "", out response);

            Console.WriteLine("CloseFiscal - " + response);
        }

        public void PrintSellItem(string productName, int taxGroup, string price, string qnty, string discount)
        {
            string response = "";
            string discVal = discount.Length > 0 ? ",-" + discount+"%" : "";
            string pars = String.Format("{0}{1}{2}{3}{4}", productName, "\t", "A", price, discVal);
            Console.WriteLine("PrintSellItem - params - " + pars);
            bool executed = printer.SendCommand(DatecsFP550Commands.PrintSellItem, pars, out response);

            Console.WriteLine("PrintSellItem - " + response);
        }

        public double CalcSum()
        {
            string response = "";
            string pars = String.Format("{0}{1}", "1", "0");
            bool executed = printer.SendCommand(DatecsFP550Commands.CalcTotal, pars, out response);

            Console.WriteLine("CalcSum - " + response);
            string result = response.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return double.Parse(result);
        }

        public void PerformPay(string sum, int payType)
        {
            string response = "";
            string pars = String.Format("{0}{1}{2}{3}", "text11", "\t", "P", sum);
            bool executed = printer.SendCommand(DatecsFP550Commands.PerformPay, pars, out response);

            Console.WriteLine("CalcSum - " + response);
            string result = response.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        public void OpenPort()
        {
            printer.OpenPort(config.port, config.rate);
        }

        public void ClosePort()
        {
            printer.ClosePort();
        }
        

        //NOT implemented yet

        public void OpenNonFiscalBon()
        {
            throw new NotImplementedException();
        }

        public void CloseNonFiscalBon()
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(out string error)
        {
            throw new NotImplementedException();
        }

        public string SetDateTime(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void Storno(string productName, int taxGroup, string price, string qnty)
        {
            throw new NotImplementedException();
        }
    }
}
