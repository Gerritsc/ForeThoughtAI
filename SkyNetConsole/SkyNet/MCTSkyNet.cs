using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;

[System.Serializable]
public class SkyNetNode
{
    public SkyNetNode parent;
    public GameMove move;
    public bool playerOne;
    public float visitCnt;
    public float winCnt;
    public bool terminal;
    public string boardHash;
    public string hand;
    public List<SkyNetNode> children;
    public List<ICard> winningHand;
    public HANDTYPE winningType;

    public int level;

    public SkyNetNode(string boardHash, bool playerOne, string hand)
    {
        this.move = null;
        this.parent = null;
        this.children = new List<SkyNetNode>();
        this.playerOne = playerOne;
        this.boardHash = boardHash;
        this.hand = hand;
        this.terminal = false;
        this.winCnt = 1;
        this.visitCnt = 1;
        this.level = 0;
    }

    public SkyNetNode(GameMove move, SkyNetNode prevNode, bool playerOne, string boardHash, string hand)
    {
        this.move = move;
        this.parent = prevNode;
        this.children = new List<SkyNetNode>();
        this.playerOne = playerOne;
        this.terminal = false;
        this.boardHash = boardHash;
        this.hand = hand;
        this.winCnt = 1;
        this.visitCnt = 1;
        this.level = parent.level + 1;
    }

    public void incWinAndVisit(bool playerOne, bool stalemate)
    {
        visitCnt++;
        if (this.playerOne == playerOne && !stalemate)
        {
            winCnt++;
        }
        //if()
        //Console.Write(this.ToString());
    }

    public void setTerminalFlag(bool term, List<ICard> hand, HANDTYPE type)
    {
        this.terminal = term;
        this.winningHand = hand;
        this.winningType = type;
    }

    public bool isTerminal()
    {
        return terminal;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || (this.GetType() != obj.GetType()))
        {
            return false;
        }
        if (obj == this)
        {
            return true;
        }
        bool bothMove = (this.move != null) && (((SkyNetNode)obj).move != null);
        bool bothNotMove = (this.move == null) && (((SkyNetNode)obj).move == null);
        bool equalNoParOrMov = this.terminal.Equals(((SkyNetNode)obj).terminal) &&
                                this.boardHash.Equals(((SkyNetNode)obj).boardHash) &&
                                this.hand.Equals(((SkyNetNode)obj).hand) &&
                                this.playerOne.Equals(((SkyNetNode)obj).playerOne);
        bool bothAreBatman = (this.parent == null) && (((SkyNetNode)obj).parent == null);
        if ((bothMove || bothNotMove) && equalNoParOrMov)
        {
            bool parents = bothAreBatman ? true : ((SkyNetNode)obj).parent.Equals(this.parent);
            bool moveGood = bothMove ? ((SkyNetNode)obj).move.Equals(this.move) : bothNotMove;
            return parents && moveGood;
        }
        return false;
    }

    public override string ToString()
    {
        string winPercent = visitCnt == 0 ? "0" : (winCnt / visitCnt).ToString();
        string movStr = move != null ? move.ToString() : "";
        string dispStr = String.Format("\n[SkyNetNode]\nMove: {0}\nWin Percent: {1}\nNum Children: {2}\nPlayer One: {3}\nTerminal: {4}\nVisit: {5}\nWin: {6}\nLevel: {7}\nHash: {8}", movStr, winPercent, children.Count.ToString(), this.playerOne.ToString(), this.terminal.ToString(), visitCnt, winCnt, level, boardHash);
        return dispStr;
    }
}

public class MCTSkyNet
{

    SkyNetNode rootNode;
    //ZobristKiller hashAndSlasher;

    float maxWait;

    static Mutex rootMut = new Mutex();
    private bool printStates = false;

    public MCTSkyNet(IGame game, int numIters, float maxWait, int gameNum, int bundleNum, int numGames)
    {
        this.maxWait = maxWait;
        //hashAndSlasher = ZobristKiller.GetTheKiller();
        string localBString = game.getBoardAsString(game.getBoard(), !game.isPlayerOneTurn());
        rootNode = new SkyNetNode(localBString, !game.isPlayerOneTurn(), game.getHandAsString(!game.isPlayerOneTurn()));
        RLBrain.RequestExistingRoot(ref rootNode, gameNum, bundleNum, numGames, numIters);
    }

    public void SetRoot(SkyNetNode newRoot)
    {
        this.rootNode = newRoot;
    }

