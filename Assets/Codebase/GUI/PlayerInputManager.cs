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
using System.Collections;

/**
 * PlayerInputManager handles updating the policy
 */
public class PlayerInputManager : MonoBehaviour {
	private const string START_OF_TEXT ="/Users/mguzdial/MinecraftEnvironment/PlayerPolicyFeedback/policy";
	private const string END_OF_TEXT =".txt";
	private string changeList;

	public string feedbackDescriptor = "Test";

	public PolicyFollower policyFollower;

	void Start(){
		if(policyFollower==null){
			Debug.LogError("Policy Follower was NULL. FIX IT.");
		}
		changeList ="";
	}


	// Use this for GUI display because its easy
	void OnGUI () {
		GUI.skin.button.fontSize = 30;
		if(GUI.Button(new Rect(Screen.width-100,0,100,100),"+")){
			changeList+="CurrState: "+policyFollower.GetCurrentIndex()+". CurrAction: "+policyFollower.GetCurrentAction()+". Feedback: Yes\n";
		}

		if(GUI.Button(new Rect(Screen.width-100,100,100,100),"-")){
			changeList+="CurrState: "+policyFollower.GetCurrentIndex()+". CurrAction: "+policyFollower.GetCurrentAction()+". Feedback: No\n";
		}


	}

	void OnDestroy(){
		System.IO.File.WriteAllText(START_OF_TEXT+feedbackDescriptor+END_OF_TEXT, changeList);
	}
}
