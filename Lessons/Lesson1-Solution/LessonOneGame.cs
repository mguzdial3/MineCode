using UnityEngine;

public class LessonOneGame : MonoBehaviour {
	float timeSpent = 0;
	int blocksPlaced = 0;

	// This chunk of code (called a function) is run at the beginning of play time
	void Start () {

		//jumpValue determines the height of the player's jump
		float jumpValue = 1;
		//gravityValue determines how fast the player falls after a jump
		float gravityValue =20;
		//speedValue determines how fast the player moves
		float speedValue = 5;
		//npcSpeed determines the speed that the npcs move at
		float npcSpeed = 0.5f;

		LessonOneGenerator.SetUpGame (jumpValue, gravityValue, speedValue, npcSpeed);
	}
	
	//This chunk of code (called a function) is run at every frame
	void Update () {
		timeSpent = timeSpent + Time.deltaTime;
		int timeSpentInt = (int)timeSpent;

		blocksPlaced = blocksPlaced + Selector.blocksLastTick;
		float blocksPlacedRate = blocksPlaced / timeSpent;

		//Check to see whether any of the enemies have reached the center
		if (LessonOneGenerator.PlayerLost ()) {
			string lostString = "Player Lost! Time Spent: " + timeSpentInt + " seconds. Block placement rate: " + blocksPlacedRate;
			LessonOneGenerator.EndGame (lostString);
		}

		//Check to see whether all the enemies cannot reach the center
		if (LessonOneGenerator.PlayerWon ()) {
			string wonString = "Player Won! Time Spent: " + timeSpentInt + " seconds. Block placement rate: " + blocksPlacedRate;
			LessonOneGenerator.EndGame (wonString);
		}
	}

}
