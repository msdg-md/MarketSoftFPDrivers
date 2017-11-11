using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATECS550TLib
{
    public class DatecsFP550TConfig
    {
        public byte port { get; set; }
        public short rate { get; set; }

        public string operatorNmb { get; set; }
        public string operatorPass { get; set; }
        public string casaNmb { get; set; }

    }
}
