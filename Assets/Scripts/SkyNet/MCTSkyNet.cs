using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkyNetNode : GameMove{
	public SkyNetNode prevNode;
	int player;
	int visitCnt;
	int winCnt;
	bool terminal;

	public SkyNetNode(GameMove move, SkyNetNode prevNode, int player, bool terminal) : base(move){
		this.prevNode = prevNode;
		this.player = player;
		this.terminal = terminal;
		this.winCnt = 0;
		this.visitCnt = 0;
	}

	public void incVisit(){
		visitCnt++;
	}

	public void incWin(){
		winCnt++;
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
		if (this.prevNode == null && ((SkyNetNode)obj).prevNode == null) {
			return base.Equals (obj)  && this.terminal == ((SkyNetNode) obj).terminal;
		}
		return base.Equals (obj) && ((SkyNetNode) obj).prevNode.Equals(this.prevNode) && this.terminal == ((SkyNetNode) obj).terminal;
	}
}

public class MCTSkyNet {

	Dictionary<string, List<SkyNetNode>> shortTermMemory;

	ZobristKiller hashAndSlasher;

	string[][] localBString;
	IGame localGame;
	IBoard localBoard;
	int myTurn;

	public MCTSkyNet(IGame game, int playerTurn){
		hashAndSlasher = ZobristKiller.GetTheKiller ();
		shortTermMemory = new Dictionary<string, List<SkyNetNode>> ();
		myTurn = playerTurn;
		localGame = game;
		localBoard = game.getBoard ();
		localBString = game.getBoardAsString (localBoard, myTurn);

	}

	public void MCTS(int playerTurn, SkyNetNode prevNode){
		IBoard board = localGame.getBoard();
		int ind = 0;
		bool expanding = true;
		while(expanding){

			string[][] bString = localGame.getBoardAsString (board, playerTurn);
			string bHash = hashAndSlasher.HashIt (bString);

			List<GameMove> availMoves = localGame.getAllPlayerMoves (board, playerTurn);

			if (!shortTermMemory.ContainsKey (bHash)) {
				shortTermMemory.Add (bHash, new List<SkyNetNode> ());
			}

			List<SkyNetNode> movesInMem = shortTermMemory[bHash];
			GameMove selected = availMoves [ind];
			SkyNetNode selectAsNode = new SkyNetNode (selected, prevNode, playerTurn, false);
			int memInd = movesInMem.IndexOf (selectAsNode);
			if (memInd != -1) { // We've done this move from this state before
				selectAsNode = movesInMem[memInd];
				selectAsNode.prevNode = prevNode;
			} else {
				movesInMem.Add (selectAsNode);
			}
			selectAsNode.incVisit ();

			board = mockMove (board, selectAsNode);
			if (!CheckTerminalNode (board)) {
				
			} else {
				expanding = false;
				backpropWin (selectAsNode);
			}
		}
	}

	public void MCTSLoop(){

	}

	public void ChargeMCTSCannons(string[][] board, List<GameMove> availMoves){

	}

	public void FireMCTSWhenReady(string[][] board){
		int winCnt = 0;
		int visitCnt = 0;
		//for
	}

	private bool Load(){
		return true;
	}

	private void Save(){

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

	public IBoard mockMove(IBoard board, SkyNetNode move){
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
		return board;
	}

	public void backpropWin(SkyNetNode endNode){
		SkyNetNode cur = endNode;
		while (cur != null) {
			cur.incWin ();
			cur = cur.prevNode;
		}
	}
}
