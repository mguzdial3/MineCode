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
using System.IO;
using System.Collections;
using System.Collections.Generic;


/**
 * Map handles the representation of the collection of blocks that compose the map.
 * It also handles saving and loading of this information via the Saveable interface.
 */
public class Map: Saveable {
	//The set of blocks being used for this map
	private BlockSet blockSet;

	//Representation of blocks on map (broken into chunks)
	private Chunk[][][] chunks = null;

	//Initial Size of the map
	private const int INITIAL_SIZE = 10;
	//Place the BlockData starts in chunk space
	private Vector3i minimum; 
	public Vector3i Minimum { get { return minimum; } }

	//Save information
	private List<string> changeList;
	public bool savesUpdates { get { return true; } }
	public string filename  { get { return "mapSave"; } }
	public string playerpref  { get { return "MAP PLAYERPREF"; } }

	//Instance reference for singleton behavior
	public static Map Instance;

	public Map(BlockSet _blockSet) {
		this.blockSet = _blockSet;
		//Initial set up of chunks of initial size
		chunks = new Chunk[INITIAL_SIZE][][];
		for (int x = 0; x<chunks.Length; x++) {
			chunks[x] = new Chunk[INITIAL_SIZE][];
			for(int y = 0; y<chunks[x].Length; y++){
				chunks[x][y] = new Chunk[INITIAL_SIZE];
			}
		}

		minimum = new Vector3i (-INITIAL_SIZE/2.0f, -INITIAL_SIZE/2.0f, -INITIAL_SIZE/2.0f);

		Instance = this;		
		changeList = new List<string> ();
	}

	/**
	 * Build out the mesh used for rendering.
	 */
	public void BuildMesh(){
		//Compute initial lighting
		for (int x = GetMinX(); x<GetMaxX(); x++) {

			for (int y = GetMaxY(x)-1; y>=GetMinY(); y--) {
				for (int z = GetMinZ(); z<GetMaxZ(x,y); z++) {
					byte light = LightComputer.MAX_LIGHT;
					Vector3i pos = new Vector3i(x,y,z);
					//Determine current light value
					if(GetBlock(pos+Vector3i.up)!=null){
						light = LightComputer.MIN_LIGHT+LightComputer.STEP_LIGHT;
					}
					else if(y<GetMaxY(x)-1){
						light = (byte)(GetLight(pos+Vector3i.up)+LightComputer.STEP_LIGHT);

						if(light>LightComputer.MAX_LIGHT){
							light = LightComputer.MAX_LIGHT;
						}
					}

					BlockData b = GetBlock(pos);
					if(b==null || b.IsAlpha()){
						SetLight(light,pos);//If we have no block there set the curr light
					}
					else{
						SetLight(LightComputer.MIN_LIGHT+LightComputer.STEP_LIGHT,pos);//If block there just set to min light
					}
				}
			}
		}

		//Render each chunk
		for (int x = 0; x<chunks.Length; x++) {
			for(int y = 0; y<chunks[x].Length; y++){
				for(int z = 0; z<chunks[x][y].Length; z++){
					if(chunks[x][y][z]!=null){
						chunks[x][y][z].BuildMesh();
					}
				}
			}
		}
	}



	//Get length of map along x dimension
	public int GetLengthDimension(int dimension){
		return GetLengthDimension (dimension, 0, 0);
	}

	//Get length of map along y dimension for a particular x
	public int GetLengthDimension(int dimension, int x){
		return GetLengthDimension (dimension, x, 0);
	}

	//Get the length of the map within a certain dimension as expressed in world (unity) space
	public int GetLengthDimension(int dimension, int x, int y){
		x -= minimum.x;
		y -= minimum.y;
		if (dimension == 0) {
			return chunks.Length*Chunk.CHUNK_SIZE;
		} else if (dimension == 1) {
			return chunks [x].Length*Chunk.CHUNK_SIZE;
		} else if(dimension==2){
			return chunks[x][y].Length*Chunk.CHUNK_SIZE;
		}

		return -1;
	}

	#region SAVING

