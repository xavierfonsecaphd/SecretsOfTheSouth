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



// https://github.com/Elringus/UnityGoogleDrive

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using GameToolkit.Localization;

using PlayFab;
using PlayFab.ClientModels;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using PlayFab.Json;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MenuManager : MonoBehaviour {
	public Dropdown routeFilterSelected;

	public static MenuManager instance { set; get ; }
	// This is a singleton
	private static MenuManager singleton;

	// All the references for the icons of the challenges being generated in runtime
	public GameObject containerForLeaderboardPlayersIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject leaderboardPlayersIcon;	// this is the whole icon being generated
	public GameObject feedbackMessagePanel;
	public DisplayManager_Menu_Scene_4 _referenceDisplayManager;

	public Dropdown languageDropDown;

	public Text selectedOptionText;
	public List<GameObject> panels;
	// 0 - player panel
	// 1 - Leaderboard panel
	// 2 - settings panel
	// 3 - Evaluator panel
	// 4 - Eval Small panel 1
	// 5 - Eval Small panel 2
	// 6 - Teams Ranking Panel
	// 7 - Observer panel
	// 8 - Administrator tools panel

	public List<Image> buttons;		// images of buttons to play with colours
	// 0 - player button
	// 1 - leaderboard button
	// 2 - settings button
	// 3 - First to Solve Badge
	// 4 - Challenges Solved
	// 5 - People Met
	// 6 - Player Avatar Image
	// 7 - Evaluator button
	// 8 - Teams Rankings button
	// 9 - Observer Button
	// 10- Administrator Button

	public List<Text> playerStatTexts;	// 0 - gold; 1 - challenges created; 2 - first to solve; 3 - Challenges Completed; 5 - TeamChallengesSolved
	public List<GameObject> generatedIconsOnLeaderboard;
	public Text usernameLabelForTheInformationPanel;
//	public Text passwordLabelForTheInformationPanel;
//	public Text playfabIDLabelForTheInformationPanel;

	private bool cameraInitialized = false;
	public Quaternion baseRotation;
	private WebCamTexture webcamTexture;
	private BarcodeReader barCodeReader;
	public RawImage QRCameraRawImage;
	public Text playerNameLabelContainer;
	public Text teamNameHolderTextForEvaluation;

	[HideInInspector]
	public string teamIDToEvaluate, teamNameToEvaluate, teamIconToEvaluate;

	// permission 0 -> no record

	// permission 1 -> evaluation
	// permission 2 -> observation
	// permission 3 -> administration
	// permission 4 -> evaluation, observation
	// permission 5 -> evaluation, administration
	// permission 6 -> observation, administration
	// permission 7 -> evaluation, observation, administration




	// button[8] is the evaluation only							// permission 1 
	// button[9] is the observation	only						// permission 2
	// button[10] will be the administration of the game	// permission 10


	// Use this for initialization
	void Start () {

		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = MenuManager.singleton;

			// in case you did not do the mistake of firing up this scene only
			if (GameManager.instance != null) 
			{
				// clean previous scene after loading this one
				GameManager.instance.UnloadObjectsFromGameScene ();
				GameManager.instance.isAnyWindowOpen = true;

				routeFilterSelected.value = MasterManager.route;

				if (PermissionAsEvaluator()) {
					// button for evaluation
					buttons [7].gameObject.SetActive (true);
				} else {
					buttons [7].gameObject.SetActive (false);
				}

				// activate observer for the player if
				if (PermissionAsObserver()) {
					// button for evaluation
					buttons [9].gameObject.SetActive (true);
				} else {
					buttons [9].gameObject.SetActive (false);
				}

				// activate administrator for the player if
				if (PermissionAsAdministrator()) {
					// button for evaluation
					buttons [10].gameObject.SetActive (true);
					buttons [8].gameObject.SetActive (true);	
				} else {
					buttons [10].gameObject.SetActive (false);
					buttons [8].gameObject.SetActive (false);	
				}


				// Start the routine responsible to constantly update the content in the menu
				StartCoroutine (UpdateLabels ());
				StartCoroutine (StartTheDisplayManagerProperly());

				feedbackMessagePanel.SetActive (false);
				// set default option
				if (MasterManager.instance.playerHasTeam) {
					buttons [8].gameObject.SetActive (true);	
				} else {
					buttons [8].gameObject.SetActive (false);	
				}

				StartCoroutine (AskForCameraPermission());

				StartCoroutine (LoadPlayerAvatarAsync ());
			}

		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}


	}

	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("_reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		int count = 20;
		while ((!_referenceDisplayManager.isProperlyInitialized) &&(count > 0))
		{
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[MenuManager] is properly initialized callback timeout");}
		Debug.Log ("[MenuManager] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}

	public void DeleteFile()
	{
		if (File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			File.Delete (Application.persistentDataPath + "/SotSPlayerInfo.dat");

		}
	}
	public void UpdateRouteFilter() {
		if (MasterManager.route != routeFilterSelected.value) { 
			MasterManager.route = routeFilterSelected.value;

			string email;
			string password;
			double desiredMax_DistanceOfChallenges;
			bool isFacebookAccessToken = false;

			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();

				email = data.playFabEmail;
				password = data.playFabPassword;
				isFacebookAccessToken = data.isFacebookAccessToken;
				desiredMax_DistanceOfChallenges = data.desiredMax_DistanceOfChallenges;

				// then, delete the file with the previous name
				DeleteFile ();

				// then, create a new file, and re-store all the info in it
				bf = new BinaryFormatter ();

				if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
					file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
					file.Close ();
				}

				file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

				data = new SotSPlayerInfo ();
				data.isFacebookAccessToken = isFacebookAccessToken;
				data.playFabEmail = email;
				data.playFabPassword = password;
				data.activePlayerName = MasterManager.activePlayerName;
				data.activePlayFabId = MasterManager.activePlayFabId;
				data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
				data.desiredMax_DistanceOfChallenges = desiredMax_DistanceOfChallenges;
				data.teamID = MasterManager.instance.teamID;
				data.teamName = MasterManager.instance.teamName;
				data.teamRefIcon = MasterManager.instance.teamRefIcon;
				data.route = MasterManager.route;
				data.language = MasterManager.language;

				bf.Serialize (file, data);
				file.Close ();


			} else {
				//txt37
				string [] displayMessage10 = {"Could not update the Route. Try it later.",
					"Kan de route niet bijwerken. Probeer het later nog eens.",
					"Não foi possível atualizar o filtro da rota. Tenta de novo mais tarde."};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage10[MasterManager.language]);
			}


			GameManager.instance.reloadChallengesFromCloud = true;	
		}

	}

	void AddPlayerToChallenge() {

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerGetTitleData", 
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[MenuManager] ServerGetTitleData CloudScript Done");
				if(success.FunctionResult != null) {
					Debug.Log("[MenuManager] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}






	public IEnumerator LoadsAvatarImage(string url, Image holder){
		Texture2D texture;

		using (WWW www = new WWW(url))
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
				if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
					WWW wwwDefault = new WWW ("https://cdn3.iconfinder.com/data/icons/lineato-basic-business/94/lineato_torn_document-512.png");
					timer = 0;
					 timeOut = 10;
					 failed = false;
					while (!wwwDefault.isDone)
					{
						if (timer > timeOut) { failed = true; break; }
						timer += Time.deltaTime;
						yield return null;
					}
					if (failed || !string.IsNullOrEmpty (wwwDefault.error)) {
						wwwDefault.Dispose ();
						yield break;
					} else {
						texture = wwwDefault.texture;
					}

					//yield return wwwDefault;
				} else {
					// assign texture
					texture = www.texture;
				}
				www.Dispose ();
			}
			// Wait for download to complete
			//yield return www;



		}
			
		Rect rec = new Rect(0, 0, texture.width, texture.height);
		//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);
		Sprite img = Sprite.Create(texture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
		//buttons [6].sprite = img;


		// return img;
		callbackFunction (holder, img);
	}
	private void callbackFunction(Image holder, Sprite spr)
	{
		holder.sprite = spr;
	}

	public void LanguageChange () {
		// languageDropDown
		if (MasterManager.language != languageDropDown.value) {
			Debug.Log("[MenuManager] Language index changed to " + languageDropDown.value);

			MasterManager.language = languageDropDown.value;
			switch (MasterManager.language)
			{
			case 0:// English
				Localization.Instance.CurrentLanguage = SystemLanguage.English;
				break;
			case 1: // Dutch
				Localization.Instance.CurrentLanguage = SystemLanguage.Dutch;
				break;
			case 2: // Português
				Localization.Instance.CurrentLanguage = SystemLanguage.Portuguese;
				break;
			default: // English
				Localization.Instance.CurrentLanguage = SystemLanguage.English;
				break;
			}


			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				Debug.Log("[MenuManager] Saving language preferences in player's profile: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();

				data.language = MasterManager.language;

				file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				//Debug.LogError("[MenuManager] Language written: " + data.language);
				bf.Serialize (file, data);
				file.Close ();
			}
		}
	}



	private IEnumerator UpdateLabels()
	{
		// update the leaderboard every minute
		while (true) {
			playerStatTexts [0].text = PlayerStatisticsServer.instance.playerStats [0].ToString ();	// gold Stat text
			playerStatTexts [1].text = PlayerStatisticsServer.instance.playerStats [1].ToString ();	// challenges created Stat text
			playerStatTexts [2].text = PlayerStatisticsServer.instance.playerStats [2].ToString ();	// First to Solve Stat text
			playerStatTexts [3].text = PlayerStatisticsServer.instance.playerStats [3].ToString ();	// Challenges Solved Stat text
			playerStatTexts [4].text = PlayerStatisticsServer.instance.playerStats [4].ToString ();	// People Met Stat text
			playerStatTexts [5].text = PlayerStatisticsServer.instance.playerStats [5].ToString ();	// Team Challenges Solved text
			//Debug.LogError("TeamChallengesSolved From Server: " + PlayerStatisticsServer.instance.playerStats [5].ToString ());

			// *************************************************
			// Handle the Badges' alphas, based on how many the player has
			// *************************************************
			// First to Solve Badge
			// *************************************************
			if (PlayerStatisticsServer.instance.playerStats [2] < 1) {
				buttons [3].sprite = Resources.Load<Sprite> ("Basic Handpainted RPG Weapons/Icons/256/icon_sword_01_2_faded");
				//} else if (PlayerStatisticsServer.instance.playerStats [2] < 10) {
				//	buttons [3].sprite = Resources.Load<Sprite>("Basic Handpainted RPG Weapons/Icons/256/icon_sword_01_2");
			} else {
				buttons [3].sprite = Resources.Load<Sprite> ("Basic Handpainted RPG Weapons/Icons/256/icon_sword_01_2_orange");
			}
			// *************************************************
			// Challenges Solved Badge
			// *************************************************
			if (PlayerStatisticsServer.instance.playerStats [3] < 1) {
				buttons [4].sprite = Resources.Load<Sprite> ("Icons/Chest_01_faded");
			} else {
				buttons [4].sprite = Resources.Load<Sprite> ("Icons/Chest_01");
			}
			// *************************************************
			// People Met Badge
			// *************************************************
			if (PlayerStatisticsServer.instance.playerStats [4] < 1) {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/1 - smile");
			} else if (PlayerStatisticsServer.instance.playerStats [4] < 3) {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/2 - smile");
			} else if (PlayerStatisticsServer.instance.playerStats [4] < 5) {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/3 - smile");
			} else if (PlayerStatisticsServer.instance.playerStats [4] < 10) {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/4 - smile");
			} else if (PlayerStatisticsServer.instance.playerStats [4] < 15) {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/5 - smile");
			} else {
				buttons [5].sprite = Resources.Load<Sprite> ("PlayerStatsImages/6 - smile");
			}

			yield return new WaitForSeconds (1);	// each 10 seconds, ask for an updated version of the leaderboard.
		}
	}

	// Manage the windows selected
	// 0 - settings button
	// 1 - player button
	// 2 - leaderboard button
	// 3 - First to Solve Badge
	// 4 - Challenges Solved
	// 5 - People Met
	// 6 - Player Avatar Image
	// 7 - information button
	public void UpdateSelectedOption (int index)
	{
		routeFilterSelected.value = MasterManager.route;
		languageDropDown.value = MasterManager.language;

		StartCoroutine (UpdateSelectedOptionThread (index));
	}

	public void ReloadAppPermissions ()
	{
		GameManager.instance.loadedGamePermissionsFromServer = false;
		StartCoroutine (GameManager.instance.LoadGamePermissionsForPlayer());
	}
	public IEnumerator CorrectIconsOnScreenBasedOnPermissions () {
		// activate evaluation for the player if
		if (PermissionAsEvaluator()) {
			// button for evaluation
			buttons [7].gameObject.SetActive (true);
		} else {
			buttons [7].gameObject.SetActive (false);
		}

		// activate observer for the player if
		if (PermissionAsObserver()) {
			// button for evaluation
			buttons [9].gameObject.SetActive (true);
		} else {
			buttons [9].gameObject.SetActive (false);
		}



		// activate administrator for the player if
		if (PermissionAsAdministrator()) {
			// button for evaluation
			buttons [10].gameObject.SetActive (true);
			buttons [8].gameObject.SetActive (true);	
		} else {
			buttons [10].gameObject.SetActive (false);
			if (MasterManager.instance.playerHasTeam) {
				buttons [8].gameObject.SetActive (true);	
			} else {
				buttons [8].gameObject.SetActive (false);	
			}
		}
		yield break;
	}

	public bool PermissionAsEvaluator()
	{
		if (GameManager.instance.loadedGamePermissionsFromServer) {
			// find out the permissions that a player has in the game
			// permission 0 -> no record
			// permission 1 -> evaluation
			// permission 2 -> observation
			// permission 3 -> administration   [E30E387C456145E0]
			// permission 4 -> evaluation, observation
			// permission 5 -> evaluation, administration
			// permission 6 -> observation, administration
			// permission 7 -> evaluation, observation, administration

			// activate evaluation for the player if
			if ((GameManager.instance.playerGamePermissions == 1) ||
			    (GameManager.instance.playerGamePermissions == 4) ||
			    (GameManager.instance.playerGamePermissions == 5) ||
			    (GameManager.instance.playerGamePermissions == 7)) {
				// button for evaluation
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}

	}

	public bool PermissionAsObserver()
	{
		if (GameManager.instance.loadedGamePermissionsFromServer) {
			// find out the permissions that a player has in the game
			// permission 0 -> no record
			// permission 1 -> evaluation
			// permission 2 -> observation
			// permission 3 -> administration   [E30E387C456145E0]
			// permission 4 -> evaluation, observation
			// permission 5 -> evaluation, administration
			// permission 6 -> observation, administration
			// permission 7 -> evaluation, observation, administration

			// activate evaluation for the player if
			if ((GameManager.instance.playerGamePermissions == 2) ||
				(GameManager.instance.playerGamePermissions == 4) ||
				(GameManager.instance.playerGamePermissions == 6) ||
				(GameManager.instance.playerGamePermissions == 7)) {
				// button for evaluation
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}

	}
	public bool PermissionAsAdministrator()
	{
		if (GameManager.instance.loadedGamePermissionsFromServer) {
			// find out the permissions that a player has in the game
			// permission 0 -> no record
			// permission 1 -> evaluation
			// permission 2 -> observation
			// permission 3 -> administration   [E30E387C456145E0]
			// permission 4 -> evaluation, observation
			// permission 5 -> evaluation, administration
			// permission 6 -> observation, administration
			// permission 7 -> evaluation, observation, administration

			// activate evaluation for the player if
			if ((GameManager.instance.playerGamePermissions == 3) ||
				(GameManager.instance.playerGamePermissions == 5) ||
				(GameManager.instance.playerGamePermissions == 6) ||
				(GameManager.instance.playerGamePermissions == 7)) {
				// button for evaluation
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}

	}

	public IEnumerator UpdateSelectedOptionThread (int index)
	{
		_referenceDisplayManager.statusPanel.SetActive (false);
		// Stop the camera in the Administrator's menu
		AdministratorToolsMenuManager.instance.StopQRCamera ();


		// activate evaluation for the player if
		if (PermissionAsEvaluator()) {
			// button for evaluation
			buttons [7].gameObject.SetActive (true);
		} else {
			buttons [7].gameObject.SetActive (false);
		}

		// activate observer for the player if
		if (PermissionAsObserver()) {
			// button for evaluation
			buttons [9].gameObject.SetActive (true);
		} else {
			buttons [9].gameObject.SetActive (false);
		}

		// activate administrator for the player if
		if (PermissionAsAdministrator()) {
			// button for evaluation
			buttons [10].gameObject.SetActive (true);
			buttons [8].gameObject.SetActive (true);	
		} else {
			buttons [10].gameObject.SetActive (false);
			if (MasterManager.instance.playerHasTeam) {
				buttons [8].gameObject.SetActive (true);	
			} else {
				buttons [8].gameObject.SetActive (false);	
			}
		}

		// set every panel to false, so that we only show the one we want
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		for (int i = 0; i < buttons.Count; i++) {
			buttons [i].color = Color.white;
		}

		switch (index) {
		case 0:	// player statistics
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			string [] displayMessage1 = {"Player's Statistics",
				"Speler statistieken",
				"As tuas estatísticas"};
			selectedOptionText.text = displayMessage1[MasterManager.language];


			if (string.Compare (MasterManager.activePlayerName, "") == 0) {
				string [] displayMessage2 = {"Name not defined.",
					"Naam niet gedefinieerd.",
					"Nome não definido."};
				playerNameLabelContainer.text = displayMessage2[MasterManager.language];					
			} else {
				playerNameLabelContainer.text = MasterManager.activePlayerName;
			}

			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [0].color = Color.yellow;
			usernameLabelForTheInformationPanel.text = MasterManager.activePlayFabId;

			panels [0].SetActive (true);
			break;
		case 1:	// leaderboard challenges solved
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			string [] displayMessage3 = {"Leaderboard of People Met",
				"Leaderboard van Mensen Ontmoet",
				"Ranking das Amizades Feitas"};
			selectedOptionText.text = displayMessage3[MasterManager.language];

			string [] displayMessage4 = {"Loading Leaderboard...",
				"Leaderboard laden...",
				"A carregar o ranking..."};
			_referenceDisplayManager.DisplaySystemMessageNonFading (displayMessage4[MasterManager.language]);
			feedbackMessagePanel.SetActive (true);

			// leaderboard panel
			// *************************************************
			// Generate the icons of the players according to the leaderboard position (challenges solved)
			// *************************************************
			// if there are players in the leaderboard at all
			if (PlayerStatisticsServer.instance.leaderboardPeopleMet.Count > 0)
			{
				// if so, I need to keep track of the objects I instantiate dynamically
				if (generatedIconsOnLeaderboard.Count > 0) {
					for (int i = generatedIconsOnLeaderboard.Count - 1; i >= 0; i--) {
						Destroy (generatedIconsOnLeaderboard [i]);
					}
				}
				// create the exact number of icons to show
				generatedIconsOnLeaderboard = new List<GameObject> (PlayerStatisticsServer.instance.leaderboardPeopleMet.Count);

				//GameObject	genericPlayerIcon;
				//LeaderboardPlayerInfo playerInfo;
				for (int i = 0; i < PlayerStatisticsServer.instance.leaderboardPeopleMet.Count; i++)
				{
					generatedIconsOnLeaderboard.Add(Instantiate<GameObject> (leaderboardPlayersIcon, Vector3.zero, Quaternion.identity));
					generatedIconsOnLeaderboard[i].SetActive (true);
					generatedIconsOnLeaderboard[i].transform.SetParent (containerForLeaderboardPlayersIcons.transform);
					generatedIconsOnLeaderboard[i].transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					generatedIconsOnLeaderboard[i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

					//playerInfo = generatedIconsOnLeaderboard[i].GetComponent<LeaderboardPlayerInfo> ();
					//yield return new WaitForSecondsRealtime(3);

					generatedIconsOnLeaderboard[i].GetComponent<LeaderboardPlayerInfo> ().challengesSolvedAmount.text = 
						PlayerStatisticsServer.instance.leaderboardPeopleMet [i].StatValue.ToString ();
					generatedIconsOnLeaderboard[i].GetComponent<LeaderboardPlayerInfo> ().playerName.text = 
						PlayerStatisticsServer.instance.leaderboardPeopleMet [i].DisplayName;

					while (!PlayerStatisticsServer.instance.playersImagesReady) {
						yield return new WaitForSeconds (1);
					}

					for (int j = 0; j < PlayerStatisticsServer.instance.playersIDsForTheImages.Count; j++) {
						if (string.Compare(PlayerStatisticsServer.instance.playersIDsForTheImages[j], PlayerStatisticsServer.instance.leaderboardPeopleMet [i].DisplayName) == 0) 
						{
							generatedIconsOnLeaderboard [i].GetComponent<LeaderboardPlayerInfo> ().playerImage.sprite = PlayerStatisticsServer.instance.playersImages [j];
							break;
						}
					}

					// load each player's avatar image to the leaderboard
					//yield return LoadsAvatarImage(PlayerStatisticsServer.instance.leaderboardPeopleMet [i].Profile.AvatarUrl, 
					//	generatedIconsOnLeaderboard[i].GetComponent<LeaderboardPlayerInfo> ().playerImage);
				}

				_referenceDisplayManager.statusPanel.SetActive (false);
				feedbackMessagePanel.SetActive (false);
				//
			}

			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [1].color = Color.yellow;
			panels [1].SetActive (true);

			break;
		case 2:	// settings
			string [] displayMessage5 = {"Settings",
				"Instellingen",
				"Definições"};
			selectedOptionText.text = displayMessage5[MasterManager.language];

			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [2].color = Color.yellow;

			panels [2].SetActive (true);


			languageDropDown.value = MasterManager.language;
			languageDropDown.captionText.text = languageDropDown.options [languageDropDown.value].text;
			break;
		
		
		
	/*	case 3:	// information panel
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();

			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			selectedOptionText.text = "Your Credentials \nSecrets of the South";

			//usernameLabelForTheInformationPanel.text = MasterManager.activePlayFabId;
			//passwordLabelForTheInformationPanel.text = MasterManager.activePlayerPassword;
			//playfabIDLabelForTheInformationPanel.text = MasterManager.activePlayFabId;
			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [7].color = Color.yellow;

			panels [3].SetActive (true);
			break;*/
		case 3:	// Evaluation panel
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();

			string [] displayMessage6 = {"Evaluation Panel",
				"Evaluatiepaneel",
				"Painel das Avaliações"};
			selectedOptionText.text = displayMessage6[MasterManager.language];

			if (!EvaluatorRating.instance.evaluationSuccessfullyDone) {
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
			}



			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [7].color = Color.yellow;

			panels [3].SetActive (true);
			break;
		case 4:	// Team's ranking panel
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();

			string [] displayMessage7 = {"Teams' Ranking",
				"Ranglijst van teams",
				"Ranking das Equipas"};
			selectedOptionText.text = displayMessage7[MasterManager.language];

			GenerateTeamRanking.instance.FetchTeamsRankings ();

			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [8].color = Color.yellow;

			panels [6].SetActive (true);
			break;
		case 5:	// Observer's panel
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();

			string [] displayMessage8 = {"Observer of Team's performance",
				"Observer van de prestaties van Team",
				"Observador do Desempenho das Equipas"};
			selectedOptionText.text = displayMessage8[MasterManager.language];


			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [9].color = Color.yellow;

			panels [7].SetActive (true);
			break;
		case 6:	// Administrator panel
			// get player statistics from the server
			//	PlayerStatisticsServer.instance.LoadPlayerStatistics ();

			string [] displayMessage9 = {"Administrator Tools",
				"Beheerderstools",
				"Ferramentas do Administrador"};
			
			selectedOptionText.text = displayMessage9[MasterManager.language];
			AdministratorToolsMenuManager.instance.UpdateSelectedOption (0);


			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [10].color = Color.yellow;

			panels [8].SetActive (true);
			break;
		}
	}

	// 0 - player panel
	// 1 - Leaderboard panel
	// 2 - settings panel
	// 3 - Evaluator panel
	// 4 - Eval Small panel 1
	// 5 - Eval Small panel 2
	// 6 - Teams Ranking Panel
	// 7 - Observer panel
	// 8 - Administrator tools panel

	// 0 - player button
	// 1 - leaderboard button
	// 2 - settings button
	// 3 - First to Solve Badge
	// 4 - Challenges Solved
	// 5 - People Met
	// 6 - Player Avatar Image
	// 7 - Evaluator button
	// 8 - Teams Rankings button
	// 9 - Observer Button
	// 10- Administrator Button

	public void ActivateQRReader ()
	{
		panels [4].gameObject.SetActive (true);


		// Activate 
		barCodeReader = new BarcodeReader ();
		webcamTexture = new WebCamTexture ();
		if (webcamTexture != null) {


			QRCameraRawImage.texture = webcamTexture;
			QRCameraRawImage.material.mainTexture = webcamTexture;

			baseRotation = transform.rotation;
			QRCameraRawImage.gameObject.SetActive (true);
			QRCameraRawImage.gameObject.transform.localScale = new Vector3(1,1,1);

			webcamTexture.Play ();
		}



		cameraInitialized = true;
	}

	// Update is called once per frame
	public void ReturnToGameGUI()
	{
		if (webcamTexture != null) {
			if (webcamTexture.isPlaying) {
				webcamTexture.Stop ();
				QRCameraRawImage.gameObject.SetActive (false);
				cameraInitialized = false;
			}
		}

		GameManager.instance.isAnyWindowOpen = false;

		SceneManager.UnloadSceneAsync (3);
		//DestroyObjectsFromScene ();
		GameManager.instance.LoadObjectsFromGameSceneMeteor ();
	}	


	private IEnumerator AskForCameraPermission()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

		// start this, so that we can show messages in this scene
		yield return StartTheDisplayManagerProperly();


		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {

			feedbackMessagePanel.SetActive (false);
			// set default option
			UpdateSelectedOption (0);



		} else {
			string [] displayMessage = {"Could not use this functionality. Please enable the use of the Webcam and try again...",
				"Kon deze functionaliteit niet gebruiken. Schakel het gebruik van de webcam in en probeer het opnieuw...",
				"Não foi possível usar esta funcionalidade. Por favor ativa o uso da camara nas definições e tenta de novo..."};
			QRCodeManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage[MasterManager.language]);
		}
	}

	private IEnumerator LoadPlayerAvatarAsync()
	{
		while (!GameManager.instance.loadedImageFromServerForMenu) {
			yield return new WaitForSeconds (1);
		}

		buttons[6].sprite = GameManager.instance.playerImageForMenu;
		
	}


	private void Update()
	{


		if (cameraInitialized)
		{
			transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);
			//transform.Rotate(0, 90, 0);

			//if( Application.platform == RuntimePlatform.Android ) 
			{

			}

			try
			{
				if (webcamTexture == null)
				{
					return;
				}
				var data = barCodeReader.Decode(webcamTexture.GetPixels32(), webcamTexture.width, webcamTexture.height);
				if (data != null)
				{
					// QRCode detected.
					MenuManager.instance._referenceDisplayManager.transform.SetAsLastSibling();


					//MenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Identifying QR Code...");
					webcamTexture.Stop();
					QRCameraRawImage.gameObject.SetActive(false);
					cameraInitialized = false;
					//panels[5].gameObject.SetActive(false);
					StartCoroutine(HandleTeam
						(data.Text));
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}    

	// 0 - player panel
	// 1 - Leaderboard panel
	// 2 - settings panel
	// 3 - Evaluator panel
	// 4 - Eval Small panel 1
	// 5 - Eval Small panel 2
	// 6 - Teams Ranking Panel
	// 7 - Observer panel
	// 8 - Administrator tools panel

	public IEnumerator HandleTeam(string data)
	{
		// Person Met: 201805252050088_My darling_TeamIcons/43
		//// Getting the Team's Data out of the new URL format
		/// 0x001c00 -> TeamID
		// [website]_[code]_[teamID]_[team name]_[team icon]
		// http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x001c00_201811091313274_bla 6_TeamIcons/54

		string[] theTemporaryArray = data.Split ('_');
		Debug.Log("[MenuManager] Found " + theTemporaryArray.Length + " word count to handle. These are:");
		foreach(string s in theTemporaryArray)
		{
			Debug.Log(s);
		}
		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";

		if (string.Compare (theTemporaryArray [0], urlToCompare) == 0) {
			if (string.Compare (theTemporaryArray [1], "0x001c00") == 0) {
				teamIDToEvaluate = theTemporaryArray [2];
				teamNameToEvaluate = theTemporaryArray [3];
				teamIconToEvaluate = theTemporaryArray [4];

				// ask whether the data received is from a team
				string url;
				UnityWebRequest request;
				List<string> teamplayers = new List<string> ();

				// **************************************************
				// first, find out whether anybody joined this team
				// **************************************************
				url = MasterManager.serverURL + "/api/numberofplayersinteam?TeamID=" + teamIDToEvaluate;
				request = UnityWebRequest.Get(url);
				request.timeout = 10;
				yield return request.SendWebRequest();

				if (request.isNetworkError) {
					Debug.LogError ("[MenuManager] Error While Sending: " + request.error);
					Debug.LogError ("[MenuManager] URL: " + url);
				} else {
					Debug.Log("[MenuManager] Request with: " + url);
					Debug.Log("[MenuManager] Received: " + request.downloadHandler.text);

					teamplayers = 
						JsonWrapper.DeserializeObject<List<string>>(request.downloadHandler.text);

					if (teamplayers.Count < 1) {
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
						string [] displayMessage1 = {"Not a valid team ID. No players in it.",
							"Geen geldige team-ID. Geen spelers erin.",
							"Não é um identificador de equipa válido. Não há jogadores nesta equipa."};
						MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage1[MasterManager.language]);
						panels [5].gameObject.SetActive (false);
						panels [6].gameObject.SetActive (false);
						yield return new WaitForSeconds (3);
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
						yield break;
					}/* else if (teamplayers.Count < 2) {
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
						MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Invalid team. Not enough members in this team to even rate.");
						panels [5].gameObject.SetActive (false);
						panels [6].gameObject.SetActive (false);
						yield return new WaitForSeconds (3);
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
						yield break;
				}*/  else {

						panels [5].gameObject.SetActive (true);
						teamNameHolderTextForEvaluation.text = teamNameHolderTextForEvaluation.text + " " + teamNameToEvaluate;
					}
				}






			} else {
				Debug.Log ("[MenuManager] the QR code read is not from a team: " + data);
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
				string [] displayMessage2 = {"This is not a Team QR code",
					"Dit is geen Team QR-code",
					"Isto não é um código QR duma equipa"};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage2[MasterManager.language]);
				yield return new WaitForSeconds (3);
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
			}


		} else {
			//queryInExecutionBeforeExit = false;
			Debug.Log ("[MenuManager] the QR code does not appear to have the website prefix ok. What was read was: " + data);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			string [] displayMessage3 = {"This is not a Team QR code. Is this a legacy QR code?",
				"Dit is geen Team QR-code. Is dit een oudere QR-code?",
				"Isto não é um código QR duma equipa. Será que é um código QR antigo?"};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage3[MasterManager.language]);
			yield return new WaitForSeconds (3);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
		}



		yield return null;
	}



	public void ExitGame()
	{
		// save any game data here
		#if UNITY_EDITOR
			// EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