    private void DebugPrint(string msg)
    {
        if (printStates)
        {
            Console.Write(msg);
        }
    }

    public SkyNetNode PickOfficialMove(IGame game, SkyNetNode gameNode, int numIters, int gameNum)
    {
        //DateTime curTime = DateTime.UtcNow;
        //DateTime endTime = DateTime.UtcNow.AddSeconds(maxWait);
        //for(int i = 0; i < numIters && DateTime.Compare(curTime, endTime) <= 0; i++)
        for (int i = 0; i < numIters; i++)
        {
            //RLBrain.Log(gameNum, String.Format("ITERATION {0}/{1}", i + 1, numIters));
            object[] objArr = new object[] { game.CopyGame(), gameNode, i };
            ///ThreadPool.QueueUserWorkItem(MCTSSingleIterationThreaded, objArr);
            MCTSSingleIteration(game.CopyGame(), gameNode, i);
            //curTime = DateTime.UtcNow;
        }

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
        //     //Console.WriteLine(String.Format("{0} RUNNING -- {1} AVAILABLE", numRunning, numAvail));
        // } while (numRunning > 0);

        SkyNetNode chosen = MCTSSelect(gameNode, -1);
        return chosen;
    }

    public void MCTSSingleIterationThreaded(object obj)
    {
        object[] objArr = obj as object[];
        IGame curGame = (IGame)objArr[0];
        SkyNetNode gameNode = (SkyNetNode)objArr[1];
        int iterNum = (int)objArr[2];
        //Console.WriteLine(String.Format("ENTERING ITERATION -- {0}", iterNum));
        MCTSExpand(curGame, gameNode, iterNum);
        SkyNetNode pickedChild = MCTSSelect(gameNode, iterNum);
        bool stalemate = false;
        bool playerOneWin = MCTSRandSimPlayout(curGame, pickedChild, ref stalemate, iterNum);
        MCTSBackpropWin(pickedChild, playerOneWin, stalemate, iterNum);
        //RLBrain.Save(this.GetRoot());
        //Console.WriteLine(String.Format("EXITING ITERATION -- {0}", iterNum));
    }

    public void MCTSSingleIteration(IGame curGame, SkyNetNode gameNode, int iterNum)
    {

        //Console.WriteLine(String.Format("ENTERING ITERATION -- {0}", iterNum));
        MCTSExpand(curGame, gameNode, iterNum);
        SkyNetNode pickedChild = MCTSSelect(gameNode, iterNum);
        bool stalemate = false;
        bool playerOneWin = MCTSRandSimPlayout(curGame, pickedChild, ref stalemate, iterNum);
        MCTSBackpropWin(pickedChild, playerOneWin, stalemate, iterNum);
        //RLBrain.Save(this.GetRoot());
        //Console.WriteLine(String.Format("EXITING ITERATION -- {0}", iterNum));
    }

    public SkyNetNode GetRoot()
    {
        return rootNode;
    }

    private void MCTSExpand(IGame curGame, SkyNetNode gameNode, int iterNum)
    {
        if (printStates)
        {
            DebugPrint("ENTERING MCTS EXPANSION -- " + iterNum.ToString() + "\n");
            //DebugPrint(gameNode.ToString());
        }
        Debug.Assert(curGame.isPlayerOneTurn() != gameNode.playerOne);
        IBoard localBoard = curGame.getBoard();
        List<GameMove> availMoves = curGame.getAllPlayerMoves(localBoard, curGame.isPlayerOneTurn());
        int cnt = availMoves.Count;
        CheckTerminalNode(curGame, localBoard, gameNode);
        rootMut.WaitOne();
        if (cnt > 0 && !gameNode.isTerminal())
        {
            if (gameNode.children == null) { gameNode.children = new List<SkyNetNode>(); }
            for (int i = 0; i < cnt; i++)
            {
                IGame tmpGame = curGame.CopyGame();
                MakeMove(tmpGame, availMoves[i]);
                Debug.Assert(tmpGame.isPlayerOneTurn() == gameNode.playerOne);
                string hash = tmpGame.getBoardAsString(tmpGame.getBoard(), curGame.isPlayerOneTurn());
                SkyNetNode toAdd = new SkyNetNode(availMoves[i], gameNode, curGame.isPlayerOneTurn(), hash, tmpGame.getHandAsString(curGame.isPlayerOneTurn()));
                if (!gameNode.children.Contains(toAdd))
                {
                    gameNode.children.Add(toAdd);
                    //Console.WriteLine(String.Format("Added new Child Node at Index: {0}{1}\n", i.ToString(), toAdd.ToString()));
                }
            }
        }
        rootMut.ReleaseMutex();
        DebugPrint("EXITING MCTS EXPANSION -- " + iterNum.ToString() + "\n");
    }

