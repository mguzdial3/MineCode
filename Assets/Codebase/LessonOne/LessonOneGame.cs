using UnityEngine;

public class LessonOneGame : MonoBehaviour {

	// This chunk of code (called a function) is run at the beginning of play time
	void Start () {

		//jumpValue determines the height of the player's jump
		float jumpValue = 1;
		//gravityValue determines how fast the player falls after a jump
		float gravityValue =20;
		//speedValue determines how fast the player moves
		float speedValue = 1;
		//npcSpeed determines the speed that the npcs move at
		float npcSpeed = 2;

		LessonOneGenerator.SetUpGame (jumpValue, gravityValue, speedValue, npcSpeed);
	}

	//This chunk of code (called a function) is run at every frame
	void Update () {

	}

}
