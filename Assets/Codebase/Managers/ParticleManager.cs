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

public class ParticleManager : MonoBehaviour {
	public GameObject[] particleEffects;
	public static ParticleManager Instance;

	// Use this for initialization
	void Start () {
		Instance = this;
	}

	public static void CreateEffect(string effectName, GameObject target){
		CreateEffect(effectName,target.transform.position);
	}
	
	public static void CreateEffect(string effectName, Vector3 pos){
		GameObject particleToUse = null;

		foreach (GameObject particle in Instance.particleEffects) {
			if (particle.name == effectName) {
				particleToUse = particle;
			}
		}

		if (particleToUse != null) {
			GameObject particleObj = Instantiate<GameObject> (particleToUse.gameObject);
			particleObj.transform.position = pos;
		}
	}
}
