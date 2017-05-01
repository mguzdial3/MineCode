using UnityEngine;
using System.Collections;

public class RangeAttackController : MonoBehaviour {
	//The Projectile this attacker will use
	public Projectile projectile;
	//The shotTimer (counts down from 0)
	protected float shotTimer = 0;
	//A reference to the Enemy this is connected to
	public Enemy enemy;

	void Start(){
		if (enemy == null) {
			enemy = gameObject.GetComponent<Enemy>();
		}
	}

	//Handles whether or not this attacker can shoot a projectile
	void Update(){
		if (shotTimer > 0) {
			shotTimer-=Time.deltaTime;

			if(shotTimer<0){
				shotTimer = 0;
			}
		}
	}

	//Returns the max range of this projectile
	public virtual float GetProjectileRange(){
		return projectile.GetMaxLifeTime()*projectile.GetSpeed();
	}

	//Called to shoot a projectile 
	public virtual void Fire(GameObject target){
		if (shotTimer == 0) {
			GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
			projectileObject.transform.position = transform.position;

			Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();

			firedProjectile.Fire (target.transform.position);
			shotTimer = 1.0f/enemy.GetFiringRate();
		}
	}


}
