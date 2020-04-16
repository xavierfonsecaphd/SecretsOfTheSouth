/**
Copyright (c)  2019, Francisco Xavier Dos Santos Fonseca (Ordem dos Engenheiros n.º 84598), and Technical University of Delft. 
All rights reserved. 

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 

3. All advertising materials mentioning features or use of this software must display the following acknowledgement: 
This product includes software developed by the Technical University of Delft. 

4. Neither the name of  the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY  COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL  COPYRIGHT HOLDER BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity;

public class CompassAutoRotation : MonoBehaviour {

	public static CompassAutoRotation instance { set; get ; }
	// This is a singleton
	private static CompassAutoRotation singleton;

	[HideInInspector]
	public bool compassEnabled = false; 
	private GameObject objectToFollow;
	private Vector3 distanceVector;
	private float distance;
	private double distanceLatLong;

	// Use this for initialization
	void Awake () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad (gameObject);
			instance = CompassAutoRotation.singleton;

			distanceVector = new Vector3 (0.0f, 0.0f, 0.0f);
			objectToFollow = null;
			SetActive (false);
		}
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (compassEnabled) {
			if (objectToFollow != null) {
				
				// rotation that the compass has in regard to the camera (Starting position)
				//float angle = Quaternion.Angle(transform.rotation, GameManager.instance._referenceCamera.transform.rotation);
				Vector3 heading = GameManager.instance._referencePlayer.transform.position - objectToFollow.transform.position;
				heading.y = 0;

				float strength = 1.0f;
				Quaternion targetRotation = Quaternion.LookRotation (heading);
				float str = Mathf.Min (strength * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);

				//ChallengeData cd = objectToFollow.GetComponent<ChallengeData> ();

				distanceVector.x = GameManager.instance._referencePlayer.transform.position.x - objectToFollow.transform.position.x;
				distanceVector.y = GameManager.instance._referencePlayer.transform.position.y - objectToFollow.transform.position.y;
				distanceVector.z = GameManager.instance._referencePlayer.transform.position.z - objectToFollow.transform.position.z;
				distance = Mathf.Sqrt (distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y + distanceVector.z * distanceVector.z);


				// string.Equals objectToFollow.tag, "MultiplayerChallenge" ClosedChessChallenge

				if (objectToFollow) { // if the tag exists
					if (string.Equals (objectToFollow.tag, "ClosedChessChallenge")) {

						//txt20
						string [] displayMessage1 = {"Quiz Challenge at ",
							"Quiz Challenge op ",
							"Desafio Quiz a "};
						
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language] +
						objectToFollow.GetComponent<ChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowChallenge (null, false);
						}

					} else if (string.Equals (objectToFollow.tag, "MultiplayerChallenge")) {
						//txt20
						string [] displayMessage2 = {"Multiplayer Challenge at ",
							"Multiplayer Challenge op ",
							"Desafio Multiplayer a "};
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage2[MasterManager.language] +
						objectToFollow.GetComponent<MultiplayerChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowMultiplayerChallenge (null, false);
						}
					}
					else if (string.Equals (objectToFollow.tag, "HunterChallenge")) {
						//txt20
						string [] displayMessage3 = {"Hunter Challenge at ",
							"Hunter Challenge op ",
							"Desafio Hunter a "};
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage3[MasterManager.language] +
							objectToFollow.GetComponent<HunterChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowHunterChallenge (null, false);
						}
					}
					else if (string.Equals (objectToFollow.tag, "VotingChallenge")) {
						//txt20
						string [] displayMessage4 = {"Voting Challenge at ",
							"Voting Challenge op ",
							"Desafio com voto a "};
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage4[MasterManager.language] +
							objectToFollow.GetComponent<VotingChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowVotingChallenge (null, false);
						}
					}
					else if (string.Equals (objectToFollow.tag, "TimedTaskChallenge")) {
						//txt20
						string [] displayMessage5 = {"TimedTask Challenge at ",
							"TimedTask Challenge op ",
							"Desafio com tempo a "};
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage5[MasterManager.language] +
							objectToFollow.GetComponent<TimedTaskChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowTimedTaskChallenge (null, false);
						}
					}
					else if (string.Equals (objectToFollow.tag, "OpenQuizChallenge")) {
						//txt20
						string [] displayMessage6 = {"OpenQuiz Challenge at ",
							"OpenQuiz Challenge op ",
							"Desafio OpenQuiz a "};
						GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage6[MasterManager.language] +
							objectToFollow.GetComponent<OpenQuizChallengeInfo> ().DistanceToString ());

						if ( distance < 70.0f) {
							GameManager.instance.FollowOpenQuizChallenge (null, false);
						}
					}
				}
			}
		}
	}

	// this function is to set the compass to automatically point to other objects in the game
	// to make the compass work, send an object to follow as parameter. if you want the compass to stop, send a null parameter as an object
	public void SetObjectToFollow (GameObject obj)
	{



		if (obj != null) {
			objectToFollow = obj;
			compassEnabled = true;
			SetActive (true);
		} else {
			compassEnabled = false;
			objectToFollow = null;
			SetActive (false);
		}
	}

	private void SetActive(bool active)
	{
		if (active) {
			gameObject.SetActive (true);
		} else {
			gameObject.SetActive (false);
		}
	}

	// this function calculates the distance between two LatLonG GPS points on the map. 
	private double DistanceToPlayer(Vector2d latLongTargetObject, Vector2d latlongPlayer)
	{
		double circumference = 40075.0; // Earth's circumference at the equator in km
		double distance = 0.0;



		//Calculate radians
		double latitude1Rad = latLongTargetObject.x * Mathd.PI / 180.0;
		double longitude1Rad = latLongTargetObject.y * Mathd.PI / 180.0;
		double latititude2Rad = latlongPlayer.x * Mathd.PI / 180.0;
		double longitude2Rad = latlongPlayer.y * Mathd.PI / 180.0;

		double logitudeDiff = Mathd.Abs(longitude1Rad - longitude2Rad);
		if (logitudeDiff > Mathd.PI)
		{
			logitudeDiff = 2.0 * Mathd.PI - logitudeDiff;
		}

		double angleCalculation =
			Mathd.Acos(
				Mathd.Sin(latititude2Rad) * Mathd.Sin(latitude1Rad) +
				Mathd.Cos(latititude2Rad) * Mathd.Cos(latitude1Rad) * Mathd.Cos(logitudeDiff));

		distance = circumference * angleCalculation / (2.0 * Mathd.PI);

		return distance;
	}

	// this pr
	public string DistanceToString(double distanceToShow)
	{

		if (distanceToShow < 1) {
			distanceToShow *= 1000.0; // remove the 0, ...
			distanceToShow = System.Math.Round (distanceToShow, 0);
			//txt20
			string [] displayMessage1 = {" meters",
				" meter",
				" metros"};
			return "" + distanceToShow.ToString () + displayMessage1[MasterManager.language];
		} else {
			int km = (int) System.Math.Round (Mathd.Floor(distanceToShow), 0); 

			double floor = distanceToShow % 1.0;
			if (floor > 1.0) {
				floor -= 1.0;
			}
			floor *= 100.0; // remove the 0, but work with only 2 decimals after
			floor = System.Math.Round (floor, 0);
			if (floor >= 10)
			{
				return "" + km.ToString () + "." + floor + " km";
			}
			else {
				return "" + km.ToString () + ".0" + floor + " km";
			}
		}
	}
}
