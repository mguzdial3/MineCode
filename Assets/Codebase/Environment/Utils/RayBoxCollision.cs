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
 * RayBoxCollision singleton handles a very simplified raycasting for use with the blocks
 * You probably shouldn't mess with this.
 */
public class RayBoxCollision {	


	public static Vector3? Intersection(Map map, Ray ray, float distance) {
		Vector3 start = ray.origin;
		Vector3 end = ray.origin+ray.direction*distance;
		int startX = Mathf.RoundToInt(start.x);
		int startY = Mathf.RoundToInt(start.y);
		int startZ = Mathf.RoundToInt(start.z);
		int endX = Mathf.RoundToInt(end.x);
		int endY = Mathf.RoundToInt(end.y);
		int endZ = Mathf.RoundToInt(end.z);
		
		if(startX>endX) {
			int tmp = startX;
			startX = endX;
			endX = tmp;
		}
		
		if(startY>endY) {
			int tmp = startY;
			startY = endY;
			endY = tmp;
		}
		
		if(startZ>endZ) {
			int tmp = startZ;
			startZ = endZ;
			endZ = tmp;
		}
		
		float minDistance = distance;
		for(int z=startZ; z<=endZ; z++) {
			for(int y=startY; y<=endY; y++) {
				for(int x=startX; x<=endX; x++) {
					BlockData block = map.GetBlock(x, y, z);
					if(block==null || block.IsEmpty()) continue;
					float dis = RayBoxIntersection(ray, new Vector3(x, y, z));
					minDistance = Mathf.Min(minDistance, dis);
				}
			}
		}
		
		if(minDistance != distance) return ray.origin + ray.direction * minDistance;
		return null;
	}

	public static float RayBoxIntersection(Ray ray, Vector3 center) {
		const float ext = 0.5f; // size/2
  		float tnear = float.MinValue;
  		float tfar = float.MaxValue;

		for(int i = 0; i < 3; i++) {
    		float min = center[i] - ext;
    		float max = center[i] + ext;

    		float pos = ray.origin[i];
    		float dir = ray.direction[i];

    		// check for ray parallel to planes
    		if(Mathf.Abs(dir) <= float.Epsilon) {
      			// ray parallel to planes
      			if((pos < min) || (pos > max)) return float.MaxValue;
    		}

    		// ray not parallel to planes, so find parameters of intersections
    		float t0 = (min - pos) / dir;
    		float t1 = (max - pos) / dir;

    		// check ordering
			if( t0 > t1 ) {
				float tmp = t0;
				t0 = t1;
				t1 = tmp;
			}

    		// compare with current values
			tnear = Mathf.Max(t0, tnear);
			tfar = Mathf.Min(t1, tfar);

    		// check if ray misses entirely
    		if(tnear > tfar) return float.MaxValue;
    		if(tfar < 0.0f) return float.MaxValue;
  		}
		
		if(tnear > 0.0f) {
			return tnear;
		} else {
			return tfar;
		}
	}
	
	
}
