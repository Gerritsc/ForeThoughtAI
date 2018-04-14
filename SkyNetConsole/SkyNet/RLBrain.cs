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

    [System.NonSerialized]
    private static Mutex fileMut = new Mutex();

    [System.NonSerialized]
    private static Mutex rootMut = new Mutex();
    [System.NonSerialized]
    private static RLBrain squishy = new RLBrain();
    [System.NonSerialized]
    private int numPlayouts = 2;

    private IGame constGame = new Game();

    public static readonly int maxMoves = 20000;

    private RLBrain()
    {
    }

    public static RLBrain FindSquishy()
    {
        Load();
        Save();
        return squishy;
    }

    public static void SelfTeach(int numThoughts, int numPlaythroughs)
    {
        for (int i = 0; i < numThoughts; i++)
        {
            object[] objArr = new object[] { numThoughts, i, numPlaythroughs };
            ThreadPool.QueueUserWorkItem(squishy.Test, objArr);
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

        Log(i, "STARTING");
        IGame game = new Game();
        MCTSkyNet squishyThought = new MCTSkyNet(game, numPlaythroughs, 5.0f);
        bool curBoardTerminal = false;
        int moveCnt = 0;
        SkyNetNode gameNode = squishyThought.GetRoot();
        while (!curBoardTerminal)
        {
            gameNode = squishyThought.PickOfficialMove(game, gameNode, numPlaythroughs, i);
            Debug.Assert(gameNode.playerOne == game.isPlayerOneTurn());

            if (gameNode.isTerminal())
            {
                curBoardTerminal = true;
                int winningPlayer = gameNode.playerOne ? 1 : 2;
                Log(i, String.Format("ENDING -- PLAYER {0} WIN", winningPlayer));
                continue;
            }
            MakeMove(game, gameNode.move);
            //Console.Write(gameNode.ToString());
            moveCnt++;
            //Console.WriteLine(moveCnt);
            string winPercent = gameNode.visitCnt == 0 ? "0" : (gameNode.winCnt / gameNode.visitCnt).ToString();
            Console.WriteLine(String.Format("Game: {2} -- Move: {0} -- Win Rate: {1}\n{3}", moveCnt, winPercent, i, gameNode.move.ToString()));
            WriteToFile("./SkyNetData/Game_" + i.ToString() + "_Stats.txt", String.Format("Move: {0} -- Num Iters: {1} -- {2}\n{3}", moveCnt, numPlaythroughs, i, gameNode.move.ToString()));
            if (moveCnt >= maxMoves)
            {
                curBoardTerminal = true;
                Log(i, "GMAE ENDING -- STALEMATE");
                continue;
            }
        }

        updateRootList(squishyThought.GetRoot());
    }

    private void TrainOfThought(object stateInfo)
    {
        IGame game = constGame.CopyGame();
        MCTSkyNet squishyThought = new MCTSkyNet(game, numPlayouts, 5.0f);
        Debug.Assert(game.isPlayerOneTurn() != squishyThought.GetRoot().playerOne);
        bool curBoardTerminal = false;
        int moveCnt = 0;
        SkyNetNode gameNode = squishyThought.GetRoot();
        while (!curBoardTerminal && moveCnt < 100)
        {
            //game.getBoard().PrintBoard();
            //Console.WriteLine(game.isPlayerOneTurn());
            //Console.WriteLine(gameNode.ToString());
            //gameNode = squishyThought.PickOfficialMove(game, gameNode);
            Debug.Assert(gameNode.playerOne == game.isPlayerOneTurn());
            // String[][] bStringArr = game.getBoardAsString(game.getBoard(), game.isPlayerOneTurn());
            // for (int i = 0; i < bStringArr.Length; i++)
            // {
            //    string toPrint = "";
            //    for (int j = 0; j < bStringArr[i].Length; j++)
            //    {
            //        toPrint += bStringArr[i][j];
            //    }
            //    Console.WriteLine(toPrint);
            // }
            if (gameNode.isTerminal())
            {
                curBoardTerminal = true;
                int winningPlayer = gameNode.playerOne ? 1 : 2;
                Console.WriteLine(String.Format("Game Ended. Player {0} wins", winningPlayer));
                continue;
            }

            MakeMove(game, gameNode.move);
            Console.Write(gameNode.ToString());
            moveCnt++;
            Console.WriteLine(moveCnt);
        }
        updateRootList(squishyThought.GetRoot());
    }

    private void updateRootList(SkyNetNode newRoot)
    {
        rootMut.WaitOne();
        int oldInd = skyNetTreeRoots.IndexOf(newRoot);
        if (oldInd >= 0)
        {
            MergeTrees(skyNetTreeRoots[oldInd], newRoot);
        }
        else
        {
            skyNetTreeRoots.Add(newRoot);
        }
        rootMut.ReleaseMutex();
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
                Console.WriteLine("Child Node Error in Merge: NewRoot has child OldRoot does not.");
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

    public static void Save()
    {
        fileMut.WaitOne();
        BinaryFormatter bf = new BinaryFormatter();
        Console.WriteLine("SAVED SQUISHY!!!");
        FileStream file = File.Create("./SkyNetData/rlbrain.dat");
        rootMut.WaitOne();
        bf.Serialize(file, skyNetTreeRoots);
        rootMut.ReleaseMutex();
        file.Close();
        fileMut.ReleaseMutex();
    }
    public static bool Load()
    {
        fileMut.WaitOne();
        if (File.Exists("./SkyNetData/rlbrain.dat"))
        {
            Console.WriteLine("FOUND SQUISHY!!!");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./SkyNetData/rlbrain.dat", FileMode.Open);
            rootMut.WaitOne();
            skyNetTreeRoots = (List<SkyNetNode>)bf.Deserialize(file);
            rootMut.ReleaseMutex();
            file.Close();
            fileMut.ReleaseMutex();
            return true;
        }
        fileMut.ReleaseMutex();
        return false;
    }

    public static void RequestExistingRoot(SkyNetNode root)
    {
        int ind = skyNetTreeRoots.IndexOf(root);
        if (ind >= 0)
        {
            Console.WriteLine("EXISTING ROOT FOUND-------------------------------------------------------------------");
            root = skyNetTreeRoots[ind];
        }
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

    public static void Log(int gameNum, string msg)
    {
        string toPrint = String.Format("Game: {0} -- {1}", gameNum.ToString().PadRight(4), msg);
        Console.WriteLine(toPrint);
        WriteToFile("./SkyNetData/Game_" + gameNum.ToString() + "_Stats.txt", toPrint);
    }
}
