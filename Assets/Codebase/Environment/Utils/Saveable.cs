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

using System.IO;

/**
 * Interface for those classes capable of saving
 */
public interface Saveable {

	//Whether this Saveable saves its entirety every time (false) or just updates (true)
	bool savesUpdates {
		get;
	}

	//Filename for the saves for this 
	string filename {
		get;
	}

	//PlayerPref string to get the current update number for this 
	string playerpref{
		get;
	}

	//Whether or not this saveable has changes to save
	bool HasChanges();
	
	//Use this for a full save (used initially only for Saveables that saveUpdates; used every time for Saveables that 
	void WriteFullSave(BinaryWriter wr);
	
	//Handles the update of this save. Either a full save (if savesUpdate false) or just an update (if savesUpdate true)
	void WriteUpdateSave(BinaryWriter wr);
	
	//Loads an initial save
	void LoadSave(BinaryReader br);
	
	//Loads an update (or an initial save if savesUpdates is false). Load determines if it's loading or unloading this update.
	void LoadUpdate(BinaryReader br, bool load);

}
