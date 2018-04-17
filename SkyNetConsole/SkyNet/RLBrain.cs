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

    private static List<IDeck> constDecks = new List<IDeck>();

    [System.NonSerialized]
    private static Mutex fileMut = new Mutex();

    [System.NonSerialized]
    private static Mutex rootMut = new Mutex();
    [System.NonSerialized]
    private static RLBrain squishy = new RLBrain();
    [System.NonSerialized]
    private int numPlayouts = 2;
    [System.NonSerialized]
    private static Mutex deckMut = new Mutex();

    //private IGame constGame = new Game();

    public static readonly int maxMoves = 1000;

    private static List<int> usedDeckInd = new List<int>();

    public static int startInd = 29;
    public static int endInd = 34;

    private RLBrain()
    {
        // Console.WriteLine("CREATING ROOTS");
        // List<ICard> allICards = new PlayingCardDeck().getDeck();
        // List<List<ICard>> allstartHands = new List<List<ICard>>((int)Math.Pow(allICards.Count, 5));
        // int[] indexes = new int[] { 0, 0, 0, 0, 0 };
        // Debug.Assert(allICards.Count == 52);
        // while (indexes[4] < Math.Pow(allICards.Count, 5))
        // {
        //     List<ICard> toAdd = new List<ICard>();
        //     toAdd.Add(allICards[indexes[0]]);
        //     toAdd.Add(allICards[indexes[1]]);
        //     toAdd.Add(allICards[indexes[2]]);
        //     toAdd.Add(allICards[indexes[3]]);
        //     toAdd.Add(allICards[indexes[4]]);
        //     allstartHands.Add(toAdd);
        //     indexes[4] = (indexes[4] + 1) % 52;
        //     if (indexes[4] % 52 == 0)
        //     {
        //         indexes[3] = (indexes[3] + 1) % 52;
        //     }
        //     if (indexes[3] % 52 == 0)
        //     {
        //         indexes[2] = (indexes[2] + 1) % 52;
        //     }
        //     if (indexes[2] % 52 == 0)
        //     {
        //         indexes[1] = (indexes[1] + 1) % 52;
        //     }
        //     if (indexes[1] % 52 == 0)
        //     {
        //         indexes[0] = (indexes[0] + 1) % 52;
        //     }
        // }
        // allICards.Clear();
        // allICards = null;
        // List<string> allBoards = new List<string>();
        // List<string> allHands = new List<string>();
        // foreach (List<ICard> board in allstartHands)
        // {
        //     allBoards.Add((new GameBoard(board.ToArray()).getBoardAsString(false)));
        //     string tmp = "";
        //     foreach (ICard c in board)
        //     {
        //         tmp += c.getFullCard();
        //     }
        //     allHands.Add(tmp);
        // }
        // allstartHands = null;
        // int count = 0;
        // List<SkyNetNode> roots = new List<SkyNetNode>();
        // foreach (string b in allBoards)
        // {
        //     foreach (string h in allHands)
        //     {
        //         roots.Add(new SkyNetNode(b, false, h));
        //         count++;
        //         if (count % 52 == 0)
        //         {
        //             if (File.Exists("./SkyNetData/roots_" + ((int)(count / 52)).ToString() + ".dat"))
        //             {
        //                 BinaryFormatter bf = new BinaryFormatter();
        //                 Console.WriteLine("SAVED SQUISHY!!!");
        //                 FileStream file = File.Create("./SkyNetData/roots_" + ((int)(count / 52)).ToString() + ".dat");
        //                 bf.Serialize(file, roots);
        //                 file.Close();

        //             }
        //             roots = null;
        //             roots = new List<SkyNetNode>();
        //         }
        //     }
        // }
        // //Debug.Assert(skyNetTreeRoots.Count == Math.Pow(allICards.Count, 5));
        // Console.WriteLine("DONE ROOTS");
    }

    public static RLBrain FindSquishy()
    {
        Load();
        return squishy;
    }

    public static void SelfTeach(int numGames, int numPlaythroughs, int bundleNum)
    {
        usedDeckInd = new List<int>();
        for (int i = startInd; i < endInd; i++)
        {
            // deckMut.WaitOne();
            // Random rand = new Random();
            // int deckInd = rand.Next(0, constDecks.Count);
            // while (usedDeckInd.Contains(deckInd))
            // {
            //     deckInd = rand.Next(0, constDecks.Count);
            // }
            // usedDeckInd.Add(i);
            // deckMut.ReleaseMutex();
            object[] objArr = new object[] { endInd - startInd, i, numPlaythroughs, bundleNum };
            ThreadPool.QueueUserWorkItem(squishy.Test, objArr);
            //squishy.Test(objArr);
        }

        int numAvail = 0;
        int other1 = 0;
        int maxThreads = 0;
        int other2 = 0;
        int numRunning = 0;

        do
        {
            ThreadPool.GetAvailableThreads(out numAvail, out other1);
            ThreadPool.GetMaxThreads(out maxThreads, out other2);
            numRunning = (maxThreads - numAvail) + (other2 - other1);
        } while (numRunning > 0);
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

            if (picked.isTerminal())
            {
                curBoardTerminal = true;
                int winningPlayer = picked.playerOne ? 1 : 2;
                Log(i, String.Format("ENDING -- PLAYER {0} WIN", winningPlayer), bundleNum, numGames, numPlaythroughs);
                continue;
            }
            Debug.Assert(picked.playerOne == game.isPlayerOneTurn());
            MakeMove(game, picked.move);
            //Console.Write(gameNode.ToString());
            moveCnt++;
            //Console.WriteLine(moveCnt);
            float winRate = picked.visitCnt == 0 ? 0 : (picked.winCnt / picked.visitCnt);
            string winPercent = winRate.ToString();
            //Log(i, String.Format("Move: {0}", moveCnt), bundleNum, numGames, numPlaythroughs);
            Log(i, String.Format("Move: {0} -- Win Rate: {1}", moveCnt, winPercent), bundleNum, numGames, numPlaythroughs);
            if (moveCnt >= maxMoves)
            {
                curBoardTerminal = true;
                Log(i, "GAME ENDING -- STALEMATE", bundleNum, numGames, numPlaythroughs);
                continue;
            }
            //updateRootList(squishyThought.GetRoot());
            gameNode = picked;
        }

        //updateRootList(squishyThought.GetRoot());
        Save(squishyThought.GetRoot(), bundleNum, numGames, numPlaythroughs);
    }

    // private void TrainOfThought(object stateInfo)
    // {
    //     IGame game = constGame.CopyGame();
    //     MCTSkyNet squishyThought = new MCTSkyNet(game, numPlayouts, 5.0f);
    //     Debug.Assert(game.isPlayerOneTurn() != squishyThought.GetRoot().playerOne);
    //     bool curBoardTerminal = false;
    //     int moveCnt = 0;
    //     SkyNetNode gameNode = squishyThought.GetRoot();
    //     while (!curBoardTerminal && moveCnt < 100)
    //     {
    //         //game.getBoard().PrintBoard();
    //         //Console.WriteLine(game.isPlayerOneTurn());
    //         //Console.WriteLine(gameNode.ToString());
    //         //gameNode = squishyThought.PickOfficialMove(game, gameNode);
    //         Debug.Assert(gameNode.playerOne == game.isPlayerOneTurn());
    //         // String[][] bStringArr = game.getBoardAsString(game.getBoard(), game.isPlayerOneTurn());
    //         // for (int i = 0; i < bStringArr.Length; i++)
    //         // {
    //         //    string toPrint = "";
    //         //    for (int j = 0; j < bStringArr[i].Length; j++)
    //         //    {
    //         //        toPrint += bStringArr[i][j];
    //         //    }
    //         //    Console.WriteLine(toPrint);
    //         // }
    //         if (gameNode.isTerminal())
    //         {
    //             curBoardTerminal = true;
    //             int winningPlayer = gameNode.playerOne ? 1 : 2;
    //             Console.WriteLine(String.Format("Game Ended. Player {0} wins", winningPlayer));
    //             continue;
    //         }

    //         MakeMove(game, gameNode.move);
    //         Console.Write(gameNode.ToString());
    //         moveCnt++;
    //         Console.WriteLine(moveCnt);
    //     }
    //     updateRootList(squishyThought.GetRoot());
    // }

    // private void updateRootList(SkyNetNode newRoot)
    // {
    //     rootMut.WaitOne();
    //     int oldInd = skyNetTreeRoots.IndexOf(newRoot);
    //     if (oldInd >= 0)
    //     {
    //         MergeTrees(skyNetTreeRoots[oldInd], newRoot);
    //     }
    //     else
    //     {
    //         skyNetTreeRoots.Add(newRoot);
    //         int cnt = RootMap.Count;
    //         RootMap.Add(newRoot.boardHash + newRoot.hand, cnt);
    //     }
    //     rootMut.ReleaseMutex();
    // }

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
        fileMut.WaitOne();
        rootMut.WaitOne();
        if (RootMap.ContainsKey(toSave.boardHash + toSave.hand))
        {
            int key = RootMap[toSave.boardHash + toSave.hand];
            BinaryFormatter bf1 = new BinaryFormatter();
            FileStream file1 = File.Create("./SkyNetData/Root_" + key.ToString() + ".dat");
            FileStream file2 = File.Create(String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/Root_{3}.dat", bundleNum.ToString(), numGames.ToString(), numIters.ToString(), key.ToString()));

            bf1.Serialize(file1, toSave);
            bf1.Serialize(file2, toSave);
            file1.Close();
            file2.Close();
        }
        Console.WriteLine("SAVED SQUISHY!!!");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("./SkyNetData/RootMap.dat");
        FileStream file0 = File.Create(String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/RootMap.dat", bundleNum.ToString(), numGames.ToString(), numIters.ToString()));
        bf.Serialize(file, RootMap);
        bf.Serialize(file0, RootMap);
        rootMut.ReleaseMutex();
        file.Close();
        file0.Close();
        fileMut.ReleaseMutex();
    }
    public static bool Load()
    {
        fileMut.WaitOne();
        if (!File.Exists("./SkyNetData/ConstDecks.dat"))
        {
            InitDecks();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./SkyNetData/ConstDecks.dat", FileMode.Open);
            constDecks = (List<IDeck>)bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists("./SkyNetData/RootMap.dat"))
        {
            Console.WriteLine("FOUND SQUISHY!!!");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./SkyNetData/RootMap.dat", FileMode.Open);
            rootMut.WaitOne();
            RootMap = (Dictionary<string, int>)bf.Deserialize(file);
            rootMut.ReleaseMutex();
            file.Close();
            fileMut.ReleaseMutex();
            return true;
        }
        fileMut.ReleaseMutex();
        return false;
    }

    public static bool RequestExistingRoot(ref SkyNetNode root, int gameNum, int bundleNum, int numGames, int numIters)
    {
        string hashKey = root.boardHash + root.hand;
        rootMut.WaitOne();
        if (!RootMap.ContainsKey(hashKey))
        {
            RootMap.Add(hashKey, gameNum);

        }
        int key = RootMap[hashKey];
        if (File.Exists("./SkyNetData/Root_" + key.ToString() + ".dat"))
        {
            fileMut.WaitOne();
            Log(gameNum, "LOADED ROOT", bundleNum, numGames, numIters);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./SkyNetData/Root_" + key.ToString() + ".dat", FileMode.Open);
            root = (SkyNetNode)bf.Deserialize(file);
            Debug.Assert(hashKey.Equals(root.boardHash + root.hand));
            file.Close();
            fileMut.ReleaseMutex();
            rootMut.ReleaseMutex();
            return true;
        }
        //Save(root, bundleNum, numGames, numIters);
        rootMut.ReleaseMutex();
        return false;
    }

    public static void PrintTree(int treeNum)
    {
        rootMut.WaitOne();
        foreach (SkyNetNode n in skyNetTreeRoots)
        {
            string filestr = "./SkyNetData/Tree" + treeNum.ToString() + ".txt";
            WriteToFile(filestr, n.ToString());
            PrintTreeHelper(n, 0, filestr);
        }
        rootMut.ReleaseMutex();
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
        string toPrint = String.Format("Game: {0} -- {1}", gameNum.ToString().PadRight(4), msg);
        Console.WriteLine(toPrint);
        //WriteToFile("./SkyNetData/Game_" + gameNum.ToString() + "_Stats.txt", toPrint + "\n");
        //string file = String.Format("./SkyNetData/Root_Bundle_{0}_{1}G_{2}I/Game_{3}_Stats.txt", bundleNum.ToString(), numGames.ToString(), numIters.ToString(), gameNum.ToString());
        //WriteToFile(file, toPrint + "\n");
    }

    private static void InitDecks()
    {
        fileMut.WaitOne();
        for (int i = 0; i < 101; i++)
        {
            IDeck toAdd = new PlayingCardDeck();
            toAdd.ShuffleDeck();
            constDecks.Add(toAdd);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("./SkyNetData/ConstDecks.dat");
        bf.Serialize(file, constDecks);
        file.Close();
        fileMut.ReleaseMutex();
    }
}
