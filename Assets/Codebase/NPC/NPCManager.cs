/**
 * Copyright (c) 2015 Entertainment Intelligence Lab.
 * Last edited by Matthew Guzdial 06/2015
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.IO;
using System.Collections;

/**
 * NPCManager handles controlling, creating, loading, and saving all NPCs
 */
public class NPCManager : MonoBehaviour, Saveable {
	public GameObject enemyPrefab; //Prefab of all enemies for this level
	public int numToSpawn=30; //Number of enemies to spawn at the moment

	public GameObject[] npcPrefabs;//Prefab 


	private NPCUnit[] npcs;//The current initialized npcs 
	private int controlledUnit=-1;//the current controlled unit index

	public Selector player;//Reference to the player

	private bool takeOver;

	//Instant information
	public static NPCManager Instance;

	//Whether or not there are changes
	private bool hasChanges = false;

	//Whether this Saveable saves its entirety every time (false) or just updates (true)
	public bool savesUpdates { get{ return false;}}
	
	//Filename for the saves for this 
	public string filename {get{ return "npcSave"; }}
	
	//PlayerPref string to get the current update number for this 
	public string playerpref{get{return "NPC_MANAGER_PLAYERPREF";}}

	//Determine whether or not to automatically spawn the enemy prefabs
	public bool autoSpawn = true;
	//Determine where to autoSpawn from
	public Vector3 spawnPosition = new Vector3(0,0,0);
	//Determine how wide of an area to spawn around (-1 defaults to enemy's max area
	public float spawnArea = -1;

	public void InitializeNPCManager(){
		Instance = this;
	}

	void OnGUI(){
		if (controlledUnit != -1) {
			GUI.color = npcs[controlledUnit].GetColor();
			GUI.Label(new Rect (0,30,30,30),""+controlledUnit);	
			GUI.Label(new Rect(0,50,100,30), "Praise: R");
			GUI.Label(new Rect(0,70,100,30), "Critique: F");
			GUI.Label(new Rect(0,90,100,30), "Take Over: V");
			GUI.Label(new Rect(0,110,100,30), "Place Goal: G");
			GUI.Label(new Rect(0,130,100,30), "Leave: Z");
			GUI.color = Color.white;
		}
	}

	public FriendlyNPC SpawnNPC(int npcType, float x, float y, float z){
		npcType = Mathf.Clamp (npcType, 0, npcPrefabs.Length);
		GameObject go = (GameObject)Instantiate(npcPrefabs[npcType]);

		go.transform.position = new Vector3 (x, y, z);
		FriendlyNPC friendly = go.GetComponent<FriendlyNPC> ();

		return friendly;
	}
	
	public FriendlyNPC SpawnCustomNPC(int npcType, float x, float y, float z, string npcTexture, string clothingTexture){
		return SpawnMostCustomNPC (npcType, x, y, z, clothingTexture, npcTexture, npcTexture, clothingTexture, npcTexture, npcTexture);
	}

	public FriendlyNPC SpawnMostCustomNPC(int npcType, float x, float y, float z, string hatTexture, string headTexture, string noseTexture, string bodyTexture, string leftLegTexture, string rightLegTexture){
		FriendlyNPC friendly = SpawnNPC (npcType, x, y, z);
		NPCBodyTexturer bodyTexturer = friendly.GetComponent<NPCBodyTexturer> ();
		bodyTexturer.TextureNPC (hatTexture, headTexture, noseTexture, bodyTexture, leftLegTexture, rightLegTexture);
		return friendly;
	}

	public void SpawnEnemies(int numSpawn, float x, float y, float z, float range = -1){
		npcs = null;
		numToSpawn = numSpawn;
		npcs= new NPCUnit[numToSpawn];
		for (int i = 0; i<numToSpawn; i++) {
			GameObject go = (GameObject)Instantiate(enemyPrefab);

			go.transform.position = new Vector3(x,y,z);

			NPCMovementController npc = go.GetComponent<NPCMovementController>();
			NPCAppearanceController npcA = go.GetComponent<NPCAppearanceController>();
			
			if(npcA!=null){
				npcA.Init();
			}
			
			if(npc!=null){
				
				//Naming for ease
				npc.gameObject.name = i.ToString("0000"); //assuming we have less than 10,000
				npcs[i] = npc.GetComponent<NPCUnit>();

				//Movement update
				if(range>0){
					npc.maxDistance = range;
					npc.randomPositionGenerate = true;
				}
			}
		}
	}

