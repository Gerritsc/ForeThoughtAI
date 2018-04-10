using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyNetConsole
{
    class Program
    {
        static int numGames = 100;

        static void Main(string[] args)
        {
            RLBrain squishy = RLBrain.FindSquishy();
            squishy.SelfTeach(numGames);
        }
    }
}
