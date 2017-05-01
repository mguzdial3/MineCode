using UnityEngine;
using System.Collections;

public class PlatformerGame : MonoBehaviour {
	public NPCManager npcManager;

	// Use this for initialization
	void Start () {
		FriendlyNPC friendlyNPC = npcManager.SpawnMostCustomNPC (2,96, -28.5f, 0, "None", "Pumpkin","Pumpkin", "Snow", "Pumpkin", "Pumpkin");
		friendlyNPC.SetQuest ("EndQuestPlatformer");

	}

}
