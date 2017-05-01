using UnityEngine;
using System.Collections;

public class MysteryQuestStart : Quest {

	public override void CannotStart (){
		base.CannotStart ();
	}

	public override void QuestEnd (){
		base.QuestEnd ();
		GameManager.EndGame ("That's it! You found it!");
	}

	public override void QuestStart (){
		base.QuestStart ();
		GUIManager.SetDisplayTextColor ("There's been a murder! Find the Weapon!", 5, Color.white);
	}

	public override bool CanStart (){
		return true;
	}

	public override bool CanEnd (){
		return ItemHandler.GetCountCollected ("Sword") > 0;
	}
}
