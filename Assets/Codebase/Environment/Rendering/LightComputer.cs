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
using System.Collections.Generic;

/**
 * LightComputer is the lighting engine and has all the methods for calculating lighting.
 */
public class LightComputer {
	//Constant values for use in the lighting engine
	public const byte MIN_LIGHT = 1;
	public const byte MAX_LIGHT = 15;
	public const byte STEP_LIGHT = 1;

	//Computer the average value for this light based on the surrounding lights
	public static byte AverageLight(Map map, Vector3i pos) {
		BlockData block = map.GetBlock(pos);
		if(block!=null || (block!=null && !block.IsAlpha() )) return MIN_LIGHT;

		//Sum up all the nearby lights
		float light = (float)map.GetLight(pos);
		foreach(Vector3i dir in Vector3i.directions) {
			if (dir!= Vector3i.down){
				light+= (float)(map.GetLight( pos+dir ));
			}
		}

		//Average across the directions
		light /= (Vector3i.directions.Length-1);
		byte byteLight = (byte)Mathf.Clamp (light, LightComputer.MIN_LIGHT+LightComputer.STEP_LIGHT, LightComputer.MAX_LIGHT);
		return byteLight;
	}

	//Compute the lighting for a specific 3D positions
	private static byte ComputeLight(Map map, Vector3i pos) {
		BlockData block = map.GetBlock(pos);
		if(block!=null || (block!=null && !block.IsAlpha() )) return MIN_LIGHT;

		int light = MIN_LIGHT;
		foreach(Vector3i dir in Vector3i.directions) {
			int newLight = map.GetLight( pos+dir ) - STEP_LIGHT;
			light = light>newLight ?light : newLight;
			if(light == MAX_LIGHT-STEP_LIGHT) return (byte) light;
		}
		return (byte) light;
	}
}