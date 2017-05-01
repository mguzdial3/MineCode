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
using System;

enum Axis {
	X = 0,
	Y = 1,
	X_AND_Y = 2
}

public class MouseLook : MonoBehaviour {
	
	[SerializeField] private Axis axis = Axis.X_AND_Y;
	
	private float sensitivity = 5f;
	
	private float minimumY = -90f;
	private float maximumY = 90f;
	
	private Vector2 angles = Vector2.zero;
	
	void Start() {
		angles = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {

		float x = Input.GetAxis("Mouse X");
		float y = -Input.GetAxis("Mouse Y");	
		Vector2 delta = new Vector2(y, x);
		
		angles += delta * sensitivity;
		angles.x = Mathf.Clamp(angles.x, minimumY, maximumY);
		
		if(axis == Axis.Y) angles.x = transform.localEulerAngles.x;
		if(axis == Axis.X) angles.y = transform.localEulerAngles.y;
		
		Quaternion targetRotation = Quaternion.Euler(angles);
		transform.localRotation = Quaternion.Slerp( transform.localRotation, targetRotation, 25*Time.deltaTime );
	}
	
}
