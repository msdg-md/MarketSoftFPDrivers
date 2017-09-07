using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Devices.Printers.Elicom
{
    internal static class Consts
    {
        public static int TextDocLineLength
        {
            get
            {
                return 46;
            }
        }

        public static int DisplayTextLength
        {
            get
            {
                return 20;
            }
        }

        public static string CashierPassword
        {
            get
            {
                return "0000";
            }
        }
    }
}
