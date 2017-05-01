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
 * InventoryGUI handles displaying the player's available blocks and currently selected one.
 */
public class InventoryGUI : MonoBehaviour {
	
	private BlockSet blockSet;
	private Selector builder;
	
	private bool show = false;
	private Vector2 scrollPosition = Vector3.zero;
	public bool InventoryOn = true;

	// Grabs required information at start
	void Start () {
		blockSet = Map.Instance.GetBlockSet();
		builder = (Selector) GameObject.FindObjectOfType( typeof(Selector) );
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Determine whether to begin displaying Inventory
	void Update () {
		if( Input.GetKeyDown(KeyCode.E) && GUIManager.IsPlaying && InventoryOn ) {
			show = !show;
			Cursor.visible = show;
			if(show){
				Cursor.lockState = CursorLockMode.None;
			}
			else{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}
	
	void OnGUI() {
		if(show) {
			Rect window = new Rect(0, 0, Screen.width*0.5f, Screen.height*0.6f);
			window.center = new Vector2(Screen.width, Screen.height)/2f;
			GUILayout.Window(0, window, DoInventoryWindow, "Inventory");
		}
	}
	
	
	private void DoInventoryWindow(int windowID) {
		Block selected = builder.GetSelectedBlock();
		selected = DrawInventory(blockSet, ref scrollPosition, selected);
		builder.SetSelectedBlock(selected);
    }

	/**
	 * DrawInventory draws the current Inventory and handles choosing the new selected block
	 */
	private static Block DrawInventory(BlockSet blockSet, ref Vector2 scrollPosition, Block selected) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int index=0, y=0; index<blockSet.GetCount(); y++) {
			GUILayout.BeginHorizontal();
			for(int x=0; x<8; x++, index++) {
				Block block = blockSet.GetBlock(index);
				if( DrawBlock(block, block == selected && selected != null) ) {
					selected = block;
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		return selected;
	}

	/**
	 * Handles drawing out a single block and determines whether the current Block is selected or not.
	 */
	private static bool DrawBlock(Block block, bool selected) {
		Rect rect = GUILayoutUtility.GetAspectRect(1f);
		
		if(selected) {
			GUI.Box(rect, GUIContent.none);
		}
		
		Vector3 center = rect.center;
		rect.width -= 5;
		rect.height -= 5;
		rect.center = center;
		
		if(block != null) return block.DrawPreview(rect);
		return false;
	}
	
	
}
