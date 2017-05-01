using UnityEngine;
using System.Collections;

public class Key : Item {

	public override void Pickup (){
		base.Pickup ();
	}

	public override bool Use (GameObject hitObject){
		return base.Use (hitObject);
	}
}