	//Returns true if there are any changes to save
	public bool HasChanges(){
		return changeList.Count != 0;
	}

	//Use this for a full save (used initially only for Saveables that saveUpdates; used every time for Saveables that 
	public void WriteFullSave(BinaryWriter wr){
		List<BlockData> blocks = new List<BlockData> (); //List for blocks to store
		List<Vector3i> positions = new List<Vector3i> (); //blocks don't hold their positions so we need to store this separately

		//First grab all the info we need to save
		for(int x = 0; x<chunks.Length; x++){
			for(int y = 0; y<chunks[x].Length; y++){
				for(int z = 0; z<chunks[x][y].Length; z++){
					//Determine if chunk is even non null
					if(chunks[x][y][z]!=null){
						//Iterate through chunk to get all the blocks
						for(int cx = 0; cx<Chunk.CHUNK_SIZE; cx++){
							for(int cy = 0; cy<Chunk.CHUNK_SIZE; cy++){
								for (int cz = 0; cz<Chunk.CHUNK_SIZE; cz++){
									Vector3i worldPos = LocalToWorld(new Vector3i(x,y,z));
									worldPos+=new Vector3i(cx,cy,cz);
									BlockData b = chunks[x][y][z].GetBlock(worldPos);
									//If non null, add the blocks and positions info
									if(b!=null){
										blocks.Add(b);
										positions.Add(worldPos);
									}
								}
							}
						}
					}
				}
			}
		}

		//Actually save the information
		wr.Write (blocks.Count);//First store the number of blocks we'll be storing

		//Save out all the information
		for (int i = 0; i<blocks.Count; i++) {
			wr.Write((""+blocks[i].BlockId)); //Save the id as a string as we may have -0 values
			wr.Write(positions[i].x);
			wr.Write(positions[i].y);
			wr.Write(positions[i].z);
			wr.Write(blocks[i].Rotation);
		}
	}
	
	//Handles the update of this save. Either a full save (if savesUpdate false) or just an update (if savesUpdate true)
	public void WriteUpdateSave(BinaryWriter wr){
		//First off save the number of blocks to store
		wr.Write (changeList.Count);

		for (int i = 0; i<changeList.Count; i++) {
			string[] splits = changeList[i].Split(new string[]{","}, StringSplitOptions.None);
			wr.Write(splits[0]);
			for(int s = 1; s<splits.Length; s++){
				wr.Write(int.Parse(splits[s]));
			}
		}
		changeList = new List<string> ();
	}
	
	//Loads a full save
	public void LoadSave(BinaryReader br){
		//Reset any and all chunk values
		Chunk[][][] newChunks = new Chunk[chunks.Length][][];
		for (int x = 0; x<chunks.Length; x++) {
			newChunks[x] = new Chunk[chunks[x].Length][];
			for (int y = 0; y<chunks[x].Length; y++){
				newChunks[x][y] = new Chunk[chunks[x][y].Length];
				for(int z = 0; z<chunks[x][y].Length; z++){
					if(chunks[x][y][z]!=null){
						chunks[x][y][z].DestroyMesh();
					}
				}
			}
		}
		chunks = newChunks;//Set chunks to the empty new chunks

		//Load save from initial values
		int countToLoad  = br.ReadInt32 ();

		for (int i = 0; i<countToLoad; i++) {
			int blockIndex = int.Parse(br.ReadString());
			int x = br.ReadInt32();
			int y = br.ReadInt32();
			int z = br.ReadInt32();
			int r = br.ReadInt32();

			SetBlockNoSave(blockIndex,x,y,z,r);
		}

		BuildMesh (); //Rebuild the mesh to store these changes
	}
	
