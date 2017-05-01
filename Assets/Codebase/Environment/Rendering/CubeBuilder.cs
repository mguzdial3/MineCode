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
 * This script handles building up mesh information for Cubes 
 */
public class CubeBuilder {	
	
	//CubeFace handles the location of different faces in terms of ordered faces
	private static CubeFace[] faces = new CubeFace[] {
		CubeFace.Left,
		CubeFace.Right,
		
		CubeFace.Bottom,
		CubeFace.Top,
		
		CubeFace.Back,
		CubeFace.Front
	};

	//Defines directions of faces in a mesh
	private static Vector3i[] directions = new Vector3i[] {
		new Vector3i(-1, 0, 0), //left
		new Vector3i( 1, 0, 0), //right
		
		new Vector3i(0, -1, 0), //bottom
		new Vector3i(0,  1, 0), //top
		
		new Vector3i(0, 0, -1), //back
		new Vector3i(0, 0,  1)  //front
	};

	//Defines the ordering of vertices for a Cube
	private static Vector3[][] vertices = new Vector3[][] {
		//Front
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f, -0.5f, 0.5f),
		},
		//Back
		new Vector3[] {
			new Vector3( 0.5f, -0.5f, -0.5f),
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
		},
		//Right
		new Vector3[] {
			new Vector3(0.5f, -0.5f,  0.5f),
			new Vector3(0.5f,  0.5f,  0.5f),
			new Vector3(0.5f,  0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
		},
		//Left
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			
		},
		//Top
		new Vector3[] {
			new Vector3( 0.5f, 0.5f, -0.5f),
			new Vector3( 0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
		},
		//Bottom
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),
		},
	};

	//Defines the ordering of normals in a Cube
	private static Vector3[][] normals = new Vector3[][] {
		new Vector3[] {
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
		},
		new Vector3[] {
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
		},
		new Vector3[] {
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
		},
		new Vector3[] {
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
		},
		new Vector3[] {
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
		},
		new Vector3[] {
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
		},
	};
	
	/**
	 * Build up a single cube at a given local "chunk" position and a specific worldPosition within a MeshData mesh.
	 */
	public static void BuildCube(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		BlockData block = Map.Instance.GetBlock(worldPos);
		Cube cube = (Cube)block.block;
		BlockDirection direction = block.GetDirection ();
	
		//Build up for each face
		for (int i=0; i<6; i++) {
			CubeFace face = faces [i];
			Vector3i dir = directions [i];
			Vector3i nearPos = worldPos + dir;
			if (IsFaceVisible (nearPos, cube)) {
				if (!onlyLight)BuildFace (face, cube, direction, new Vector3 (localPos.x, localPos.y, localPos.z), mesh);
				BuildFaceLight (face, worldPos, mesh);
			}
		}

	}

	/**
	 * Determines whether a current face (pos) for a given cube (currentCube) is visible
	 */
	private static bool IsFaceVisible(Vector3i pos, Cube currentCube) {
		BlockData blockData = Map.Instance.GetBlock (pos);
		if (blockData==null || (blockData != null && !(blockData.block is Cube))) {
			return true;
		}
	
		return blockData!=null || (blockData!=null && blockData.block!=null && blockData.block.IsAlpha() && blockData.block != currentCube);
	}
	
	private static void BuildFace(CubeFace face, Cube cube, BlockDirection direction, Vector3 position, MeshData mesh) {
		List<int> indices = mesh.GetIndices(cube.GetAtlasID());
		Rect texCoord = cube.GetFace(face, direction);
		int iFace = (int)face;
		
		BuildUtils.AddFaceIndices(mesh.vertices.Count, indices);
		BuildUtils.AddVertices(mesh.vertices, vertices[iFace], position);
		mesh.normals.AddRange( normals[iFace] );
		BuildUtils.AddFaceUV(texCoord, mesh.uv);
	}
	
	private static void BuildFaceLight(CubeFace face, Vector3i pos, MeshData mesh) {
		foreach(Vector3 ver in vertices[(int)face]) {
			float sun = GetVertexSunLight( pos, ver, face);
			Color32 color = new Color(sun, sun, sun, sun);//No all values are important
			mesh.colors.Add( color );
		}
	}

	/**
	 * Calculate the amount of lighting for a particular vertex (vertex) for a given cube's face (face).
	 * Based on the lighting on the cubes around it
	 */
	private static float GetVertexSunLight(Vector3i pos, Vector3 vertex, CubeFace face) {
		int dx = (int)Mathf.Sign( vertex.x );
		int dy = (int)Mathf.Sign( vertex.y );
		int dz = (int)Mathf.Sign( vertex.z );
		
		Vector3i a, b, c, d;
		//Determine all the lights to 
		if(face == CubeFace.Left || face == CubeFace.Right) { // X
			a = pos + new Vector3i(dx, 0,  0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0,  dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else 
		if(face == CubeFace.Bottom || face == CubeFace.Top) { // Y
			a = pos + new Vector3i(0,  dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else { // Z
			a = pos + new Vector3i(0,  0,  dz);
			b = pos + new Vector3i(dx, 0,  dz);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		}
		
		float sun = 0;
		BlockData blockB = Map.Instance.GetBlock (b);
		BlockData blockC = Map.Instance.GetBlock (c);
		if((blockB==null || Map.Instance.GetBlock(b).IsAlpha()) || (blockC==null || Map.Instance.GetBlock(c).IsAlpha())) {
			sun =( Map.Instance.GetLight(a) + Map.Instance.GetLight(b) + Map.Instance.GetLight(c) + Map.Instance.GetLight(d));
			sun /= 4.0F;
		} else {
			sun =(Map.Instance.GetLight(a) + Map.Instance.GetLight(b) + Map.Instance.GetLight(c));
			sun /= 3.0F;
		}

		return (float)(sun / LightComputer.MAX_LIGHT);
	}
	
	
}