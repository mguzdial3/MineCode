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
 * NPCMovementController handles movement for a single NPC
 */
public class NPCMovementController : MonoBehaviour {
	public Vector3 _currGoal;//The current location of the NPCs goal
	private float _minDist= 0.05f; //Minimum distance between points for them to be considered equivalent
	public float speed = 5f; //Speed for movement purposes
	public float gravitySpeed = 1f;//Speed of gravity
	public float maxDistance = 10f;//Use for the random generation of a position as a radius

	private int maxChecksPerFrame = 150; //Maximum number of nodes to check in A*

	//Path
	public Vector3[] path; 
	public int pathIndex;

	//States maybe
	private int currState = 0;
	private const int PATHING = 0;
	private const int ACTION = 1;

	private bool pause;

	//Appearance
	public NPCAppearanceController appearanceHandler;
	//Get the difference between where an empty space and the npc's origin should be
	public float midPointHeight = 0.7f;
	//Get the difference between where an empty space is and where an npc's eyes/head should be pointed
	public float eyeHeight = 0.8f;

	//Check for random position generation
	public bool randomPositionGenerate = false;

	//Whether or not the NPC is presently moving
	public bool moving = false;
	
	// Use this for initialization not from save
	public void Init () {
		if (randomPositionGenerate && Map.Instance!=null) {
			//Set up initial position
			Vector3 initialPos = GenerateRandomPosition ();
			transform.position = initialPos;
			Vector3i vec = Map.Instance.getEmptyLOC (new Vector3i(transform.position.x,transform.position.y,transform.position.z))+Vector3i.down;
			transform.position = GetNPCPositionFromPathPoint(vec);
		}

		//Make sure you're not stuck in a wall
		_currGoal = transform.position;
	}

	//Use this for initialization from save
	public void InitFromSave(Vector3 _location, Vector3 _goal){
		transform.position = _location;
		_currGoal = _goal;

		if(path==null && transform.position!=_currGoal){
			path = AStar(transform.position,_currGoal);
			pathIndex = 0;
		}
	}

	public void SetPause(bool _pause){
		pause = _pause;
	}

	// Update is called once per frame
	public bool UpdateMovement () {
		bool changed = false;
		if(!pause){
			//gravity
			if (Map.Instance.IsPositionOpen (GetExpectedGroundPosition())) {
				transform.position-=Vector3.up*Time.deltaTime*gravitySpeed;
				changed=true;
			}

			//pathing handling
			if(currState==PATHING){
				//If no path, find one
				if (path == null || path.Length==0) {
					if(transform.position!=_currGoal){
						path = AStar(transform.position,_currGoal);
						pathIndex = 0;

						if(path!=null){
							appearanceHandler.SetNewGoal(GetNPCEyePositionFromPathPoint(path[pathIndex]));
						}
					}
				}
				//If path, then move between points
				else{
					//Distance to the next node
					Vector3 distToNext = GetNPCPositionFromPathPoint(path[pathIndex])-transform.position;
					//Are we done at this point
					if(distToNext.magnitude<_minDist){
						transform.position = GetNPCPositionFromPathPoint(path[pathIndex]);
						pathIndex++;
						changed = true;//Altered the path index

						if(pathIndex>=path.Length){
							path=null; //Did we reach the end of the path
						}
						else{
							//Ensure that the next position is still available
							if(IsViablePosition(new Vector3i(path[pathIndex]))){
								appearanceHandler.SetNewGoal(GetNPCEyePositionFromPathPoint(path[pathIndex])); //On to the next point in the path
								_currGoal = path[pathIndex];
							}
							else{
								//REPLAN
								path = null;
							}
						}
					}
					else{
						//Moving towards the next one
						float speedToUse = speed;

						//Make sure the npc can escape gravity
						if(changed){
							speedToUse = gravitySpeed+speed;
						}

						transform.position+=distToNext.normalized*Time.deltaTime*speedToUse;
						changed = true;
					}
				}
			}
		}

		moving = changed;

		return changed;
	}