    private SkyNetNode MCTSSelect(SkyNetNode curNode, int iterNum)
    {
        DebugPrint("ENTERING MCTS SELECTION -- " + iterNum.ToString() + "\n");
        SkyNetNode toRet = curNode;
        float topWinRate = -1.0f;
        rootMut.WaitOne();
        foreach (SkyNetNode skyNode in curNode.children)
        {
            float childWinRate = skyNode.visitCnt == 0 ? 0 : skyNode.winCnt / skyNode.visitCnt;
            if (childWinRate > topWinRate)
            {
                toRet = skyNode;
                topWinRate = childWinRate;
                DebugPrint("Found More Valuable Child\n");
            }
        }
        rootMut.ReleaseMutex();
        //Debug.Assert(to)
        DebugPrint("EXITING MCTS SELECTION -- " + iterNum.ToString() + "\n");
        return toRet;
    }

    private bool MCTSRandSimPlayout(IGame curGame, SkyNetNode curNode, ref bool stalemate, int iterNum)
    {
        DebugPrint("ENTERING MCTS SIM PLAYOUT -- " + iterNum.ToString() + "\n");
        IGame tmpGame = curGame.CopyGame();
        if (curNode.isTerminal())
        {
            DebugPrint("Player One Win: " + curNode.playerOne.ToString() + "\n");
            DebugPrint("EXITING MCTS SIM PLAYOUT -- " + iterNum.ToString() + "\n");
            return tmpGame.isPlayerOneTurn();
        }
        Debug.Assert(tmpGame.isPlayerOneTurn() == curNode.playerOne);
        MakeMove(tmpGame, curNode.move);
        IBoard tmpBoard = tmpGame.getBoard();
        bool curBoardTerminal = curNode.isTerminal();
        Random rand = new Random();
        float moveCnt = 0;
        // float numMins = 0;
        // DateTime curTime = DateTime.UtcNow;
        // DateTime nextPrint = curTime.AddMinutes(1);
        // float movesPerSec = 0.0f;
        while (!curBoardTerminal)
        {
            List<GameMove> availMoves = tmpGame.getAllPlayerMoves(tmpBoard, tmpGame.isPlayerOneTurn());
            int cnt = availMoves.Count;
            int randMoveInd = rand.Next(0, availMoves.Count);
            GameMove randMove = availMoves[randMoveInd];
            //DebugPrint(String.Format("Move Made!\nPlayer One's: {0}\nMove: {1}\n", tmpGame.isPlayerOneTurn().ToString(), randMove.ToString()));
            MakeMove(tmpGame, randMove);
            // curTime = DateTime.UtcNow;
            // if (curTime.CompareTo(nextPrint) > 0)
            // {
            //     nextPrint = curTime.AddMinutes(1);
            //     numMins++;
            //     movesPerSec = moveCnt / (numMins * 60);
            //     //DebugPrint(String.Format("Moves: {0} --- Mins: {1} --- MPS: {2}\n", moveCnt, numMins, movesPerSec));
            // }
            moveCnt++;
            if (moveCnt >= (RLBrain.maxMoves - curNode.level))
            {
                stalemate = true;
                DebugPrint("EXITING MCTS SIM PLAYOUT -- " + iterNum.ToString() + "\n");
                return !tmpGame.isPlayerOneTurn();
            }
            curBoardTerminal = CheckTerminalBoard(tmpGame, tmpBoard);
            if (curBoardTerminal)
            {
                //same thing as "return !curGame.isPlayerOneTurn();"
                tmpGame.switchTurn();
            }
        }
        DebugPrint("Player One Win: " + tmpGame.isPlayerOneTurn().ToString() + "\n");
        DebugPrint("EXITING MCTS SIM PLAYOUT -- " + iterNum.ToString() + "\n");
        return tmpGame.isPlayerOneTurn();
    }

    private void MCTSBackpropWin(SkyNetNode endNode, bool playerOneWin, bool stalemate, int iterNum)
    {
        rootMut.WaitOne();
        DebugPrint("ENTERING MCTS BACKPROPOGATION -- " + iterNum.ToString() + "\n");
        SkyNetNode cur = endNode;
        while (cur != null)
        {
            cur.incWinAndVisit(playerOneWin, stalemate);
            //DebugPrint(cur.ToString());
            cur = cur.parent;
        }
        rootMut.ReleaseMutex();
        DebugPrint("EXITING MCTS BACKPROPOGATION -- " + iterNum.ToString() + "\n");
    }

