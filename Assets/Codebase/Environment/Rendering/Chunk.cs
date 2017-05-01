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
 * Chunk represents a "chunk" of Map. Needs to be used in order to get around Unity render limitations.
 * Handles both lighting and Block info for this chunk.
 */
public class Chunk {

	//The position in world (unity) coordinates of 
	private Vector3i minPosition;
	public Vector3i Min { get { return minPosition; } }

	//Data representation of this chunk of blocks
	private BlockData[][][] blocks;
	//The held lighting information for this chunk
	private byte[][][] lighting; 

	//The size of each chunk in terms of its diameter in blocks
	public static readonly int CHUNK_SIZE = 10;

	//The rendering mesh for this chunk
	private MeshFilter meshFilter;
	//The collision mesh for this chunk
	private MeshCollider meshCollider;
	//The internal representation of the mesh
	private MeshData meshData;

	//Constructor for Chunk
	public Chunk(Vector3i _position){
		minPosition = _position;
		//Block array instantiation
		blocks = new BlockData[CHUNK_SIZE][][];
		for (int x = 0; x<CHUNK_SIZE; x++) {
			blocks[x] = new BlockData[CHUNK_SIZE][];
			for(int y = 0; y<CHUNK_SIZE; y++){
				blocks[x][y] = new BlockData[CHUNK_SIZE];
			}
		}
		//Lighting array instantiation
		lighting = new byte[CHUNK_SIZE][][];
		for (int x = 0; x<CHUNK_SIZE; x++) {
			lighting[x] = new byte[CHUNK_SIZE][];
			for(int y = 0; y<CHUNK_SIZE; y++){
				lighting[x][y] = new byte[CHUNK_SIZE];
				for(int z = 0; z<CHUNK_SIZE; z++){
					lighting[x][y][z] = LightComputer.MAX_LIGHT;
				}
			}
		}

		//Gameobject creation
		GameObject go = new GameObject ("Chunk " + minPosition.x + " " + minPosition.y + " " + minPosition.z);
		go.transform.position = new Vector3 (minPosition.x, minPosition.y, minPosition.z);

		//Mesh creation
		go.AddComponent<MeshRenderer> ().sharedMaterials = Map.Instance.GetBlockSet().GetMaterials();
		meshFilter = go.AddComponent<MeshFilter> ();
		go.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		go.GetComponent<Renderer>().receiveShadows = false;
		meshCollider = go.AddComponent<MeshCollider> ();
		meshData = new MeshData (2);
	}

	//Build the actual mesh for rendering from blocks 
	public void BuildMesh(){
		meshCollider.sharedMesh = null;
		if (meshFilter.mesh != null) {
			meshCollider.sharedMesh = meshFilter.mesh;
		}
		//meshFilter.mesh.colors32 = meshData.colors32.ToArray();

		//Clear away old meshData
		meshData.Clear();
		for(int x=0; x<blocks.Length; x++) {
			for(int y=0; y<blocks[x].Length; y++) {
				for(int z=0; z<blocks[x][y].Length; z++) {
					BlockData blockData = blocks[x][y][z];
					if(blockData!=null){
						Block block = blockData.block;
						if(block != null) {
							Vector3i localPos = new Vector3i(x, y, z);
							Vector3i worldPos = new Vector3i(minPosition.x+x, minPosition.y+y, minPosition.z+z);
							block.Build(localPos, worldPos, meshData, false);
						}
					}
				}
			}
		}

		Mesh mesh = meshData.ToMesh (meshFilter.mesh);
		meshFilter.mesh =mesh;
		
		meshCollider.sharedMesh = null;
		if (meshFilter.mesh != null) {
			meshCollider.sharedMesh = meshFilter.mesh;
		}
	}

	//Use this method to destroy this Chunk's mesh
	public void DestroyMesh(){
		GameObject.Destroy (meshCollider.gameObject);
	}

	//Alter MinPosition to a given value and update the gameobject's position
	public void UpdatePosition(Vector3i updatePosition){
		minPosition = updatePosition;
		meshFilter.gameObject.transform.position = new Vector3 (minPosition.x, minPosition.y, minPosition.z);
	}

	//Set BlockData (block) based on world position (worldVec).
	public void SetBlock(Vector3i worldVec, BlockData block){
		Vector3i localPos = WorldToLocalPosition (worldVec);
		if (localPos.x < CHUNK_SIZE && localPos.y < CHUNK_SIZE && localPos.z < CHUNK_SIZE) {
			blocks [localPos.x] [localPos.y] [localPos.z] = block;
		}
	}

	//Get BlockData based on world (unity) space
	public BlockData GetBlock(Vector3i worldVec){
		Vector3i localPos = WorldToLocalPosition (worldVec);

		if (localPos.x < CHUNK_SIZE && localPos.y < CHUNK_SIZE && localPos.z < CHUNK_SIZE) {
			return blocks [localPos.x] [localPos.y] [localPos.z];
		} else {
			return null;
		}
	}

	//Set light (byte) based on world position (worldVec).
	public void SetLight(Vector3i worldVec, byte _light){
		Vector3i localPos = WorldToLocalPosition (worldVec);
		if (localPos.x < CHUNK_SIZE && localPos.y < CHUNK_SIZE && localPos.z < CHUNK_SIZE) {
			lighting [localPos.x] [localPos.y] [localPos.z] = _light;
		}
	}

	//Get light (byte) based on world position (worldVec)
	public byte GetLight(Vector3i worldVec){
		Vector3i localPos = WorldToLocalPosition (worldVec);

		if (localPos.x < CHUNK_SIZE && localPos.y < CHUNK_SIZE && localPos.z < CHUNK_SIZE) {
			return lighting [localPos.x] [localPos.y] [localPos.z];
		} else {
			return LightComputer.MIN_LIGHT+LightComputer.STEP_LIGHT;
		}
	}

	//World (unity) to Local Position Translation for this Chunk
	public Vector3i WorldToLocalPosition(Vector3i worldVec){
		Vector3i transition = new Vector3i (worldVec.x - minPosition.x, worldVec.y - minPosition.y, worldVec.z - minPosition.z);
		return transition;
	}

	//Local to World (unity) Position Translation for this Chunk
	public Vector3i LocalToWorldPosition(Vector3i localVec){
		return new Vector3i(localVec.x+minPosition.x,localVec.y+minPosition.y, localVec.z+minPosition.z);
	}
	
	//Translation of a position from World (unity) space to Chunk (Map chunks) space
	public static Vector3i WorldToChunkSpace(Vector3i vec){
		return new Vector3i(WorldToChunkSpace(vec.x), WorldToChunkSpace(vec.y),WorldToChunkSpace(vec.z));
	}

	//Translation of a single value from World (unity) space to Chunk (Map chunks) space
	public static int WorldToChunkSpace(int val){
		if (val >= 0) {//positive chunk values can just be brought to the nearest 16th multiple
			return (val / CHUNK_SIZE);
		} else {//negative chunks have to be special cased as otherwise -1 to -16 would go to 0 to -1 instead of all to -1
			if (val%CHUNK_SIZE==0){
				val+=1;	
			}
			return (val /CHUNK_SIZE)-1;
		}
	}
}
