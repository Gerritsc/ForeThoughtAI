using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

[System.Serializable]
public class RLBrain
{
    //I will name you Squishy, and you will be mine! My Squishy!

    private static List<SkyNetNode> skyNetTreeRoots = new List<SkyNetNode>();

    private static Dictionary<string, int> RootMap = new Dictionary<string, int>();

    public static List<IDeck> constDecks = new List<IDeck>();

 //   [System.NonSerialized]
    //private static Mutex fileMut = new Mutex();

 //   [System.NonSerialized]
    //private static Mutex rootMut = new Mutex();
    [System.NonSerialized]
    private static RLBrain squishy = new RLBrain();
    [System.NonSerialized]
    private int numPlayouts = 2;
  //  [System.NonSerialized]
    //private static Mutex deckMut = new Mutex();

    //private IGame constGame = new Game();

    public static readonly int maxMoves = 1000;

    private static List<int> usedDeckInd = new List<int>();

    public static int startInd = 29;
    public static int endInd = 30;

    private RLBrain()
    {
        
    }

    public static RLBrain FindSquishy()
    {
        Load();
        return squishy;
    }

    public static void SelfTeach(int numGames, int numPlaythroughs, int bundleNum)
    {
        //usedDeckInd = new List<int>();
        //for (int i = startInd; i < endInd; i++)
        //{
            // deckMut.WaitOne();
            // Random rand = new Random();
            // int deckInd = rand.Next(0, constDecks.Count);
            // while (usedDeckInd.Contains(deckInd))
            // {
            //     deckInd = rand.Next(0, constDecks.Count);
            // }
            // usedDeckInd.Add(i);
            // deckMut.ReleaseMutex();
            object[] objArr = new object[] {numGames, startInd, numPlaythroughs, bundleNum };
            //ThreadPool.QueueUserWorkItem(squishy.Test, objArr);
            squishy.Test(objArr);
        //}

        // int numAvail = 0;
        // int other1 = 0;
        // int maxThreads = 0;
        // int other2 = 0;
        // int numRunning = 0;

        // do
        // {
        //     ThreadPool.GetAvailableThreads(out numAvail, out other1);
        //     ThreadPool.GetMaxThreads(out maxThreads, out other2);
        //     numRunning = (maxThreads - numAvail) + (other2 - other1);
        // } while (numRunning > 0);
    }

    public void Test(object obj)
    {
        object[] objArr = obj as object[];
        int numGames = (int)objArr[0];
        int i = (int)objArr[1];
        int numPlaythroughs = (int)objArr[2];
        int bundleNum = (int)objArr[3];
        Log(i, "STARTING", bundleNum, numGames, numPlaythroughs);

        IGame game = new Game(constDecks[i]);

        MCTSkyNet squishyThought = new MCTSkyNet(game, numPlaythroughs, 5.0f, i, bundleNum, numGames);
        bool curBoardTerminal = false;
        int moveCnt = 0;
        SkyNetNode gameNode = squishyThought.GetRoot();
        while (!curBoardTerminal)
        {
            SkyNetNode picked = squishyThought.PickOfficialMove(game, gameNode, numPlaythroughs, i, moveCnt);

        
           // Debug.Assert(picked.isTerminal() || picked.playerOne == game.isPlayerOneTurn());
            MakeMove(game, picked.move);
            //Console.Write(gameNode.ToString());
            moveCnt++;
            //Console.WriteLine(moveCnt);
            float winRate = (picked.winCnt / picked.visitCnt);
            //Log(i, String.Format("Move: {0}", moveCnt), bundleNum, numGames, numPlaythroughs);
            Log(i, String.Format("Move: {0} -- Win Rate: {1}", moveCnt, winRate.ToString()), bundleNum, numGames, numPlaythroughs);
            if (picked.isTerminal())
            {
                curBoardTerminal = true;
                int winningPlayer = picked.playerOne ? 1 : 2;
                Log(i, String.Format("ENDING -- PLAYER {0} WIN -- MOVES {1}", winningPlayer, moveCnt), bundleNum, numGames, numPlaythroughs);
                continue;
            }
            if (moveCnt >= maxMoves)
            {
                curBoardTerminal = true;
                Log(i, "GAME ENDING -- STALEMATE -- MOVES " + moveCnt.ToString(), bundleNum, numGames, numPlaythroughs);
                continue;
            }
            //updateRootList(squishyThought.GetRoot());
            gameNode = picked;
        }

        //updateRootList(squishyThought.GetRoot());
        Save(squishyThought.GetRoot(), bundleNum, numGames, numPlaythroughs);
    }
		

