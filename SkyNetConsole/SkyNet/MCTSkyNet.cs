using System.Collections;
using System.Collections.Generic;
using System;

public class SkyNetNode {
	public SkyNetNode parent;
	public GameMove move;
	public bool playerOne;
	public int visitCnt;
	public int winCnt;
	bool terminal;
	string boardHash;
	public List<SkyNetNode> children;

	public SkyNetNode(string boardHash, bool playerOne){
		this.move = null;
		this.parent = null;
		this.children = null;
		this.playerOne = playerOne;
		this.boardHash = boardHash;
		this.terminal = false;
		this.winCnt = 0;
		this.visitCnt = 0;
	}

	public SkyNetNode(GameMove move, SkyNetNode prevNode, bool playerOne, string boardHash){
		this.move = move;
		this.parent = prevNode;
		this.playerOne = playerOne;
		this.terminal = false;
		this.boardHash = boardHash;
		this.winCnt = 0;
		this.visitCnt = 0;
	}

	public void incWinAndVisit(bool playerOne){
		visitCnt++;
		if (this.playerOne == playerOne) {
			winCnt++;
		}
	}

	public void setTerminalFlag(bool term){
		terminal = term;
	}

	public bool isTerminal(){
		return terminal;
	}

	public override bool Equals(object obj){
		if (obj == null || (this.GetType() != obj.GetType())) {
			return false;
		}
		bool equalExceptParent = this.terminal.Equals (((SkyNetNode)obj).terminal) && this.boardHash.Equals (((SkyNetNode)obj).boardHash) && this.move.Equals (((SkyNetNode)obj).move) && this.playerOne.Equals(((SkyNetNode)obj).playerOne);
		bool bothAreBatman = (this.parent == null) && (((SkyNetNode)obj).parent == null);
		return (bothAreBatman) ? equalExceptParent : ((SkyNetNode) obj).parent.Equals(this.parent) && equalExceptParent;
	}

	public override string ToString ()
	{
		string winPercent = (winCnt / visitCnt).ToString ();
		string dispStr = String.Format("\n[SkyNetNode]\nMove: {0}\nWin Percent: {1}\nNum Children: {2}\nPlayer One: {3}\nTerminal: {4}\n", move.ToString(), winPercent, children.Count.ToString(), this.playerOne.ToString(), this.terminal.ToString());
		return dispStr;
	}
}

public class MCTSkyNet {

	SkyNetNode rootNode;
    SkyNetNode curNode;
	ZobristKiller hashAndSlasher;

	IGame curGame;
    bool isPlayerOne;

    int numIters;
    float maxWait;

	public MCTSkyNet(IGame game, int numIters, float maxWait){
        this.maxWait = maxWait;
        this.numIters = numIters;
		hashAndSlasher = ZobristKiller.GetTheKiller ();
        isPlayerOne = game.isPlayerOneTurn();
        String[][] localBString = curGame.getBoardAsString(curGame.getBoard(), isPlayerOne);
		rootNode = new SkyNetNode (hashAndSlasher.HashIt(localBString), isPlayerOne);
        curNode = rootNode;
	}