	//Loads an update (or an initial save if savesUpdates is false)
	public void LoadUpdate(BinaryReader br, bool load){
		//Load save from initial values
		int countToLoad  = br.ReadInt32 ();

		for (int i = 0; i<countToLoad; i++) {
			string blockString = (br.ReadString());

			int x = br.ReadInt32();
			int y = br.ReadInt32();
			int z = br.ReadInt32();
			int r = br.ReadInt32();

			//Determine whether to add this block or remove it
			bool add = (load && blockString[0]!='-') || (!load && blockString[0] == '-');

			//Determine blockIndex
			int blockIndex = -1;
			if(blockString[0]!='-'){
				blockIndex = int.Parse(blockString);
			}
			else{
				blockIndex = int.Parse(blockString.Substring(0));
			}

			if (add){//Add the block
				SetBlockIndexAndRecompute(blockIndex,x,y,z,r, false);
			}
			else{//Remove the block
				SetBlockIndexAndRecompute(-1, x,y, z, -1, false);
			}
		}
	}

	#endregion SAVING

	//Minimum Getters
	//x is in world (unity) space
	public int GetMinX(){
		return minimum.x*Chunk.CHUNK_SIZE;
	}

	//y is in world (unity) space
	public int GetMinY(){
		return minimum.y*Chunk.CHUNK_SIZE;
	}

	//z is in world (unity) space
	public int GetMinZ(){
		return minimum.z*Chunk.CHUNK_SIZE;
	}

	//Maximum Getters
	public int GetMaxX(){//Maximum X in World Space
		return minimum.x*Chunk.CHUNK_SIZE+chunks.Length*Chunk.CHUNK_SIZE;
	}

	//x is in world (unity) space
	public int GetMaxY(int worldX){//Maximum Y in World Space at X x
		int chunkX = Chunk.WorldToChunkSpace (worldX);
		return minimum.y*Chunk.CHUNK_SIZE + chunks[chunkX-minimum.x].Length*Chunk.CHUNK_SIZE;
	}

	//x and y are in world (unity) space
	public int GetMaxZ(int worldX, int worldY){//Maximum Z in World Space at X x and Y y
		int chunkX = Chunk.WorldToChunkSpace (worldX);
		int chunkY = Chunk.WorldToChunkSpace (worldY);
		return minimum.z*Chunk.CHUNK_SIZE+chunks[chunkX-minimum.x][chunkY-minimum.y].Length*Chunk.CHUNK_SIZE;
	}

	//Transfer from local (chunk) space to world (unity) space
	public Vector3i LocalToWorld(Vector3i pos){
		return new Vector3i (pos.x*Chunk.CHUNK_SIZE + minimum.x*Chunk.CHUNK_SIZE, pos.y*Chunk.CHUNK_SIZE + minimum.y*Chunk.CHUNK_SIZE, pos.z*Chunk.CHUNK_SIZE + minimum.z*Chunk.CHUNK_SIZE);
	}

	//Transfer from world (unity) space to local (array) space
	public Vector3i WorldToLocal(Vector3i pos){
		Vector3i chunkPos = Chunk.WorldToChunkSpace (pos);
		return new Vector3i (chunkPos.x - minimum.x, chunkPos.y - minimum.y, chunkPos.z - minimum.z);
	}

	//Make a column of space buildable
	public void MakeColumnBuildable(int cx, int cz, int height) {
		for(int cy=minimum.y; cy<height; cy++) {
			MakeBuildable( new Vector3i(cx, cy, cz) );
		}
	}

