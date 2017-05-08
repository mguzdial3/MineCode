using UnityEngine;

public class Quest : MonoBehaviour {
	//This variable tracks whether or not this quest has started
	private bool started = false;
	//This variable tracks whether or not this quest has ended
	private bool ended = false;

	//Reference to the player (protected indicates that this variable can be seen in the children of this class)
	protected GameObject player;
	//Reference to the NPC that this Quest is attached to
	protected FriendlyNPC myNPC;

	//This function is used to set the value for the player variable
	public void SetPlayer(GameObject playerReference){
		player = playerReference;
	}

	//This function is used to set the value for the myNPC variable
	public void SetNPC(FriendlyNPC npcReference){
		myNPC = npcReference;
	}

	//Returns the current value of started, whether or not this quest has started
	public bool HasStarted(){
		return started;
	}

	//Returns the current value of ended, whether or not this quest has ended
	public bool HasEnded(){
		return ended;
	}

	//Returns whether or not this quest can start
	public virtual bool CanStart(){
		return false;
	}

	//This function is called when the quest is first started
	public virtual void QuestStart(){
		started = true;
	}

	//Do what must to be done if the player has approached the npc but the quest cannot start
	public virtual void CannotStart(){
		GUIManager.SetDisplayText ("Quest cannot start!", 2);
	}

	//This function is called every frame after the quest has started
	public virtual void QuestUpdate(){
		
	}

	//Returns whether or not this quest can end
	public virtual bool CanEnd(){
		return false;
	}

	//This function is called when the quest ends.
	public virtual void QuestEnd(){
		ended = true;
	}
}
