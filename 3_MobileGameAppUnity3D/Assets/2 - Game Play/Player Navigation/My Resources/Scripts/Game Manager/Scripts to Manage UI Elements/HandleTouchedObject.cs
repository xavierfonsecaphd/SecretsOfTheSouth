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
using UnityEngine.Events;
using UnityEngine.UI;
using System;

using PlayFab.Json;
using UnityEngine.Networking;

// this script was made to be attached to the Player. 
public class HandleTouchedObject : MonoBehaviour {

	private Quiz_ModalWindow quiz_modalPanel;
	private Multiplayer_ModalWindow multiplayer_modalPanel;
	private Hunter_ModalWindow hunter_modalPanel;
	private UnityAction myAction1, myAction2, myAction3;
	private GameObject foundObject;
	private ChallengeInfo activeChallenge;
	private MultiplayerChallengeInfo activeMultiplayerChallenge;
	private HunterChallengeInfo activeHunterChallenge;
	private VotingChallengeInfo activeVotingChallenge;
	private TimedTaskChallengeInfo activeTimedTaskChallenge;
	private OpenQuizChallengeInfo activeOpenQuizChallenge;


	[HideInInspector]
	public bool isProperlyInitialized = false;

	// actual materials assigned to this script
	public Material colourNotSolved;
	public Material colourSolved;
	//private List <string> teamplayers = new List<string>();

	void Start()
	{
		quiz_modalPanel = Quiz_ModalWindow.Instance ();
		//ar_modalPanel = AR_ModalWindow.Instance ();
		multiplayer_modalPanel = Multiplayer_ModalWindow.Instance ();
		hunter_modalPanel = Hunter_ModalWindow.instance;
	}


	void Update()
	{
		// Handle eventual game objects (like challenges, orther players, POI, etc).
		if (isProperlyInitialized) {
			if (Input.touchCount == 1) { //if only one finger touching the phone
				if (Input.touches [0].phase == TouchPhase.Began) {
					// was it a challenge?
					SearchForChallenge ();	
					// was it something else? Do you want to handle that?
				}
			}
		}
	}
		


