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
using System.Collections;
using System.Collections.Generic;
using System.IO; 

public class SaveManager {
	//SaveManager singleton reference
	public static SaveManager Instance;

	//Saveables to Track
	private List<Saveable> saveables;
	
	//Path information for saving
	private string DIRECTORY ="/Users/mguzdial/MinecraftEnvironment/Snapshots/";
	private const string INITIAL_FILE ="initial";
	private const string FILE_TYPE =".dat";
	//The filename for where we store the reference to what update number we're on for the Saveables
	private const string UPDATE_FILENAME = "update";
	//PlayerPref used to store the current update number
	private const string UPDATE_PLAYERPREF = "UPDATEPLAYERPREF";
	//Current frames passed since last 
	private int frames=0;
	//Save once every FRAME_TO_SAVE frames
	private const int FRAME_TO_SAVE = 30;

	//Whether or not the SaveManager can presently save
	private bool canSave = true;
	//Stop all saving
	public bool savingAllowed = false;

	// Use this for initialization
	public SaveManager () {
		//Instance of the SaveManager set to itself (so that we can use it like a singleton
		Instance = this;		


		//Set up list of saveable classes
		saveables = new List<Saveable> ();
		saveables.Add (Map.Instance);
		saveables.Add (NPCManager.Instance);

		//Determine parent directory of directory (Prints /Users/mguzdial/MinecraftEnvironment)
		string currDirectory = System.Environment.CurrentDirectory;

		#if UNITY_EDITOR_OSX
			currDirectory+= "/SaveData/";
		#elif UNITY_STANDALONE_OSX
			currDirectory+= "/SaveData/";
		#else
			currDirectory+= r"\SaveData\";
		#endif 

		if (!Directory.Exists(currDirectory)){
			Directory.CreateDirectory(currDirectory);
		}

		DIRECTORY = currDirectory;
	}

	//Call this method to first check if an update should save and (if so) save it
	public void SaveGeneralUpdate(){
		//First check if this is the right frame to save on
		if(frames>=FRAME_TO_SAVE && canSave && savingAllowed){
			//Reset frames back to 0
			frames=0;
			//Check each saveable and see if it has changes, update it if so
			foreach(Saveable s in saveables){
				if(s.HasChanges()){
					UpdateSave(s);
				}
			}
			//Save the general update file for this frame (pointing to each of the current individual Saveable update files)
			IncrementGeneralUpdateNumber();//Set to a new update number
			//Create the file
			string filename = DIRECTORY+UPDATE_FILENAME+GetGeneralUpdateNumber()+FILE_TYPE;
			BinaryWriter wr = new BinaryWriter(File.Open(filename, FileMode.Create));
			foreach(Saveable s in saveables){
				wr.Write(GetCurrUpdateNumber(s)); //Write out the current individual updates for each Saveable
			}
			wr.Close();//Close out the writeable
		}
		else{
			//Increment frames value if it was not correct
			frames++;
		}
	}

	//Set whether or not the SaveManager can save at present
	public void SetCanSave(bool _canSave){
		canSave = _canSave;
	}

	#region SAVING

	//Call this method after setting up all the saveables from whatever generator you use and not from a save
	public void SaveInitial(){
		if(savingAllowed){
			FullSave(INITIAL_FILE);
			foreach(Saveable s in saveables){
				SetCurrUpdateNumber(s,-1);//Starting over for each Saveable's update number
			}
			SetGeneralUpdateNumber(-1);//Start over for the generable SaveManager update
		}
	}
	
	//Used to save the entire current values for all saveables for a "special" named save
	public void SaveSpecial(string special){
		if(savingAllowed){
			FullSave(special);
		}
	}

	//This handles a full 
	private void FullSave(string saveString){
		foreach(Saveable s in saveables){
			string filename  =DIRECTORY +s.filename+saveString+FILE_TYPE;
			BinaryWriter wr = new BinaryWriter(File.Open(filename, FileMode.Create));
			s.WriteFullSave(wr);
			wr.Close();//Close out the writeable
		}
	}
	
	//Update Save
	private void UpdateSave(Saveable s){
		//First increment the current update value
		IncrementCurrUpdateNumber(s);
		//Then save at that current value
		string filename  =DIRECTORY +s.filename+GetCurrUpdateNumber(s)+FILE_TYPE;
		BinaryWriter wr = new BinaryWriter(File.Open(filename, FileMode.Create));
		s.WriteUpdateSave(wr);
		wr.Close();//Close out the writeable
	}

	#endregion SAVING
	
	#region LOADING

	//Call this method to load a special save 
	public void LoadSpecial(string saveString){
		foreach(Saveable s in saveables){
			string filename = DIRECTORY+s.filename+saveString+FILE_TYPE;
			BinaryReader br = new BinaryReader(File.Open(filename,FileMode.Open));
			s.LoadSave(br);
			br.Close();//Close out the BinaryReader now that we're done with it
		}
	}

