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
using UnityEditor;
using System.Collections;

/**
 * This script is used as a singleton to handle drawing out an atlas texture and atlas editor in the unity editor. 
 * You probably shouldn't mess with this.
 */
public class AtlasViewer {

	/**
	 * Method called to actually draw the atlas texture in the editor
	 */
	public static Rect DrawAtlas(Atlas atlas, Rect texCoords) {
		return DrawTexture(atlas.GetMaterial().mainTexture, texCoords);
	}

	/**
	 * Backend of method called to draw the atlas texture onto the editor
	 */
	private static Rect DrawTexture(Texture texture, Rect texCoords) {
		GUILayout.BeginVertical("box");
		Rect rect = GUILayoutUtility.GetAspectRect((float)texture.width/texture.height);
		GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
		GUILayout.EndVertical();
		return rect;
	}

	/**
	 * Draw the cursor to select parts of the atlas when the user wants to edit it
	 */
	public static void DrawCursor(Rect cursorRect, Rect viewRect, Rect texCoords) {
		cursorRect = FromWindowCoord(cursorRect, texCoords);
		cursorRect.y = 1-cursorRect.y;
		cursorRect.height *= -1;
		cursorRect = Scale(cursorRect, viewRect.width, viewRect.height);
		
		GUI.BeginGroup(viewRect);
		DrawRect(cursorRect, Color.green);
		GUI.EndGroup();
		
		cursorRect = Scale(cursorRect, 1.0f/viewRect.width, 1.0f/viewRect.height);
		cursorRect.height *= -1;
		cursorRect.y = 1-cursorRect.y;
		cursorRect = ToWindowCoord(cursorRect, texCoords);
	}

	/**
	 * Helper method used to draw a selection rectangle
	 */
	public static void DrawRect(Rect rect, Color color) {
		Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
		Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
		Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
		Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);
		
		Handles.color = color;
		Handles.DrawLine(a, b);
		Handles.DrawLine(b, c);
		Handles.DrawLine(c, d);
		Handles.DrawLine(d, a);
	}

	/**
	 * Helper method to transfer a rectangle to window coordinates
	 */
	private static Rect ToWindowCoord(Rect rect, Rect window) {
		rect = Scale(rect, window.width, window.height);
		rect.x += window.x;
		rect.y += window.y;
		return rect;
	}

	/**
	 * Helper method to transfer a rectangle from window coordinates to "local" ones
	 */
	private static Rect FromWindowCoord(Rect rect, Rect window) {
		rect.x -= window.x;
		rect.y -= window.y;
		return Scale(rect, 1.0f/window.width, 1.0f/window.height);
	}

	/**
	 * Helper method to scale a rectangle
	 */
	private static Rect Scale(Rect rect, float x, float y) {
		rect.xMin *= x;
		rect.xMax *= x;
		rect.yMin *= y;
		rect.yMax *= y;
		return rect;
	}

	/**
	 * Helper method to move a single x,y point to "window" coordinates
	 */
	public static Vector2 ToWindowCoord(Vector2 v, Rect window) {
		v.x *= window.width;
		v.y *= window.height;
		v.x += window.x;
		v.y += window.y;
		return v;
	}

	/**
	 * Helper method to move a single x,y point from "window" coordinates to "local" ones
	 */
	public static Vector2 FromWindowCoord(Vector2 v, Rect window) {
		v.x -= window.x;
		v.y -= window.y;
		v.x /= window.width;
		v.y /= window.height;
		return v;
	}

	/**
	 * Method used to draw the atlas editor when the user wants to make changes to the atlas
	 */
	public static void DrawAtlasEditor(Atlas atlas, ref Rect texCoords, ref Rect position) {
		Rect viewRect = DrawAtlas(atlas, texCoords);
		
		DrawCursor(position, viewRect, texCoords);
		
		if(!viewRect.Contains(Event.current.mousePosition)) return;
		
		Vector2 mouse = FromWindowCoord(Event.current.mousePosition, viewRect);
		mouse.y = 1-mouse.y;
			
		if(Event.current.type == EventType.ScrollWheel) {
			float scale = 0.95f;
			if(Event.current.delta.y > 0) scale = 1.0f/scale;
			Rect oldTexCoords = texCoords;
				
			texCoords.width = Mathf.Clamp01(texCoords.width*scale);
			texCoords.height = Mathf.Clamp01(texCoords.height*scale);
			texCoords.x -= (texCoords.width-oldTexCoords.width)*mouse.x;
			texCoords.y -= (texCoords.height-oldTexCoords.height)*mouse.y;
				
			texCoords = FixTexCoords(texCoords);
			Event.current.Use();
			GUI.changed = true;
		}
		if(Event.current.type == EventType.MouseDrag && Event.current.button == 1) {
			texCoords.x -= Event.current.delta.x/viewRect.width*texCoords.width;
			texCoords.y += Event.current.delta.y/viewRect.height*texCoords.height;
				
			texCoords = FixTexCoords(texCoords);
			GUI.changed = true;
		}
			
			
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0) {
			Vector3 pos = ToWindowCoord(mouse, texCoords);
			position.x = pos.x - pos.x%atlas.GetTileSizeX();
			position.y = pos.y - pos.y%atlas.GetTileSizeY();
			position.width = atlas.GetTileSizeX();
			position.height = atlas.GetTileSizeY();
			GUI.changed = true;
		}
	}

	/**
	 * Helper method to clamp rect (texture) values
	 */
	private static Rect FixTexCoords(Rect rect) {
		rect.x = Mathf.Clamp01(rect.x);
		rect.y = Mathf.Clamp01(rect.y);
		if(rect.xMax > 1) rect.x -= rect.xMax%1;
		if(rect.yMax > 1) rect.y -= rect.yMax%1;
		return rect;
	}
	
	
}
