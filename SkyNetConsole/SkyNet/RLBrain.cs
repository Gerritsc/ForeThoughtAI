using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class RLBrain {
	//I will name you Squishy, and you will be mine! My Squishy!
	private static RLBrain squishy = new RLBrain();

	private List<SkyNetNode> skyNetTreeRoots;

	private int numPlaythroughs = 100;

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
		MCTSkyNet squishyThought = new MCTSkyNet (game, game.isPlayerOneTurn ());
		for (int i = 0; i < numPlaythroughs; i++) {
			squishyThought.MCTSSingleIteration ();
		}
		SkyNetNode newRoot = squishyThought.GetRoot ();

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
			if (oldRoot.children.Contains (newchild)) {

			}
		}
	}

}
