using UnityEngine;
using System.Collections;

public class SnowmanController : MonoBehaviour {
	public NPCMovementController npcMovementController; //Controls this NPC moving around the map
	public NPCAppearanceController npcAppearanceController; //Handles the appearance of this npc

	private float edgeOfWorld = 50;

	private Vector3 endGoal;

	public void SetGoal(Vector3 goal){
		npcMovementController.SetCurrGoal (goal);
		endGoal = goal;
	}

	void Start(){
		//Place this snowman somewhere toward the ends of the world
		float x = Random.Range (edgeOfWorld * 0.75f, edgeOfWorld);
		float z = Random.Range (edgeOfWorld * 0.75f, edgeOfWorld);
		x*= (Random.Range(0,2)==0) ? -1: 1;
		z*= (Random.Range(0,2)==0) ? -1: 1;

		transform.position = new Vector3 (x, 2, z);

		npcMovementController.Init ();
		npcAppearanceController.Init ();
		SetGoal (LessonOneGenerator.goal);
	}

	void Update(){
		bool moved = npcMovementController.UpdateMovement ();
		npcAppearanceController.UpdateAppearance ();

		if(npcMovementController.GetGoal()!=endGoal && !moved){
			npcMovementController.SetCurrGoal(endGoal);
		}
	}
}