using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyNetConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RLBrain squishy = RLBrain.FindSquishy();
            squishy.SelfTeach(100);
        }
    }
}
