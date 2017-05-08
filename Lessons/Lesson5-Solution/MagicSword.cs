using UnityEngine;
using System.Collections;

public class MagicSword : Sword {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void HitEnemy (Enemy enemy){
		base.HitEnemy (enemy);
		base.HitEnemy(GameManager.GetClosestEnemy (gameObject));
	}
}

