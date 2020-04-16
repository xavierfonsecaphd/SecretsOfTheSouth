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
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity;
using System;

// to store data about the challenges. later on, it is going to be my server doing this
using PlayFab;
using PlayFab.ClientModels;

//using FullSerializer;
using PlayFab.Json;
using UnityEngine.Networking;

public class ChallengesCanvasMenuManager : MonoBehaviour {

	public GameObject feedbackMessagePanel;
	public Text titleText;	
	public GameObject challengesCanvas;	// this is the Canvas of the Teams menu 
	public GameObject gamePlayCanvas;	// this is the Canvas of the Teams menu 
	public DisplayManager_Menu_Scene_6 _referenceDisplayManager;

	public static ChallengesCanvasMenuManager instance { set; get ; }
	private static ChallengesCanvasMenuManager singleton;

	public List<GameObject> panels;
	// 1 - All Challenges
	// 1 - Quiz Challenges
	// 2 - Multiplayer Challenges
	// 3 - Hunter Challenges
	// 4 - Voting Challenges
	// 5 - TimedTask Challenges
	// 6 - Open Quiz Challenges
	public List<GameObject> buttons;		// images of buttons to play with colours
	// 0 - All Challenges Button
	// 1 - Quiz Challenges Button
	// 2 - Multiplayer Challenges Button
	// 3 - Hunter Challenges Button
	// 4 - Voting Challenges Button
	// 5 - TimedTask Challenges Button
	// 6 - OpenQuiz Challenges Button
	public List<GameObject> showMultiplayerChallengesOrNot;


	// All the references for the icons of the challenges being generated in runtime
	public GameObject containerForChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject challengeIcon;	// this is the whole icon being generated
	// All the references for the icons of the challenges being generated in runtime
	public GameObject containerForMultiplayerChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject containerForHunterChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject containerForAllChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject multiplayerChallengeIcon;	// this is the whole icon being generated
	public GameObject hunterChallengeIcon;	// this is the whole icon being generated
	//public ChallengeData temporaryChallengeToFollow;
	public List <GameObject> allChallengeIcons;
	public List <GameObject> challengeIcons;
	public ChallengeIconModalPanel challengeIconModalPanel;

	public List <GameObject> multiplayerChallengeIcons;
	public List <GameObject> hunterChallengeIcons;

	// this is to load the challenges from server
	//public List<ChallengeInfo> challengesFromServerMeteor;
	//public List <ChallengeInfo> challengesOnScreenMeteor = new List<ChallengeInfo> (0);
	//public List < Sprite> challengeImages = new List<Sprite>(0);
	//public bool challengeImageSpritesLoaded = false;
	public  bool challengesLoadedFromServer = false;
	private bool initiated = false;
	public ChallengeIconGUIScript IconGUIScript;
	public MultiplayerChallengeIconGUIScript MultiplayerIconGUIScript;
	public HunterChallengeIconGUIScript HunterIconGUIScript;


	public GameObject containerForVotingChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject votingChallengeIcon;	// this is the whole icon being generated
	public List <GameObject> votingChallengeIcons;
	public VotingChallengeIconGUIScript VotingIconGUIScript;

	public GameObject containerForTimedTaskChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject timedTaskChallengeIcon;	// this is the whole icon being generated
	public List <GameObject> timedTaskChallengeIcons;
	//public TimedTaskChallengeIconGUIScript TimedTaskIconGUIScript;

	/**
	 * Open Quiz related variables
	 * 
	 */
	public GameObject containerForOpenQuizChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject openQuizChallengeIcon;	// this is the whole icon being generated
	public List <GameObject> openQuizChallengeIcons;

	// Use this for initialization
	void Awake () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = ChallengesCanvasMenuManager.singleton;

