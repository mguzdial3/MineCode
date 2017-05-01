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
 * BuildUtils is a singleton that handles adding faces, indices, and vertices to lists for mesh information
 */
class BuildUtils {
	public static void AddIndices(List<int> list, int[] indices, int offset) {
		foreach(int index in indices) {
			list.Add( index + offset );
		}
	}
	
	public static void AddVertices(List<Vector3> list, Vector3[] vertices, Vector3 offset) {
		foreach(Vector3 v in vertices) {
			list.Add( v + offset );
		}
	}

	/**
	 * Add the different tex coordinates for face information
	 */
	public static void AddFaceUV(Rect texCoord, List<Vector2> uv) {
		uv.Add( new Vector2(texCoord.xMax, texCoord.yMin) );
		uv.Add( new Vector2(texCoord.xMax, texCoord.yMax) );
		uv.Add( new Vector2(texCoord.xMin, texCoord.yMax) );
		uv.Add( new Vector2(texCoord.xMin, texCoord.yMin) );
	}

	/**
	 * Add face index information
	 */
	public static void AddFaceIndices(int start, List<int> indices) {
		indices.Add( start+2 );
		indices.Add( start+1 );
		indices.Add( start+0 );
		
		indices.Add( start+3 );
		indices.Add( start+2 );
		indices.Add( start+0 );
	}
}

/**
 * MeshData is an info holder for mesh information so an entire rendered mesh doesn't need to be held.
 */
public class MeshData {
	//All the various mesh information needed to make a mesh from here
	public readonly List<Vector3> vertices = new List<Vector3>();
	public readonly List<Vector2> uv = new List<Vector2>();
	public readonly List<Vector3> normals = new List<Vector3>();
	public readonly List<Color32> colors = new List<Color32>();
	private List<int>[] indices = new List<int>[0];
	
	public MeshData(int subMeshCount) {
		indices = new List<int>[subMeshCount];
		for(int i=0; i<subMeshCount; i++) {
			indices[i] = new List<int>();
		}
	}
	
	public List<int> GetIndices(int index) {
		if(index >= indices.Length) {
			List<int>[] oldIndices = indices;
			indices = new List<int>[index+1];
			for(int i=0; i<indices.Length; i++) {
				if(i < oldIndices.Length) {
					indices[i] = oldIndices[i];
				} else {
					indices[i] = new List<int>();
				}
			}
		}
		return indices[index];
	}
	
	public void Clear() {
		vertices.Clear();
		uv.Clear();
		normals.Clear();
		colors.Clear();
		foreach(List<int> list in indices) {
			list.Clear();
		}
	}

	/**
	 * ToMesh translate MeshData into a Mesh
	 */
	public Mesh ToMesh(Mesh mesh) {
		if (vertices.Count == 0) {
			Debug.Log ("Vertices was 0");
			GameObject.Destroy (mesh);
			return null;
		}
		
		if(mesh == null) mesh = new Mesh();

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors32 = colors.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();

		//Added the minus 1
		mesh.subMeshCount = indices.Length-1;
		for(int i=0; i<mesh.subMeshCount; i++) {
			mesh.SetTriangles( indices[i].ToArray(), i );
		}

		return mesh;
	}
	
}