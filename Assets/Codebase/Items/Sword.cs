using UnityEngine;

public class Sword : Item {

	//This method is called when the sword hits an enemy
	public virtual void HitEnemy(Enemy enemy){
		if (enemy != null) {
			Destroy (enemy.gameObject);
		}

		//TODO; Add code here for handling the win condition

	}

	//This function is called when the sword is held and the "destroy" button is hit. 
	public override bool Use (GameObject hitObject){
		Enemy enemy = hitObject.GetComponent<Enemy> ();

		//Is the thing we hit an enemy?
		if (enemy != null) {
			//If it is, call HitEnemy and return true
			HitEnemy(enemy);
			return true;
		}

		//return false if we did not hit an enemy, as the sword was not "used"
		return false;
	}
}

