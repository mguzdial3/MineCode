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

using System;

/**
 * SubmarineGenerator is a child of Generator that handles creating a randomized submarine environment. 
 */
public class SubmarineGenerator: Generator {
	//The block indexes for each of the different submarine blocks
	private const int floorBlock = 0;
	private const int wallBlock1 = 2;
	private const int wallBlock2 = 2;
	private const int radioactive = 4;
	private const int radioactivity = 5;

	//Size of each of the sub rooms
	private const int ROOM_SIZE=16;//radius

	private int radioactiveRadius = 2;//The radius of radioactive blocks to place

	/**
	 *  GenerateMap is what's actually called to create the "Submarine" area via Map calls.
	 */
	public override void GenerateMap (){
		//This does the floor
		for (int i =-1*ROOM_SIZE; i<ROOM_SIZE; i++) {
			for (int j =-1*ROOM_SIZE; j<ROOM_SIZE; j++) {
				Vector3i current = new Vector3i(i,0,j);
				
				int cx = current.x;
				int cz = current.z;

				GenerateFloor(cx, cz);
				Map.Instance.MakeColumnBuildable(cx,cz, 10);
				
			}		
		}
		
		//Walls
		for (int i = -16; i<16; i++) {
			GenerateWall(i,-16);
			GenerateWall(i,16-1);
			
			if(Math.Abs(i)!=8){
				GenerateWall(i,0);
			}
		}
		for (int i = -16; i<16; i++) {
			GenerateWall(-16,i);
			GenerateWall(16-1,i);
			
			if(Math.Abs(i)!=8){
				GenerateWall(0,i);
			}
		}
		
		//Generate some random radioactivity
		int numRandomRadioactive = 8;
		
		while (numRandomRadioactive>0) {
			int x = UnityEngine.Random.Range(-16,16);
			int z = UnityEngine.Random.Range(-16,16);
			
			GenerateRadioactive(x,1,z);
			
			numRandomRadioactive--;	
		}
	}

	//Helper method to build a single point of the floor block
	private void GenerateFloor(int cx, int cz) {
		Map.Instance.SetBlockNoSave (floorBlock, cx, 1, cz);
	}

	//Helper method to build a column of wall
	private void GenerateWall(int cx, int cz) {
		Map.Instance.SetBlockNoSave (wallBlock1, cx, 2, cz);
		Map.Instance.SetBlockNoSave (wallBlock2,cx, 3, cz);
		Map.Instance.SetBlockNoSave (wallBlock2, cx, 4, cz);
	}

	//Helper method to generate out a radioactive area.
	private void GenerateRadioactive(int x, int y, int z) {
		Map.Instance.SetBlockNoSave(radioactive, x, y, z);
		for(int i = x-radioactiveRadius; i<x+1+radioactiveRadius; i++){
			for(int j = z-radioactiveRadius; j<z+1+radioactiveRadius; j++){
				if((i!=x || j!=z) && ((!Map.Instance.CheckEquivalentBlocks(radioactive,i,y,j)))){
					Map.Instance.SetBlockNoSave(radioactivity, i, y, j);
				}
			}
		}
	}
}


