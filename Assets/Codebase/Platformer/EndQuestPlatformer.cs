using UnityEngine;
using System.Collections;

public class EndQuestPlatformer : Quest {

	public override bool CanStart (){
		return true;
	}

	public override void QuestStart (){
		GameManager.EndGame ("You made it!");
	}
}
