using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool addInfo = false;

	// Use this for initialization
	void Awake () {
		if (addInfo) {
			gameObject.AddComponent<PlayerInfo>();
		}
	}
}
