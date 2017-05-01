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
 * Atlas is an Info Holder class for the atlas, which holds the large material that holds all of the individual textures. 
 * You probably shouldn't mess with this.
 */
public class Atlas : ScriptableObject {
	[SerializeField] private int width = 1, height = 1; //dimensions of this atlas
	[SerializeField] private Material material; //The material that this atlas uses
	[SerializeField] private bool alpha = false; //Whether or not this atlas has elements transparent 
	
	public void SetMaterial(Material material) {
		this.material = material;
	}
	public Material GetMaterial() {
		return material;
	}
	
	public void SetWidth(int width) {
		this.width = width;
	}
	public int GetWidth() {
		return width;
	}
	
	public void SetHeight(int height) {
		this.height = height;
	}
	public int GetHeight() {
		return height;
	}
	
	public void SetAlpha(bool alpha) {
		this.alpha = alpha;
	}
	public bool IsAlpha() {
		return alpha;
	}
	
	public float GetTileSizeX() {
		return 1.0f/width;
	}
	public float GetTileSizeY() {
		return 1.0f/height;
	}
	
}
