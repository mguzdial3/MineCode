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

public class FPSInputController : MonoBehaviour {
	
	private CharacterMotor motor;
	private float jumpPressedTime = -100;

	// Use this for initialization
	void Start () {
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		// Get the input vector from kayboard or analog stick
		Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (direction != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = direction.magnitude;
			direction = direction / directionLength;
		
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
		
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
		
			// Multiply the normalized direction vector by the modified length
			direction = direction * directionLength;
		}
	
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.TransformDirection(direction);
		
		if(Input.GetButtonDown("Jump")) {
			jumpPressedTime = Time.time;
		}
		if( !Input.GetButton("Jump") ) {
			jumpPressedTime = -100;
		}
		motor.inputJump = Time.time - jumpPressedTime <= 0.2f; // кнопка была нажата в последнии 0.2 секунды
		if(motor.IsJumping() && !motor.IsGrounded()) {
			motor.inputJump = Input.GetButton("Jump");
		}
	}
	
	void OnJump() {
		jumpPressedTime = -100;
	}
	
	
}