    public SkyNetNode pickOfficialMove()
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
        while (curNode != null)
        {//while the node has children
            curNode = MCTSSelect(curNode);
        }
        MCTSExpand(curNode);
        SkyNetNode pickedChild = MCTSSelect(curNode);
        bool playerOneWin = MCTSRandSimPlayout(pickedChild);
        MCTSBackpropWin(pickedChild, playerOneWin);
	}

	public SkyNetNode GetRoot(){
		return rootNode;
	}

	private void MCTSExpand(SkyNetNode curNode){
		Console.WriteLine ("ENTERING MCTS EXPANSION");
		bool playerOne = !curNode.playerOne;
        IBoard localBoard = curGame.getBoard();
		List<GameMove> availMoves = curGame.getAllPlayerMoves (localBoard, playerOne);
		int cnt = availMoves.Count;
		string hash = hashAndSlasher.HashIt(curGame.getBoardAsString (localBoard, playerOne));
		bool curBoardTerminal = CheckTerminalNode (localBoard);
		if (cnt > 0 && !curBoardTerminal) {
			curNode.children = new List<SkyNetNode>();
			for (int i = 0; i < cnt; i++) {
				curNode.children.Add(new SkyNetNode (availMoves [i], curNode, playerOne, hash));
				Console.WriteLine (String.Format("Added new Child Node at Index: {1}{2}", i.ToString(), curNode.children[i].ToString()));
			}
		} else {
			Console.WriteLine (String.Format("Found Terminal Node {1}", curNode.ToString()));
			curNode.setTerminalFlag (true);
		}
		Console.WriteLine ("EXITING MCTS EXPANSION");
	}

	private SkyNetNode MCTSSelect(SkyNetNode curNode){
		Console.WriteLine ("ENTERING MCTS SELECTION");
		SkyNetNode toRet = null;
		float topWinRate = -1.0f;
		foreach (SkyNetNode skyNode in curNode.children) {
			float childWinRate = skyNode.winCnt / skyNode.visitCnt;
			if (childWinRate > topWinRate) {
				toRet = skyNode;
				topWinRate = childWinRate;
				Console.WriteLine (String.Format("Found More Valuable Child {1}", toRet.ToString()));
			}
		}
		Console.WriteLine ("EXITING MCTS SELECTION");
		return toRet;
	}

	private bool MCTSRandSimPlayout(SkyNetNode curNode){
		Console.WriteLine ("ENTERING MCTS SIM PLAYOUT");
		IBoard tmpBoard = curGame.getBoard().Copy();
		bool curBoardTerminal = CheckTerminalNode (tmpBoard);
		Random rand = new Random ();
		bool playerOne = curNode.playerOne;
		while(!curBoardTerminal){
			List<GameMove> availMoves = curGame.getAllPlayerMoves (tmpBoard, playerOne);
			int cnt = availMoves.Count;
			int randMoveInd = rand.Next (0, availMoves.Count);
			GameMove randMove = availMoves [randMoveInd];
			Console.WriteLine (String.Format("Move Made!\nPlayer One's: {1}\nMove: {2}", playerOne.ToString(), randMove.ToString()));
			makeMove(tmpBoard, randMove);
			curBoardTerminal = CheckTerminalNode(tmpBoard);
			if (!curBoardTerminal) {
				playerOne = !playerOne;
			}
		}
		Console.WriteLine("Player One Win: " + playerOne.ToString());
		Console.WriteLine ("EXITING MCTS SIM PLAYOUT");
		return playerOne;
	}

	private void MCTSBackpropWin(SkyNetNode endNode, bool playerOneWin){
		Console.WriteLine ("ENTERING MCTS BACKPROPOGATION");
		SkyNetNode cur = endNode;
		while (cur != null) {
			cur.incWinAndVisit (playerOneWin);
			Console.Write (cur.ToString ());
			cur = cur.parent;
		}
		Console.WriteLine ("EXITING MCTS BACKPROPOGATION");
	}

	private bool CheckTerminalNode(IBoard board){
		bool terminal = false;
		List<ICard> toTestDiag1 = new List<ICard>();
		List<ICard> toTestDiag2 = new List<ICard>();
		for (int i = 0; i < 5 && !terminal; i++) {
			List<ICard> toTestHoriz = new List<ICard>();
			List<ICard> toTestVert = new List<ICard>();
			toTestDiag1.Add (board.GetCardAtSpace(i, i));
			toTestDiag2.Add (board.GetCardAtSpace(i, 4 - i));
			for (int j = 0; j < 5; j++) {
				toTestHoriz.Add (board.GetCardAtSpace(i, j));
				toTestVert.Add (board.GetCardAtSpace(j, i));
			}
			foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE))) {
				terminal = terminal && curGame.CheckGameOverClaim (toTestHoriz, t) && curGame.CheckGameOverClaim (toTestVert, t);
			}
		}
		foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE))) {
			terminal = terminal && curGame.CheckGameOverClaim (toTestDiag1, t) && curGame.CheckGameOverClaim (toTestDiag2, t);
		}
		return terminal;
	}

	private void makeMove(IBoard board, GameMove move){
        int player = (curGame.isPlayerOneTurn()) ? 0 : 1;
		switch (move.type) {
		    case MoveType.ADD:
			    board.addCard (move.x1, move.y1, move.card);
			    break;
		    case MoveType.REMOVE:
                board.removeCard (move.x1, move.y1);
			    break;
		    case MoveType.SWAP:
                board.swapCards (move.x1, move.y1, move.x2, move.y2);
			    break;
            case MoveType.PEEK:
                //board.peek(move.x1, move.y1);
                break;
		}
	}
}
