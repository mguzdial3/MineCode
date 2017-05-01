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

/**
 * NPCUnit is the main controller for a single NPC unit
 */
public class NPCUnit : MonoBehaviour {
	//Reference to all the various controllers for this NPC unit
	public NPCMovementController movementController;
	public NPCAppearanceController appearanceController;

	//Information to use for saving a single NPC in NPCManager
	public int Appearance { get { return appearanceController.ColorIndex; } }
	public Vector3 Position { get { return transform.position; } }
	public Vector3 Goal { get { return movementController.GetGoal (); } }
	
	//Called to update this unit, returns true if anything in this unit has changed
	public bool UpdateUnit(){
		bool changed = false;

		//Update movement, and determine if movement change occured
		if (movementController.UpdateMovement ()) {
			changed = true;
		}

		//Update appearance, and determine if appearance change occured
		if (appearanceController.UpdateAppearance ()) {
			changed = true;
		}

		return changed;
	}

	public Color GetColor(){
		return appearanceController.GetColor ();
	}

	public void SetInvisible(){
		appearanceController.SetInvisible ();
		movementController.SetPause (true);
	}
	
	public void SetVisible(){
		appearanceController.SetVisible ();
		movementController.SetPause (false);
	}

	public Vector3 GetGoal(){
		return movementController.GetGoal ();
	}

	public void SetCurrGoal(Vector3 goal){
		movementController.SetCurrGoal (goal);
	}

}
