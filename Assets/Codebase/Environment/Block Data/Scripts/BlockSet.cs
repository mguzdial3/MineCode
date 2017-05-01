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
using System.Collections.Generic;

/**
 * This script is an infoholder for a given BlockSet (a collection of primitive Blocks and that Atlases used to visualize them)
 */
public class BlockSet : MonoBehaviour {
	
	[SerializeField] private Atlas[] atlases = new Atlas[0]; //The list of atlases this BlockSet uses (initially empty)
	[SerializeField] private List<Cube> cubes = new List<Cube>(); //A list of Cubes, a kind of Block
	[SerializeField] private List<Cross> crosses = new List<Cross>();//A list of Cross, a kind of Block
	
	private Material[] materials; //The list of materials (from the Atlases)

	/**
	 * Init sets up the list of materials from the atlases and initializes each of the primitive Blocks
	 */
	public void Init() {
		materials = new Material[atlases.Length];
		for(int i=0; i<materials.Length; i++) {
			materials[i] = atlases[i].GetMaterial();
		}
		
		for(int i=0; i<GetCount(); i++) {
			GetBlock(i).Init(this, i);
		}
	}
	
	public void SetAtlases(Atlas[] atlases) {
		this.atlases = atlases;
	}
	public Atlas[] GetAtlases() {
		return atlases;
	}
	
	/**
	 * Add a new primitive Block object to the BlockSet, putting it into one of the two lists based on type.
	 * Returns the index/id of the block in the BlockSet
	 */
	public int Add(Block block) {
		if(block is Cube) {
			cubes.Add( (Cube)block );
			return cubes.Count-1;
		}
		if(block is Cross) {
			crosses.Add( (Cross)block );
			return cubes.Count + crosses.Count-1;
		}
		return -1;
	}

	/**
	 * Remove a Block from one of the two lists based on its id/index
	 */
	public void Remove(int index) {
		if(index >= 0 && index < cubes.Count) {
			cubes.RemoveAt(index);
			return;
		}
		index -= cubes.Count;
		if(index >= 0 && index < crosses.Count) {
			crosses.RemoveAt(index);
			return;
		}
	}
	
	public Block GetBlock(int index) {
		if(index >= 0 && index<cubes.Count) return cubes[index];
		index -= cubes.Count;
		if(index >= 0 && index < crosses.Count) return crosses[index];
		return null;
	}
	
	public Block GetBlock(string name) {
		foreach(Block block in cubes) {
			if(block.GetName() == name) return block;
		}
		foreach(Block block in crosses) {
			if(block.GetName() == name) return block;
		}
		return null;
	}

	public string[] GetStringArray(){
		string[] toReturn = new string[cubes.Count+crosses.Count];
		int index = 0;
		foreach(Block block in cubes) {
			toReturn[index] = block.GetName();
			index++;
		}
		foreach(Block block in crosses) {
			toReturn[index] = block.GetName();
			index++;
		}

		return toReturn;
	}
	
	public int GetCount() {
		return cubes.Count + crosses.Count;
	}
	
	public Material[] GetMaterials() {
		return materials;
	}

	public int GetBlockIndex(Block block){
		int index = cubes.IndexOf ((Cube)block);

		if (index == -1) {
			index = crosses.IndexOf((Cross) block)+cubes.Count;	
		}

		return index;
	}
	
}
