using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LessonOneGenerator : Generator {
	private static int worldSize = 50;
	private static int centralPowerSize = 2;
	public static Vector3 goal = Vector3.zero+Vector3.up; 
	private static float maxDifference = 2; //Max difference for player end state stuff
	private static float maxDepth = -40;

	//A collection (called an array) of all the enemies
	private static GameObject[] enemies;


	//This function is called to generate out the map
	public override void GenerateMap (){
		for (int x = worldSize*-1; x<worldSize; x++) {
			for(int z = worldSize*-1; z<worldSize; z++){
				int height = 0;
				int depth = 0;

				if(Mathf.Abs(x)<centralPowerSize && Mathf.Abs(z)<centralPowerSize && x!=0 && z!=0){
					//make central thing
					for(int y = 10; y>=-5; y--){
						MapBuilderHelper.BuildBlock ("Crystal", x , y , z );
					}
				}
				else{

					for(int y = height; y>-5; y--){
						
						MapBuilderHelper.BuildBlock ("SnowBlock", x , y , z );

						depth+=1;
					}
				}
			}
		}
	}

	//Determine whether the player wins
	public static bool PlayerWon(){
		if (Time.deltaTime == 0) {
			return false;
		}
		int numEnemiesStuck = 0;

		//Iterate through all the enemies. Determine if all are stuck
		foreach (GameObject e in enemies) {
			NPCMovementController movement = e.GetComponent<NPCMovementController>();

			if(movement!=null && e.transform.position.magnitude<worldSize/3f){
				Vector3 evenYGoal = new Vector3(movement.GetGoal().x,e.transform.position.y,movement.GetGoal().z);
				Vector3 diff = evenYGoal-e.transform.position;
				Vector3 evenYGoal2 = new Vector3(goal.x,e.transform.position.y,goal.z);
				Vector3 diff2 = evenYGoal2-e.transform.position;

				Vector3[] path = movement.GetPath();

				//Is this enemy not heading directly for the goal (they've been blocked)
				if(Vector3.Angle(diff.normalized,diff2.normalized)>40 || (path==null || (path[path.Length-1]-goal).magnitude>maxDifference*2)){
					numEnemiesStuck++;
				}
			}
		}

		return numEnemiesStuck==enemies.Length;
	}

	//Determine whether the player lost
	public static bool PlayerLost(){
		if (Time.deltaTime == 0) {
			return false;
		}

		//Check if player is too low
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (player.transform.position.y < maxDepth) {
			return true;
		}

		//Any close enough
		foreach(GameObject e in enemies){
			NPCMovementController movement = e.GetComponent<NPCMovementController>();
			if(movement!=null){
				if((e.transform.position-goal).magnitude<maxDifference){
					return true;
				}
			}
		}
		return false;
	}

	//This chunk of code is called to end the game that this generator is associated with
	public static void EndGame(string str){
		GameObject.FindGameObjectWithTag ("GUI").GetComponent<Text>().text = str;
		Time.timeScale = 0f;//Pause time
	}


	public static void SetUpGame(float jumpingVal, float gravityVal, float speedVal, float npcSpeed){

		CharacterMotor motor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMotor>();
		if (motor != null) {
			motor.jumping.baseHeight = jumpingVal;
			motor.movement.maxFallSpeed = gravityVal;
			motor.movement.maxForwardSpeed = speedVal;
			motor.movement.maxBackwardsSpeed = speedVal;
			motor.movement.maxSidewaysSpeed = speedVal;
		}

		//Grab a reference to all enemies
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		//Set all the speed values for every single enemy
		foreach (GameObject e in enemies) {
			e.GetComponent<NPCMovementController>().speed = npcSpeed;
		}


	}
}
