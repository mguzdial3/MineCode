using UnityEngine;
using System.Collections;

public class BossAttackController : RangeAttackController {

	public override float GetProjectileRange ()
	{
		return base.GetProjectileRange ();
	}

	public override void Fire (GameObject target){
		Vector3 differenceVector = target.transform.position-transform.position;
		float distToPlayer = differenceVector.magnitude;

		if (distToPlayer < GetProjectileRange () / 2) {
			FireSpread (target);
		} 
		else {
			FireFast (target);
		}
	}

	public void FireSpread(GameObject target){
		if (shotTimer == 0) {
			int x = 0; 
			Vector3 targetPosition = target.transform.position;
			while (x < 3) {
				GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
				projectileObject.transform.position = transform.position;

				Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();

				firedProjectile.Fire (targetPosition);
				targetPosition.x += 1;
				shotTimer = 1.0f / enemy.GetFiringRate ();
				x = x + 1;
			}
		}
	}

	public void FireFast(GameObject target){
		if (shotTimer == 0) {
			GameObject projectileObject = Instantiate<GameObject> (projectile.gameObject);
			projectileObject.transform.position = transform.position;

			Projectile firedProjectile = projectileObject.GetComponent<Projectile> ();
			firedProjectile.projectileSpeed = firedProjectile.projectileSpeed * 2;
			firedProjectile.Fire (target.transform.position);
			shotTimer = 1.0f/enemy.GetFiringRate();
		}
	}
}