	//Make an specific position buildable in world (unity) space 
	private bool MakeBuildable(Vector3i pos) {
		bool positionChanged = false;

		//X update
		if (pos.x < GetMinX()) {//If this x is less than the minimum
			int difference = (GetMinX()-pos.x)/Chunk.CHUNK_SIZE;
			difference+=1;
			Chunk[][][] newChunks = new Chunk[chunks.Length+difference][][];
			Array.Copy(chunks,0,newChunks,difference,chunks.Length);
			for(int x = 0; x<difference; x++){
				newChunks[x] = new Chunk[INITIAL_SIZE][];
				for(int y = 0; y<INITIAL_SIZE; y++){
					newChunks[x][y] = new Chunk[INITIAL_SIZE];
				}
			}
			minimum.x = minimum.x-difference;
			chunks = newChunks;
			positionChanged = true;
		} else if (pos.x >= GetMaxX()) {//If this x is greater than the maximum
			int difference = (pos.x-GetMaxX())/Chunk.CHUNK_SIZE;
			difference+=1;
			Chunk[][][] newChunks = new Chunk[chunks.Length+difference][][];
			Array.Copy(chunks,newChunks,chunks.Length);
			for(int x = chunks.Length; x<newChunks.Length; x++){
				newChunks[x] = new Chunk[INITIAL_SIZE][];
				for(int y = 0; y<INITIAL_SIZE; y++){
					newChunks[x][y] = new Chunk[INITIAL_SIZE];
				}
			}
			chunks = newChunks;
		}

		//Y Update
		if (pos.y < GetMinY()) {//If this y is less than the minimum
			int difference = (GetMinY()-pos.y)/Chunk.CHUNK_SIZE;
			difference+=1;
			//Copy over all changes to y dimension
			for(int x = 0; x<chunks.Length; x++){
				Chunk[][] newChunks = new Chunk[chunks[x].Length+difference][];
				Array.Copy(chunks[x],0,newChunks,difference,chunks[x].Length);
				for(int y = 0; y<difference; y++){
					newChunks[y] = new Chunk[INITIAL_SIZE];
				}
				chunks[x] = newChunks;
			}
			minimum.y = minimum.y-difference;
			positionChanged = true;
		} else if (pos.y >= GetMaxY(pos.x)) {//If this y is greater than the maximum
			int difference = (pos.y-GetMaxY(pos.x))/Chunk.CHUNK_SIZE;
			difference+=1;
			//Copy over all changes to y dimension
			for(int x = 0; x<chunks.Length; x++){
				Chunk[][] newChunks = new Chunk[chunks.Length + difference][];
				Array.Copy(chunks[x],newChunks,chunks[x].Length);
				for(int y = chunks[x].Length; y<newChunks.Length; y++){
					newChunks[y] = new Chunk[INITIAL_SIZE];
				}
				chunks[x] = newChunks;
			}
		}

		//Z Update 
		if (pos.z < GetMinZ()) {//If this z is less than the minimum
			int difference = (GetMinZ()-pos.z)/Chunk.CHUNK_SIZE;
			difference+=1;
			//Copy over all changes to z dimension
			for(int x = 0; x<chunks.Length; x++){
				for(int y = 0; y<chunks[x].Length; y++){
					Chunk[] newChunks = new Chunk[chunks[x][y].Length+difference];
					Array.Copy (chunks[x][y], 0, newChunks, difference, chunks[x][y].Length);
					chunks[x][y] = newChunks;
				}
			}
			minimum.z = minimum.z-difference;
			positionChanged = true;
		} else if (pos.z >= GetMaxZ(pos.x,pos.y)) {//If this z is greater than the maximum
			int difference = (pos.z-GetMaxZ(pos.x,pos.y))/Chunk.CHUNK_SIZE;
			difference+=1;
			//Copy over all changes to z dimension
			for(int x = 0; x<chunks.Length; x++){
				for(int y = 0; y<chunks[x].Length; y++){
					Chunk[] newChunks = new Chunk[chunks[x][y].Length+difference];
					Array.Copy(chunks[x][y],newChunks,chunks[x][y].Length);
					chunks[x][y] = newChunks;
				}
			}
		}
		return positionChanged;
	}

	//Returns whether or not the map has a blockset
	private bool HasBlockSet(){
		return blockSet!=null;
	}

	//Get's a block's id at a given position
	public int GetBlockIdentityAtPosition(int x, int y, int z){
		BlockData block = GetBlock (x, y, z);
		if (block != null) {
			return block.BlockId;
		}
		return -1;
	}

	#region SETBLOCK
	//Call this to set a block and save it. Set block based on x, y, and z coordinates in world (unity) space.
	public void SetBlockSave(int blockIndex, int x, int y, int z){
		BlockData b = new BlockData (GetBlockByIndex (blockIndex));
		SetBlock (b, x, y, z, true);
	}

	public void SetBlockSave(BlockData block, int x, int y, int z){
		SetBlock (block, x, y, z, true);
	}

	//Call this to set a block and not save it (during construction of save state). 
	public void SetBlockNoSave(int blockIndex, int x, int y, int z) {
		BlockData b = new BlockData (GetBlockByIndex (blockIndex));
		SetBlockNoSave (b, x, y, z);
	}