			if (gamePlayCanvas == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference gamePlayCanvas is null!");
			if (challengesCanvas == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference challengesCanvas is null!");
			//if (teamName == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference teamName is null!");
			if (titleText == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference titleText is null!");
			//if (teamImage == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference teamImage is null!");
			//if (yourTeamQRIDImage == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference yourTeamQRIDImage is null!");
			if (feedbackMessagePanel == null) throw new System.Exception ("[ChallengesCanvasMenuManager] reference feedbackMessagePanel is null!");

			if (showMultiplayerChallengesOrNot == null)
				throw new System.Exception ("[ChallengesCanvasMenuManager] reference showMultiplayerChallengesOrNot is null!");
			// deactivate this canvas at the beginning of the game
			challengesCanvas.gameObject.SetActive (false);
			feedbackMessagePanel.SetActive (false);
			feedbackMessagePanel.transform.SetAsLastSibling ();


			StartCoroutine (StartTheDisplayManagerProperly());


		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	
	// Update is called once per frame
	void Update () {
		if (initiated) {
			// This recalculates the distance of the icons to the player at GameManager 
			// (because the GPS is still changing while in this menu, and then re-display the distance per icon)
			// Quiz Challenges
			for (int i = 0; i < GameManager.instance.challengesOnScreenMeteor.Count; i++) {
				GameManager.instance.challengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.challengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.challengesOnScreenMeteor [i].latitude, 
							GameManager.instance.challengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);

				if (challengeIcons [i]) {
					challengeIcons [i].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
						GameManager.instance.challengesOnScreenMeteor [i].DistanceToString ();
				}

			}

			// Multiplayer Challenges
			for (int i = 0; i < GameManager.instance.multiplayerChallengesOnScreenMeteor.Count; i++) {
				GameManager.instance.multiplayerChallengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.multiplayerChallengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.multiplayerChallengesOnScreenMeteor [i].latitude, 
							GameManager.instance.multiplayerChallengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);

				if (multiplayerChallengeIcons.Count > i) {
					if (multiplayerChallengeIcons [i]) {
						multiplayerChallengeIcons [i].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
						GameManager.instance.multiplayerChallengesOnScreenMeteor [i].DistanceToString ();
					}
				} else {
					Debug.Log ("[ChallengesCanvasMEnuManager] The multiplayerChallengeIcons is not the same size as multiplayerChallengeIcons. Investigate");
				}

			}

			// Hunter Challenges
			for (int i = 0; i < GameManager.instance.hunterChallengesOnScreenMeteor.Count; i++) {
				GameManager.instance.hunterChallengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.hunterChallengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.hunterChallengesOnScreenMeteor [i].latitude, 
							GameManager.instance.hunterChallengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);


				// this is to update the distance of the icons that are left (those that are still to be solved)
				HunterChallengeInfo tmp;
				for (int j=0; j < hunterChallengeIcons.Count; j++){
					tmp = hunterChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().hunterChallenge;
					if (tmp != null) {
						if (string.Compare (tmp._id, GameManager.instance.hunterChallengesOnScreenMeteor [i]._id) == 0) {
							hunterChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
								GameManager.instance.hunterChallengesOnScreenMeteor [i].DistanceToString ();
						}
					}
				}

			}

			// Voting Challenges
			for (int i = 0; i < GameManager.instance.votingChallengesOnScreenMeteor.Count; i++) {
				GameManager.instance.votingChallengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.votingChallengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.votingChallengesOnScreenMeteor [i].latitude, 
							GameManager.instance.votingChallengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);


				// this is to update the distance of the icons that are left (those that are still to be solved)
				VotingChallengeInfo tmp;
				for (int j=0; j < votingChallengeIcons.Count; j++){
					tmp = votingChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().votingChallenge;
					if (string.Compare (tmp._id, GameManager.instance.votingChallengesOnScreenMeteor [i]._id) == 0) {
						votingChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
							GameManager.instance.votingChallengesOnScreenMeteor [i].DistanceToString ();
					}

				}

			}


			// TimedTask Challenges
			for (int i = 0; i < GameManager.instance.timedTaskChallengesOnScreenMeteor.Count; i++) {
				GameManager.instance.timedTaskChallengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.timedTaskChallengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.timedTaskChallengesOnScreenMeteor [i].latitude, 
							GameManager.instance.timedTaskChallengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);


				// this is to update the distance of the icons that are left (those that are still to be solved)
				TimedTaskChallengeInfo tmp;
				for (int j=0; j < timedTaskChallengeIcons.Count; j++){
					tmp = timedTaskChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().timedTaskChallenge;
					if (tmp != null) {
						if (string.Compare (tmp._id, GameManager.instance.timedTaskChallengesOnScreenMeteor [i]._id) == 0) {
							timedTaskChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
								GameManager.instance.timedTaskChallengesOnScreenMeteor [i].DistanceToString ();
						}
					}
				}

			}



