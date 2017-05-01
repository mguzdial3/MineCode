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

//Locked rotation values
public enum BlockDirection {
	Z_PLUS,
	X_PLUS,
	Z_MINUS,
	X_MINUS
}

/**
 * An infoholder for an actual "block" on the map. Holds a primitive/archetypal Block reference 
 */
public class BlockData {
	public Block block;//Link to the type of block this is using
	private BlockDirection rotation;//Variable for the current rotation
	public int Rotation {get {return (int)rotation;}}//Public read-only variable for easy access to rotation for saving
	public int BlockId { get { return block.BlockId; } }//Public read-only variable for easy access to the blockId for saving
	
	
	public BlockData(Block block) {
		this.block = block;
		rotation = BlockDirection.Z_PLUS;
	}

	//Change the rotation according to the above enum
	public void SetRotation(BlockDirection rotation) {
		this.rotation = rotation;
	}

	public BlockDirection GetDirection() {
		return rotation;
	}

	//Get the light this "block" should cast. At default all return max light for now
	public byte GetLight() {
		return LightComputer.MAX_LIGHT;
	}
	
	public bool IsEmpty() {
		return block == null;
	}
	
	public bool IsAlpha() {
		return IsEmpty() || block.IsAlpha();
	}

	public bool IsBreakable(){
		return block.IsBreakable ();
	}
	
}
