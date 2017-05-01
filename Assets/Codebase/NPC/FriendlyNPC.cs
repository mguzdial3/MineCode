using UnityEngine;
using System.Collections;

public class FriendlyNPC : MonoBehaviour {
	Vector3 playerPos;
	public GameObject player;
	NPCAppearanceController appearancreController;
	public float height = 1;
	public Quest myQuest;
	public float followDistance = 5;

	void Start(){
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		appearancreController = gameObject.GetComponent<NPCAppearanceController> ();
		appearancreController.Init ();
	}

	// Update is called once per frame
	void Update () {
		if(playerPos!=player.transform.position && (transform.position-player.transform.position).magnitude<followDistance){
			playerPos = player.transform.position;
			appearancreController.SetNewGoal(playerPos+Vector3.up*height);
		}
		else{
			appearancreController.SetNewGoal(transform.forward+transform.position);
		}
		appearancreController.UpdateAppearance ();

		if (myQuest != null) {
			if(myQuest.HasStarted()){
				myQuest.QuestUpdate();
			}
		}
	}

	public void SetQuest(string questName){
		if (myQuest != null && myQuest.CanEnd() && ! myQuest.HasEnded()) {
			//End Quest
			myQuest.QuestEnd();
			RemoveMyQuest();
		}
		myQuest =  (Quest) gameObject.AddComponent (System.Type.GetType(questName));

		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		myQuest.SetPlayer (player);
		myQuest.SetNPC (this);
	}


	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			PlayerEnteredArea();
		}
	}

	void OnTriggerStay(Collider other){
		if (other.tag == "Player" && !GUIManager.HasDisplayText ()) {
			PlayerEnteredArea();
		}
	}

	private void RemoveMyQuest(){
		myQuest = null;
	}

	private bool HasQuest(){
		return myQuest != null;
	}


	private void PlayerEnteredArea(){
		if (HasQuest()) {
			if(!myQuest.HasStarted()){
				if (myQuest.CanStart ()) {
					myQuest.QuestStart ();
				} else {
					myQuest.CannotStart ();
				}
			}
			else{
				if(myQuest.CanEnd()){
					Quest oldQuest = myQuest;
					RemoveMyQuest();
					oldQuest.QuestEnd();

				}
			}
		}
	}

}