    private void MergeTrees(SkyNetNode oldRoot, SkyNetNode newRoot)
    {
        oldRoot.visitCnt += newRoot.visitCnt;
        oldRoot.winCnt += newRoot.winCnt;
        foreach (SkyNetNode newchild in newRoot.children)
        {
            int existInd = oldRoot.children.IndexOf(newchild);
            if (existInd != -1)
            {
                MergeTrees(oldRoot.children[existInd], newchild);
            }
            else
            {
                oldRoot.children.Add(newchild);
            }
        }
    }



    private void MakeMove(IGame game, GameMove move)
    {
        int playerInt = game.isPlayerOneTurn() ? 0 : 1;
        switch (move.type)
        {
            case MoveType.ADD:
                {
                    game.PlayCard(playerInt, move.x1, move.y1, move.card);
                    break;
                }
            case MoveType.SWAP:
                {
                    game.SwapCards(playerInt, move.x1, move.y1, move.x2, move.y2);
                    break;
                }
            case MoveType.REMOVE:
                {
                    game.RemoveCard(playerInt, move.x1, move.y1);
                    break;
                }
            case MoveType.PEEK:
                {
                    game.addPeekToKnown(move.x1, move.x2);
                    break;
                }
        }
    }

    public static void Save(SkyNetNode toSave, int bundleNum, int numGames, int numIters)
    {
        //fileMut.WaitOne();
        //rootMut.WaitOne();
        if (RootMap.ContainsKey(toSave.boardHash + toSave.hand))
        {
            int key = RootMap[toSave.boardHash + toSave.hand];
            BinaryFormatter bf1 = new BinaryFormatter();
            BinaryFormatter bf2 = new BinaryFormatter();
			FileStream file1 = File.Create("./SkyNetConsole/SkyNetData/Root_" + key.ToString() + ".dat");
			FileStream file2 = File.Create(String.Format("./SkyNetConsole/SkyNetData/Root_Bundle_{0}_{1}G_{2}I/Root_{3}.dat", bundleNum.ToString(), numGames.ToString(), numIters.ToString(), key.ToString()));

            bf1.Serialize(file1, toSave);
            bf2.Serialize(file2, toSave);
            file1.Close();
            file2.Close();
        }
        Console.WriteLine("SAVED SQUISHY!!!");
        BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create("./SkyNetConsole/SkyNetData/RootMap.dat");
		FileStream file0 = File.Create(String.Format("./SkyNetConsole/SkyNetData/Root_Bundle_{0}_{1}G_{2}I/RootMap.dat", bundleNum.ToString(), numGames.ToString(), numIters.ToString()));
        bf.Serialize(file, RootMap);
        bf.Serialize(file0, RootMap);
        //rootMut.ReleaseMutex();
        file.Close();
        file0.Close();
        //fileMut.ReleaseMutex();
    }
    public static bool Load()
    {
        //fileMut.WaitOne();
		if (!File.Exists("./SkyNetConsole/SkyNetData/ConstDecks.dat"))
        {
            InitDecks();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./SkyNetConsole/SkyNetData/ConstDecks.dat", FileMode.Open);
            constDecks = (List<IDeck>)bf.Deserialize(file);
            file.Close();
        }
		if (File.Exists("./SkyNetConsole/SkyNetData/RootMap.dat"))
        {
            Console.WriteLine("FOUND SQUISHY!!!");
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open("./SkyNetConsole/SkyNetData/RootMap.dat", FileMode.Open);
            //rootMut.WaitOne();
            RootMap = (Dictionary<string, int>)bf.Deserialize(file);
            //rootMut.ReleaseMutex();
            file.Close();
            //fileMut.ReleaseMutex();
            return true;
        }
        //fileMut.ReleaseMutex();
        return false;
    }

