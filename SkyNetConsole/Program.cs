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
        static int numGames = 50;
        private static int startBundleNum = 1;

        private static int numRounds = 100;

        private static int numIters = 100;
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
                Directory.CreateDirectory(String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/", i.ToString(), 101, numIters.ToString()));
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