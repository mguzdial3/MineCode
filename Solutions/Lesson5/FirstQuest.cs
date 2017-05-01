using UnityEngine;

public class FirstQuest : Quest {
	//Called when the quest starts
	public override void QuestStart (){
		base.QuestStart ();
		GUIManager.SetDisplayTextColor ("Bob: Hey! You don't look tough, go get a sword!", 3, Color.green);
	}

	public override void QuestUpdate (){
		base.QuestUpdate ();
	}

	public override bool CanStart (){
		return true;
	}

	public override bool CanEnd (){
		return ItemHandler.GetCountCollected ("Sword") > 0;
	}

	public override void QuestEnd (){
		base.QuestEnd ();
		GUIManager.SetDisplayTextColor ("Bob: Whoa much better!", 3, Color.green);
		myNPC.SetQuest ("SecondQuest");
	}
}
