using UnityEngine;

public class Enemy : MonoBehaviour {
	//A reference to this enemy's id (generated at start)
	public string id;
	//The speed this enemy will move at
	private float speed = 3;
	//The firing rate when the enemy is still
	private float firingRate = 2;
	//A reference to the GameObject of the player
	public GameObject player;
	//A reference to this enemy's NPCMovementController, which handles movement
	public NPCMovementController movementController;
	//A reference to this enemy's RangeAttackController, which handles attacking
	public RangeAttackController attackController;

	bool seesPlayer = false;
	
	//Returns the speed of this enemy
	public float GetSpeed(){
		return speed;
	}

	//Returns the firing rate based on whether or not the enemy is moving
	public float GetFiringRate(){
		//Returns different values if the enemy is moving
		if (movementController.moving) {
			return firingRate * 0.5f;//Firing rate is half when moving
		} 
		else {
			return firingRate;	
		}
	}

	// Use this for initialization
	void Start () {
		movementController.SetRandomGoal ();
	}

	// Update is called once per frame
	void Update () {
		bool nowSees = movementController.CanSeeTarget (player);

		if (nowSees && !seesPlayer) {
			movementController.SetTargetPositionGoal(player);
		}
		seesPlayer = nowSees;

		if (seesPlayer) {
			if(movementController.GetDistanceToTarget(player)<attackController.GetProjectileRange()){
				attackController.Fire(player);
			}
			if(movementController.ReachedGoal()){
				movementController.SetTargetPositionGoal(player);
			}
		}
		else{
			if(movementController.ReachedGoal()){
				movementController.SetRandomGoal ();
			}
		}
	}
}
