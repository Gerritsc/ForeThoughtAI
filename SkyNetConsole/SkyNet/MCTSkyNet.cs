using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class SkyNetNode
{
    public SkyNetNode parent;
    public GameMove move;
    public bool playerOne;
    public int visitCnt;
    public int winCnt;
    bool terminal;
    string boardHash;
    public List<SkyNetNode> children;
    private List<ICard> winningHand;
    private HANDTYPE winningType;

    public SkyNetNode(string boardHash, bool playerOne)
    {
        this.move = null;
        this.parent = null;
        this.children = new List<SkyNetNode>();
        this.playerOne = playerOne;
        this.boardHash = boardHash;
        this.terminal = false;
        this.winCnt = 1;
        this.visitCnt = 1;
    }

    public SkyNetNode(GameMove move, SkyNetNode prevNode, bool playerOne, string boardHash)
    {
        this.move = move;
        this.parent = prevNode;
        this.children = new List<SkyNetNode>();
        this.playerOne = playerOne;
        this.terminal = false;
        this.boardHash = boardHash;
        this.winCnt = 1;
        this.visitCnt = 1;
    }

    public void incWinAndVisit(bool playerOne, bool stalemate)
    {
        visitCnt++;
        if (this.playerOne == playerOne && !stalemate)
        {
            winCnt++;
        }
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
        bool equalExceptParent = this.terminal.Equals(((SkyNetNode)obj).terminal) && this.boardHash.Equals(((SkyNetNode)obj).boardHash) && this.move.Equals(((SkyNetNode)obj).move) && this.playerOne.Equals(((SkyNetNode)obj).playerOne);
        bool bothAreBatman = (this.parent == null) && (((SkyNetNode)obj).parent == null);
        return (bothAreBatman) ? equalExceptParent : ((SkyNetNode)obj).parent.Equals(this.parent) && equalExceptParent;
    }

    public override string ToString()
    {
        string winPercent = visitCnt == 0 ? "0" : (winCnt / visitCnt).ToString();
        string movStr = move != null ? move.ToString() : "";
        string dispStr = String.Format("\n[SkyNetNode]\nMove: {0}\nWin Percent: {1}\nNum Children: {2}\nPlayer One: {3}\nTerminal: {4}\n", movStr, winPercent, children.Count.ToString(), this.playerOne.ToString(), this.terminal.ToString());
        return dispStr;
    }
}

public class MCTSkyNet
{

    SkyNetNode rootNode;
    SkyNetNode curNode;
    ZobristKiller hashAndSlasher;

    IGame curGame;
    int numIters;
    float maxWait;

    private bool printStates = false;

    public MCTSkyNet(IGame game, int numIters, float maxWait)
    {
        this.maxWait = maxWait;
        this.numIters = numIters;
        hashAndSlasher = ZobristKiller.GetTheKiller();
        curGame = game;
        String[][] localBString = curGame.getBoardAsString(curGame.getBoard(), curGame.isPlayerOneTurn());
        rootNode = new SkyNetNode(hashAndSlasher.HashIt(localBString), !curGame.isPlayerOneTurn());
        curNode = rootNode;
    }

    private void DebugPrint(string msg)
    {
        if (printStates)
        {
            Console.Write(msg);
        }
    }

    public SkyNetNode PickOfficialMove()
    {
        IBoard localBoard = curGame.getBoard();
        DateTime curTime = DateTime.UtcNow;
        DateTime endTime = DateTime.UtcNow.AddSeconds(maxWait);
        for (int i = 0; ((i < numIters) && (DateTime.Compare(curTime, endTime) <= 0)); i++)
        {
            MCTSSingleIteration(curNode);
            curTime = DateTime.UtcNow;
        }
        SkyNetNode chosen = MCTSSelect(curNode);
        curNode = chosen;
        return chosen;
    }

    public void MCTSSingleIteration(SkyNetNode localRoot)
    {
        SkyNetNode curNode = localRoot;
        while (curNode.children.Count > 0)
        {//while the node has children
            curNode = MCTSSelect(curNode);
        }
        MCTSExpand(curNode);
        SkyNetNode pickedChild = MCTSSelect(curNode);
        bool stalemate = false;
        bool playerOneWin = MCTSRandSimPlayout(pickedChild, ref stalemate);
        MCTSBackpropWin(pickedChild, playerOneWin, stalemate);
    }

    public SkyNetNode GetRoot()
    {
        return rootNode;
    }

    private void MCTSExpand(SkyNetNode curNode)
    {
        if (printStates)
        {
            DebugPrint("ENTERING MCTS EXPANSION\n");
        }
        bool playerOne = !curNode.playerOne;
        IBoard localBoard = curGame.getBoard();
        List<GameMove> availMoves = curGame.getAllPlayerMoves(localBoard, playerOne);
        int cnt = availMoves.Count;
        string hash = hashAndSlasher.HashIt(curGame.getBoardAsString(localBoard, playerOne));
        CheckTerminalNode(localBoard, curNode);
        if (cnt > 0 && !curNode.isTerminal())
        {
            curNode.children = new List<SkyNetNode>();
            for (int i = 0; i < cnt; i++)
            {
                curNode.children.Add(new SkyNetNode(availMoves[i], curNode, playerOne, hash));
                DebugPrint(String.Format("Added new Child Node at Index: {0}{1}\n", i.ToString(), curNode.children[i].ToString()));
            }
        }
        DebugPrint("EXITING MCTS EXPANSION\n");
    }