			// OpenQuiz Challenges
			for (int i = 0; i < GameManager.instance.openQuizChallengesOnScreenMeteor.Count; i++) {
				GameManager.instance.openQuizChallengesOnScreenMeteor [i].distanceToPlayer = 
					GameManager.instance.openQuizChallengesOnScreenMeteor [i].DistanceToPlayer (
						new Vector2d (
							GameManager.instance.openQuizChallengesOnScreenMeteor [i].latitude, 
							GameManager.instance.openQuizChallengesOnScreenMeteor [i].longitude),	
						GPSLocationProvider_Xavier.instance.latlong);


				// this is to update the distance of the icons that are left (those that are still to be solved)
				OpenQuizChallengeInfo tmp;
				for (int j=0; j < openQuizChallengeIcons.Count; j++){
					tmp = openQuizChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().openQuizChallenge;
					if (string.Compare (tmp._id, GameManager.instance.openQuizChallengesOnScreenMeteor [i]._id) == 0) {
						openQuizChallengeIcons [j].GetComponent<GenericChallengeIconGUIScript> ().challengeDistance.text = 
							GameManager.instance.openQuizChallengesOnScreenMeteor [i].DistanceToString ();
					}

				}

			}
		}
	}
	public void UpdateSelectedOption (int index)
	{

		StartCoroutine (UpdateSelectedOptionThread(index));
	}
	private  IEnumerator UpdateSelectedOptionThread (int index)
	{
		// set every panel to false, so that we only show the one we want
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		/*for (int i = 0; i < buttons.Count; i++) {
			buttons [i].color = Color.white;
		}*/
		//buttons [1].color = new Color32 (0x4f, 0xff, 0x50, 0xff); // green colour for the Multiplayer Challenges


		switch (index) {
		case 0:	// Your QR Code Wallet

			// txt22
			string [] displayMessage1 = {"List of All Challenges",
				"Lijst met alle uitdagingen",
				"Lista com todos os desafios"};
			// 
			titleText.text = displayMessage1[MasterManager.language];


			//buttons [0].color = Color.yellow;

			panels [0].SetActive (true);


			break;
		case 1:	// Your QR Code Wallet


			// txt22
			string [] displayMessage2 = {"Quiz Challenges",
				"Quiz Challenges",
				"Desafios Quiz"};
			titleText.text = displayMessage2[MasterManager.language];


			//buttons [0].color = Color.yellow;

			panels [1].SetActive (true);


			break;
		case 2:	// QR Code Reader
			string [] displayMessage3 = {"Multiplayer Challenges",
				"Multiplayer Challenges",
				"Desafios Multiplayer"};
			titleText.text = displayMessage3[MasterManager.language];

			//buttons [1].color = Color.yellow;
			//buttons [1].color = new Color32 (0x4f, 0xff, 0x50, 0x00); // green colour for the Multiplayer Challenges

			panels [2].SetActive (true);

			if (MasterManager.instance.playerHasTeam) {
				showMultiplayerChallengesOrNot [0].gameObject.SetActive (true);
				showMultiplayerChallengesOrNot [1].gameObject.SetActive (false);
			}
			else {
				showMultiplayerChallengesOrNot [0].gameObject.SetActive (false);
				showMultiplayerChallengesOrNot [1].gameObject.SetActive (true);
			}

			break;

		case 3:	
			string [] displayMessage4 = {"Hunter Challenges",
				"Jager Challenges",
				"Desafios Caçador"};
			titleText.text = displayMessage4[MasterManager.language];

			panels [3].SetActive (true);

			break;
		case 4:	
			string [] displayMessage5 = {"Voting Challenges",
				"Stemming Challenges",
				"Desafios com Voto"};
			titleText.text = displayMessage5[MasterManager.language];

			panels [4].SetActive (true);

			break;
		case 5:
			string [] displayMessage6 = {"Timed Task Challenges",
				"Tijdelijke taak Challenges",
				"Desafios com Tempo"};
			titleText.text = displayMessage6[MasterManager.language];

			panels [5].SetActive (true);
			break;
		case 6:
			string [] displayMessage7 = {"Open Quiz Challenges",
				"Open Quiz Challenges",
				"Desafios Quiz Abertos"};
			titleText.text = displayMessage7[MasterManager.language];

			panels [6].SetActive (true);
			break;
		}

		yield break;
	}


		

	public void OpenCanvas()
	{
		MenuSlide.instance.ResetAnimation ();

		MasterManager.instance._referenceMap.SetActive (false);
		feedbackMessagePanel.SetActive (false);
		challengesCanvas.gameObject.SetActive (true);
		gamePlayCanvas.gameObject.SetActive (false);



		StartCoroutine (LoadChallengesMenuAsync());

		UpdateSelectedOption (0);

	}
	public void CloseCanvas()
	{
		initiated = false;

		MasterManager.instance._referenceMap.SetActive (true);
		GameManager.instance.LoadObjectsFromGameSceneMeteor ();

		/*
		for (int i = challengeIcons.Count - 1; i >= 0; i--) {
			//Destroy (challengeIcons [i]);
			challengeIcons.RemoveAt (i);
		}
		//Destroy (challengeIcons);
		for (int i = multiplayerChallengeIcons.Count - 1; i >= 0; i--) {
			//Destroy (multiplayerChallengeIcons [i]);
			multiplayerChallengeIcons.RemoveAt (i);
		}
		///Destroy (multiplayerChallengeIcons);
		for (int i = hunterChallengeIcons.Count - 1; i >= 0; i--) {
			//Destroy (hunterChallengeIcons [i]);
			hunterChallengeIcons.RemoveAt (i);
		}
		//Destroy (hunterChallengeIcons);
		*/

		// clean all challenge objects in 3D world
		/*for (int i = 0; i <GameManager.instance.objectsToDestroyOnLoadScene.Count; i++) {
			Destroy (GameManager.instance.objectsToDestroyOnLoadScene [i]);
		}
		// clean this list
		GameManager.instance.objectsToDestroyOnLoadScene = new List<GameObject>();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("ClosedChessChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("MultiplayerChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("HunterChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("HunterChallengeSolved");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}*/

		GameObject[] objs = GameObject.FindGameObjectsWithTag("ItemToEliminate");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		// 
		/*
		// and now regenerate the ones that should be there.
		for (int j = GameManager.instance.challengesOnScreenMeteor.Count - 1; j >= 0; j--) {
			// only generate items that have not been solved
			if (!GameManager.instance.challengesOnScreenMeteor [j].solved) {
				//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
				GameManager.instance.GenerateChallengeOnScreenMeteor (GameManager.instance.challengesOnScreenMeteor [j]);
			}
		}

		for (int j = GameManager.instance.multiplayerChallengesOnScreenMeteor.Count - 1; j >= 0; j--) {
			// only generate items that have not been solved
			if (!GameManager.instance.multiplayerChallengesOnScreenMeteor [j].solved) {
				//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
				GameManager.instance.GenerateMultiplayerChallengeOnScreenMeteor (GameManager.instance.multiplayerChallengesOnScreenMeteor [j]);
			}
		}

		for (int j = GameManager.instance.hunterChallengesOnScreenMeteor.Count - 1; j >= 0; j--) {
			// only generate items that have not been solved
			if (!GameManager.instance.hunterChallengesOnScreenMeteor [j].solved) {
				GameManager.instance.GenerateHunterChallengeOnScreenMeteor (GameManager.instance.hunterChallengesOnScreenMeteor [j]);
			} else {
				GameManager.instance.GenerateHunterChallengeSolvedOnScreenMeteor (GameManager.instance.hunterChallengesOnScreenMeteor [j]);
			}
		}

		*/


		challengesCanvas.gameObject.SetActive (false);
		gamePlayCanvas.gameObject.SetActive (true);

		for (int i = 0; i < challengeIcons.Count; i++) {
			Destroy (challengeIcons[i]);
		}
		challengeIcons = new List<GameObject> ();

		for (int i = 0; i < multiplayerChallengeIcons.Count; i++) {
			Destroy (multiplayerChallengeIcons[i]);
		}
		multiplayerChallengeIcons = new List<GameObject> ();

		for (int i = 0; i < hunterChallengeIcons.Count; i++) {
			Destroy (hunterChallengeIcons[i]);
		}
		hunterChallengeIcons = new List<GameObject> ();

		for (int i = 0; i < votingChallengeIcons.Count; i++) {
			Destroy (votingChallengeIcons[i]);
		}
		votingChallengeIcons = new List<GameObject> ();

		for (int i = 0; i < timedTaskChallengeIcons.Count; i++) {
			Destroy (timedTaskChallengeIcons[i]);
		}
		timedTaskChallengeIcons = new List<GameObject> ();

		for (int i = 0; i < allChallengeIcons.Count; i++) {
			Destroy (allChallengeIcons[i]);
		}


		for (int i = 0; i < openQuizChallengeIcons.Count; i++) {
			Destroy (openQuizChallengeIcons[i]);
		}
		openQuizChallengeIcons = new List<GameObject> ();


		allChallengeIcons = new List<GameObject> ();

		GameManager.instance.RespawnChallengeObjects ();
	}

	// this is to load the settings menu
	public IEnumerator LoadChallengesMenuAsync()
	{
		feedbackMessagePanel.SetActive (true);
		ChallengesCanvasMenuManager.instance._referenceDisplayManager.DisplaySystemMessageNonFading ("Loading Challenges around you...");

		challengeIcons = new List<GameObject> ();
		multiplayerChallengeIcons = new List<GameObject> ();
		hunterChallengeIcons = new List<GameObject> ();
		votingChallengeIcons = new List<GameObject> ();
		timedTaskChallengeIcons = new List<GameObject> ();
		openQuizChallengeIcons = new List<GameObject> ();

		ChallengesCanvasMenuManager.instance._referenceDisplayManager.DisplaySystemMessageNonFading ("Loading Quiz Challenges around you...");

		challengeIconModalPanel = ChallengeIconModalPanel.Instance();
		// ***********************************************************
		// This is to load the Quiz Challenges into clickable icons
		// ***********************************************************
		if (GameManager.instance.challengesOnScreenMeteor.Count > 0) 
		{
			
			ChallengeInfo temporaryHolderQuiz = new ChallengeInfo();
			Sprite temporaryHolderSpriteQuiz = new Sprite ();
			for (int j = 0; j < GameManager.instance.challengesOnScreenMeteor.Count - 1; j++) 
			{
				
				for (int k = j + 1; k < GameManager.instance.challengesOnScreenMeteor.Count; k++) 
				{
					if (GameManager.instance.challengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.challengesOnScreenMeteor[k].distanceToPlayer) {

						// sort multiplayer challenges according to distance
						temporaryHolderQuiz = GameManager.instance.challengesOnScreenMeteor [j];
						GameManager.instance.challengesOnScreenMeteor [j] = GameManager.instance.challengesOnScreenMeteor [k];
						GameManager.instance.challengesOnScreenMeteor [k] = temporaryHolderQuiz;

						// sorting their images in the GameManager according to their location to the user
						temporaryHolderSpriteQuiz = GameManager.challengeImages[j];
						GameManager.challengeImages [j] = GameManager.challengeImages [k];
						GameManager.challengeImages [k] = temporaryHolderSpriteQuiz;
					}
				}
			}



			// containerForAllChallengeIcons
			GameObject genericChallengeIcon;
			GameObject genericChallengeIconForAllChallengesView;
			for (int i = 0; i < GameManager.instance.challengesOnScreenMeteor.Count; i++) 
			{
				// generate for the normal quiz challenge icons
				genericChallengeIcon = Instantiate(challengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
				genericChallengeIcon.SetActive (true);
				genericChallengeIcon.transform.SetParent (containerForChallengeIcons.transform);
				genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
				genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
				//ChallengeIconGUIScript scriptOfTheObject = genericChallengeIcon.GetComponent<ChallengeIconGUIScript> ();
				GenericChallengeIconGUIScript scriptOfTheObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();

				// do the same for the all the challenges view
				genericChallengeIconForAllChallengesView = Instantiate(challengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
				genericChallengeIconForAllChallengesView.SetActive (true);
				genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
				genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
				genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
				GenericChallengeIconGUIScript scriptOfTheObjectForAllChallengesView = 
					genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();


				// use one script for both objects
				scriptOfTheObject.challengeTitle.text = GameManager.instance.challengesOnScreenMeteor [i].challenge_name;
				scriptOfTheObject.challengeDistance.text = "" + GameManager.instance.challengesOnScreenMeteor[i].DistanceToString();
				scriptOfTheObject.challengeImage.sprite = GameManager.challengeImages[i];
				scriptOfTheObject.typeOfChallenge = 1;	// Quiz
				scriptOfTheObject.challenge = GameManager.instance.challengesOnScreenMeteor [i];
				scriptOfTheObject.multiplayerChallenge = null;
				scriptOfTheObject.openQuizChallenge = null;
				scriptOfTheObject.hunterChallenge = null;
				scriptOfTheObject.votingChallenge = null;
				scriptOfTheObject.timedTaskChallenge = null;

				scriptOfTheObjectForAllChallengesView.challengeTitle.text = GameManager.instance.challengesOnScreenMeteor [i].challenge_name;
				scriptOfTheObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.challengesOnScreenMeteor[i].DistanceToString();
				scriptOfTheObjectForAllChallengesView.challengeImage.sprite = GameManager.challengeImages[i];
				scriptOfTheObjectForAllChallengesView.typeOfChallenge = 1;	// Quiz
				scriptOfTheObjectForAllChallengesView.challenge = GameManager.instance.challengesOnScreenMeteor [i];
				scriptOfTheObjectForAllChallengesView.multiplayerChallenge = null;
				scriptOfTheObjectForAllChallengesView.openQuizChallenge = null;
				scriptOfTheObjectForAllChallengesView.hunterChallenge = null;
				scriptOfTheObjectForAllChallengesView.votingChallenge = null;
				scriptOfTheObjectForAllChallengesView.timedTaskChallenge = null;


				genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.challengesOnScreenMeteor [i]);
				genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.challengesOnScreenMeteor [i]);

				challengeIcons.Add (genericChallengeIcon);
				allChallengeIcons.Add (genericChallengeIconForAllChallengesView);


			}

			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
			feedbackMessagePanel.SetActive (false);

		}







		if (MasterManager.instance.playerHasTeam) {
			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (true);
			feedbackMessagePanel.SetActive (true);
			ChallengesCanvasMenuManager.instance._referenceDisplayManager.DisplaySystemMessageNonFading ("Loading Multiplayer Challenges around you...");

			// ***********************************************************
			// This is to load the Multiplayer Challenges into clickable icons
			// ***********************************************************

			if (GameManager.instance.multiplayerChallengesOnScreenMeteor.Count > 0) 
			{
				//Debug.Log ("Yes, Generating icons for multiplayer challenges.");
				// items = items.OrderBy(w => w.startPos).ToList();

				MultiplayerChallengeInfo temporaryHolder = new MultiplayerChallengeInfo();
				Sprite temporaryHolderSprite = new Sprite ();
				for (int j = 0; j < GameManager.instance.multiplayerChallengesOnScreenMeteor.Count - 1; j++) 
				{
					/*GameManager.instance.multiplayerChallengesOnScreenMeteor.Sort(delegate(MultiplayerChallengeInfo x, MultiplayerChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});*/

					for (int k = j + 1; k < GameManager.instance.multiplayerChallengesOnScreenMeteor.Count; k++) 
					{
						if (GameManager.instance.multiplayerChallengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.multiplayerChallengesOnScreenMeteor[k].distanceToPlayer) {

							// sort multiplayer challenges according to distance
							temporaryHolder = GameManager.instance.multiplayerChallengesOnScreenMeteor [j];
							GameManager.instance.multiplayerChallengesOnScreenMeteor [j] = GameManager.instance.multiplayerChallengesOnScreenMeteor [k];
							GameManager.instance.multiplayerChallengesOnScreenMeteor [k] = temporaryHolder;

							// sorting their images in the GameManager according to their location to the user
							temporaryHolderSprite = GameManager.multiplayerChallengeImages[j];
							GameManager.multiplayerChallengeImages [j] = GameManager.multiplayerChallengeImages [k];
							GameManager.multiplayerChallengeImages [k] = temporaryHolderSprite;
						}
					}


					//GameManager.multiplayerChallengeImages[i];
				}

				GameObject genericChallengeIcon;
				GameObject genericChallengeIconForAllChallengesView;
				for (int i = 0; i < GameManager.instance.multiplayerChallengesOnScreenMeteor.Count; i++) 
				{

					genericChallengeIcon = Instantiate(multiplayerChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForMultiplayerChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					//MultiplayerChallengeIconGUIScript scriptOfTheMultiplayerObject = genericChallengeIcon.GetComponent<MultiplayerChallengeIconGUIScript> ();
					//Texture2D SpriteTexture;
					GenericChallengeIconGUIScript scriptOfTheMultiplayerObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();


					genericChallengeIconForAllChallengesView = Instantiate(multiplayerChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIconForAllChallengesView.SetActive (true);
					genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
					genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

					GenericChallengeIconGUIScript scriptOfTheMultiplayerObjectForAllChallengesView = 
						genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();

					// use one script for both objects
					scriptOfTheMultiplayerObject.challengeTitle.text = GameManager.instance.multiplayerChallengesOnScreenMeteor [i].challenge_name;
					scriptOfTheMultiplayerObject.challengeDistance.text = "" + GameManager.instance.multiplayerChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheMultiplayerObject.challengeImage.sprite = GameManager.multiplayerChallengeImages[i];
					scriptOfTheMultiplayerObject.typeOfChallenge = 2;	// Multiplayer
					scriptOfTheMultiplayerObject.multiplayerChallenge = GameManager.instance.multiplayerChallengesOnScreenMeteor [i];
					scriptOfTheMultiplayerObject.challenge = null;
					scriptOfTheMultiplayerObject.hunterChallenge = null;
					scriptOfTheMultiplayerObject.openQuizChallenge = null;
					scriptOfTheMultiplayerObject.votingChallenge = null;
					scriptOfTheMultiplayerObject.timedTaskChallenge = null;

					scriptOfTheMultiplayerObjectForAllChallengesView.challengeTitle.text = GameManager.instance.multiplayerChallengesOnScreenMeteor [i].challenge_name;
					scriptOfTheMultiplayerObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.multiplayerChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheMultiplayerObjectForAllChallengesView.challengeImage.sprite = GameManager.multiplayerChallengeImages[i];
					scriptOfTheMultiplayerObjectForAllChallengesView.typeOfChallenge = 2;	// Multiplayer
					scriptOfTheMultiplayerObjectForAllChallengesView.multiplayerChallenge = GameManager.instance.multiplayerChallengesOnScreenMeteor [i];
					scriptOfTheMultiplayerObjectForAllChallengesView.challenge = null;
					scriptOfTheMultiplayerObjectForAllChallengesView.hunterChallenge = null;
					scriptOfTheMultiplayerObjectForAllChallengesView.openQuizChallenge = null;
					scriptOfTheMultiplayerObjectForAllChallengesView.votingChallenge = null;
					scriptOfTheMultiplayerObjectForAllChallengesView.timedTaskChallenge = null;


					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.multiplayerChallengesOnScreenMeteor [i]);
					genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.multiplayerChallengesOnScreenMeteor [i]);

					multiplayerChallengeIcons.Add (genericChallengeIcon);
					allChallengeIcons.Add (genericChallengeIconForAllChallengesView);
				}

				ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
				feedbackMessagePanel.SetActive (false);
			}
		}




		// hunter challenges
		if (GameManager.instance.hunterChallengesOnScreenMeteor.Count > 0) 
		{
			//Debug.LogError ("Generrating icons for hunter challenges.");
			// items = items.OrderBy(w => w.startPos).ToList();

			HunterChallengeInfo temporaryHolder = new HunterChallengeInfo();
			Sprite temporaryHolderSprite = new Sprite ();
			for (int j = 0; j < GameManager.instance.hunterChallengesOnScreenMeteor.Count - 1; j++) 
			{
				/*GameManager.instance.multiplayerChallengesOnScreenMeteor.Sort(delegate(MultiplayerChallengeInfo x, MultiplayerChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});*/

				for (int k = j + 1; k < GameManager.instance.hunterChallengesOnScreenMeteor.Count; k++) 
				{
					if (GameManager.instance.hunterChallengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.hunterChallengesOnScreenMeteor[k].distanceToPlayer) {

						// sort multiplayer challenges according to distance
						temporaryHolder = GameManager.instance.hunterChallengesOnScreenMeteor [j];
						GameManager.instance.hunterChallengesOnScreenMeteor [j] = GameManager.instance.hunterChallengesOnScreenMeteor [k];
						GameManager.instance.hunterChallengesOnScreenMeteor [k] = temporaryHolder;

						// sorting their images in the GameManager according to their location to the user
						temporaryHolderSprite = GameManager.hunterChallengeImages[j];
						GameManager.hunterChallengeImages [j] = GameManager.hunterChallengeImages [k];
						GameManager.hunterChallengeImages [k] = temporaryHolderSprite;
					}
				}



			}

			GameObject genericChallengeIcon;
			GameObject genericChallengeIconForAllChallengesView;
			for (int i = 0; i < GameManager.instance.hunterChallengesOnScreenMeteor.Count; i++) 
			{
				if (!GameManager.instance.hunterChallengesOnScreenMeteor[i].solved) 
				{
					genericChallengeIcon = Instantiate(hunterChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForHunterChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheHunterObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();


					genericChallengeIconForAllChallengesView = Instantiate(hunterChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIconForAllChallengesView.SetActive (true);
					genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
					genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheHunterObjectForAllChallengesView = 
						genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();


					// use one script for both objects
					scriptOfTheHunterObject.challengeTitle.text = GameManager.instance.hunterChallengesOnScreenMeteor [i].name;
					scriptOfTheHunterObject.challengeDistance.text = "" + GameManager.instance.hunterChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheHunterObject.challengeImage.sprite = GameManager.hunterChallengeImages[i];
					scriptOfTheHunterObject.typeOfChallenge = 3;	// Hunter
					scriptOfTheHunterObject.hunterChallenge = GameManager.instance.hunterChallengesOnScreenMeteor [i];
					scriptOfTheHunterObject.multiplayerChallenge = null;
					scriptOfTheHunterObject.openQuizChallenge = null;
					scriptOfTheHunterObject.challenge = null;
					scriptOfTheHunterObject.votingChallenge = null;
					scriptOfTheHunterObject.timedTaskChallenge = null;

					scriptOfTheHunterObjectForAllChallengesView.challengeTitle.text = GameManager.instance.hunterChallengesOnScreenMeteor [i].name;
					scriptOfTheHunterObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.hunterChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheHunterObjectForAllChallengesView.challengeImage.sprite = GameManager.hunterChallengeImages[i];
					scriptOfTheHunterObjectForAllChallengesView.typeOfChallenge = 3;	// Hunter
					scriptOfTheHunterObjectForAllChallengesView.hunterChallenge = GameManager.instance.hunterChallengesOnScreenMeteor [i];
					scriptOfTheHunterObjectForAllChallengesView.multiplayerChallenge = null;
					scriptOfTheHunterObjectForAllChallengesView.challenge = null;
					scriptOfTheHunterObjectForAllChallengesView.openQuizChallenge = null;
					scriptOfTheHunterObjectForAllChallengesView.votingChallenge = null;
					scriptOfTheHunterObjectForAllChallengesView.timedTaskChallenge = null;


					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.hunterChallengesOnScreenMeteor [i]);
					genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.hunterChallengesOnScreenMeteor [i]);

					hunterChallengeIcons.Add (genericChallengeIcon);
					allChallengeIcons.Add (genericChallengeIconForAllChallengesView);	
				}


			}

			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
			feedbackMessagePanel.SetActive (false);
		}



		// Voting challenges
		if (GameManager.instance.votingChallengesOnScreenMeteor.Count > 0) 
		{
			//Debug.LogError ("Generrating icons for voting challenges.");
			// items = items.OrderBy(w => w.startPos).ToList();

			VotingChallengeInfo temporaryHolder = new VotingChallengeInfo();
			Sprite temporaryHolderSprite = new Sprite ();
			for (int j = 0; j < GameManager.instance.votingChallengesOnScreenMeteor.Count - 1; j++) 
			{
				/*GameManager.instance.multiplayerChallengesOnScreenMeteor.Sort(delegate(MultiplayerChallengeInfo x, MultiplayerChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});*/

				for (int k = j + 1; k < GameManager.instance.votingChallengesOnScreenMeteor.Count; k++) 
				{
					if (GameManager.instance.votingChallengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.votingChallengesOnScreenMeteor[k].distanceToPlayer) {

						// sort multiplayer challenges according to distance
						temporaryHolder = GameManager.instance.votingChallengesOnScreenMeteor [j];
						GameManager.instance.votingChallengesOnScreenMeteor [j] = GameManager.instance.votingChallengesOnScreenMeteor [k];
						GameManager.instance.votingChallengesOnScreenMeteor [k] = temporaryHolder;

						// sorting their images in the GameManager according to their location to the user
						temporaryHolderSprite = GameManager.votingChallengeImages[j];
						GameManager.votingChallengeImages [j] = GameManager.votingChallengeImages [k];
						GameManager.votingChallengeImages [k] = temporaryHolderSprite;
					}
				}



			}

			GameObject genericChallengeIcon;
			GameObject genericChallengeIconForAllChallengesView;
			for (int i = 0; i < GameManager.instance.votingChallengesOnScreenMeteor.Count; i++) 
			{
				if (!GameManager.instance.votingChallengesOnScreenMeteor[i].solved) 
				{
					genericChallengeIcon = Instantiate(votingChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForVotingChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheVotingObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();


					genericChallengeIconForAllChallengesView = Instantiate(votingChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIconForAllChallengesView.SetActive (true);
					genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
					genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheVotingObjectForAllChallengesView = 
						genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();


					// use one script for both objects
					scriptOfTheVotingObject.challengeTitle.text = GameManager.instance.votingChallengesOnScreenMeteor [i].name;
					scriptOfTheVotingObject.challengeDistance.text = "" + GameManager.instance.votingChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheVotingObject.challengeImage.sprite = GameManager.votingChallengeImages[i];
					scriptOfTheVotingObject.typeOfChallenge = 4;	// Voting
					scriptOfTheVotingObject.votingChallenge = GameManager.instance.votingChallengesOnScreenMeteor [i];
					scriptOfTheVotingObject.multiplayerChallenge = null;
					scriptOfTheVotingObject.challenge = null;
					scriptOfTheVotingObject.openQuizChallenge = null;
					scriptOfTheVotingObject.hunterChallenge = null;
					scriptOfTheVotingObject.timedTaskChallenge = null;

					scriptOfTheVotingObjectForAllChallengesView.challengeTitle.text = GameManager.instance.votingChallengesOnScreenMeteor [i].name;
					scriptOfTheVotingObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.votingChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheVotingObjectForAllChallengesView.challengeImage.sprite = GameManager.votingChallengeImages[i];
					scriptOfTheVotingObjectForAllChallengesView.typeOfChallenge = 4;	// Voting
					scriptOfTheVotingObjectForAllChallengesView.votingChallenge = GameManager.instance.votingChallengesOnScreenMeteor [i];
					scriptOfTheVotingObjectForAllChallengesView.multiplayerChallenge = null;
					scriptOfTheVotingObjectForAllChallengesView.challenge = null;
					scriptOfTheVotingObjectForAllChallengesView.openQuizChallenge = null;
					scriptOfTheVotingObjectForAllChallengesView.hunterChallenge = null;
					scriptOfTheVotingObjectForAllChallengesView.timedTaskChallenge = null;


					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.votingChallengesOnScreenMeteor [i]);
					genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.votingChallengesOnScreenMeteor [i]);

					votingChallengeIcons.Add (genericChallengeIcon);
					allChallengeIcons.Add (genericChallengeIconForAllChallengesView);	
				}


			}

			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
			feedbackMessagePanel.SetActive (false);
		}



		// TimedTask challenges
		if (GameManager.instance.timedTaskChallengesOnScreenMeteor.Count > 0) 
		{
			//Debug.LogError ("Generrating icons for voting challenges.");
			// items = items.OrderBy(w => w.startPos).ToList();

			TimedTaskChallengeInfo temporaryHolder = new TimedTaskChallengeInfo();
			Sprite temporaryHolderSprite = new Sprite ();
			for (int j = 0; j < GameManager.instance.timedTaskChallengesOnScreenMeteor.Count - 1; j++) 
			{
				/*GameManager.instance.multiplayerChallengesOnScreenMeteor.Sort(delegate(MultiplayerChallengeInfo x, MultiplayerChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});*/

				for (int k = j + 1; k < GameManager.instance.timedTaskChallengesOnScreenMeteor.Count; k++) 
				{
					if (GameManager.instance.timedTaskChallengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.timedTaskChallengesOnScreenMeteor[k].distanceToPlayer) {

						// sort multiplayer challenges according to distance
						temporaryHolder = GameManager.instance.timedTaskChallengesOnScreenMeteor [j];
						GameManager.instance.timedTaskChallengesOnScreenMeteor [j] = GameManager.instance.timedTaskChallengesOnScreenMeteor [k];
						GameManager.instance.timedTaskChallengesOnScreenMeteor [k] = temporaryHolder;

						// sorting their images in the GameManager according to their location to the user
						temporaryHolderSprite = GameManager.timedTaskChallengeImages[j];
						GameManager.timedTaskChallengeImages [j] = GameManager.timedTaskChallengeImages [k];
						GameManager.timedTaskChallengeImages [k] = temporaryHolderSprite;
					}
				}



			}

			GameObject genericChallengeIcon;
			GameObject genericChallengeIconForAllChallengesView;
			for (int i = 0; i < GameManager.instance.timedTaskChallengesOnScreenMeteor.Count; i++) 
			{
				if (!GameManager.instance.timedTaskChallengesOnScreenMeteor[i].solved) 
				{
					genericChallengeIcon = Instantiate(timedTaskChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForTimedTaskChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheTimedTaskObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();


					genericChallengeIconForAllChallengesView = Instantiate(timedTaskChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIconForAllChallengesView.SetActive (true);
					genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
					genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheTimedTaskObjectForAllChallengesView = 
						genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();


					// use one script for both objects
					scriptOfTheTimedTaskObject.challengeTitle.text = GameManager.instance.timedTaskChallengesOnScreenMeteor [i].name;
					scriptOfTheTimedTaskObject.challengeDistance.text = "" + GameManager.instance.timedTaskChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheTimedTaskObject.challengeImage.sprite = GameManager.timedTaskChallengeImages[i];
					scriptOfTheTimedTaskObject.typeOfChallenge = 5;	// TimedTask
					scriptOfTheTimedTaskObject.timedTaskChallenge = GameManager.instance.timedTaskChallengesOnScreenMeteor [i];
					scriptOfTheTimedTaskObject.multiplayerChallenge = null;
					scriptOfTheTimedTaskObject.openQuizChallenge = null;
					scriptOfTheTimedTaskObject.challenge = null;
					scriptOfTheTimedTaskObject.hunterChallenge = null;
					scriptOfTheTimedTaskObject.votingChallenge = null;

					scriptOfTheTimedTaskObjectForAllChallengesView.challengeTitle.text = GameManager.instance.timedTaskChallengesOnScreenMeteor [i].name;
					scriptOfTheTimedTaskObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.timedTaskChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheTimedTaskObjectForAllChallengesView.challengeImage.sprite = GameManager.timedTaskChallengeImages[i];
					scriptOfTheTimedTaskObjectForAllChallengesView.typeOfChallenge = 5;	// TimedTask
					scriptOfTheTimedTaskObjectForAllChallengesView.timedTaskChallenge = GameManager.instance.timedTaskChallengesOnScreenMeteor [i];
					scriptOfTheTimedTaskObjectForAllChallengesView.multiplayerChallenge = null;
					scriptOfTheTimedTaskObjectForAllChallengesView.openQuizChallenge = null;
					scriptOfTheTimedTaskObjectForAllChallengesView.challenge = null;
					scriptOfTheTimedTaskObjectForAllChallengesView.hunterChallenge = null;
					scriptOfTheTimedTaskObjectForAllChallengesView.votingChallenge = null;


					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.timedTaskChallengesOnScreenMeteor [i]);
					genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.timedTaskChallengesOnScreenMeteor [i]);

					timedTaskChallengeIcons.Add (genericChallengeIcon);
					allChallengeIcons.Add (genericChallengeIconForAllChallengesView);	
				}


			}

			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
			feedbackMessagePanel.SetActive (false);
		}


		// OpenQuiz challenges
		if (GameManager.instance.openQuizChallengesOnScreenMeteor.Count > 0) 
		{
			//Debug.LogError ("Generrating icons for voting challenges.");
			// items = items.OrderBy(w => w.startPos).ToList();

			OpenQuizChallengeInfo temporaryHolder = new OpenQuizChallengeInfo();
			Sprite temporaryHolderSprite = new Sprite ();
			for (int j = 0; j < GameManager.instance.openQuizChallengesOnScreenMeteor.Count - 1; j++) 
			{
				/*GameManager.instance.multiplayerChallengesOnScreenMeteor.Sort(delegate(MultiplayerChallengeInfo x, MultiplayerChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});*/

				for (int k = j + 1; k < GameManager.instance.openQuizChallengesOnScreenMeteor.Count; k++) 
				{
					if (GameManager.instance.openQuizChallengesOnScreenMeteor[j].distanceToPlayer > GameManager.instance.openQuizChallengesOnScreenMeteor[k].distanceToPlayer) {

						// sort multiplayer challenges according to distance
						temporaryHolder = GameManager.instance.openQuizChallengesOnScreenMeteor [j];
						GameManager.instance.openQuizChallengesOnScreenMeteor [j] = GameManager.instance.openQuizChallengesOnScreenMeteor [k];
						GameManager.instance.openQuizChallengesOnScreenMeteor [k] = temporaryHolder;

						// sorting their images in the GameManager according to their location to the user
						temporaryHolderSprite = GameManager.openQuizChallengeImages[j];
						GameManager.openQuizChallengeImages [j] = GameManager.openQuizChallengeImages [k];
						GameManager.openQuizChallengeImages [k] = temporaryHolderSprite;
					}
				}



			}

			GameObject genericChallengeIcon;
			GameObject genericChallengeIconForAllChallengesView;
			for (int i = 0; i < GameManager.instance.openQuizChallengesOnScreenMeteor.Count; i++) 
			{
				if (!GameManager.instance.openQuizChallengesOnScreenMeteor[i].solved) 
				{
					genericChallengeIcon = Instantiate(openQuizChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForOpenQuizChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheOpenQuizObject = genericChallengeIcon.GetComponent<GenericChallengeIconGUIScript> ();


					genericChallengeIconForAllChallengesView = Instantiate(openQuizChallengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIconForAllChallengesView.SetActive (true);
					genericChallengeIconForAllChallengesView.transform.SetParent (containerForAllChallengeIcons.transform);
					genericChallengeIconForAllChallengesView.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIconForAllChallengesView.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					GenericChallengeIconGUIScript scriptOfTheOpenQuizObjectForAllChallengesView = 
						genericChallengeIconForAllChallengesView.GetComponent<GenericChallengeIconGUIScript> ();


					// use one script for both objects
					scriptOfTheOpenQuizObject.challengeTitle.text = GameManager.instance.openQuizChallengesOnScreenMeteor [i].name;
					scriptOfTheOpenQuizObject.challengeDistance.text = "" + GameManager.instance.openQuizChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheOpenQuizObject.challengeImage.sprite = GameManager.openQuizChallengeImages[i];
					scriptOfTheOpenQuizObject.typeOfChallenge = 6;	// OpenQuiz
					scriptOfTheOpenQuizObject.openQuizChallenge = GameManager.instance.openQuizChallengesOnScreenMeteor [i];
					scriptOfTheOpenQuizObject.timedTaskChallenge = null;
					scriptOfTheOpenQuizObject.multiplayerChallenge = null;
					scriptOfTheOpenQuizObject.challenge = null;
					scriptOfTheOpenQuizObject.hunterChallenge = null;
					scriptOfTheOpenQuizObject.votingChallenge = null;

					scriptOfTheOpenQuizObjectForAllChallengesView.challengeTitle.text = GameManager.instance.openQuizChallengesOnScreenMeteor [i].name;
					scriptOfTheOpenQuizObjectForAllChallengesView.challengeDistance.text = "" + GameManager.instance.openQuizChallengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheOpenQuizObjectForAllChallengesView.challengeImage.sprite = GameManager.openQuizChallengeImages[i];
					scriptOfTheOpenQuizObjectForAllChallengesView.typeOfChallenge = 6;	// TimedTask
					scriptOfTheOpenQuizObjectForAllChallengesView.openQuizChallenge = GameManager.instance.openQuizChallengesOnScreenMeteor [i];
					scriptOfTheOpenQuizObjectForAllChallengesView.timedTaskChallenge = null;
					scriptOfTheOpenQuizObjectForAllChallengesView.multiplayerChallenge = null;
					scriptOfTheOpenQuizObjectForAllChallengesView.challenge = null;
					scriptOfTheOpenQuizObjectForAllChallengesView.hunterChallenge = null;
					scriptOfTheOpenQuizObjectForAllChallengesView.votingChallenge = null;


					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.openQuizChallengesOnScreenMeteor [i]);
					genericChallengeIconForAllChallengesView.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.openQuizChallengesOnScreenMeteor [i]);

					timedTaskChallengeIcons.Add (genericChallengeIcon);
					allChallengeIcons.Add (genericChallengeIconForAllChallengesView);	
				}


			}

			ChallengesCanvasMenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
			feedbackMessagePanel.SetActive (false);
		}



		initiated = true;


		yield return null; 

	}
	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("[TeamMenuManager] _reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		while (!_referenceDisplayManager.isProperlyInitialized)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[TeamMenuManager] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}


}
