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
 * PauseGUI handles actually displaying pause information after being prompted by GUIManager.
 * It also handles displaying the means of altering the current state with save data.
 */
public class PauseGUI : MonoBehaviour {
	private string saveText="";

	void OnResume() {
		enabled = false;
		SaveManager.Instance.SetCanSave (true);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void OnPause() {
		enabled = true;
		SaveManager.Instance.SetCanSave (false);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	void OnGUI() {
		GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
		GUILayout.FlexibleSpace();
			DrawResumeButton();
			DrawTextField ();
			DrawSaveButton ();
			DrawVCRField ();
			DrawHelpText();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
	
	private void DrawResumeButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Resume", GUILayout.ExpandWidth(false))) {
			SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	//Writes out the current update number
	private void DrawHelpText() {
		string text = "Current Snapshot Number: "+SaveManager.Instance.GetGeneralUpdateNumber();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Box(text, GUILayout.ExpandWidth(false));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawQuitButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Quit", GUILayout.ExpandWidth(false))) {
			Application.Quit();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	//Draws and handles the button presses for the special save and load
	private void DrawSaveButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Save", GUILayout.ExpandWidth(false))) {
			SaveManager.Instance.SaveSpecial(saveText);//Save out the current game state tagged with the current 'saveText'
		}

		if(GUILayout.Button("Load", GUILayout.ExpandWidth(false))) {
			SaveManager.Instance.LoadSpecial(saveText);//Load the current game state tagged with the current 'saveText'
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	//Draws the text field that determines the value for the special save in SaveManager
	private void DrawTextField() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		saveText = GUILayout.TextField (saveText, GUILayout.Width(90));

		if (saveText.Contains (" ")) {
			saveText=saveText.Substring(0,saveText.IndexOf(' '));		
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	//Draws the steps back and forwards through time, and calls SaveManager
	private void DrawVCRField() {

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if(GUILayout.Button("<<", GUILayout.ExpandWidth(false))) {
			SaveManager.Instance.LoadDifferentUpdate(-1);
		}

		if(GUILayout.Button(">>", GUILayout.ExpandWidth(false))) {
			SaveManager.Instance.LoadDifferentUpdate(1);
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

	}

}