	//Call this method to load all the saveables with their initial data
	public void LoadInitial(){
		foreach(Saveable s in saveables){
			string filename = DIRECTORY+s.filename+INITIAL_FILE+FILE_TYPE;
			BinaryReader br = new BinaryReader(File.Open(filename,FileMode.Open));
			s.LoadSave(br);
			br.Close();//Close out the BinaryReader now that we're done with it
			int currUpdate = GetCurrUpdateNumber(s);
			if(currUpdate!=-1){//If we actually have a current update number
				if(s.savesUpdates){
					//Load all updates iteratively
					for(int i = 0; i<currUpdate; i++){
						LoadUpdate(s,i,true);
					}
				}
				else{
					//Load just a single update
					LoadUpdate(s,currUpdate,true);
				}
			}

		}
	}

	//Helper method to load a specific update from a Saveable
	private void LoadUpdate(Saveable s, int currUpdate, bool additive){
		string filename = DIRECTORY+s.filename+currUpdate+FILE_TYPE;
		BinaryReader br = new BinaryReader(File.Open(filename,FileMode.Open));
		s.LoadUpdate(br, additive);
		br.Close();//Close out the BinaryReader now that we're done with it
	}

	//Call this method to move to a different "current" update. difference is the amount to change the update number by
	public bool LoadDifferentUpdate(int difference){
		int currUpdate = GetGeneralUpdateNumber()+difference;
		//First let's check out if the desired update even exists
		string desiredFilename =DIRECTORY+UPDATE_FILENAME+currUpdate+FILE_TYPE;

		//If it exists, load it up
		if(System.IO.File.Exists(desiredFilename)){
			BinaryReader desiredUpdateReader = new BinaryReader(File.Open(desiredFilename,FileMode.Open));
			foreach(Saveable s in saveables){
				int saveableNewCurrUpdate = desiredUpdateReader.ReadInt32();//The desired current update number for this saveable

				//Whether or not we have to load up all the in between 
				if(s.savesUpdates){
					int saveableOldCurrUpdate = GetCurrUpdateNumber(s);//The "current" (about to be prev) update number for this saveable
					if(saveableOldCurrUpdate!=saveableNewCurrUpdate){
						//Determine the amount to change the curr value by
						int updateVal = (saveableNewCurrUpdate-saveableOldCurrUpdate)/Mathf.Abs(saveableNewCurrUpdate-saveableOldCurrUpdate);
						//Load until we have hit the new current update value
						while(saveableOldCurrUpdate!=saveableNewCurrUpdate){
							LoadUpdate(s,saveableOldCurrUpdate,updateVal>0);//Load this new update as additive if its later, or not if its earlier
							saveableOldCurrUpdate+=updateVal;
						}
					}
					
				}
				else{
					if(saveableNewCurrUpdate!=GetCurrUpdateNumber(s)){
						LoadUpdate(s,saveableNewCurrUpdate,true);//Load this new update
					}
				}
				SetCurrUpdateNumber(s,saveableNewCurrUpdate);//Set the current update to this new value for this Saveable
			}
			desiredUpdateReader.Close();//Close out the BinaryReader now that we're done with it
			SetGeneralUpdateNumber(currUpdate);
			return true;
		}
		SetGeneralUpdateNumber(currUpdate);
		//If it does not exist, we can't load to that
		return false;
	}

	#endregion LOADING

	//Returns the current update number for the passed in Saveable
	private int GetCurrUpdateNumber(Saveable s){
		return PlayerPrefs.GetInt(s.playerpref, -1);
	}

	//Sets the PlayerPref update number for the passed in Saveable
	private void SetCurrUpdateNumber(Saveable s, int currUpdate){
		PlayerPrefs.SetInt(s.playerpref,currUpdate);
	}

	//Increment the current update number and store it in the Saveable's PlayerPrefs
	private void IncrementCurrUpdateNumber(Saveable s){
		int currUpdate = GetCurrUpdateNumber(s);
		currUpdate+=1;
		SetCurrUpdateNumber(s,currUpdate);
	}

	//Returns the current general update number for the savemanager
	public int GetGeneralUpdateNumber(){
		return PlayerPrefs.GetInt(UPDATE_PLAYERPREF, -1);
	}
	
	//Sets the current general update number for the savemanager
	private void SetGeneralUpdateNumber(int value){
		PlayerPrefs.SetInt(UPDATE_PLAYERPREF, value);
	}

	//Increment current general update number for the savemanager
	private void IncrementGeneralUpdateNumber(){
		int currUpdate = GetGeneralUpdateNumber();
		currUpdate+=1;
		SetGeneralUpdateNumber(currUpdate);
	}


}
