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

public class OscReceiver : MonoBehaviour
{
    //You can set these variables in the scene because they are public 
    public string RemoteIP = "127.0.0.1";
    public int SendToPort = 50001;
    public int ListenerPort= 57131;

    //You'll have to set these references yourself
    public UDPPacketIO controller; 
    public Osc handler;

	private bool doPolicy = false;
	public PolicyFollower policyFollower;
	private double[,] policy;	
    
    
    // Use this for initialization
    void Start ()
    {
        //Initializes on start up to listen for messages
        //make sure this game object has both UDPPackIO and OSC script attached
        controller.init(RemoteIP, SendToPort, ListenerPort);
        handler.init(controller);

		handler.SetAddressHandler ("/policy", HandlePolicyInput);
       
    }

    void Update(){
       //All updating of variables happens here

		if (doPolicy) {
			policyFollower.BeginRun(policy);	
			doPolicy = false;
		}
    }

	public void HandlePolicyInput(OscMessage oscMessage){
		int c =oscMessage.Values.Count;

		policy = new double[(c-1),5];

		for(int i = 0; i<(c); i++){
			string s = oscMessage.Values[i].ToString();
			string[] vals = s.Split(',');

			for(int j=0; j<vals.Length; j++){
				policy[i,j] = double.Parse(vals[j]);
			}

		}
		doPolicy = true;

	}


}
