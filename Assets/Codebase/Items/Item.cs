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

/**
 * Item is the parent class for all items in the game
 */
public class Item : MonoBehaviour {
	[SerializeField]private string itemName;
	public string ItemName{ get { return itemName; } }
	[SerializeField]private float itemScale=1.0f;
	public float ItemScale{ get { return itemScale; } }
	[SerializeField]private bool autoReplace;
	public bool AutoReplace{ get { return autoReplace; } }

	//Handles what happens when this item is picked up in a child class
	public virtual void Pickup(){}

	//Handles when the item is held and used. Returns true if used
	public virtual bool Use(GameObject hitObject){return false;}

	void Start(){
		gameObject.name = itemName;
	}

	public GameObject CreateHeldItem(){
		GameObject go = Instantiate<GameObject> (gameObject);

		BoxCollider boxCollider = go.GetComponent<BoxCollider> ();
		Destroy (boxCollider);

		go.transform.localScale = transform.localScale * itemScale;

		return go;
	}
}