    private void CheckTerminalNode(IGame curGame, IBoard board, SkyNetNode curNode)
    {
        List<ICard> toTestDiag1 = new List<ICard>();
        List<ICard> toTestDiag2 = new List<ICard>();
        for (int i = 0; i < 5 && !curNode.isTerminal(); i++)
        {
            List<ICard> toTestHoriz = new List<ICard>();
            List<ICard> toTestVert = new List<ICard>();
            ICard card1 = board.GetCardAtSpace(i, i);
            ICard card2 = board.GetCardAtSpace(i, 4 - i);
            if (card1 != null)
            {
                toTestDiag1.Add(card1);
            }
            if (card2 != null)
            {
                toTestDiag2.Add(card2);
            }
            for (int j = 0; j < 5 && (toTestHoriz.Count == j || toTestVert.Count == j); j++)
            {
                ICard card3 = board.GetCardAtSpace(i, j);
                ICard card4 = board.GetCardAtSpace(j, i);
                if (card3 != null)
                {
                    toTestHoriz.Add(card3);
                }
                if (card4 != null)
                {
                    toTestVert.Add(card4);
                }
            }


            if (toTestVert.Count != 5 && toTestHoriz.Count != 5)
            {
                continue;
            }
            rootMut.WaitOne();
            foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
            {
                if (curGame.CheckGameOverClaim(toTestHoriz, t))
                {
                    curNode.setTerminalFlag(true, toTestHoriz, t);
                }
                else if (curGame.CheckGameOverClaim(toTestVert, t))
                {
                    curNode.setTerminalFlag(true, toTestVert, t);
                }

            }
            rootMut.ReleaseMutex();
        }
        rootMut.WaitOne();
        foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
        {
            if (curGame.CheckGameOverClaim(toTestDiag1, t))
            {
                curNode.setTerminalFlag(true, toTestDiag1, t);
            }
            else if (curGame.CheckGameOverClaim(toTestDiag2, t))
            {
                curNode.setTerminalFlag(true, toTestDiag2, t);
            }
        }
        rootMut.ReleaseMutex();
    }

    private bool CheckTerminalBoard(IGame curGame, IBoard board)
    {
        bool terminal = false;
        List<ICard> toTestDiag1 = new List<ICard>();
        List<ICard> toTestDiag2 = new List<ICard>();
        for (int i = 0; i < 5 && !terminal; i++)
        {
            List<ICard> toTestHoriz = new List<ICard>();
            List<ICard> toTestVert = new List<ICard>();
            ICard card1 = board.GetCardAtSpace(i, i);
            ICard card2 = board.GetCardAtSpace(i, 4 - i);
            if (card1 != null)
            {
                toTestDiag1.Add(card1);
            }
            if (card2 != null)
            {
                toTestDiag2.Add(card2);
            }
            for (int j = 0; j < 5 && (toTestHoriz.Count == j || toTestVert.Count == j); j++)
            {
                ICard card3 = board.GetCardAtSpace(j, i);
                ICard card4 = board.GetCardAtSpace(i, j);
                if (card3 != null)
                {
                    toTestHoriz.Add(card3);
                }
                if (card4 != null)
                {
                    toTestVert.Add(card4);
                }
            }

            if (toTestVert.Count != 5 && toTestHoriz.Count != 5)
            {
                continue;
            }

            foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
            {
                terminal = curGame.CheckGameOverClaim(toTestHoriz, t) ||
                    curGame.CheckGameOverClaim(toTestVert, t);

                if (terminal)
                {
                    return terminal;
                }
            }
        }
        foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
        {
            terminal = terminal || curGame.CheckGameOverClaim(toTestDiag1, t) || curGame.CheckGameOverClaim(toTestDiag2, t);
        }
        return terminal;
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

    public void PrintTree(SkyNetNode root)
    {
        PrintTreeHelper(root, 0);
    }

    private void PrintTreeHelper(SkyNetNode curNode, int lev)
    {
        if (curNode.winCnt / curNode.visitCnt < 1.0f)
        {
            Console.Write("\n Level: " + lev.ToString() + "\n" + curNode.ToString());
        }
        int nxtlev = lev + 1;
        foreach (SkyNetNode child in curNode.children)
        {
            PrintTreeHelper(child, nxtlev);
        }
    }
}
