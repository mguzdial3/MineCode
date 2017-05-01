using UnityEngine;

public class SecondQuest : Quest {
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
		base.CannotStart ();//Replace this line
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

