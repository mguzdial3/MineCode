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

/**
 * This script handles the appearance of an Atlas in the editor.
 * You probably shouldn't mess with this.
 */
[CustomEditor(typeof(Atlas))]
public class AtlasEditor : Editor {
	private Atlas atlas;//The backend atlas representation

	/**
	 * CreateAtlas creates a new atlas when the menu command Map>Create Atlas is used
	 */
	[MenuItem ("Map/Create Atlas")]
	private static void CreateAtlas() {
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Atlas>(), "Assets/NewAtlas.asset");
	}

	/**
	 * OnEnable is called by Unity when the component is enabled, just sets this atlas reference to the target.
	 */
	void OnEnable() {
		atlas = (Atlas) target;
	}

	/**
	 * This method actually determines what the atlas looks like in the editor.
	 */
	public override void OnInspectorGUI() {		
		int w = (int)EditorGUIUtility.fieldWidth;
		atlas.SetWidth(w);
		
		int h = EditorGUILayout.IntField("Height", atlas.GetHeight());
		atlas.SetHeight(h);
		
		Material material = (Material) EditorGUILayout.ObjectField("Material", atlas.GetMaterial(), typeof(Material), true);
		atlas.SetMaterial(material);
		
		bool alpha = EditorGUILayout.Toggle("Alpha", atlas.IsAlpha());
		atlas.SetAlpha( alpha );
		
		if(atlas.GetMaterial() != null && atlas.GetMaterial().mainTexture != null) {
			AtlasViewer.DrawAtlas(atlas, new Rect(0, 0, 1, 1));
		}
		
		if(GUI.changed) EditorUtility.SetDirty(atlas);
	}
}
