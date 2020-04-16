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
//using Mapbox.Map;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class CompassIconRotation : MonoBehaviour {

	[HideInInspector]
	public bool compassEnabled = false; 
	//private Vector3 distanceVector = new Vector3 (0.0f, 0.0f, 0.0f);
	private float distance;
	private double distanceLatLong;
	//private ChallengeData challenge;
	private GameObject challengeToFollow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (compassEnabled) {
			if (challengeToFollow != null) {
				// first, get the object rotation, and then snap the compass to horizontal orientaton
				Quaternion previousRotation = GetComponent<RectTransform>().transform.rotation;

				GetComponent<RectTransform> ().transform.eulerAngles = new Vector3 (0.0f,0.0f,0.0f);

				// calculate the angle to rotate
				Vector3 heading;
				Quaternion targetRotation;
				float angleToRotate = 0.0f;

				heading = GameManager.instance._referencePlayer.transform.position - challengeToFollow.transform.position;//obj.transform.position;
				heading.y = 90;


				targetRotation = Quaternion.LookRotation (heading);
				//targetRotation *= Quaternion.Euler(0, 0, 45); // this adds a 90 degrees Y rotation
				//float str = Mathf.Min (2.0f * Time.deltaTime, 1);

				GetComponent<RectTransform>().transform.rotation = Quaternion.Lerp (
					GetComponent<RectTransform>().transform.rotation, 
					targetRotation,1); // this is the strength of the rotation
				
				angleToRotate = GetComponent<RectTransform> ().transform.eulerAngles.y;

				// snap compass back to vertical position, and apply the calculated rotation
				GetComponent<RectTransform>().transform.eulerAngles = new Vector3 (90.0f, 0.0f, 0.0f);
				GetComponent<RectTransform> ().transform.rotation *= Quaternion.AngleAxis (180 - angleToRotate, Vector3.up);

				//Debug.Log ("[compassiconrotation] challenge " + challengeToFollow.GetComponent<ChallengeData> ().challenge_Name +
				//	" has the angle " + angleToRotate + " ; with rotation: "+GetComponent<RectTransform>().transform.eulerAngles.ToString());
				Destroy (challengeToFollow);
				compassEnabled = false;
			} else {
				compassEnabled = false;
				throw new UnityException ("[CompassIconRotation] object to follow is null. Deactivating compass.");
			}
		}
	}
	public object getChallengeDetails(long id, int typeOfChallenge)
	{
		if (typeOfChallenge == 0)
		{
			foreach (Challenge_QuizData q in GameManager.instance.quizChallengesOnScreen) {
				if (q.challengeID == id) {
					return q;
				}
			}
		}	
		if (typeOfChallenge == 1)
		{
			foreach (Challenge_ARData a in GameManager.instance.arChallengesOnScreen) {
				if (a.challengeID == id) {
					return a;
				}
			}
		}

		throw new UnityException("[CompassIconRotation] unrecognizable type of challenge details to attach to the 3D object.");
	}

	public void FollowChallenge(ChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.challengesPrefab, Vector3.zero, Quaternion.identity);
		//DontDestroyOnLoad (genericChallenge);
		//challenge = objToFollow;

		challengeToFollow.AddComponent<ChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		compassEnabled = true;
	}

	public void FollowChallenge(MultiplayerChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.multiplayerChallengesPrefab, Vector3.zero, Quaternion.identity);
		//DontDestroyOnLoad (genericChallenge);
		//challenge = objToFollow;

		challengeToFollow.AddComponent<MultiplayerChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		compassEnabled = true;
	}

	public void FollowChallenge(HunterChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.hunterChallengesPrefab, Vector3.zero, Quaternion.identity);
		//DontDestroyOnLoad (genericChallenge);
		//challenge = objToFollow;

		challengeToFollow.AddComponent<HunterChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		else if (challengeToFollow.CompareTag ("HunterChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			

		}
		compassEnabled = true;
	}
	public void FollowChallenge(VotingChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.votingChallengesPrefab, Vector3.zero, Quaternion.identity);

		challengeToFollow.AddComponent<VotingChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		else if (challengeToFollow.CompareTag ("HunterChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);


		}
		else if (challengeToFollow.CompareTag ("VotingChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);


		}
		compassEnabled = true;
	}
	public void FollowChallenge(TimedTaskChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.timedTaskChallengesPrefab, Vector3.zero, Quaternion.identity);

		challengeToFollow.AddComponent<TimedTaskChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		else if (challengeToFollow.CompareTag ("HunterChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);


		}
		else if (challengeToFollow.CompareTag ("VotingChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
		}
		else if (challengeToFollow.CompareTag ("TimedTaskChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
		}
		compassEnabled = true;
	}

	public void FollowChallenge(OpenQuizChallengeInfo objToFollow)
	{
		challengeToFollow = Instantiate<GameObject>(GameManager.instance.openQuizChallengesPrefab, Vector3.zero, Quaternion.identity);

		challengeToFollow.AddComponent<OpenQuizChallengeInfo> ();
		AttachChallengeToGameObject (objToFollow);
		// set challenge at the given location
		challengeToFollow.transform.MoveToGeocoordinate(objToFollow.latitude,objToFollow.longitude, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
		// put the object in different locations depending on the type of the object
		// raise the game object asset in Y to 10, to be easily seen
		if (challengeToFollow.CompareTag ("Challenge")) {
			//		Debug.Log ("Challenge is being set");
			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y + 10, challengeToFollow.transform.position.z);
			//		Debug.Log ("Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);
		}
		else if (challengeToFollow.CompareTag ("ClosedChessChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

		}
		else if (challengeToFollow.CompareTag ("HunterChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);


		}
		else if (challengeToFollow.CompareTag ("VotingChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
		}
		else if (challengeToFollow.CompareTag ("TimedTaskChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
		}
		else if (challengeToFollow.CompareTag ("OpenQuizChallenge")) {

			challengeToFollow.transform.position = new Vector3 (challengeToFollow.transform.position.x, 
				challengeToFollow.transform.position.y, challengeToFollow.transform.position.z);
		}
		compassEnabled = true;
	}
		

	private void AttachChallengeToGameObject(ChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<ChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<ChallengeInfo> ().challenge_name = challenge.challenge_name;
		challengeToFollow.GetComponent<ChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<ChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<ChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<ChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<ChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<ChallengeInfo> ().question = challenge.question;
		challengeToFollow.GetComponent<ChallengeInfo> ().answer = challenge.answer;
		challengeToFollow.GetComponent<ChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<ChallengeInfo> ().validated = challenge.validated;
	}
	private void AttachChallengeToGameObject(MultiplayerChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().challenge_name = challenge.challenge_name;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().task = challenge.task;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<MultiplayerChallengeInfo> ().validated = challenge.validated;
	}
	private void AttachChallengeToGameObject(HunterChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<HunterChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().name = challenge.name;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().question = challenge.question;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().answer = challenge.answer;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().content_text = challenge.content_text;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().content_picture = challenge.content_picture;
		challengeToFollow.GetComponent<HunterChallengeInfo> ().validated = challenge.validated;
	}
	private void AttachChallengeToGameObject(VotingChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<VotingChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().name = challenge.name;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().task = challenge.task;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().listOfImagesAndVotes = challenge.listOfImagesAndVotes;
		challengeToFollow.GetComponent<VotingChallengeInfo> ().validated = challenge.validated;
	}
	private void AttachChallengeToGameObject(TimedTaskChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().name = challenge.name;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().task = challenge.task;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().questionHowMany = challenge.questionHowMany;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().timer = challenge.timer;
		challengeToFollow.GetComponent<TimedTaskChallengeInfo> ().validated = challenge.validated;
	}

	private void AttachChallengeToGameObject(OpenQuizChallengeInfo challenge)
	{
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ()._id = challenge._id;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().name = challenge.name;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().description = challenge.description;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().latitude = challenge.latitude;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().longitude = challenge.longitude;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().question = challenge.question;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().imageURL = challenge.imageURL;
		challengeToFollow.GetComponent<OpenQuizChallengeInfo> ().validated = challenge.validated;
	}

}
