using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkyNetConsole
{
    [System.Serializable]

    class Program
    {
        static int numGames = 5;
        private static int startBundleNum = 0;

        private static int numRounds = 200;

        private static int numIters = 300;
        static void Main(string[] args)
        {
            RLBrain squishy = RLBrain.FindSquishy();
            //RLBrain.PrintTree(0);
            if (File.Exists("./SkyNetData/BundleNum.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open("./SkyNetData/BundleNum.dat", FileMode.Open);
                startBundleNum = (int)bf.Deserialize(file);
                file.Close();
            }
            for (int i = startBundleNum; i < (startBundleNum + numRounds); i++)
            {
                Directory.CreateDirectory(String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/", i.ToString(), RLBrain.endInd - RLBrain.startInd, numIters.ToString()));
                RLBrain.SelfTeach(numGames, numIters, i);
            }
            BinaryFormatter bf1 = new BinaryFormatter();
            FileStream file1 = File.Create("./SkyNetData/BundleNum.dat");
            bf1.Serialize(file1, startBundleNum);
            file1.Close();
            //RLBrain.SelfTeach(2, 10);
            //squishy.Test(2);
            //RLBrain.Save();
        }
    }
}