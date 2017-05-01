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
 * NPCAppearanceController handles the appearance for a single NPC
 */
public class NPCAppearanceController : MonoBehaviour {
	public Transform headJoint;

	private Quaternion bodyLookTarget;
	private Quaternion headLookTarget;

	//Rotation speed of head and body
	private float _headSpeed=2.0f;
	private float _bodySpeed=2.0f;

	//Appearance information
	public Renderer[] toColorRenderers;
	private Color[] _colorOptions = {Color.gray, Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta};
	private int _colorIndex;
	public int ColorIndex { get { return _colorIndex; } }


	//Determine whether or not to randomly change appearance
	public bool randomizeAppearance = false;


	public virtual void Init(){
		headLookTarget = headJoint.rotation;
		bodyLookTarget = transform.rotation;

		//Enforces specific appearance instead of random for now
		if(randomizeAppearance){
			_colorIndex = Random.Range(0,_colorOptions.Length);
			foreach(Renderer r in toColorRenderers){
				r.material.color = Color.Lerp(_colorOptions [_colorIndex], Color.white, Random.Range(0.45f,0.55f));//Make it pastel
			}
		}
	}

	public void InitFromSave(int colorIndex){
		headLookTarget = headJoint.rotation;
		bodyLookTarget = transform.rotation;
		//Update color of body
		_colorIndex = colorIndex;
		foreach(Renderer r in toColorRenderers){
			r.material.color = _colorOptions [_colorIndex];
		}
	}

	// Call UpdateAppearance from NPCUnit
	public virtual bool UpdateAppearance () {
		bool changed = false;
		if (bodyLookTarget != transform.rotation) {
			transform.rotation=Quaternion.Lerp(transform.rotation,bodyLookTarget,Time.deltaTime*_bodySpeed);
		}

		if ( headLookTarget != headJoint.rotation) {
			headJoint.rotation=Quaternion.Lerp(headJoint.rotation,headLookTarget,Time.deltaTime*_headSpeed);
		}
		//At present do not save if only head or body rotation changes
		return changed;
	}

	//Sets the current goal of this NPC for rotation reasons
	public virtual void SetNewGoal(Vector3 goal){
		Vector3 goalEvenY = goal;
		goalEvenY.y = transform.position.y;
		if((goalEvenY-transform.position)!=Vector3.zero){
			bodyLookTarget = Quaternion.LookRotation (goalEvenY - transform.position, Vector3.up);
		}

		if (goal.y != transform.position.y) {
			if((goal-headJoint.position)!=Vector3.zero){
				headLookTarget = Quaternion.LookRotation (goal - headJoint.position, Vector3.up);
			}
		}
		else{
			headLookTarget = bodyLookTarget;
		}
	}

	public Color GetColor(){
		return _colorOptions[_colorIndex];
	}

	public void SetInvisible(){
		foreach(Renderer r in toColorRenderers){
			r.enabled = false;
		}
		headJoint.gameObject.SetActive (false);
	}

	public void SetVisible(){
		foreach(Renderer r in toColorRenderers){
			r.enabled = true;
		}
		headJoint.gameObject.SetActive (true);
	}
}
