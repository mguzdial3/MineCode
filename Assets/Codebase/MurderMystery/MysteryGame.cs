using UnityEngine;
using System.Collections;

public class MysteryGame : MonoBehaviour {
	public NPCManager npcManager;

	// Use this for initialization
	void Start () {
		FriendlyNPC friendlyNPC = npcManager.SpawnMostCustomNPC (1, 1.5f, 1.5f, 5, "None", "Pumpkin","Pumpkin", "Snow", "Pumpkin", "Pumpkin");
		friendlyNPC.SetQuest ("MysteryQuestStart");

		ItemHandler.SpawnItemFromString ("Sword", 38, 1.2f, -5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
