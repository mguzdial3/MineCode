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
 * This script is a perlin noise generator for 2D values. Used in the TerrainGenerator.
 * You probably shouldn't mess with this
 */
public class PerlinNoise2D {
	private float scale;//The particular scale to generate values to, set by the initializaton 
	private float persistence = 0.5f; 
	private int octaves = 5;
	private Vector2 offset = Vector2.zero; //Randomly generated initial offset
	
	public PerlinNoise2D(float scale) {
		this.scale = scale;

		offset = new Vector2 (Random.Range (-100f, 100f), Random.Range (-100f, 100f));

		Debug.Log ("Offset: " + offset);
	}

	//Use this for not randomly generated perlin noise
	public PerlinNoise2D(float scale, Vector2 _offset){
		this.scale = scale;

		offset = _offset;
	}
	
	public PerlinNoise2D SetPersistence(float persistence) {
		this.persistence = persistence;
		return this;
	}
	
	public PerlinNoise2D SetOctaves(int octaves) {
		this.octaves = octaves;
		return this;
	}
	
	/**
	 * Method to add values to a 2D map in a smoothed out way.
	 */
	public void Noise(float[,] map, float offsetX, float offsetY) {
		int width = map.GetLength(0);
		int height = map.GetLength(1);
        const int delta = 4;
        for(int x=0; x<width; x+=delta) {
            for(int y=0; y<height; y+=delta) {
                float x1 = x+offsetX;
                float y1 = y+offsetY;
                float x2 = x+delta+offsetX;
                float y2 = y+delta+offsetY;
                
                float v1 = Noise(x1, y1);
                float v2 = Noise(x2, y1);
                float v3 = Noise(x1, y2);
                float v4 = Noise(x2, y2);
                
                for(int tx=0; tx<delta && x+tx<width; tx++) {
                    for(int ty=0; ty<delta && y+ty<height; ty++) {
                        float fx = (float)tx/delta;
                        float fy = (float)ty/delta;
                        float i1 = Mathf.Lerp(v1, v2, fx);
                        float i2 = Mathf.Lerp(v3, v4, fx);
                        int px = x+tx;
                        int py = y+ty;
                        map[px, py] = Mathf.Lerp(i1, i2, fy);
                    }
                }
            }
        }
    }
	
	/**
	 * Method to generate a smooth value for a point
	 */
	public float Noise(float x, float y) {
		x = x*scale + offset.x;
		y = y*scale + offset.y;
        float total = 0;
        float frq = 1, amp = 1;
        for (int i = 0; i < octaves; i++) {
            if(i >= 1) {
                frq *= 2;
                amp *= persistence;
            }
            total += InterpolatedSmoothNoise(x*frq, y*frq) * amp;
        }
        return total;
    }

	/**
	 * InterpolatedSmoothNoise is a static method to generate a smoothed and interpolated float value from a 2D vector.
	 */
    private static float InterpolatedSmoothNoise(float X, float Y) {
        int ix = Mathf.FloorToInt(X);
        float fx = X - ix;
        int iy = Mathf.FloorToInt(Y);
        float fy = Y - iy;

        float v1 = SmoothNoise(ix, iy);
        float v2 = SmoothNoise(ix + 1, iy);
        float v3 = SmoothNoise(ix, iy + 1);
        float v4 = SmoothNoise(ix + 1, iy + 1);

        float i1 = Mathf.Lerp(v1, v2, fx);
        float i2 = Mathf.Lerp(v3, v4, fx);

        return Mathf.Lerp(i1, i2, fy);
    }
    
	/**
	 * InterpolatedSmoothNoise is a static method to generate a smoothed (not interpolated) float value from a 2D vector.
	 */
    private static float SmoothNoise(int x, int y) {
        float corners = ( Noise(x-1, y-1)+Noise(x+1, y-1)+Noise(x-1, y+1)+Noise(x+1, y+1) ) / 16f;
        float sides   = ( Noise(x-1, y)  +Noise(x+1, y)  +Noise(x, y-1)  +Noise(x, y+1) ) /  8f;
        float center  =  Noise(x, y) / 4f;
        return corners + sides + center;
    }
    
	/**
	 * The base PerlinNoise definition
	 */
    private static float Noise(int x, int y) {
        int n = x + y * 57;
        n = (n<<13) ^ n;
        return ( 1 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);    
    }
	
	
}
