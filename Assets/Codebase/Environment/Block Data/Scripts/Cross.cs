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
 * This script is an infoholder for a type of Block called a Cross used for things like mushrooms or fires 
 */
[System.Serializable]
public class Cross : Block {
	
	[SerializeField] private Rect face;
	
	public Cross(string name) {
		SetName(name);
	}
	
	public override Rect GetPreviewFace() {
		return face;
	}
	
	public void SetFace(Rect face) {
		this.face = face;
	}
	public Rect GetFace() {
		return face;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		CrossBuilder.BuildCross(localPos, worldPos, mesh, onlyLight);
	}
	
}