	//Quick implementation of A* pathing
	private Vector3[] AStar(Vector3 start, Vector3 goal){
		List<PathingNode> closedSet = new List<PathingNode> ();
		List<PathingNode> openSet = new List<PathingNode> ();

		Vector3i goalI = new Vector3i (goal.x,goal.y,goal.z);
		Vector3i startI = new Vector3i (start.x, start.y-midPointHeight, start.z);

		PathingNode currNode = new PathingNode (startI, goalI);
		openSet.Add (currNode);

		int count = 0;
	
		//Brief check to stop if we're already at the goal; or if our current position is not viable
		if (startI.Equals( goalI) || !IsViablePosition(startI)) {
			return null;
		}

		//Actual A* pathing
		while (openSet.Count!=0 && count<maxChecksPerFrame) {
			count++;
			currNode=openSet[0];

			//get best next node
			foreach(PathingNode node in openSet){
				if(node.GetFScore()<currNode.GetFScore()){
					currNode = node;
				}
			}

			//Did we make it
			if(currNode.loc.DistanceSquared(goalI)<3 || count==maxChecksPerFrame){
				return GetPathFromNode(currNode);
			}
			else{
				openSet.Remove(currNode);
				closedSet.Add(currNode);

				//Get next allowable moves
				List<Vector3i> moves = GetPossibleNextMovesFromPosition(currNode.loc);

				foreach(Vector3i move in moves){
					PathingNode potentialNode = new PathingNode(move, goalI, currNode);

					if(closedSet.Contains(potentialNode)){}
					else if(openSet.Contains(potentialNode)){ //See if we need to update the node
						int index = openSet.IndexOf(potentialNode);
						if(openSet[index].g_score > potentialNode.g_score){
							//Update the node if this version is better
							openSet[index]=potentialNode;
						}
					}
					else{
						//Add the node to the openSet if its new
						openSet.Add(potentialNode);
					}
				}
			}

		}
		return null;
	}

	//Set the current goal of this npc
	public void SetCurrGoal(Vector3 goal){
		path = null;
		_currGoal = goal;

		path = AStar(transform.position,_currGoal);
		pathIndex = 0;
	}

	//Force the enemy to replan
	public void DeletePlan(){
		path = null;
		//path = AStar(transform.position,_currGoal);
		pathIndex = 0;
	}

	//Return a Vector3 value of the current goal
	public Vector3 GetGoal(){
		return _currGoal;
	}

	//Returns a Vector3 value of the current end goal of this NPC
	public Vector3[] GetPath(){
		return path;
	}

	//Private method to trace back a node to get a path
	private Vector3[] GetPathFromNode(PathingNode goal){
		List<Vector3> protoPath = new List<Vector3>();

		if (goal.GetParent () == null) {
			protoPath.Add (new Vector3(goal.loc.x,goal.loc.y,goal.loc.z));
			goal = goal.GetParent();	
		}
		else{
			while (goal.GetParent()!=null) {
				protoPath.Add (new Vector3(goal.loc.x,goal.loc.y,goal.loc.z));
				goal = goal.GetParent();
			}
		}

		protoPath.Reverse ();
		return protoPath.ToArray ();
	}

	//Check if position is viable for this NPC
	private bool IsViablePosition(Vector3i position){
		return Map.Instance.IsPositionOpen (position) && Map.Instance.IsPositionOpen (position + Vector3i.up*midPointHeight*2) && !Map.Instance.IsPositionOpen (position + Vector3i.down);
	}

	//Returns a list of possible next moves from a position
	private List<Vector3i> GetPossibleNextMovesFromPosition(Vector3i position){
		List<Vector3i> nextMoves = new List<Vector3i>();

		//Left
		nextMoves.Add ( position + Vector3i.left);

		//Right
		nextMoves.Add ( position + Vector3i.right);

		//Forward
		nextMoves.Add ( position + Vector3i.forward);

		//Back
		nextMoves.Add ( position + Vector3i.back);

		//DIAGONALS

		// Left-Forward
		if (Map.Instance.IsPositionOpen (position + Vector3i.left) || Map.Instance.IsPositionOpen (position + Vector3i.forward)) {
			nextMoves.Add (position + Vector3i.left + Vector3i.forward);
		}

		// Left-Backward
		if (Map.Instance.IsPositionOpen (position + Vector3i.left) || Map.Instance.IsPositionOpen (position + Vector3i.back)) {
			nextMoves.Add (position + Vector3i.left + Vector3i.back);
		}

		//Right-Forward
		if (Map.Instance.IsPositionOpen (position + Vector3i.right) || Map.Instance.IsPositionOpen (position + Vector3i.forward)) {
			nextMoves.Add (position + Vector3i.right + Vector3i.forward);
		}
		//Right-Backward
		if (Map.Instance.IsPositionOpen (position + Vector3i.right) || Map.Instance.IsPositionOpen (position + Vector3i.back)) {
			nextMoves.Add (position + Vector3i.right + Vector3i.back);
		}

		//Go through and check to make sure all are acceptable
		for(int i = 0; i<nextMoves.Count; i++){
			if(IsViablePosition(nextMoves[i])){ //Place to stand
				//Good to go
			}
			else if(IsViablePosition(nextMoves[i]+Vector3i.down)){
				//Jump down one
				nextMoves[i]+=Vector3i.down;
			}
			else if(IsViablePosition(nextMoves[i]+Vector3i.up)){
				//Jump up one
				nextMoves[i]+=Vector3i.up;
			}
			else{
				nextMoves[i]=position; //Way to get rid of it
			}

		}

		return nextMoves;
	}

