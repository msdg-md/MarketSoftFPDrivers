using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{
    public interface ServicePrinter<P,C>
    {
        P getPrinter();

        void setPrinter(P printer);

        void InitPrinter(C config, out string error);

        void OpenFiscalBon();

        void CloseFiscal();

        void OpenNonFiscalBon();

        void CloseNonFiscalBon();

        void PrintSellItem(string productName, int taxGroup, string price, string qnty, string discount);

        double CalcSum();

        void PerformPay(string sum, int payType);

        void Storno(string productName, int taxGroup, string price, string qnty);

        string SetDateTime(DateTime date);

        DateTime GetDateTime(out String error);


    }
}
