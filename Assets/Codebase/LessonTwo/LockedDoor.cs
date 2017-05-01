using UnityEngine;
using System.Collections;

public class LockedDoor : MonoBehaviour {
	private static bool _playerEntered = false;
	public static bool playerEntered {
		get {
			bool tempPlayerEntered = _playerEntered; 
			_playerEntered = false;
			return tempPlayerEntered;
		}
	}

	public static void OpenDoor(){
		//Remove door
		for(int xPlus = 0; xPlus<2; xPlus++){
			for(int yPlus = 0; yPlus<3; yPlus++){
				Map.Instance.SetBlockAndRecompute(null, new Vector3i(-1+xPlus, 6-yPlus,10),false,true);
			}
		}
		GameManager.EndGame ("The door opens...", Color.black / 2);
	}

	void OnTriggerEnter(Collider c){
		_playerEntered = true;
	}
}