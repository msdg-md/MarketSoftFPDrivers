using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Printer
    {
        //WTDEDITLib.FpAtl printer;

        public Printer()
        {
            //zfp = new ZekaFPClass();
            //printer = new WTDEDITLib.FpAtlClass();
        }

        public void command()
        {
            try
            {
                //printer = new WTDEDITLib.FpAtlClass();
                //printer.TypeModel = WTDEDITLib.FpTypeModel.FpTypeModel_FP550;
                //printer.TypeProtocol = WTDEDITLib.FpTypeProtocol.FpTypeProtocol_OLD;

                //printer.OpenPort(911304495, 1, 9600);
                //String outData = "";
                //bool result;
                //result = printer.Send(488839203, 38, "", out outData);
                //Console.WriteLine(result);
                //Console.WriteLine(outData);

                //result = printer.Send(488839203, 38, "", out outData);
                //Console.WriteLine(result);
                //Console.WriteLine(outData);

                //result = printer.Send(488839203, 42, "12345678901234567890", out outData);
                //Console.WriteLine(result);
                //Console.WriteLine(outData);

                //result = printer.Send(488839203, 39, "", out outData);
                //Console.WriteLine(result);
                //Console.WriteLine(outData);
                ////printer.Send(488839203, 71, "", out outData);
                ////printer.Send(488839203,44,"20", out outData);
                ////printer.Send(488839203, 35, "12345678901234567890", out outData);
                ////printer.Send(488839203, 63, "", out outData);
                ////printer.Send(488839203, 45,"", out outData);
                //Console.WriteLine(outData);
                //printer.ClosePort();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

    }
}
