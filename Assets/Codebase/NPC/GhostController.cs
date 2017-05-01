using UnityEngine;
using System.Collections;

public class GhostController : NPCUnit {
	
	void Start(){
		movementController.Init ();
		appearanceController.Init ();

		Enemy e = GetComponent<Enemy> ();
		if (e != null) {
			movementController.speed = e.GetSpeed();
			e.player = GameObject.FindGameObjectWithTag("Player");
			e.movementController = movementController;
			e.id = gameObject.name;

		}
	}
	
	void Update(){
		movementController.UpdateMovement ();
		appearanceController.UpdateAppearance ();

		if (transform.position.y < -50) {
			Destroy (gameObject);
		}

	}
}