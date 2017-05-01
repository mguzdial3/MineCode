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
using System;
using System.Collections;

/**
 * GameManager sets up and runs the game. Make changes here to change what Generator is used.
 */
public class GameManager : MonoBehaviour {
	
	//Public reference to the current blockset (blocks used to construct the map)
	public BlockSet blockset;

	////Save Section
	//This boolean check etermines whether or not to load from save data
	public bool loadFromSave;
	//This boolean check determines whether or not to load specially named save data
	public bool specialSnapshotLoad;
	//This string determines the name of the special save to load
	public string specialSnapshotString; //If not specified defaults to value stored in SNAPSHOT_TEXT PlayerPref
	public string generatorToUse = "TerrainGenerator";

	// Use this for initialization
	void Start () {
		Map map = new Map (blockset);
		//Initialize the npc manager
		NPCManager npcManager = GetComponent<NPCManager> ();
		if (npcManager != null) {
			npcManager.InitializeNPCManager ();
		}
		//Instantiate initial save manager
		SaveManager saveManager = new SaveManager ();

		//Initialize BlockSet
		blockset.Init ();

		if (loadFromSave) {
			//Load in map and enemies from saved data
			saveManager.LoadInitial();
		} else {
			object o = (Generator)System.Activator.CreateInstance(Type.GetType(generatorToUse));

			Generator mapGenerator = (Generator)o;//REPLACE THIS TO CHANGE GENERATOR TYPE
			mapGenerator.GenerateMap(); //Generate the initial map
			if (npcManager != null) {
				npcManager.InitNotFromSaveData ();//Create initial enemies
			}
			saveManager.SaveInitial();
			map.BuildMesh(); //Build up the mesh for the map
		}
	}

	void Update(){
		//Prompt the SaveManager to attempt to save an update
		SaveManager.Instance.SaveGeneralUpdate ();
	}

	public static void EndGame(string textToDisplay, Color32 color){
		GUIManager.SetDisplayText(textToDisplay,1);
		GUIManager.SetGUIBackgroundColor (color);
		Time.timeScale = 0;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	//Ends the game, but displays the passed in text first
	public static void EndGame(string textToDisplay) {
		EndGame (textToDisplay, Color.black / 2);
	}

	//Returns the closest enemy to this gameobject
	public static Enemy GetClosestEnemy(GameObject obj){
		return NPCManager.Instance.GetClosestEnemy (obj);
	}

	//Returns true if there are enemies, false otherwise
	public static bool HasEnemies(){
		return NPCManager.Instance.HasEnemies ();
	}

	//Destroys all the enemies in the world
	public static void DestroyAllEnemies(){
		NPCManager.Instance.DestroyAllNPCs ();
	}

	//Create the number of enemies specified at the specified location with the specified range
	public static void CreateEnemies(int number, float x, float y, float z, float range){
		NPCManager.Instance.SpawnEnemies (number, x, y, z, range);
	}
}
