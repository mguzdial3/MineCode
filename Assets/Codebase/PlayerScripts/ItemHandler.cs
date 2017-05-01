using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemHandler : MonoBehaviour {
	public static ItemHandler Instance;
	private float _distanceOfRaycast=10f;//The distance the player's item can "reach"

	public GameObject[] spawnableItems;
	public Dictionary<string, int> itemCounts;//The

	//This variable is used in Lesson Two
	private static List<string> _lastItems;
	public static string lastPickedUp {
		get {
			if (_lastItems.Count == 0) {
				return "";
			} 
			else {
				return _lastItems [_lastItems.Count - 1];
			}
		}
	}

	//This variable is used in Lesson Two
	public static bool justPickedKey{
		get{
			return _lastItems.Contains("Key");
		}
	}

	//This variable is used in Lesson Two
	public static bool justPickedSword{
		get{
			return lastPickedUp=="Sword";
		}
	}

	//A reference to the currently held item by the player
	[SerializeField] private Item playerHeldItem;

	//Strike information
	private bool striking = false;
	private Vector3 originalRotation;
	private float strikeAngle = 20f;

	void Start(){
		Instance = this;
		originalRotation = playerHeldItem.transform.localEulerAngles;
		_lastItems = new List<string> ();

		itemCounts = new Dictionary<string,int>();

	}

	void OnTriggerEnter(Collider c){
		if (c.tag == "Item") {
			Pickup (c.gameObject);
		}
	}

	private void SetLayerInChildren(GameObject obj){
		foreach (Transform child in obj.transform) {
			child.gameObject.layer = LayerMask.NameToLayer("Item");
			SetLayerInChildren(child.gameObject);
		}
	}

	public void Pickup(GameObject obj){
		_lastItems.Add(obj.name);
		Item item = obj.GetComponent<Item> ();
		if (item != null) {
			//Update counts collected
			if( !itemCounts.ContainsKey(item.ItemName)){
				itemCounts[item.ItemName] = 0;
			}
			itemCounts[item.ItemName] = itemCounts[item.ItemName]+1;

			obj.layer = LayerMask.NameToLayer("Item");//Change to "item" layer

			//Change children to item layer
			SetLayerInChildren(obj);

			if(item.AutoReplace){
				ReplaceHeldItem(item);
			}
			item.Pickup();
		}
		GameObject.Destroy (obj);
	}



	void Update(){
		if (striking) {
			if((playerHeldItem.transform.localEulerAngles-originalRotation).magnitude>1f){
				playerHeldItem.transform.localEulerAngles = Vector3.Lerp(playerHeldItem.transform.localEulerAngles, originalRotation,Time.deltaTime*5);
			}
			else{
				playerHeldItem.transform.localEulerAngles = originalRotation;
				striking = false;
			}
		}
	}

	void FixedUpdate(){
		if (_lastItems.Count != 0) {
			_lastItems.Clear();
		}
	}

	public bool UseHeldItem(){
		playerHeldItem.transform.localEulerAngles += Vector3.right * strikeAngle;
		striking = true;

		RaycastHit hit;
		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, _distanceOfRaycast)) {
			return playerHeldItem.Use (hit.collider.gameObject);
		}

		return false;
	}

	public void ReplaceHeldItem(Item newItem){
		if (newItem != null) {
			GameObject newItemObject = newItem.CreateHeldItem();

			newItemObject.transform.position = playerHeldItem.transform.position;
			newItemObject.transform.rotation = playerHeldItem.transform.rotation;

			newItemObject.transform.parent = playerHeldItem.transform.parent;

			Destroy (playerHeldItem.gameObject);
			playerHeldItem = newItemObject.GetComponent<Item>();
		}
	}

	//Use this function to create an item based on the passed in gameObject
	public static void SpawnItem(GameObject obj, float posX, float posY, float posZ){
		Instantiate (obj, new Vector3(posX,posY,posZ), Quaternion.identity);
	}

	//Use this function to create an item based on the string value of the first argument. If no item with that name exists in spawnableItems, do nothing
	public static void SpawnItemFromString(string itemName, float posX, float posY, float posZ){
		GameObject toSpawn = null;

		foreach (GameObject g in Instance.spawnableItems) {
			if(g.GetComponent<Item>().ItemName==itemName){
				toSpawn = g;
			}
		}

		if (toSpawn != null) {
			SpawnItem(toSpawn,posX,posY,posZ);
		}
	}
		
	public int GetCount(string itemName){
		int count = 0;

		if(itemCounts.ContainsKey(itemName)){
			count = itemCounts[itemName];
		}
		return count;
	}

	public static int GetCountCollected(string itemName){
		return Instance.GetCount(itemName);	
	}

}