    private SkyNetNode MCTSSelect(SkyNetNode curNode)
    {
        DebugPrint("ENTERING MCTS SELECTION\n");
        SkyNetNode toRet = curNode;
        float topWinRate = -1.0f;
        foreach (SkyNetNode skyNode in curNode.children)
        {
            float childWinRate = skyNode.visitCnt == 0 ? 0 : skyNode.winCnt / skyNode.visitCnt;
            if (childWinRate > topWinRate)
            {
                toRet = skyNode;
                topWinRate = childWinRate;
                DebugPrint(String.Format("Found More Valuable Child {0}\n", toRet.ToString()));
            }
        }
        DebugPrint("EXITING MCTS SELECTION\n");
        return toRet;
    }

    private bool MCTSRandSimPlayout(SkyNetNode curNode, ref bool stalemate)
    {
        DebugPrint("ENTERING MCTS SIM PLAYOUT\n");
        IBoard tmpBoard = curGame.getBoard().Copy();
        bool curBoardTerminal = curNode.isTerminal();
        Random rand = new Random();
        bool playerOne = curNode.playerOne;
        float moveCnt = 0;
        float numMins = 0;
        DateTime curTime = DateTime.UtcNow;
        DateTime nextPrint = curTime.AddMinutes(1);
        float movesPerSec = 0.0f;
        while (!curBoardTerminal)
        {
            List<GameMove> availMoves = curGame.getAllPlayerMoves(tmpBoard, playerOne);
            int cnt = availMoves.Count;
            int randMoveInd = rand.Next(0, availMoves.Count);
            GameMove randMove = availMoves[randMoveInd];
            DebugPrint(String.Format("Move Made!\nPlayer One's: {0}\nMove: {1}\n", playerOne.ToString(), randMove.ToString()));
            MockMove(tmpBoard, randMove);
            // curTime = DateTime.UtcNow;
            // if (curTime.CompareTo(nextPrint) > 0)
            // {
            //     nextPrint = curTime.AddMinutes(1);
            //     numMins++;
            //     movesPerSec = moveCnt / (numMins * 60);
            //     Console.WriteLine(String.Format("Moves: {0} --- Mins: {1} --- MPS: {2}", moveCnt, numMins, movesPerSec));
            // }
            // moveCnt++;
            // Console.WriteLine();
            // tmpBoard.PrintBoard();
            // Console.WriteLine();
            // if (moveCnt >= 1000000)
            // {
            //     stalemate = true;
            //     return !playerOne;
            // }
            curBoardTerminal = CheckTerminalBoard(tmpBoard);
            if (!curBoardTerminal)
            {
                playerOne = !playerOne;
            }
        }
        DebugPrint("Player One Win: " + playerOne.ToString() + "\n");
        DebugPrint("EXITING MCTS SIM PLAYOUT\n");
        return playerOne;
    }

    private void MCTSBackpropWin(SkyNetNode endNode, bool playerOneWin, bool stalemate)
    {
        DebugPrint("ENTERING MCTS BACKPROPOGATION\n");
        SkyNetNode cur = endNode;
        while (cur != null)
        {
            cur.incWinAndVisit(playerOneWin, stalemate);
            DebugPrint(cur.ToString());
            cur = cur.parent;
        }
        DebugPrint("EXITING MCTS BACKPROPOGATION\n");
    }

    private void CheckTerminalNode(IBoard board, SkyNetNode curNode)
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
            foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
            {
                if (toTestHoriz.Count > 0 && curGame.CheckGameOverClaim(toTestHoriz, t))
                {
                    curNode.setTerminalFlag(true, toTestHoriz, t);
                }
                else if (curGame.CheckGameOverClaim(toTestVert, t))
                {
                    curNode.setTerminalFlag(true, toTestVert, t);
                }

            }
        }
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
    }

    private bool CheckTerminalBoard(IBoard board)
    {
        bool terminal = curNode.isTerminal();
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
            for (int j = 0; j < 5; j++)
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
            foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
            {
                terminal = terminal &&
                    curGame.CheckGameOverClaim(toTestHoriz, t) &&
                    curGame.CheckGameOverClaim(toTestVert, t);

            }
        }
        foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
        {
            terminal = terminal && curGame.CheckGameOverClaim(toTestDiag1, t) && curGame.CheckGameOverClaim(toTestDiag2, t);
        }
        return terminal;
    }

    private void MockMove(IBoard board, GameMove move)
    {
        int player = (curGame.isPlayerOneTurn()) ? 0 : 1;
        switch (move.type)
        {
            case MoveType.ADD:
                board.addCard(move.x1, move.y1, move.card);
                break;
            case MoveType.REMOVE:
                board.removeCard(move.x1, move.y1);
                break;
            case MoveType.SWAP:
                board.swapCards(move.x1, move.y1, move.x2, move.y2);
                break;
            case MoveType.PEEK:
                //board.peek(move.x1, move.y1);
                break;
        }
    }
}
