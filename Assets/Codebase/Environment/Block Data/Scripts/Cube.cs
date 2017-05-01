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
 * This enum holds the different sides or faces of the cube
 */
public enum CubeFace {
	Front  = 0,
	Back   = 1,
	Right  = 2,
	Left   = 3,
	Top    = 4,
	Bottom = 5,
}

/**
 * This script is an infoholder for a type of Block called a Cross used for almost all world object
 */
[System.Serializable]
public class Cube : Block {
	
	[SerializeField] private Rect forward;
	[SerializeField] private Rect back;
	[SerializeField] private Rect right;
	[SerializeField] private Rect left;
	[SerializeField] private Rect up;
	[SerializeField] private Rect down;
	
	
	public Cube(string name) {
		SetName(name);
	}
	
	public override Rect GetPreviewFace() {
		return forward;
	}
	
	public void SetFace(Rect coord, CubeFace face) {
		switch (face) {
			case CubeFace.Front: forward = coord; return;
			case CubeFace.Back: back = coord; return;
			
			case CubeFace.Right: right = coord; return;
			case CubeFace.Left: left = coord; return;
			
			case CubeFace.Top: up = coord; return;
			case CubeFace.Bottom: down = coord; return;
		}
	}
	
	public Rect GetFace(CubeFace face) {
		switch (face) {
			case CubeFace.Front: return forward;
			case CubeFace.Back: return back;
			
			case CubeFace.Right: return right;
			case CubeFace.Left: return left;
			
			case CubeFace.Top: return up;
			case CubeFace.Bottom: return down;
		}
		return new Rect(0,0,0,0);
	}
	
	public Rect GetFace(CubeFace face, BlockDirection dir) {
		if(face != CubeFace.Top && face != CubeFace.Bottom) {
			face = TransformFace(face, dir);
		}
		
		switch (face) {
			case CubeFace.Front: return forward;
			case CubeFace.Back: return back;
			
			case CubeFace.Right: return right;
			case CubeFace.Left: return left;
			
			case CubeFace.Top: return up;
			case CubeFace.Bottom: return down;
		}
		return default(Rect);
	}

	/**
	 * Transform the face for a given direction (used for rotation) 
	 */
	private static CubeFace TransformFace(CubeFace face, BlockDirection dir) {
		//Front, Right, Back, Left
		//0      90     180   270
		
		int angle = 0;
		if(face == CubeFace.Right) angle = 90;
		if(face == CubeFace.Back)  angle = 180;
		if(face == CubeFace.Left)  angle = 270;
		
		if(dir == BlockDirection.X_MINUS) angle += 90;
		if(dir == BlockDirection.Z_MINUS) angle += 180;
		if(dir == BlockDirection.X_PLUS) angle += 270;
		
		angle %= 360;
		
		if(angle == 0) return CubeFace.Front;
		if(angle == 90) return CubeFace.Right;
		if(angle == 180) return CubeFace.Back;
		if(angle == 270) return CubeFace.Left;
		
		return CubeFace.Front;
	}
	
	
	public override void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		CubeBuilder.BuildCube(localPos, worldPos, mesh, onlyLight);
	}
	
	
}