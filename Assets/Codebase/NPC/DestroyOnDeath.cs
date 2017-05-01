using UnityEngine;
using System.Collections;

public class DestroyOnDeath : MonoBehaviour {
	ParticleSystem pS;

	// Use this for initialization
	void Start () {
		pS = gameObject.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (pS != null) {
			if (!pS.IsAlive ()) {
				Destroy (gameObject.transform.parent.gameObject);//Destroy if particle system animation is done
			}
		}
	}
}
