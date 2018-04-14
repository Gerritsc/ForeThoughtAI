using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyNetConsole
{
    class Program
    {
        static int numGames = 1;

        static void Main(string[] args)
        {
            RLBrain squishy = RLBrain.FindSquishy();
            RLBrain.SelfTeach(1000, 10);
            //RLBrain.SelfTeach(2, 10);
            //squishy.Test(2);
            RLBrain.Save();
        }
    }
}