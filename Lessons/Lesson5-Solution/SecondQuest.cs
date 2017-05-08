using UnityEngine;

public class SecondQuest : Quest {

	//Called when the quest starts
	public override void QuestStart (){
		base.QuestStart ();
		GUIManager.SetDisplayTextColor ("Bob: Alright cool you have a sword, can you find my lucky coin?", 3, Color.green);
		int numSpawned = 0;

		while (numSpawned < 100) {
			float x = Random.Range (0, 20);
			float z = Random.Range (0, 20);
			ItemHandler.SpawnItemFromString ("Coin", x, 3, z);
			numSpawned = numSpawned +1;
		}
	}
	
	public override void CannotStart (){
		GUIManager.SetDisplayTextColor ("You don't have a sword!", 3, Color.blue);//Replace this line
	}
	
	public override bool CanStart (){
		return ItemHandler.GetCountCollected ("Sword") > 0;
	}
	
	public override bool CanEnd (){
		return ItemHandler.GetCountCollected ("Coin") > 99;
	}
	
	public override void QuestEnd (){
		base.QuestEnd ();
		GUIManager.SetDisplayTextColor ("Bob: That's it! That's my lucky coin! Thanks!", 3, Color.green);
		ItemHandler.SpawnItemFromString ("Magic Sword", 0, 1, 0);
	}
}

