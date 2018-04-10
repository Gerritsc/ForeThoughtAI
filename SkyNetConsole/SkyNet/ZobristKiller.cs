using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

[System.Serializable]
public class ZobristKiller {

	//25 board spaces by 
	private string[]boardHashConst;
	private List<string> allStrings;
	private const int boardWidth = 5;
	private const int boardHeight = 5;

	[System.NonSerialized]
	private static ZobristKiller hashAndSlasher = new ZobristKiller();

	private ZobristKiller(){
		if (!Load ()) {
			allStrings = new List<string>(54);
			boardHashConst = new string[boardWidth * boardHeight * allStrings.Count];
			List<string> allStringCombs = new List<string> ();
			allStrings[52] = "uk";
			allStrings[53] = "none";
			for (int i = 0; i < 4; i++) {
				for (int j = 2; j <= 14; j++) {
					//int ind = (i * 13) + (j - 2);
					ICard tmpPC = new PlayingCard ((Suit)i, j);
					string desc = tmpPC.getFullCard ();
					allStrings.Add(desc);
				}
			}

			for (int i = 0; i < allStrings.Count; i++) {
				for (int j = 0; j < boardWidth; j++) {
					for (int k = 0; k < boardHeight; k++) {
						int ind = boardPosToHashInd (j, k, allStrings [i]);
						byte[] randBString = new byte[16];
						(new RNGCryptoServiceProvider ()).GetBytes (randBString);
						boardHashConst [ind] = Encoding.UTF8.GetString (randBString);
					}
				}
			}

			//Save ();
		}
	}

	public static ZobristKiller GetTheKiller(){
		return hashAndSlasher;
	}

	private int boardPosToHashInd(int x, int y, string card){
		int flatBInd = (x * 5) + y; // flattens the board pos [0-24]
		int cardInd = allStrings.IndexOf(card); //card ind[0-53]
		int hashInd = (flatBInd * 54) + cardInd;
		return hashInd;
	}

	public string HashIt(string[][] board){
		byte[] hash = new byte[16];
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board [i].Length; j++){
				byte[] constHash = new byte[16];
				int index = boardPosToHashInd (i, j, board [i] [j]);
				string boardHash = boardHashConst [index];
				constHash = Encoding.UTF8.GetBytes (boardHash);
				hash = xorByteArrays (hash, constHash, 16);
			}
		}
		return Encoding.UTF8.GetString(hash);
	}

	public static void Save()
	{
		//BinaryFormatter bf = new BinaryFormatter();
		//Debug.Log(Application.persistentDataPath + "/SkyNetData/zobristHash.dat");
		//FileStream file = File.Create(Application.persistentDataPath + "/SkyNetData/zobristHash.dat");
		//bf.Serialize (file, hashAndSlasher.boardHashConst);
		//file.Close();
	}

	public byte[] xorByteArrays(byte[] a1, byte[] a2, int size){
		for (int i = 0; i < size; i++) {
			a1 [i] ^= a2 [i];
		}
		return a1;
	}

	public static bool Load()
	{
		//if (File.Exists(Application.persistentDataPath + "/SkyNetData/zobristHash.dat"))
		//{
		//	BinaryFormatter bf = new BinaryFormatter();
		//	FileStream file = File.Open(Application.persistentDataPath + "/SkyNetData/zobristHash.dat", FileMode.Open);
		//	hashAndSlasher.boardHashConst = (string[])bf.Deserialize(file);
		//	file.Close();
		//	return true;
		//}
		return false;
	}
}
