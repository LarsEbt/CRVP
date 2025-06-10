using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public interface ISolver
    {
        CVRPInstance Instance { get; set; }

        public CVRPSolution Solve(); 
       
    }
}