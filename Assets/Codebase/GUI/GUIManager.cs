/**
 * Copyright (c) 2015 Entertainment Intelligence Lab.
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
using UnityEngine.UI;
using System.Collections;

/**
 * 
 * GUIManager determines if the game is currently in play or pause mode
 */
public class GUIManager : MonoBehaviour {
	private static Text displayText;
	private static float displayTimer = 0;
	public static Image background; 
	private bool lockMouse = true;

	// Lock the cursor and make it invisible
	void Awake () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		displayText = GameObject.FindGameObjectWithTag ("GUI").GetComponent<Text> ();
		GameObject panelObj = GameObject.Find ("Panel");
		background = panelObj.GetComponent<Image> ();
	}

	void Start(){
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Determine whether to pause or unpause the game
	void Update () {
		if (lockMouse) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if(Input.GetKeyDown(KeyCode.Escape) && Time.timeScale!=0) {
			//Send a message to PauseGUI to tell it to begin displaying
			if(IsPause) {
				lockMouse = true;
				SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
			} else {
				lockMouse = false;
				SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);
			}
		}

		if (displayTimer > 0) {
			displayTimer-=Time.deltaTime;
			if(displayTimer<=0){
				displayText.text = "";
				displayText.color = Color.white;//Reset color
			}
		}

	}

	public static void SetDisplayText(string _displayText, float _displayTime){
		displayText.text = _displayText;
		displayTimer = _displayTime;
	}

	public static void SetDisplayTextColor(string _displayText, float _displayTime, Color colorToUse){
		displayText.text = _displayText;
		displayTimer = _displayTime;
		displayText.color = colorToUse;
	}

	public static bool HasDisplayText(){
		return displayText.text.Length > 0;
	}

	public static void SetGUIBackgroundColor(Color32 color){
		background.color = color;
	}
	
	public static bool IsPause {
		get {
			return Time.timeScale <= 0.1f;
		}
	}
	public static bool IsPlaying {
		get {
			return !IsPause;
		}
	}
	
	void OnResume() {
		Time.timeScale = 1f;
	}
	
	void OnPause() {
		Time.timeScale = 1f/10000f;
	}
}
