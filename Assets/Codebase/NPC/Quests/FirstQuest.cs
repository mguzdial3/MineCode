using UnityEngine;

public class FirstQuest : Quest {
	//Called when the quest starts
	public override void QuestStart (){
		base.QuestStart ();

		//Add code here
	}

	public override void QuestUpdate (){
		base.QuestUpdate ();

		//Add code here
	}

	public override void CannotStart (){
		GUIManager.SetDisplayTextColor ("I can't start for some reason!", 3, Color.red);//Replace this line
	}

	public override bool CanStart (){
		return base.CanStart ();//Replace this line
	}

	public override bool CanEnd (){
		return base.CanEnd ();//Replace this line
	}

	public override void QuestEnd (){
		base.QuestEnd ();

		//Add code here
	}
}