	//Generates a random position for this NPC (for use in original placement/random goals
	private Vector3 GenerateRandomPosition(){
		Vector3 vec = transform.position + Vector3.right * (Random.Range (-1f, 1f) * maxDistance)
			+ Vector3.forward * (Random.Range (-1f, 1f) * maxDistance);
	
		vec.x = Mathf.Clamp (vec.x, maxDistance * -1, maxDistance);
		vec.z = Mathf.Clamp (vec.z, maxDistance * -1, maxDistance);

		return vec;
	}
	
	//Get where the npc's midpoint should be
	private Vector3 GetNPCPositionFromPathPoint(Vector3i pathPosition){
		return pathPosition.ToVector3 () + Vector3.up * midPointHeight;
	}

	private Vector3 GetNPCPositionFromPathPoint(Vector3 pathPosition){
		return pathPosition + Vector3.up * midPointHeight;
	}

	//Get where we'd expect the npc's eyes to be 
	private Vector3 GetNPCEyePositionFromPathPoint(Vector3 pathPosition){
		return pathPosition + Vector3.up * eyeHeight;
	}

	//Get where we'd expect to see ground
	private Vector3i GetExpectedGroundPosition(){
		Vector3i position = new Vector3i (transform.position+midPointHeight*Vector3.down);
		position += Vector3i.down; 
		return position;
	}

	//Returns the current euclidean distance from this enemy to the player
	public float GetDistanceToTarget(GameObject target){
		Vector3 toPlayerVector = target.transform.position - transform.position;
		return toPlayerVector.magnitude;
	}

	//Generates a random position and set that to the current goal
	public void SetRandomGoal(){
		Vector3 randomPos = GenerateRandomPosition ();
		if (Map.Instance != null) {
			Vector3i vec = Map.Instance.getEmptyLOC (new Vector3i (randomPos));

			//Set current goal to the random position
			SetCurrGoal (vec.ToVector3 ());
		}
	}

	//Set the passed in gameobject's current position to the goal of this NPC
	public void SetTargetPositionGoal(GameObject target){
		SetCurrGoal (target.transform.position);
	}

	//Stops the NPC immediately wherever it is located
	public void Stop(){
		path = null;
		_currGoal = transform.position;
	}

	//Returns true if the NPC has reached its current goal, false otherwise
	public bool ReachedGoal(){
		return path == null || path.Length==0 || _currGoal==transform.position;
	}

	//Returns true if the target GameObject can be seen, false otherwise
	public bool CanSeeTarget(GameObject target){
		Vector3 toTarget = target.transform.position - transform.position;
		Vector3 forward = transform.forward;

		return Vector3.Angle (toTarget.normalized, forward.normalized)<100;
	}

}

/**
 * PathingNode is a info holder representation for a node in the A* pathing in NPCMovementController
 */
public class PathingNode {
	public Vector3i loc; //3D location of this node
	public int g_score=0; //Score of this node
	private int f_score=0;
	private PathingNode _parent; //The node that occurs before this one in the potential path
	
	public PathingNode(Vector3i _loc, Vector3i goal){
		this.loc = _loc;
		f_score = (int)( (loc.DistanceSquared (goal)));
	}
	
	public PathingNode(Vector3i _loc, Vector3i goal, PathingNode parent): this(_loc, goal){
		_parent = parent;
		g_score = parent.g_score +1;

	}
	
	public int GetFScore(){
		return g_score + f_score;
	}
	
	public PathingNode GetParent(){
		return _parent;
	}
	
	public override bool Equals (object obj){
		PathingNode objNode = (PathingNode)obj;
		
		return obj != null && objNode != null && loc.Equals(objNode.loc);
	}
	
	public override int GetHashCode (){
		return loc.GetHashCode();
	}
}

