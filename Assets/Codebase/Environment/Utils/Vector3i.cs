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

/**
 * A 3D Vector but only using integer values in order to store indexes
 */
public struct Vector3i {

	//x,y,z values
	public int x, y, z;

	//Constant and normalized values
	public static readonly Vector3i zero = new Vector3i(0,0,0);
	public static readonly Vector3i one = new Vector3i(1, 1, 1);
	public static readonly Vector3i forward = new Vector3i(0, 0, 1);
	public static readonly Vector3i back = new Vector3i(0, 0, -1);
	public static readonly Vector3i up = new Vector3i(0, 1, 0);
	public static readonly Vector3i down = new Vector3i(0, -1, 0);
	public static readonly Vector3i left = new Vector3i(-1, 0, 0);
	public static readonly Vector3i right = new Vector3i(1, 0, 0);

	//Major directions
	public static readonly Vector3i[] directions = new Vector3i[] {
		left, right,
		back, forward,
		down, up,
	};
	
	public Vector3i(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3i(int x, int y) {
		this.x = x;
		this.y = y;
		this.z = 0;
	}

	public Vector3i(float x, float y, float z) {
		this.x = Mathf.RoundToInt(x);
		this.y = Mathf.RoundToInt(y);
		this.z = Mathf.RoundToInt(z);
	}

	//Constructor from Vector3
	public Vector3i(Vector3 position){
		this.x = Mathf.RoundToInt(position.x);
		this.y = Mathf.RoundToInt(position.y);
		this.z = Mathf.RoundToInt(position.z);
	}
	
	public static int DistanceSquared(Vector3i a, Vector3i b) {
		int dx = b.x-a.x;
		int dy = b.y-a.y;
		int dz = b.z-a.z;
		return dx*dx + dy*dy + dz*dz;
	}
	
	public int DistanceSquared(Vector3i v) {
		return DistanceSquared(this, v);
	}

	public Vector3 ToVector3(){
		return new Vector3 (x, y, z);
	}
	
	public override int GetHashCode () {
		return x.GetHashCode () ^ y.GetHashCode () << 2 ^ z.GetHashCode () >> 2;
	}

	public override bool Equals(object other) {
		if (!(other is Vector3i)) return false;
		Vector3i vector = (Vector3i) other;
		return x == vector.x && 
			   y == vector.y && 
			   z == vector.z;
	}
	
	public override string ToString() {
		return "Vector3i("+x+" "+y+" "+z+")";
	}
	
	public static bool operator == (Vector3i a, Vector3i b) {
		return a.x == b.x && 
			   a.y == b.y && 
			   a.z == b.z;
	}
	
	public static bool operator != (Vector3i a, Vector3i b) {
		return a.x != b.x ||
			   a.y != b.y ||
			   a.z != b.z;
	}
	
	public static Vector3i operator - (Vector3i a, Vector3i b) {
		return new Vector3i( a.x-b.x, a.y-b.y, a.z-b.z);
	}
	
	public static Vector3i operator + (Vector3i a, Vector3i b) {
		return new Vector3i( a.x+b.x, a.y+b.y, a.z+b.z);
	}
	
	public static Vector3i operator * (Vector3i a, int b){
		return new Vector3i( a.x*b, a.y*b, a.z*b);
	}

	public static Vector3i operator * (Vector3i a, float b){
		return new Vector3i( a.x*b, a.y*b, a.z*b);
	}

	public static Vector3i operator * (int b, Vector3i a){
		return new Vector3i( a.x*b, a.y*b, a.z*b);
	}

	public static Vector3i operator * (float b, Vector3i a){
		return new Vector3i( a.x*b, a.y*b, a.z*b);
	}
	
}
