using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	//The speed this projectile will move at
	public float projectileSpeed = 10;
	//The current life (counts up till maxLifeTime)
	private float lifeTime = 0;
	//The max possible amount of time (in seconds) the projectile can be alive
	private const float maxLifeTime = 1f;
	//Destination of this projectile
	private Vector3 destination;


	//This function is called when the player is hit
	private void HitPlayer(PlayerInfo playerInfo){
		//TODO; What should happen when you hit the player?

		//Destroy the projectile
		Destroy (gameObject);
	}

	// Update is called once per frame
	void Update () {
		//If max life time of the projectile has not been hit
		if (lifeTime < maxLifeTime) {
			lifeTime += Time.deltaTime;	

			if((destination-transform.position).magnitude<Time.deltaTime*projectileSpeed){
				Destroy(gameObject);
			}

			Vector3 newPosition = transform.position+(destination-transform.position).normalized*Time.deltaTime*projectileSpeed;

			RaycastHit hit;

			if(Physics.Raycast(transform.position,(newPosition-transform.position).normalized,out hit,(newPosition-transform.position).magnitude)){
				if(hit.collider.tag=="Player"){
					PlayerInfo playerInfo = hit.collider.gameObject.GetComponent<PlayerInfo>();
					
					if(playerInfo!=null){
						HitPlayer(playerInfo);
					}
				}
				else if(hit.collider.tag!="Projectile"){
					Destroy(gameObject);
				}
			}

			transform.position = newPosition;
			transform.LookAt (destination);
		} 
		else {
			//Destroy projectile if it has outlived its max possible life 
			Destroy(gameObject);	
		}
	}

	//Shoots this projectile at a given position
	public void Fire(Vector3 goalPosition){
		transform.LookAt (goalPosition);
		destination = goalPosition;
	}

	//Returns the max amount of life this projectile can last
	public float GetMaxLifeTime(){
		return maxLifeTime;
	}

	//Returns the speed of this projectile
	public float GetSpeed(){
		return projectileSpeed;
	}

	//Handled by Unity engine 
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			PlayerInfo playerInfo = other.gameObject.GetComponent<PlayerInfo>();

			if(playerInfo!=null){
				HitPlayer(playerInfo);
			}
		}
	}

	//Handled by Unity engine 
	void OnCollisionEnter(Collision other){
		if(other.gameObject.tag=="Player"){
			PlayerInfo playerInfo = other.gameObject.GetComponent<PlayerInfo>();
			
			if(playerInfo!=null){
				HitPlayer(playerInfo);
			}
		}
	}
}