	// Use this for initialization if not loaded from save data
	public void InitNotFromSaveData () {
		if (autoSpawn) {
			SpawnEnemies (numToSpawn,spawnPosition.x, spawnPosition.y, spawnPosition.z, spawnArea);
		}
	}

	//Destroy all NPCS is used when switching to different save states
	public void DestroyAllNPCs(){
		for (int i = 0; i<npcs.Length; i++) {
			if(npcs[i]!=null){
				Destroy (npcs[i].gameObject);
			}
		}
	}

	public bool HasEnemies(){
		return npcs.Length > 0;
	}

	public Enemy GetClosestEnemy(GameObject obj){
		Enemy e = null;
		float closest = 100000;

		foreach (NPCUnit enemy in npcs) {
			if(enemy!=null && enemy.GetComponent<Enemy>()!=null){
				float dist = (obj.transform.position-enemy.transform.position).sqrMagnitude;
				if (dist<closest){
					closest = dist;
					e = enemy.GetComponent<Enemy>();
				}
			}
		}
		return e;
	}

	public void SetControlledUnit(NPCMovementController go){

		if (go != null) {
			int index = 0;

			while(index<npcs.Length && controlledUnit==-1){
				if(npcs[index] !=null && go.gameObject==npcs[index].gameObject){
					controlledUnit = index;
				}

				index++;
			}
		}
	}

	public bool CanSetControlledUnit(){
		return controlledUnit == -1;
	}

	#region SAVING

	//Whether or not this saveable has changes to save
	public bool HasChanges(){
		return hasChanges;
	}
	
	//Use this for a full save (used initially only for Saveables that saveUpdates; used every time for Saveables that 
	public void WriteFullSave(BinaryWriter wr){
		wr.Write (npcs.Length);//Write the number of npcs to write

		for (int i = 0; i<npcs.Length; i++) {
			if(npcs[i]!=null){
				//Appearance
				wr.Write(npcs[i].Appearance);
				//Save Position
				wr.Write((double)npcs[i].Position.x);
				wr.Write((double)npcs[i].Position.y);
				wr.Write((double)npcs[i].Position.z);
				//Save Goal
				wr.Write((double)npcs[i].Goal.x);
				wr.Write((double)npcs[i].Goal.y);
				wr.Write((double)npcs[i].Goal.z);
			}
		}
	}
	
	//Handles the update of this save. Either a full save (if savesUpdate false) or just an update (if savesUpdate true)
	public void WriteUpdateSave(BinaryWriter wr){
		WriteFullSave (wr); //NPC just does a full save every time
		hasChanges = false;//We've saved the changes
	}
	
	//Loads an initial save
	public void LoadSave(BinaryReader br){
		//Remove the prior values
		if (npcs != null) {
			for (int i = 0; i<npcs.Length; i++) {
				if (npcs [i] != null) {
					GameObject.Destroy (npcs [i].gameObject);
				}
			}
		}

		//Load save from initial values
		numToSpawn  = br.ReadInt32 ();
		npcs= new NPCUnit[numToSpawn];
		for (int i = 0; i<numToSpawn; i++) {
			GameObject go = (GameObject)Instantiate (enemyPrefab);
			//Set appearance from loaded data 
			go.GetComponent<NPCAppearanceController>().InitFromSave(br.ReadInt32());
			//Get position dimensions
			float posX = (float)br.ReadDouble();
			float posY = (float)br.ReadDouble();
			float posZ = (float)br.ReadDouble();

			//Get goal dimensions
			float goalX = (float)br.ReadDouble();
			float goalY = (float)br.ReadDouble();
			float goalZ = (float)br.ReadDouble();

			Vector3 position = new Vector3(posX,posY,posZ);
			Vector3 goal = new Vector3(goalX,goalY,goalZ);

			//Set Movement Info
			go.GetComponent<NPCMovementController>().InitFromSave(position,goal);

			//Set into the array
			npcs[i] = go.GetComponent<NPCUnit>();
		}
	}
	
	//Loads an update (or an initial save if savesUpdates is false). Load determines if it's loading or unloading this update.
	public void LoadUpdate(BinaryReader br, bool load){
		LoadSave (br);//NPC just does a full load every time
	}

	#endregion SAVING


}
