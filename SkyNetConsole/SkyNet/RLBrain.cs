using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class RLBrain {
	//I will name you Squishy, and you will be mine! My Squishy!
	private static RLBrain squishy = new RLBrain();

	private List<SkyNetNode> skyNetTreeRoots;

	private int numPlaythroughs = 1;

	private RLBrain(){
		skyNetTreeRoots = new List<SkyNetNode> ();
	}

	public static RLBrain FindSquishy(){
		return squishy;
	}

	public void SelfTeach(int numThoughts){
		
		for (int i = 0; i < numThoughts; i++) {
			ThreadPool.QueueUserWorkItem (TrainOfThought);
		}

		int numAvail = 0;
		int other1 = 0;
		int maxThreads = 0;
		int other2 = 0;
		int numRunning = 0;

		do{
			//ThreadPool.GetAvailableThreads(out numAvail, out other1);
			ThreadPool.GetMaxThreads (out maxThreads, out other2);
			numRunning = (maxThreads - numAvail) + (other2 - other1);
		}while(numRunning > 0);

	}

	public void TrainOfThought(object stateInfo){
		IGame game = new Game ();
		MCTSkyNet squishyThought = new MCTSkyNet (game, numPlayouts, 5.0f);
        bool curBoardTerminal = false;
        while (!curBoardTerminal)
        {
            SkyNetNode turnMove = squishyThought.PickOfficialMove();
            Debug.Assert(turnMove.playerOne == game.isPlayerOneTurn());
            //String[][] bStringArr = game.getBoardAsString(game.getBoard(), game.isPlayerOneTurn());
            //for (int i = 0; i < bStringArr.Length; i++)
            //{
            //    string toPrint = "";
            //    for (int j = 0; j < bStringArr[i].Length; j++)
            //    {
            //        toPrint += bStringArr[i][j] + " ";
            //    }
            //    Console.WriteLine(toPrint);
            //}
            if (turnMove.isTerminal())
            {
                curBoardTerminal = true;
                int winningPlayer = turnMove.playerOne ? 1 : 2;
                //Console.WriteLine(String.Format("Game Ended. Player {0} wins", winningPlayer));
            }
            else
            {
                MakeMove(game, turnMove.move);
            }
        }
		SkyNetNode newRoot = squishyThought.GetRoot ();
        updateRootList(newRoot);
	}

	private void updateRootList(SkyNetNode newRoot){
		int oldInd = skyNetTreeRoots.IndexOf (newRoot);
		if(oldInd >= 0) {
			MergeTrees (skyNetTreeRoots[oldInd], newRoot);
		} else {
			skyNetTreeRoots.Add(newRoot);
		}
	}

	private void MergeTrees(SkyNetNode oldRoot, SkyNetNode newRoot){
		oldRoot.visitCnt += newRoot.visitCnt;
		oldRoot.winCnt += newRoot.visitCnt;
		foreach (SkyNetNode newchild in newRoot.children) {
            int existInd = oldRoot.children.IndexOf(newchild);
			if (existInd != -1) {
                MergeTrees(oldRoot.children[existInd], newchild);
            } else {
                Console.WriteLine("Child Node Error in Merge: NewRoot has child OldRoot does not.");
            }
		}
	}



    public void MakeMove(IGame game, GameMove move)
    {
        int playerInt = game.isPlayerOneTurn() ? 1 : 0;
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
        game.switchTurn();
    }

}