	//Call this to set a block and not save it (during construction of save state). 
	public void SetBlockNoSave(int blockIndex, int x, int y, int z, int rotation) {
		BlockData b = new BlockData (GetBlockByIndex (blockIndex));
		b.SetRotation ((BlockDirection)rotation);
		SetBlockNoSave(b, x, y, z);
	}

	public void SetBlockNoSave(BlockData block, int x, int y, int z) {
		SetBlock (block, x, y, z, false);
	}

	//Use this only for loading up new states
	private void SetBlockIndexAndRecompute(int blockIndex, int x, int y, int z, int rotation, bool save){
		BlockData b = null;
		if (blockIndex != -1) {
			b = new BlockData(GetBlockByIndex(blockIndex));
			b.SetRotation ((BlockDirection)rotation);
		}

		SetBlockAndRecompute (b, new Vector3i (x, y, z), save);
	}

	//Sets block at position and recomputes mesh and lighting (use this for during game changes)
	public void SetBlockAndRecompute(BlockData block, Vector3i pos, bool save=true, bool overwrite=false) {
		Chunk c = SetBlock( block, pos.x,pos.y,pos.z, save, overwrite);
		List<Chunk> chunksToRebuild = new List<Chunk> ();//Store all of the chunks wose meshes we have to rebuild here
		chunksToRebuild.Add (c);

		//Update light below
		//Set the initial light values based on assumed directly above light source

		for (int x = -3; x<4; x++) {
			for (int z = -3; z<4; z++) {
				byte newLight = GetLight(new Vector3i (pos.x+x, pos.y+1, pos.z+z));
				int y = pos.y;
				bool secondBlockFound = false;
				while (y>=GetMinY() && !secondBlockFound) {
					Vector3i pos2 = new Vector3i (pos.x+x, y, pos.z+z);
					BlockData b = GetBlock (pos2);

					if (y != pos.y && b != null) {
						secondBlockFound = true;
					}

					if (b == null || b.IsAlpha ()) {
						SetLight (newLight, pos2);
					} else {
						newLight = LightComputer.MIN_LIGHT;
						SetLight (newLight, pos2);
					}

					Chunk c2 = GetChunk(pos2);
					if(c2!=null && !chunksToRebuild.Contains(c2)){
						chunksToRebuild.Add(c2);
					}

					y--;
				}
			}
		}
		if (c != null) {
			foreach (Chunk chunk in chunksToRebuild) {
				if(chunk!=null){
					chunk.BuildMesh ();
				}
			}
		}
	}

