using UnityEngine;

public class LessonTwoGame : MonoBehaviour {
	//Reference to a prototype for a sword item
	public GameObject sword;
	//Reference to a prototype for a key item
	public GameObject key;

	bool hasKey = false;
	bool hasSword = false;

	// Use this for initialization
	void Start (){
		ItemHandler.SpawnItem (key, -5, 2, -5);
		ItemHandler.SpawnItem (sword, 4, 2.5f, -5);
		GUIManager.SetDisplayText ("Attempt to Escape the Cave!", 3);


	}
	
	// Update is called once per frame
	void Update () {

		if (ItemHandler.justPickedKey) {
			hasKey = true;
			GUIManager.SetDisplayText ("Got Key!", 2);
		}

		if (ItemHandler.justPickedSword) {
			hasSword = true;
			GUIManager.SetDisplayText ("Got Sword!", 2);
		}

		bool nearDoor = LockedDoor.playerEntered;

		if (nearDoor) {
			if (hasKey && hasSword) {
				LockedDoor.OpenDoor ();
			} 
			else if (hasKey) {
				GUIManager.SetDisplayText ("You still need a sword!", 3);
			} 
			else if (hasSword) {
				GUIManager.SetDisplayText ("You still need a key!", 3);
			}
			else {
				GUIManager.SetDisplayText ("The door is locked! Find a key!", 3);
			}
		}
	}
}
