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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/**
 * This script is responsible for interacting with a BlockSet in the editor 
 * You probably shouldn't mess with this.
 */
[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor {

	private BlockSet blockSet;//The backend blockSet representation this corresponds to
	private static Vector2 blockSetScrollPosition;//The current scroll amount for the entire blockSet
	private static int selectedBlock = -1; //The index of the currently selected block (-1 if none)
	
	private static Rect atlasRect = new Rect(0, 0, 1, 1);//The location of the Atlas representation in this Blockset representation
	private static CubeFace selectedFace = CubeFace.Front;//The currently selected block (defaults to CubeFace.Front)

	/**
	 * CreateBlockset creates a new blockset when the menu command Map>Create Blockset is used
	 */
	[MenuItem ("Map/Create BlockSet")]
	private static void CreateBlockset() {
		GameObject go = new GameObject("TileSet", typeof(BlockSet));
		PrefabUtility.CreatePrefab("Assets/NewBlockSet.prefab", go);
		GameObject.DestroyImmediate(go);
	}

	/**
	 * OnEnable is called by Unity when the component is enabled, just sets this blockset reference to the target.
	 */
	void OnEnable() {
		blockSet = (BlockSet)target;
	}

	/**
	 * OnInspectorGUI defines what the BlockSet looks like in the unity editor
	 */
	public override void OnInspectorGUI() {
		
		Atlas[] list = DrawAtlasesEditor( blockSet.GetAtlases() );
		blockSet.SetAtlases( list );
		EditorGUILayout.Separator();
		
		DrawBlockSetEditor( blockSet );
		EditorGUILayout.Separator();
		
		if( blockSet.GetBlock(selectedBlock) != null ) {
			DrawBlockEditor( blockSet.GetBlock(selectedBlock), blockSet );
		}
		
		if(GUI.changed) {
			EditorUtility.SetDirty(blockSet);
			Repaint();
		}
	}

	/**
	 * DrawAtlasesEditor draws the Blockset's collections of Atlases and handles editing it
	 */
	private static Atlas[] DrawAtlasesEditor( Atlas[] list ) {
		GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
		for(int i=0; i<list.Length;) {
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("Remove") ) {
				ArrayUtility.RemoveAt<Atlas>(ref list, i);
			} else {
				list[i] = (Atlas)EditorGUILayout.ObjectField(list[i], typeof(Atlas), false);
				i++;
			}
			GUILayout.EndHorizontal();
		}
		
		if(GUILayout.Button("Add Atlas")) {
			ArrayUtility.Add<Atlas>(ref list, null);
		}
		GUILayout.EndVertical();
		return list;
	}

	/**
	 * DrawBlockSetEditor draws and handles the editing of the BlockSet
	 */
	private static void DrawBlockSetEditor(BlockSet blockSet) {
		GUILayout.BeginVertical("box");
		selectedBlock = DrawBlockList( blockSet, selectedBlock, ref blockSetScrollPosition );
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Add Cube")) {
				selectedBlock = blockSet.Add( new Cube("new Cube") );
			}
			if(GUILayout.Button("Add Cross")) {
				selectedBlock = blockSet.Add( new Cross("new Cross") );
			}
		GUILayout.EndHorizontal();
		if( GUILayout.Button("Remove") && blockSet.GetBlock(selectedBlock) != null ) {
			Undo.RecordObject( blockSet, "Remove block" );
			blockSet.Remove( selectedBlock );
			selectedBlock = Mathf.Clamp(selectedBlock, 0, blockSet.GetCount()-1);
		}
		GUILayout.EndVertical();
	}

	/**
	 * DrawBlockList draws the current BlockSet
	 */
	private static int DrawBlockList(BlockSet blocks, int selected, ref Vector2 scrollPosition) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
		
		GUIStyle normal = GUI.skin.button;
		GUIStyle active = new GUIStyle(normal);
		active.normal = active.active;
		
		for(int i=0; i<blocks.GetCount(); i++) {
			Block block = blocks.GetBlock(i);
			if(i == selected) GUI.skin.button = active;
			if( GUILayout.Button(block.GetName()) ) {
				if(selected != i) GUIUtility.keyboardControl = 0;
				selected = i;
			}
			GUI.skin.button = normal;
		}
		
		GUILayout.EndScrollView();
		return selected;
	}
	
	/**
	 * DrawBlockEditor draws and handles editing a single block
	 */
	private static void DrawBlockEditor(Block block, BlockSet blockSet) {
		string name = EditorGUILayout.TextField("Name", block.GetName());
		block.SetName(name);
		
		Atlas[] atlases = blockSet.GetAtlases();
		string[] atlasNames = new string[atlases.Length];
		for(int i=0; i<atlasNames.Length; i++) {
			Atlas tAtlas = atlases[i];
			atlasNames[i] = tAtlas != null ? tAtlas.name : "null";
		}
		int index = ArrayUtility.IndexOf(atlases, block.GetAtlas());
		index = EditorGUILayout.Popup("Atlas", index, atlasNames);
		block.SetAtlas( SafeGet(atlases, index) );

		/**
		 * For now all blocks have the same light values as a default
		int light = EditorGUILayout.IntField("Light", block.GetLight());
		light = Mathf.Clamp(light, 0, 15);
		block.SetLight((byte)light);
		*/

		bool breakable = EditorGUILayout.Toggle ("Breakable", block.IsBreakable());
		block.SetBreakable (breakable);
		
		Atlas atlas = block.GetAtlas();
		if(atlas == null || atlas.GetMaterial() == null || atlas.GetMaterial().mainTexture == null) return;
		if(block is Cube) DrawCubeBlockEditor( (Cube)block, atlas );
		if(block is Cross) DrawCrossBlockEditor( (Cross)block, atlas );
	}

	/**
	 * SafeGet safely gets an Atlas at an index in the list, or returns null
	 */
	private static Atlas SafeGet(Atlas[] list, int index) {
		if(index < 0 || index >= list.Length) return null;
		return list[index];
	}

	/**
	 * DrawCubeBlockEditor is a helper method for DrawBlockEditor
	 */
	private static void DrawCubeBlockEditor(Cube block, Atlas atlas) {
		selectedFace = DrawCubeFacesEditor(selectedFace, block, atlas);
		Rect facePosition = block.GetFace(selectedFace);
		AtlasViewer.DrawAtlasEditor(atlas, ref atlasRect, ref facePosition);
		block.SetFace(facePosition, selectedFace);
	}

	/**
	 * DrawCrossBlockEditor is a helper method for DrawBlockEditor
	 */
	private static void DrawCrossBlockEditor(Cross block, Atlas atlas) {
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(GUIContent.none, GUILayout.Width(64), GUILayout.Height(64));
			Rect rect = GUILayoutUtility.GetLastRect();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUI.DrawTextureWithTexCoords(rect, atlas.GetMaterial().mainTexture, block.GetFace());
		
		Rect position = block.GetFace();
		AtlasViewer.DrawAtlasEditor(atlas, ref atlasRect, ref position);
		block.SetFace(position);
	}

	/**
	 * DrawCubeFacesEditor is a helper method for DrawCubeBlockEditor (for drawing faces specifically)
	 */
	private static CubeFace DrawCubeFacesEditor(CubeFace face, Cube cube, Atlas atlas) {
		string[] items = new string[6];
		for(int i=0; i<6; i++) {
			items[i] = ((CubeFace)i).ToString();
		}
		Texture texture = atlas.GetMaterial().mainTexture;
		
		GUILayout.BeginVertical("box");
		face = (CubeFace)GUILayout.Toolbar((int)face, items);
		Rect bigRect = GUILayoutUtility.GetAspectRect(items.Length);
		for(int i=0; i<items.Length; i++) {
			Rect position = bigRect;
			position.width /= items.Length;
			position.x += i*position.width;
			Rect face_rect = cube.GetFace( (CubeFace) i );
			GUI.DrawTextureWithTexCoords(position, texture, face_rect);
		}
		GUILayout.EndVertical();
		
		return face;
	}
	
}
