using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class CVRPSolutionInfo
    {
        public string Dateiname { get; set; }
        public string Solver { get; set; }
        public double Kosten { get; set; }
        public int Fahrzeuge { get; set; }
        public double Auslastung { get; set; }
        public double Nachfrage { get; set; }
    }
}
