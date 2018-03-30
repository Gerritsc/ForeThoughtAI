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

	Dictionary<string, List<SkyNetNode>> shortTermMemory;
	SkyNetNode rootNode;

	ZobristKiller hashAndSlasher;

	string[][] localBString;
	IGame localGame;
	IBoard localBoard;

	public MCTSkyNet(IGame game, bool playerOne){
		hashAndSlasher = ZobristKiller.GetTheKiller ();
		shortTermMemory = new Dictionary<string, List<SkyNetNode>> ();
		localBoard = game.getBoard ();
		localBString = game.getBoardAsString (localBoard, playerOne);
		rootNode = new SkyNetNode (hashAndSlasher.HashIt(localBString), playerOne);
	}

	public void MCTSSingleIteration()
	{
		SkyNetNode curNode = rootNode;
		while (curNode != null) {//while the node has children
			curNode = MCTSSelect(curNode);
		}
		MCTSExpand (curNode);
		SkyNetNode pickedChild = MCTSSelect (curNode);
		bool playerOneWin = MCTSRandSimPlayout(pickedChild);
		MCTSBackpropWin(pickedChild, playerOneWin);
	}

	public SkyNetNode GetRoot(){
		return rootNode;
	}

//	public void MCTS(int playerTurn, SkyNetNode prevNode){
//		IBoard board = localGame.getBoard();
//		int ind = 0;
//		bool expanding = true;
//		while(expanding){
//
//			string[][] bString = localGame.getBoardAsString (board, playerTurn);
//			string bHash = hashAndSlasher.HashIt (bString);
//
//			List<GameMove> availMoves = localGame.getAllPlayerMoves (board, playerTurn);
//
//			if (!shortTermMemory.ContainsKey (bHash)) {
//				shortTermMemory.Add (bHash, new List<SkyNetNode> ());
//			}
//
//			List<SkyNetNode> movesInMem = shortTermMemory[bHash];
//			GameMove selected = availMoves [ind];
//			SkyNetNode selectAsNode = new SkyNetNode (selected, prevNode, playerTurn, false);
//			int memInd = movesInMem.IndexOf (selectAsNode);
//			if (memInd != -1) { // We've done this move from this state before
//				selectAsNode = movesInMem[memInd];
//				selectAsNode.prevNode = prevNode;
//			} else {
//				movesInMem.Add (selectAsNode);
//			}
//			selectAsNode.incVisit ();
//
//			board = mockMove (board, selectAsNode);
//			if (!CheckTerminalNode (board)) {
//				
//			} else {
//				expanding = false;
//				backpropWin (selectAsNode);
//			}
//		}
//	}

	private void MCTSExpand(SkyNetNode curNode){
		Console.WriteLine ("ENTERING MCTS EXPANSION");
		bool playerOne = !curNode.playerOne;
		List<GameMove> availMoves = localGame.getAllPlayerMoves (localBoard, playerOne);
		int cnt = availMoves.Count;
		string hash = hashAndSlasher.HashIt(localGame.getBoardAsString (localBoard, playerOne));
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
		IBoard tmpBoard = localBoard;
		bool curBoardTerminal = CheckTerminalNode (tmpBoard);
		Random rand = new Random ();
		bool playerOne = curNode.playerOne;
		while(!curBoardTerminal){
			List<GameMove> availMoves = localGame.getAllPlayerMoves (tmpBoard, playerOne);
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
				terminal = terminal && localGame.CheckGameOverClaim (toTestHoriz, t) && localGame.CheckGameOverClaim (toTestVert, t);
			}
		}
		foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE))) {
			terminal = terminal && localGame.CheckGameOverClaim (toTestDiag1, t) && localGame.CheckGameOverClaim (toTestDiag2, t);
		}
		return terminal;
	}

	private void makeMove(IBoard board, GameMove move){
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
		}
	}
}
