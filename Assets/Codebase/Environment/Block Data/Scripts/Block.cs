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
using System;

/**
 * Block is an Info Holder class for the primitive blocks used in BlockSet from which all BlockData come. 
 * You probably shouldn't mess with this.
 */
[System.Serializable]
public abstract class Block {
	
	[SerializeField] private string name; //The name of this block
	[SerializeField] private Atlas atlas; //The atlas corresponding to this block
	[SerializeField] private bool breakable = true; //Whether or not this block can be destroyed

	private bool alpha = false; //Whether or not this block includes Atlas information
	private int atlasId; //The atlas index that this Block draws from
	private int blockId; //The id of this Block (index in the BlockSet). Used for saving.
	public int BlockId { get { return blockId; } } //Public accessible means of getting blockId

	/**
	 * This method initializes a Block from an initial blockSet and the Block's index in that blockSet
	 */
	public void Init(BlockSet blockSet, int blockId) {
		if(atlas != null) alpha = atlas.IsAlpha();
		atlasId = Array.IndexOf(blockSet.GetAtlases(), atlas);
		this.blockId = blockId;
	}

	/**
	 * DrawPreview draws a 2D representation of this block. (Just one face of the block)
	 */
	public bool DrawPreview(Rect rect) {
		Rect face = GetPreviewFace();
		if(atlas != null && atlas.GetMaterial() != null && atlas.GetMaterial().mainTexture != null) {
			GUI.DrawTextureWithTexCoords(rect, atlas.GetMaterial().mainTexture, face);
		}
		return Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
	}
	public abstract Rect GetPreviewFace();
	
	public abstract void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight);
	
	public void SetName(string name) {
		this.name = name;
	}
	public string GetName() {
		return name;
	}

	public int GetBlockId(){
		return blockId;
	}
	
	public void SetAtlas(Atlas atlas) {
		this.atlas = atlas;
	}
	public Atlas GetAtlas() {
		return atlas;
	}
	public int GetAtlasID() {
		return atlasId;
	}
	
	public bool IsAlpha() {
		return alpha;
	}

	public void SetBreakable(bool breakable){
		this.breakable = breakable;
	}

	public bool IsBreakable(){
		return breakable;
	}
	
}