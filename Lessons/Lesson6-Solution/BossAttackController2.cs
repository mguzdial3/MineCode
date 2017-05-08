using UnityEngine;
using System.Collections;

public class BossAttackController : RangeAttackController {

	public override void Fire (GameObject target){
		if ((target.transform.position - transform.position).magnitude < GetProjectileRange ()/2) {
			//FireSpread(target);
			FireSpin(target);
		}
		else if ((target.transform.position - transform.position).magnitude < GetProjectileRange ()){
			FireSnipe(target);
		}
		else {
			transform.position = (target.transform.position - transform.position)/2 + transform.position+Vector3.up;
			enemy.movementController.DeletePlan();
		}
	}
	/**
	 * 
	*/

	public override float GetProjectileRange (){
		return base.GetProjectileRange ()*2;
	}

	private void FireSpread(GameObject target){
		if (shotTimer == 0) {
			int counts = 0;
			Vector3 targetPos =target.transform.position;
			targetPos.x-=1;
			while(counts<3){
				GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
				projectileObject.transform.position = transform.position;
				
				Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();
				firedProjectile.Fire (targetPos);
				targetPos.x+=1;
				counts+=1;
			}
			shotTimer = 1.0f/enemy.GetFiringRate();
		}
	}

	private void FireFast(GameObject target){
		if (shotTimer == 0) {
			GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
			projectileObject.transform.position = transform.position;
			
			Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();
			firedProjectile.projectileSpeed = firedProjectile.projectileSpeed*2;
			firedProjectile.Fire (target.transform.position);
			shotTimer = 1.0f/enemy.GetFiringRate();
		}
	}

	private void FireSpin(GameObject target){
		if (shotTimer == 0) {
			int totalAngle = 0;

			while(totalAngle<360){
				GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
				projectileObject.transform.position = transform.position;
				
				Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();
				firedProjectile.Fire (transform.position+transform.forward*GetProjectileRange());
				transform.Rotate(Vector3.up*totalAngle);
				totalAngle+=10;
			}


			shotTimer = 1.0f/enemy.GetFiringRate();
		}
	}
}