    public static bool RequestExistingRoot(ref SkyNetNode root, int gameNum, int bundleNum, int numGames, int numIters)
    {
        string hashKey = root.boardHash + root.hand;
        //rootMut.WaitOne();
        if (!RootMap.ContainsKey(hashKey))
        {
            RootMap.Add(hashKey, gameNum);

        }
        int key = RootMap[hashKey];
		if (File.Exists("./SkyNetConsole/SkyNetData/Root_" + key.ToString() + ".dat"))
        {
            //fileMut.WaitOne();
            Log(gameNum, "LOADED ROOT", bundleNum, numGames, numIters);
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open("./SkyNetConsole/SkyNetData/Root_" + key.ToString() + ".dat", FileMode.Open);
            root = (SkyNetNode)bf.Deserialize(file);
            Debug.Assert(hashKey.Equals(root.boardHash + root.hand));
            file.Close();
            //fileMut.ReleaseMutex();
            //rootMut.ReleaseMutex();
            return true;
        }
        //Save(root, bundleNum, numGames, numIters);
        //rootMut.ReleaseMutex();
        return false;
    }

    public static void PrintTree(int treeNum)
    {
        //rootMut.WaitOne();
        foreach (SkyNetNode n in skyNetTreeRoots)
        {
			string filestr = "./SkyNetConsole/SkyNetData/Tree" + treeNum.ToString() + ".txt";
            WriteToFile(filestr, n.ToString());
            PrintTreeHelper(n, 0, filestr);
        }
        //rootMut.ReleaseMutex();
    }

    private static void PrintTreeHelper(SkyNetNode curNode, int lev, string filestr)
    {
        if (curNode.winCnt / curNode.visitCnt <= 1.0f)
        {
            WriteToFile(filestr, "\n Level: " + lev.ToString() + "\n" + curNode.ToString());
        }
        int nxtlev = lev + 1;
        foreach (SkyNetNode child in curNode.children)
        {
            PrintTreeHelper(child, nxtlev, filestr);
        }
    }

    private static void WriteToFile(string filestr, string text)
    {
        //fileMut.WaitOne();
        if (!System.IO.File.Exists(filestr))
        {
            System.IO.File.WriteAllText(filestr, text);
        }
        else
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filestr, true))
            {
                file.WriteLine(text);
            }
        }
        //fileMut.ReleaseMutex();
    }

    public static void Log(int gameNum, string msg, int bundleNum, int numGames, int numIters)
    {
        //string toPrint = String.Format("Game: {0} -- {1}", gameNum.ToString().PadRight(4), msg);
        //Console.WriteLine(toPrint);
        //WriteToFile("./SkyNetData/Game_" + gameNum.ToString() + "_Stats.txt", toPrint + "\n");
        //string file = String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/Game_{3}_Stats.txt", bundleNum.ToString(), numGames.ToString(), numIters.ToString(), gameNum.ToString());
        //WriteToFile(file, toPrint + "\n");
    }

    private static void InitDecks()
    {
        //fileMut.WaitOne();
        for (int i = 0; i < 101; i++)
        {
            IDeck toAdd = new PlayingCardDeck();
            toAdd.ShuffleDeck();
            constDecks.Add(toAdd);
        }
        BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create("./SkyNetConsole/SkyNetData/ConstDecks.dat");
        bf.Serialize(file, constDecks);
        file.Close();
        //fileMut.ReleaseMutex();
    }
}
