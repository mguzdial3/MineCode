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
using System.Collections.Generic;

/**
 * This script builds out Cross type blocks
 */
public class CrossBuilder {
	
	private static Vector3[] vertices = new Vector3[] {
		// face a
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f, -0.5f,  0.5f),
		
		//face b
		new Vector3(-0.5f, -0.5f,  0.5f),
		new Vector3(-0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f, -0.5f, -0.5f),
	};
	
	private static Vector3[] normals = new Vector3[] {
		//face a
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		
		//face b
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
	};
	
	private static int[] indices = new int[] {
		//face a
		2, 1, 0,
		3, 2, 0,
		//face a
		0, 2, 3,
		0, 1, 2,
		
		//face b
		6, 5, 4,
		7, 6, 4,
		//face b
		4, 6, 7,
		4, 5, 6,
	};

	
	public static void BuildCross(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		if(!onlyLight) {
			BuildCross(new Vector3(localPos.x,localPos.y,localPos.z), worldPos, mesh);
		}
		BuildCrossLight( worldPos, mesh);
	}
	
	private static void BuildCross(Vector3 localPos, Vector3i worldPos, MeshData mesh) {
		Cross cross = (Cross) Map.Instance.GetBlock(worldPos).block;
		int atlas = cross.GetAtlasID();
		
		BuildUtils.AddIndices(mesh.GetIndices(atlas), indices, mesh.vertices.Count);
		BuildUtils.AddVertices(mesh.vertices, vertices, localPos );
		mesh.normals.AddRange( normals );
		
		Rect uvRect = cross.GetFace();
		List<Vector2> texCoords = mesh.uv;
		BuildUtils.AddFaceUV(uvRect, texCoords);
		BuildUtils.AddFaceUV(uvRect, texCoords);
	}
	
	private static void BuildCrossLight( Vector3i pos, MeshData mesh) {
		byte light = Map.Instance.GetBlock(pos).GetLight();
		AddFaceLight(light, LightComputer.MAX_LIGHT, mesh.colors);
		AddFaceLight(light, LightComputer.MAX_LIGHT, mesh.colors);
	}
	
	private static void AddFaceLight(byte light, byte sun, List<Color32> colors) {
		byte _light = (byte) (light / LightComputer.MAX_LIGHT);
		byte _sun =  (byte) (sun / LightComputer.MAX_LIGHT);
		Color32 color = new Color32(_light, _light, _light, _sun);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
	
}