	//Set block based on x, y, and z coordinates in world (unity) space. save determines whether or not to add this to the change log
	private Chunk SetBlock(BlockData block, int x, int y, int z, bool save, bool overwrite=false) {
		MakeBuildable (new Vector3i (x, y, z));
		BlockData data = GetBlock (x, y, z);

		//Have to add in a check here to make sure we don't break non-breakable blocks
		if (block == null && data != null && !data.IsBreakable () && !overwrite) {
			return null;
		}

		if (save) {
			//Save it to the changelist as csv, so we can then recreate it for saving the update
			if (block != null && block.block != null) {
				changeList.Add (block.BlockId+"," + x + "," + y + "," + z + "," + block.Rotation);
			} else {
				if (data != null && data.block != null) {
					changeList.Add ("-"+data.BlockId+"," + x + "," + y + "," + z + "," + data.Rotation);//So we can have negative zeroes
				}
			}
		}
		Vector3i chunkSpace = WorldToLocal(new Vector3i (x, y, z));
		if (chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z] == null) {
			chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z] = new Chunk((chunkSpace+minimum)*Chunk.CHUNK_SIZE);
		}
		chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z].SetBlock (new Vector3i (x, y, z), block);

		return chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z];
	}
	#endregion SETBLOCK

	//Get a chunk based on world coordinates
	public Chunk GetChunk(Vector3i worldPos){
		Vector3i chunkSpace = WorldToLocal(worldPos);
		if(!InRange(worldPos) || chunks[chunkSpace.x][chunkSpace.y][chunkSpace.z]==null){
			return null;
		}
		return chunks[chunkSpace.x][chunkSpace.y][chunkSpace.z];
	}

	//Returns whether or not a specific position in chunkSpace is in range
	private bool InRange(Vector3i chunkSpace){
		return chunkSpace.x >= 0 && chunkSpace.x < chunks.Length && chunkSpace.y >= 0 && chunkSpace.y < chunks [chunkSpace.x].Length && chunkSpace.z >= 0 && chunkSpace.z < chunks [chunkSpace.x] [chunkSpace.y].Length;
	}

	#region GETBLOCK
	//Get Block at Position (returns null if position is not gettable)
	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}

	//Get Block at Position (returns null if position is not gettable) 
	public BlockData GetBlock(int x, int y, int z) {
		if (x >= GetMinX () && x < GetMaxX () && y >= GetMinY () && y < GetMaxY (x) && z >= GetMinZ () && z < GetMaxZ (x,y) ) {
			Vector3i chunkSpace = WorldToLocal(new Vector3i(x,y,z));
			if(chunks[chunkSpace.x][chunkSpace.y][chunkSpace.z]==null){
				return null;
			}
			return chunks[chunkSpace.x][chunkSpace.y][chunkSpace.z].GetBlock(new Vector3i(x,y,z));
		}
		return null;
	}
	#endregion GETBLOCK

	#region lighting

	//Set the light based on world (unity) position.
	public void SetLight(byte light, Vector3i pos){
		//Find the chunk
		Vector3i chunkSpace = WorldToLocal(pos);

		//Call set light on that chunk
		if (chunkSpace.x>=0&& chunkSpace.y>=0 && chunkSpace.z>=0 &&chunkSpace.x < chunks.Length && chunkSpace.y < chunks [chunkSpace.x].Length && chunkSpace.z < chunks [chunkSpace.x] [chunkSpace.y].Length) {
			if (chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z]!=null){
				chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z].SetLight (pos, light);
			}
		}
	}

	public byte GetLight(Vector3i pos){
		//Find the chunk
		Vector3i chunkSpace = WorldToLocal(pos);
		//return the light at that world position within the chunk
		if (chunkSpace.x>=0&& chunkSpace.y>=0 && chunkSpace.z>=0 &&chunkSpace.x < chunks.Length && chunkSpace.y < chunks [chunkSpace.x].Length && chunkSpace.z < chunks [chunkSpace.x] [chunkSpace.y].Length) {
			if(chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z]==null){
				return LightComputer.MAX_LIGHT;//If we have no chunk here return max light
			}
			return chunks [chunkSpace.x] [chunkSpace.y] [chunkSpace.z].GetLight (pos);
		} else {
			return LightComputer.MAX_LIGHT;//If we're outside of chunk space return max light
		}
	}

	#endregion lighting

	//Check equivalence of block type at position
	public bool CheckEquivalentBlocks(int blockIndex, int x, int y, int z){
		BlockData blockData = GetBlock(x,y,z);
		return (blockData!=null && blockData.block!=null && HasBlockSet() &&  blockData.block.GetName().Equals(blockSet.GetBlock(blockIndex).GetName()));
	}

	//Check if this position in the map is open
	public bool IsPositionOpen(Vector3i pos){
		BlockData block = GetBlock (pos);
		return (block==null || block.IsEmpty () || block.IsAlpha());
	}

	//Find the first empty position at a given x and y
	public Vector3i getEmptyLOC(Vector3i location){
		int checks = 0;
		while ((!IsPositionOpen(location) || ! IsPositionOpen(location-Vector3i.up)) && checks<10) {
			location+=Vector3i.up;

			checks+=1;
		}
		return location;
	}

	//Get Block by its index in the blockSet list
	public Block GetBlockByIndex(int index){
		return (blockSet!=null ? blockSet.GetBlock(index): null);
	}
	
	//Returns the map's block set
	public BlockSet GetBlockSet() {
		return blockSet;
	}
}