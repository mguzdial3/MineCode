/**
 * Copyright (c) 2015 Entertainment Intelligence Lab.
 * Originally developed by Denis Ermolenko.
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
 * Selector handles the player's selection, both in terms of adding and removing blocks, 
 * and selecting a current NPC  
 */
public class Selector : MonoBehaviour {
	//The transform for the player camera
	private Transform cameraTrans;
	//Current block selected
	private Block selectedBlock;
	//Max Distance for the player to "reach"
	private float _distanceOfRaycast=30f;
	//Width of ray to send out
	private float _rayWidth = 0.01f;
	//Width of cube gizmo around selected block
	private float _gizmoSize = 1.05f;

	public Color gizmoColor = Color.white;

	//The number of blocks placed since blocksPlaced was last called (used in Lesson One)
	private static int _blocksPlaced=0;
	//The getter for _blocksPlaced, also resets the value (used in Lesson One)
	public static int blocksLastTick {
		get {
			int tempBlocksPlaced = _blocksPlaced; 
			_blocksPlaced = 0;
			return tempBlocksPlaced;
		}
	}

	//Called at start of play
	void Start() {
		//Grab the camera's transform to determine player line of sight
		cameraTrans = transform.GetComponentInChildren<Camera>().transform;
	}

	//Sets the passed in value to the selected block
	public void SetSelectedBlock(Block block) {
		selectedBlock = block;
	}

	//Get the selected block
	public Block GetSelectedBlock() {
		if (selectedBlock == null) {
			selectedBlock = Map.Instance.GetBlockByIndex (0);
		}

		return selectedBlock;
	}

	//Tries to get an NPC based on the player's current view
	public NPCMovementController TryGetSelectedNPC(){
		RaycastHit hit;
		NPCMovementController npcMovement = null;
		if(Physics.Raycast(cameraTrans.position,cameraTrans.forward,out hit, _distanceOfRaycast)){
			if(hit.collider!=null){
				npcMovement = hit.collider.GetComponent<NPCMovementController>();
			}
		}
		return npcMovement;
	}

	//Get a desired goal position from the player's current view
	public Vector3 GetGoalPositionFromLook(){
		RaycastHit hit;
		if(Physics.Raycast(cameraTrans.position,cameraTrans.forward,out hit,_distanceOfRaycast)){
			return hit.point;
		}
		return default(Vector3);
	}
	
	// Update is called once per frame
	void Update () {
		//If the cursor is visible; don't select anything
		if(Cursor.visible) return;

		//Delecte the current block
		if(Input.GetMouseButtonDown(0)) {
			if(!ItemHandler.Instance.UseHeldItem()){
				Vector3i? point = GetCursor(true);
				if(point.HasValue){
					Map.Instance.SetBlockAndRecompute(null, point.Value);
				}
			}
		}

		//Place the selected block
		if(Input.GetMouseButtonDown(1)) {
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				//Increment the number of placed blocks
				_blocksPlaced++;
				BlockData block = new BlockData( GetSelectedBlock() );
				block.SetRotation( GetRotation(-transform.forward) );
				Map.Instance.SetBlockAndRecompute(block, point.Value);
			}
		}
	}

	//Draw a cube around the selected block
	void OnDrawGizmos() {
		if(!Application.isPlaying) return;
		Vector3i? cursor = GetCursor(true);
		if(cursor.HasValue) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawWireCube( new Vector3(cursor.Value.x,cursor.Value.y,cursor.Value.z), Vector3.one*_gizmoSize );
		}
	}

	//Get current point if it has a value return null otherwise
	private Vector3i? GetCursor(bool inside) {
		Ray ray = new Ray(cameraTrans.position, cameraTrans.forward);
		Vector3? point =  RayBoxCollision.Intersection(Map.Instance, ray, _distanceOfRaycast/3);
		if(point.HasValue) {
			Vector3 pos = point.Value;
			if(inside) pos += ray.direction*_rayWidth;
			if(!inside) pos -= ray.direction*_rayWidth;
			int posX = Mathf.RoundToInt(pos.x);
			int posY = Mathf.RoundToInt(pos.y);
			int posZ = Mathf.RoundToInt(pos.z);
			return new Vector3i(posX, posY, posZ);
		}
		return null;
	}

	//Helper method to get the rotation of a block in terms of the passed in vector
	private static BlockDirection GetRotation(Vector3 dir) {
		if( Mathf.Abs(dir.z) >= Mathf.Abs(dir.x) ) {
			// 0 to 180
			if(dir.z >= 0) return BlockDirection.Z_PLUS;
			return BlockDirection.Z_MINUS;
		} else {
			// 90 to 270
			if(dir.x >= 0) return BlockDirection.X_PLUS;
			return BlockDirection.X_MINUS;
		}
	}

}