	public void SearchForChallenge()
	{
		string[] quizChallengeFoundText1 = {"You found a quiz challenge at more than ", "Je hebt een quiz uitdaging gevonden op meer dan ", "Encontraste um desafio quiz a mais de "};
		string[] quizChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};
		string[] multiplayerChallengeFoundText1 = {"You found a multiplayer challenge at more than ", "Je hebt een multiplayer uitdaging gevonden op meer dan ", "Encontraste um desafio multi jogador a mais de "};
		string[] multiplayerChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};
		string[] hunterChallengeFoundText1 = {"You found a hunter challenge at more than ", "Je hebt een hunter uitdaging gevonden op meer dan ", "Encontraste um desafio caçador a mais de "};
		string[] hunterChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};
		string[] votingChallengeFoundText1 = {"You found a voting challenge at more than ", "Je hebt een voting uitdaging gevonden op meer dan ", "Encontraste um desafio de voto a mais de "};
		string[] votingChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};
		string[] timedtaskChallengeFoundText1 = {"You found a timed task challenge at more than ", "Je hebt een timed task uitdaging gevonden op meer dan ", "Encontraste um desafio com tempo a mais de "};
		string[] timedtaskChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};
		string[] openQuizChallengeFoundText1 = {"You found an open quiz challenge at more than ", "Je hebt een open quiz uitdaging gevonden op meer dan ", "Encontraste um desafio quiz de resposta aberta a mais de "};
		string[] openQuizChallengeFoundText2 = {" meters. Please walk closer to look at it...", " meter. Loop dichterbij om ernaar te kijken ...", " metros. Por favor aproxima-te para o acederes..."};

		// this will only search for meaningful touches on the screen when any modal windows are not in the way
		if (!GameManager.instance.isAnyWindowOpen) {

			var ray =  GameManager.instance._referenceCamera.GetComponent<Camera>().ScreenPointToRay (Input.GetTouch (0).position);
			float distance, deltaX, deltaY, deltaZ;

			//if you hit another gameObject
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) 
			{	// and if that's a challenge

				Debug.Log ("[HandleTouchedObject] Touched: " + hit.collider.tag);
				// MultiplayerChallenge
				if (string.Equals (hit.collider.tag, "Challenge") || string.Equals (hit.collider.tag, "ClosedChessChallenge") ) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					ChallengeInfo challenge = foundObject.GetComponent<ChallengeInfo>();
					Debug.Log ("You Found the " + challenge.challenge_name + " challenge (ID: " + 
						challenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (challenge != null)) {
							// whenever you handle the challenge positively, do something with the object.
							// don't forget to communicate that to the server!

							StartCoroutine (HandleChallenge (challenge));
						} else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (quizChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt (distance) + quizChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (HandleChallenge (challenge));
					}


				} else if(string.Equals (hit.collider.tag, "MultiplayerChallenge")) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					MultiplayerChallengeInfo mchallenge = foundObject.GetComponent<MultiplayerChallengeInfo>();
					Debug.Log ("You Found the " + mchallenge.challenge_name + " Multiplayer challenge (ID: " + 
						mchallenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (mchallenge != null)) {

							// request if all the members of the team are around, otherwise he cannot see the challenge
							StartCoroutine (EnterChallengeAsTeam(mchallenge));
						}
						else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (multiplayerChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt(distance) + multiplayerChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (EnterChallengeAsTeam(mchallenge));
					}

				}
				else if(string.Equals (hit.collider.tag, "HunterChallenge")) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					HunterChallengeInfo hchallenge = foundObject.GetComponent<HunterChallengeInfo>();
					Debug.Log ("You Found the " + hchallenge.name + " Hunter challenge (ID: " + 
						hchallenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (hchallenge != null)) {

							// request if all the members of the team are around, otherwise he cannot see the challenge
							StartCoroutine (HandleChallenge(hchallenge));
						}
						else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (hunterChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt(distance) + hunterChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (HandleChallenge(hchallenge));
					}
				}
				// in here, I allow voting challenges, and solved voting challenges (for players to see what was published there)
				else if((string.Equals (hit.collider.tag, "VotingChallenge")) || (string.Equals (hit.collider.tag, "VotingChallengeSolved"))) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					VotingChallengeInfo vchallenge = foundObject.GetComponent<VotingChallengeInfo>();
					Debug.Log ("You Found the " + vchallenge.name + " Voting challenge (ID: " + 
						vchallenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (vchallenge != null)) {

							// request if all the members of the team are around, otherwise he cannot see the challenge
							StartCoroutine (HandleChallenge(vchallenge));
						}
						else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (votingChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt(distance) + votingChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (HandleChallenge(vchallenge));
					}

				}
				// in here, I allow TimedTask challenges, and solved TimedTask challenges (for players to see what was published there)
				else if((string.Equals (hit.collider.tag, "TimedTaskChallenge")) ) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					TimedTaskChallengeInfo ttchallenge = foundObject.GetComponent<TimedTaskChallengeInfo>();
					Debug.Log ("You Found the " + ttchallenge.name + " TimedTask challenge (ID: " + 
						ttchallenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (ttchallenge != null)) {

							// request if all the members of the team are around, otherwise he cannot see the challenge
							StartCoroutine (HandleChallenge (ttchallenge));
						} else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (timedtaskChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt(distance) + timedtaskChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (HandleChallenge (ttchallenge));
					}
				}
				// in here, I allow openQuiz challenges
				else if((string.Equals (hit.collider.tag, "OpenQuizChallenge")) ) {
					foundObject = hit.transform.gameObject;
					// Only allow for interaction when the player is close enough. This is the distance between two points in space XYZ
					deltaX = foundObject.transform.position.x - GameManager.instance._referencePlayer.transform.position.x;
					deltaY = foundObject.transform.position.y - GameManager.instance._referencePlayer.transform.position.y;
					deltaZ = foundObject.transform.position.z - GameManager.instance._referencePlayer.transform.position.z;

					distance = (float)Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
					OpenQuizChallengeInfo OQchallenge = foundObject.GetComponent<OpenQuizChallengeInfo>();
					Debug.Log ("You Found the " + OQchallenge.name + " OpenQuiz challenge (ID: " + 
						OQchallenge._id+ ") at  " + distance + " meters.");

					//cagaNaDistanciaDosIcones3DAteOsAtivares
					if (!MasterManager.instance.developmentMode) {
						// see whether the found challenge is close enough to the player
						if ((distance < 70.0f) && (OQchallenge != null)) {

							// request if all the members of the team are around, otherwise he cannot see the challenge
							StartCoroutine (HandleChallenge (OQchallenge));
						} else {
							GameManager.instance._referenceDisplayManager.DisplaySystemMessage (openQuizChallengeFoundText1[MasterManager.language] + Mathf.FloorToInt(distance) + openQuizChallengeFoundText2[MasterManager.language]);
						}
					} else {
						StartCoroutine (HandleChallenge (OQchallenge));
					}
				}
			}
		}


	}

	private IEnumerator EnterChallengeAsTeam (MultiplayerChallengeInfo mchallenge) {
		// /api/entermultiplayerChallenge?TeamID=201805222013493&ChallengeID=cLthJn43m3r2jGiP7

		// http://localhost:3000/api/numberofplayersinteam?TeamID=201805232007230
		if (MasterManager.instance.playerHasTeam) {
		
			Debug.Log("MasterManager.teamID Exists" );
			Debug.Log("MasterManager.teamID = " + MasterManager.instance.teamID + " -> with character count: " + MasterManager.instance.teamID.Length );
		} else {

			Debug.Log("MasterManager.teamID DOES NOT EXIST" );
		}


		if (MasterManager.instance.playerHasTeam) {
			string url;
			UnityWebRequest request;
			//teamplayers = new List<string> ();

			// **************************************************
			// first, find out whether anybody joined this team
			// **************************************************
			url = MasterManager.serverURL + "/api/numberofplayersinteam?TeamID=" + MasterManager.instance.teamID;
			request = UnityWebRequest.Get (url);
			request.timeout = 10;
			yield return request.SendWebRequest ();
			if (request.isNetworkError) {
				Debug.LogError ("[GameManager] Error While Sending: " + request.error);
				Debug.LogError ("[GameManager] URL: " + url);
			} else {
				Debug.Log ("[GameManager] Request with: " + url);
				Debug.Log ("[GameManager] Received: " + request.downloadHandler.text);

				//teamplayers = 
				//	JsonWrapper.DeserializeObject<List<string>> (request.downloadHandler.text);
				//Debug.LogError("[GameManager] This team has: " + numberofteamplayers.Count + " elements. These are: ");
				//for (int i = 0; i < numberofteamplayers.Count; i++) {
				//	Debug.LogError ("Player " + i + ": " + numberofteamplayers[i]);
				//}
				/*if (teamplayers.Count < 2) {
					GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("You can only solve this challenge as a team, and your team has " + teamplayers.Count + " player(s).");
					yield break;
				}*/
			}


			// **************************************************
			// then, find out whether everybody is within 30m of the challenge
			// **************************************************

			url = MasterManager.serverURL + "/api/entermultiplayerChallenge?TeamID=" + MasterManager.instance.teamID + "&ChallengeID=" + mchallenge._id;
			// pedir os desafios ao servidor. Este é o URL
			bool replyFromServer = false;

			request = UnityWebRequest.Get (url);
			request.timeout = 10;
			yield return request.SendWebRequest ();
			if (request.isNetworkError) {
				Debug.LogError ("[GameManager] Error While Sending: " + request.error);
				Debug.LogError ("[GameManager] URL: " + url);
			} else {
				Debug.Log ("[GameManager] Request with: " + url);
				Debug.Log ("[GameManager] Received: " + request.downloadHandler.text);


				replyFromServer = 
					JsonWrapper.DeserializeObject<bool> (request.downloadHandler.text);
				Debug.Log ("[GameManager] Answer from server: " + replyFromServer);
			}


			// if so
			if (replyFromServer) {
				yield return HandleChallenge (mchallenge);
			} else {
				/*
				 * 
				 * // before the game had this validation! then, i prefered to make the game easier to play
				Debug.LogError ("[HandleTouchedObject] Reply from server was negative. Not all the elements of the team are nearby the challenge");
				GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("You can only solve this challenge as a team. Not all the elements of your team are near the challenge.");
				yield break;
				*/
				yield return HandleChallenge (mchallenge);
			}
		} else {
			string[] displayMessage = {"You can only solve this challenge as a team, and you do not have a team.", "Je kunt deze uitdaging alleen als een team oplossen en je hebt geen team.", "Este desafio só pode ser resolvido em equipa, e tu não tens uma equipa criada."};
			GameManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage[MasterManager.language]);
			yield break;
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

		throw new UnityException("[HandleTouch] unrecognizable type of challenge details to attach to the 3D object.");
	}

	private IEnumerator HandleChallenge(ChallengeInfo challenge)
	{
		activeChallenge = challenge;
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Quiz Challenge \"" + challenge.challenge_name + "\"" +
			" ["+challenge._id+"]");

		// http://colourlex.com/wp-content/uploads/2017/04/Spinel-black-painted-swatch-47400.jpg
		//Sprite img = Resources.Load<Sprite> ("ChallengesImages/2 - challenge_wolf");
		//Sprite img = imageToSpriteConverter.LoadNewSprite ("ChallengesImages/2 - challenge_wolf", 100.0f, SpriteMeshType.Tight);
		if (challenge.typeOfChallengeIndex == 0) { // quiz
			//activeQuizChallenge = getChallengeDetails(challenge._id, challenge.typeOfChallengeIndex) as Challenge_QuizData;
			//activeChallenge = getChallengeDetails(challenge._id, challenge.typeOfChallengeIndex) as ChallengeInfo;


			myAction1 = new UnityAction (QuizShowQuestionButtonMethod);
			myAction2 = new UnityAction (QuizAnswerButtonMethod);
			myAction3 = new UnityAction (GenericButton3Method);


			quiz_modalPanel = GameManager.instance._referenceQuiz_ModalPanel;
			// snap description text to top
			quiz_modalPanel.scrollViewPanel.verticalNormalizedPosition = 1.0f;

			if (string.Compare (challenge.imageURL, string.Empty) != 0) {
				Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
				Texture2D texture;

				using (WWW www = new WWW(challenge.imageURL))
				{
					float timer = 0;
					float timeOut = 10;
					bool failed = false;
					while (!www.isDone)
					{
						if (timer > timeOut) { failed = true; break; }
						timer += Time.deltaTime;
						yield return null;
					}
					if (failed || !string.IsNullOrEmpty (www.error)) {
						www.Dispose ();
						yield break;
					} else {
						texture = www.texture;
					}
					// Wait for download to complete
					//yield return www;

					// assign texture

				}


				//Texture2D SpriteTexture = Resources.Load<Texture2D> (activeQuizChallenge.imageURL); 
				Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

				yield return quiz_modalPanel.Choice (img, challenge.challenge_name, challenge.description, challenge.answer, myAction1, myAction2, myAction3);
			}
		}

	}

	private IEnumerator HandleChallenge(MultiplayerChallengeInfo challenge)
	{
		//Debug.Log ("[HandleTouchedObject] Handle Multiplayer Challenge: " + challenge.challenge_name);

		activeMultiplayerChallenge = challenge;
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Multiplayer Challenge \"" + 
			activeMultiplayerChallenge.challenge_name + "\" ["+activeMultiplayerChallenge._id+"]");
		// http://colourlex.com/wp-content/uploads/2017/04/Spinel-black-painted-swatch-47400.jpg
		//Sprite img = Resources.Load<Sprite> ("ChallengesImages/2 - challenge_wolf");
		//Sprite img = imageToSpriteConverter.LoadNewSprite ("ChallengesImages/2 - challenge_wolf", 100.0f, SpriteMeshType.Tight);
		if (challenge.typeOfChallengeIndex == 1) { // Multiplayer
			//activeQuizChallenge = getChallengeDetails(challenge._id, challenge.typeOfChallengeIndex) as Challenge_QuizData;
			//activeChallenge = getChallengeDetails(challenge._id, challenge.typeOfChallengeIndex) as ChallengeInfo;


			myAction1 = new UnityAction (QuizShowQuestionButtonMethodMultiplayerChallenge);
			//myAction2 = new UnityAction (QuizAnswerButtonMethodMultiplayerChallenge);
			myAction3 = new UnityAction (GenericButton3MethodMultiplayer);


			multiplayer_modalPanel = GameManager.instance._referenceMultiplayer_ModalPanel;
			// snap description text to top
			multiplayer_modalPanel.scrollViewPanel.verticalNormalizedPosition = 1.0f;

			if (string.Compare (challenge.imageURL, string.Empty) != 0) {
				Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
				Texture2D texture;

				using (WWW www = new WWW(challenge.imageURL))
				{
					float timer = 0;
					float timeOut = 10;
					bool failed = false;

					while (!www.isDone)
					{
						if (timer > timeOut) { failed = true; break; }
						timer += Time.deltaTime;
						yield return null;
					}
					if (failed || !string.IsNullOrEmpty (www.error)) {
						www.Dispose ();
						yield break;
					} else {
						texture = www.texture;
					}
					// Wait for download to complete
					//yield return www;

					// assign texture

				}


				//Texture2D SpriteTexture = Resources.Load<Texture2D> (activeQuizChallenge.imageURL); 
				Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

				yield return multiplayer_modalPanel.Choice (img, challenge.challenge_name, challenge.description, myAction1, myAction3);
			}
		}


	}

	/*private IEnumerator HandleChallenge(HunterChallengeInfo challenge)
	{

		//Debug.Log ("Got here :D");
		activeHunterChallenge = challenge;


		myAction1 = new UnityAction (HunterShowQuestionButtonMethod);
		myAction2 = new UnityAction (HunterAnswerButtonMethod);
		myAction3 = new UnityAction (GenericButton3Method);


		hunter_modalPanel = GameManager.instance._referenceHunter_ModalPanel;
		// snap description text to top
		hunter_modalPanel.scrollViewPanel.verticalNormalizedPosition = 1.0f;

		if (string.Compare (challenge.imageURL, string.Empty) != 0) {
			Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
			Texture2D texture;

			using (WWW www = new WWW(challenge.imageURL))
			{
				// Wait for download to complete
				yield return www;

				// assign texture
				texture = www.texture;
			}


			//Texture2D SpriteTexture = Resources.Load<Texture2D> (activeQuizChallenge.imageURL); 
			Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

			// yield return hunter_modalPanel.Choice (img, challenge.name, challenge.description, challenge.answer, myAction1, myAction2, myAction3);

			//hunterChallengeWindow
			GameManager.instance.hunterChallengeWindow.SetActive(true);
		}

	}*/
	private IEnumerator HandleChallenge(HunterChallengeInfo challenge)
	{

		//Debug.Log ("Got here :D");
		activeHunterChallenge = challenge;
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Hunter Challenge \"" + 
			challenge.name + "\" ["+challenge._id+"]");


		myAction1 = new UnityAction (HunterShowQuestionButtonMethod);
		myAction2 = new UnityAction (HunterAnswerButtonMethod);
		myAction3 = new UnityAction (GenericButton3MethodHunter);


		//hunter_modalPanel = GameManager.instance._referenceHunter_ModalPanel;
		// snap description text to top
		//hunter_modalPanel.scrollViewPanel.verticalNormalizedPosition = 1.0f;

		if (string.Compare (challenge.imageURL, string.Empty) != 0) {
			Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
			Texture2D texture;

			using (WWW www = new WWW(challenge.imageURL))
			{
				float timer = 0;
				float timeOut = 10;
				bool failed = false;
				while (!www.isDone)
				{
					if (timer > timeOut) { failed = true; break; }
					timer += Time.deltaTime;
					yield return null;
				}
				if (failed || !string.IsNullOrEmpty (www.error)) {
					www.Dispose ();
					yield break;
				} else {
					texture = www.texture;
				}
				// Wait for download to complete
				//yield return www;

				// assign texture

			}


			//Texture2D SpriteTexture = Resources.Load<Texture2D> (activeQuizChallenge.imageURL); 
			Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

			// yield return hunter_modalPanel.Choice (img, challenge.name, challenge.description, challenge.answer, myAction1, myAction2, myAction3);

			//hunterChallengeWindow

			GameManager.instance.hunterChallengeWindowManager.ActivateWindow ();

			GameManager.instance.hunterChallengeWindowManager.titleHolder.text = challenge.name;
			GameManager.instance.hunterChallengeWindowManager.descriptionHolder.text = challenge.description;
			GameManager.instance.hunterChallengeWindowManager.questionHolder.text = challenge.question;
			GameManager.instance.hunterChallengeWindowManager.imageHolder.sprite = img;
			GameManager.instance.hunterChallengeWindowManager.hunterChallengeBeingSolved = activeHunterChallenge;
			GameManager.instance.hunterChallengeWindowManager.descriptionScrollViewPanel.verticalNormalizedPosition = 1.0f;
		}
	}
	private IEnumerator HandleChallenge(VotingChallengeInfo challenge)
	{

		//Debug.Log ("Got here :D");
		activeVotingChallenge = challenge;
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Voting Challenge \"" + 
		//	challenge.name + "\" ["+challenge._id+"]");


		//myAction1 = new UnityAction (VotingShowTaskButtonMethod);
		//myAction2 = new UnityAction (VotingTakePictureButtonMethod);
		//myAction3 = new UnityAction (GenericButton3Method);


		//hunter_modalPanel = GameManager.instance._referenceHunter_ModalPanel;
		// snap description text to top
		//hunter_modalPanel.scrollViewPanel.verticalNormalizedPosition = 1.0f;

		if (string.Compare (challenge.imageURL, string.Empty) != 0) {
			Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
			Texture2D texture;

			using (WWW www = new WWW(challenge.imageURL))
			{
				float timer = 0;
				float timeOut = 10;
				bool failed = false;
				while (!www.isDone)
				{
					if (timer > timeOut) { failed = true; break; }
					timer += Time.deltaTime;
					yield return null;
				}
				if (failed || !string.IsNullOrEmpty (www.error)) {
					www.Dispose ();
					yield break;
				} else {
					texture = www.texture;
				}
				// Wait for download to complete
				//yield return www;


			}


			Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);
			GameManager.instance.manageVotingChallenge.currentVotingChallengeBeingSolved = activeVotingChallenge;

			GameManager.instance.manageVotingChallenge.ActivateWindow ();


			GameManager.instance.manageVotingChallenge.titleHolder.text = challenge.name;
			GameManager.instance.manageVotingChallenge.descriptionHolder.text = challenge.description;
			GameManager.instance.manageVotingChallenge.taskHolder.text = challenge.task;
			GameManager.instance.manageVotingChallenge.imageHolder.sprite = img;

			GameManager.instance.manageVotingChallenge.descriptionScrollViewPanel.verticalNormalizedPosition = 1.0f;
		}
	}

	private IEnumerator HandleChallenge(TimedTaskChallengeInfo challenge)
	{
		activeTimedTaskChallenge = challenge;
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Timed Task Challenge \"" + 
		//	challenge.name + "\" ["+challenge._id+"]");


		if (string.Compare (challenge.imageURL, string.Empty) != 0) {
			Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
			Texture2D texture;

			using (WWW www = new WWW(challenge.imageURL))
			{
				float timer = 0;
				float timeOut = 10;
				bool failed = false;

				while (!www.isDone)
				{
					if (timer > timeOut) { failed = true; break; }
					timer += Time.deltaTime;
					yield return null;
				}
				if (failed || !string.IsNullOrEmpty (www.error)) {
					www.Dispose ();
					yield break;
				} else {
					texture = www.texture;
				}
				// Wait for download to complete
				//yield return www;


			}


			Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);
			GameManager.instance.manageTimedTaskChallenge.currentTimedTaskChallengeBeingSolved = activeTimedTaskChallenge;

			GameManager.instance.manageTimedTaskChallenge.ActivateWindow ();


			GameManager.instance.manageTimedTaskChallenge.titleHolder.text = challenge.name;
			GameManager.instance.manageTimedTaskChallenge.descriptionHolder.text = challenge.description;
			GameManager.instance.manageTimedTaskChallenge.taskHolder.text = challenge.task;
			GameManager.instance.manageTimedTaskChallenge.imageHolder.sprite = img;
			GameManager.instance.manageTimedTaskChallenge.titleQuestionHowManyHolder.text = challenge.questionHowMany;
			// titleQuestionHowManyHolder

			GameManager.instance.manageTimedTaskChallenge.descriptionScrollViewPanel.verticalNormalizedPosition = 1.0f;
		}
	}



	private IEnumerator HandleChallenge(OpenQuizChallengeInfo challenge)
	{
		activeOpenQuizChallenge = challenge;
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Open Quiz Challenge \"" + 
		//	challenge.name + "\" ["+challenge._id+"]");

		if (string.Compare (challenge.imageURL, string.Empty) != 0) {
			Debug.Log ("[HandleTouchedObject] URL: " + challenge.imageURL);
			Texture2D texture;

			using (WWW www = new WWW(challenge.imageURL))
			{
				float timer = 0;
				float timeOut = 10;
				bool failed = false;

				while (!www.isDone)
				{
					if (timer > timeOut) { failed = true; break; }
					timer += Time.deltaTime;
					yield return null;
				}
				if (failed || !string.IsNullOrEmpty (www.error)) {
					www.Dispose ();
					yield break;
				} else {
					texture = www.texture;
				}
				// Wait for download to complete
				//yield return www;


			}


			Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);
			GameManager.instance.manageOpenQuizChallenge.currentOpenQuizChallengeBeingSolved = activeOpenQuizChallenge;

			GameManager.instance.manageOpenQuizChallenge.ActivateWindow ();


			GameManager.instance.manageOpenQuizChallenge.titleHolder.text = challenge.name;
			GameManager.instance.manageOpenQuizChallenge.descriptionHolder.text = challenge.description;
			GameManager.instance.manageOpenQuizChallenge.questionHolder.text = challenge.question;
			GameManager.instance.manageOpenQuizChallenge.imageHolder.sprite = img;
			//GameManager.instance.manageOpenQuizChallenge.titleQuestionHowManyHolder.text = challenge.questionHowMany;

			GameManager.instance.manageOpenQuizChallenge.descriptionScrollViewPanel.verticalNormalizedPosition = 1.0f;
		}
	}


	// in truth, I need arguments in these functions, to have differential behaviour in the buttons!
	// send to the modal panel to set up the panels and methods to call
	// these are wrapped into unity actions
	// Yes button
	void GenericButton1Method()
	{
		string[] displayMessage = {"Good luck with it!", "Veel geluk ermee!", "Boa sorte!"};
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage[MasterManager.language]);
		if (foundObject.CompareTag ("Challenge")) {
			foundObject.transform.gameObject.GetComponent<Renderer> ().material = colourNotSolved;
		}
		else if (foundObject.CompareTag ("ClosedChessChallenge")) {
			foundObject.transform.GetChild (1).gameObject.SetActive (true);
		} 

	}
	// Answer Challenge Button
	void GenericButton2Method()
	{
		string[] displayMessage = {"Wow, you're so awesome! Good Job :)", "Je bent geweldig! Goed gedaan :)", "Uau, és tão espetacular! Bom trabalho :)"};
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage[MasterManager.language]);


		foundObject.transform.GetChild (1).gameObject.SetActive (false);

		ChallengeInfo challenge = foundObject.GetComponent<ChallengeInfo> ();
		// only if it was not previously solved, that I mark it as solved and attribute points
		if (!challenge.solved)
		{
			challenge.solved = true;


			// you also need to update the index of the challenges in the main scene, otherwise when you load a different scene, the change is lost
			// reload all challenges on map, rotating and all. 
			// this update consists of removing the solved challenge
			for (int j = 0; j < GameManager.instance.challengesOnScreenMeteor.Count; j++) {
				// only generate items that have not been solved
				if (GameManager.instance.challengesOnScreenMeteor [j]._id == challenge._id) {
					//GameManager.instance.challengesOnScreen [j].solved = true;
					GameManager.instance.challengesOnScreenMeteor.RemoveAt(j);
					// when you find the challenge, no need to search for others
					break;
				}
			}
				

			// attribute the points of solving the challenge
			//StartCoroutine(	PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge.challenge_ID.ToString()) );
			StartCoroutine(	PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge._id) );
		}


	}
	// in truth, I need arguments in these functions, to have differential behaviour in the buttons!
	// send to the modal panel to set up the panels and methods to call
	// these are wrapped into unity actions
	// Yes button
	void QuizShowQuestionButtonMethod()
	{
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Quiz Challenge \"" + 
		//	activeChallenge.name + "\" ["+activeChallenge._id+"]");
		
		Debug.Log ("[HandleTouchedObject] QuizShowQuestionButtonMethod fired!");
		quiz_modalPanel.questionContainer.text = activeChallenge.question;

	}
	void QuizShowQuestionButtonMethodMultiplayerChallenge()
	{
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Multiplayer Challenge \"" + 
		//	activeMultiplayerChallenge.challenge_name + "\" ["+activeMultiplayerChallenge._id+"]");
		
		Debug.Log ("[HandleTouchedObject] MultiplayerShowTaskButtonMethod fired!");
		multiplayer_modalPanel.questionContainer.text = activeMultiplayerChallenge.task;

	}
	// Answer Challenge Button
	void QuizAnswerButtonMethod()
	{
		ChallengeInfo challenge = foundObject.GetComponent<ChallengeInfo> ();
		string[] displayMessage1 = {"You already solved this challenge. Well done :)", "Je hebt deze uitdaging al opgelost. Goed gedaan :)", "Tu já resolveste este desafio. Boa :)"};

		if (challenge.solved) {
			GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage1[MasterManager.language]);
		}
		else {
			Debug.Log ("[Quiz_modalWindow] PopUpAnswerPanel fired up!");	
			string answer;
			if (string.Compare (quiz_modalPanel.answerBox.text, string.Empty) == 0) {
				answer = "-10000000000";
			} else {
				answer = quiz_modalPanel.answerBox.text;//Convert.ToInt64 (quiz_modalPanel.answerBox.text);
			}

			GameObject genericChallenge;
			if (string.Compare (answer, quiz_modalPanel.answerOfTheChallenge) == 0) {
				MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Quiz Challenge \"" + 
					challenge.challenge_name + "\" ["+challenge._id+"]");

				string[] displayMessage2 = {"Good Job, answering the question! :D You just got +10 Points.", "Goed gedaan, de vraag is beantwoord! : D Je hebt zojuist +10 punten gekregen.", "Bom trabalho a responder à pergunta! :D Acabaste de receber +10 Pontos."};
				GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage2[MasterManager.language]);
				genericChallenge = Instantiate(GameManager.instance.challengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				challenge.solved = true;
				genericChallenge.transform.position = foundObject.transform.position;
				genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
					genericChallenge.transform.position.y + 5, genericChallenge.transform.position.z);
				for (int i = 0; i < MasterManager.challengesFromServerMeteor.Count; i++) {
					if (string.Compare (MasterManager.challengesFromServerMeteor [i]._id, challenge._id) == 0) {
						MasterManager.challengesFromServerMeteor [i].solved = true;
						break;
					}
				}
				StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge._id));
			} else {
				MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Quiz Challenge \"" + 
					challenge.challenge_name + "\" ["+challenge._id+"] with half the points");
				
				string[] displayMessage3 = {"The answer of this challenge is not 100% correct! You are getting half the points.", "Het antwoord op deze uitdaging is niet 100% correct! Je krijgt de helft van de punten.", "A resposta a este desafio não está 100% correta! Recebeste apenas metade dos pontos por isso."};
				GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage3[MasterManager.language]);
				genericChallenge = Instantiate(GameManager.instance.challengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				challenge.solved = false;
				genericChallenge.transform.position = foundObject.transform.position;
				genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
					genericChallenge.transform.position.y, genericChallenge.transform.position.z);
				StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeHalfRewardsChallengeSolved (challenge._id));
			}
			genericChallenge.AddComponent<ChallengeInfo> ();

			GameManager.instance.AttachChallengeToGameObjectMeteor (genericChallenge, challenge);

			GameManager.instance.objectsToDestroyOnLoadScene.Add (genericChallenge);
			Destroy (foundObject);


		}

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Quiz Challenge \"" + challenge.challenge_name + "\"" +
			" ["+challenge._id+"]");

		quiz_modalPanel.popupAnswerPanel.SetActive (false);
		quiz_modalPanel.gameObject.SetActive (false);
		GameManager.instance.isAnyWindowOpen = false;
	}


	// deprecated
	void HunterShowQuestionButtonMethod()
	{
		Debug.Log ("[HandleTouchedObject] HunterShowQuestionButtonMethod fired!");
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Hunter Challenge \"" + 
		//	activeHunterChallenge.name + "\" ["+activeHunterChallenge._id+"]");

		hunter_modalPanel.questionContainer.text = activeChallenge.question;

	}
	// Answer Challenge Button
	public void HunterAnswerButtonMethod()
	{
		// just guaranteeing that, because it was someone calling this, that we do not loose the information
		// of the object found in the game
		activeHunterChallenge = GameManager.instance.hunterChallengeWindowManager.hunterChallengeBeingSolved;

		string answer = GameManager.instance.hunterChallengeWindowManager.answerGivenHolder.text;

		// InputField answerBox;
		if (string.Compare(answer, activeHunterChallenge.answer) == 0){
			// you solved the challenge!!!! 
			//hunter_modalPanel.popupAnswerPanel.SetActive (false);
			//hunter_modalPanel.gameObject.SetActive (false);

			GameManager.instance.hunterChallengeWindowManager.CloseWindow ();
			MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Hunter Challenge \"" + 
				activeHunterChallenge.name + "\" ["+activeHunterChallenge._id+"]");

			string[] displayMessage1 = {"Good Job, solving the hunter challenge! You just got +10 Points :D", "Goed gedaan, de hunter uitdaging is opgelost! Je hebt net +10 punten gekregen : D", "Bom trabalho a resolver o desafio de caçador! Acabaste de receber +10 Pontos :D"};
			GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage1[MasterManager.language]);


			activeHunterChallenge.solved = true;
			for (int i = 0; i < GameManager.instance.hunterChallengesOnScreenMeteor.Count; i++) {
				if (string.Compare (GameManager.instance.hunterChallengesOnScreenMeteor [i]._id, activeHunterChallenge._id) == 0) {
					GameManager.instance.hunterChallengesOnScreenMeteor [i].solved = true;
					break;
				}
			}

	
			// ****************************************************************
			// attempt to remove the extra unsolved challenge icon on the map
			// ****************************************************************
			int index = -1; bool found = false;
			for (int i = GameManager.instance.objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				if (GameManager.instance.objectsToDestroyOnLoadScene [i].CompareTag ("HunterChallenge")) {
					if (string.Compare (GameManager.instance.objectsToDestroyOnLoadScene [i].GetComponent<HunterChallengeInfo> ()._id, activeHunterChallenge._id) == 0) {
						index = i;
						found = true;
						break;
					}
				}
			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected Multiplayer challenge was not found on the screen.");
			} else if (found) {
				Destroy (GameManager.instance.objectsToDestroyOnLoadScene[index]);
				GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (index);
			}

			// generate the new icon as solved on the map
			GameManager.instance.GenerateHunterChallengeSolvedOnScreenMeteor (activeHunterChallenge);


			// attribute the points of solving the challenge
			StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (activeHunterChallenge._id));





		} else {

			MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Hunter Challenge \"" + 
				activeHunterChallenge.name + "\" ["+activeHunterChallenge._id+"] with half the points");
			
			GameManager.instance.hunterChallengeWindowManager.CloseWindow ();
			string[] displayMessage2 = {"The answer of this challenge is not 100% correct! You are only getting 5 points.", "Het antwoord op deze uitdaging is niet 100% correct! Je krijgt maar 5 punten.", "A resposta a este desafio não está 100% correta! Recebeste apenas 5 pontos por isso."};
			GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage2[MasterManager.language]);
			StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeHalfRewardsChallengeSolved (activeHunterChallenge._id));
		}

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Hunter Challenge \"" + 
			activeHunterChallenge.name + "\" ["+activeHunterChallenge._id+"]");

	}
	// Close Button
	public void GenericButton3Method()
	{
		//MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Quiz Challenge \"" + 
		//activeChallenge.name + "\" ["+activeChallenge._id+"]");
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Quiz Challenge \"" + activeChallenge.challenge_name + "\"" +
			" ["+activeChallenge._id+"]");

		string[] displayMessage = {"Bye ...", "Doei ...", "Até à próxima ..."};
		//GameManager.instance.hunterChallengeWindowManager.CloseWindow ();
		GameManager.instance.isAnyWindowOpen = false;
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage[MasterManager.language]);

	}
	// Close Button
	public void GenericButton3MethodHunter()
	{
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Hunter Challenge \"" + 
			activeHunterChallenge.name + "\" ["+activeHunterChallenge._id+"]");

		string[] displayMessage = {"Bye ...", "Doei ...", "Até à próxima ..."};
		GameManager.instance.hunterChallengeWindowManager.CloseWindow ();
		GameManager.instance.isAnyWindowOpen = false;
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage[MasterManager.language]);

	}

	public void GenericButton3MethodMultiplayer()
	{
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Multiplayer Challenge \"" + 
			activeMultiplayerChallenge.challenge_name + "\" ["+activeMultiplayerChallenge._id+"]");

		string[] displayMessage = {"Bye ...", "Doei ...", "Até à próxima ..."};
		//GameManager.instance.hunterChallengeWindowManager.CloseWindow ();

		GameManager.instance.isAnyWindowOpen = false;
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage[MasterManager.language]);

	}
}
