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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;

using UnityEngine.Events;


public class TeamMenuManager : MonoBehaviour {

	public static TeamMenuManager instance { set; get ; }
	private static TeamMenuManager singleton;

	public GameObject feedbackMessagePanel;
	public GameObject gamePlayCanvas;	// this is the Canvas of the Teams menu 
	public GameObject teamsCanvas;	// this is the Canvas of the Teams menu 
	public Text titleText;	
	public Text teamName;		
	public Image teamImage;
	public Image yourTeamQRIDImage;
	public Text createTeamName;

	// QR Code reader/Image related variables
	private WebCamTexture webcamTexture;
	public RawImage QRCameraRawImage;
	private bool cameraInitialized = false;
	public DisplayManager_Menu_Scene_5 _referenceDisplayManager;
	public Quaternion baseRotation;
	private BarcodeReader barCodeReader;

	public Text teamFoundLabel;
	public Text teamFoundName;
	public List<GameObject> panels;
	// 0 - Your Team QR Code panel
	// 1 - Your Team QR Code panel NO TEAM YET
	// 2 - Join Team Code Reader panel
	// 3 - Create your Team Panel
	public List<Image> buttons;		// images of buttons to play with colours
	// 0 - Your Team QR Code button
	// 1 - Join Team Code Reader button
	// 2 - Create your Team button
	public bool queryInExecutionBeforeExit = false;

	void Start () {

		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = TeamMenuManager.singleton;

			if (gamePlayCanvas == null) throw new System.Exception ("[TeamMenuManager] reference gamePlayCanvas is null!");
			if (teamsCanvas == null) throw new System.Exception ("[TeamMenuManager] reference teamsCanvas is null!");
			if (teamName == null) throw new System.Exception ("[TeamMenuManager] reference teamName is null!");
			if (titleText == null) throw new System.Exception ("[TeamMenuManager] reference titleText is null!");
			if (teamImage == null) throw new System.Exception ("[TeamMenuManager] reference teamImage is null!");
			if (yourTeamQRIDImage == null) throw new System.Exception ("[TeamMenuManager] reference yourTeamQRIDImage is null!");
			if (feedbackMessagePanel == null) throw new System.Exception ("[TeamMenuManager] reference feedbackMessagePanel is null!");

			// deactivate this canvas at the beginning of the game
			teamsCanvas.gameObject.SetActive (false);
			GameManager.instance.isAnyWindowOpen = true;

			StartCoroutine (AskForCameraPermission());

		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}

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
			TeamMenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("[TeamMenuManager] Could not use this functionality. Please enable the use of the Webcam and try again...");
		}
	}
	public void UpdateSelectedOption (int index)
	{

		StartCoroutine (UpdateSelectedOptionThread(index));
	}
	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("[TeamMenuManager] _reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		int count = 20;
		while ((!_referenceDisplayManager.isProperlyInitialized) &&(count > 0))
		{
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[TeamMenuManager] properly initialized timeout");}
		Debug.Log ("[TeamMenuManager] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}
	
	// Update is called once per frame
	void Update () {
		if (cameraInitialized) {
			transform.rotation = baseRotation * Quaternion.AngleAxis (webcamTexture.videoRotationAngle, Vector3.up);



			try {
				if (webcamTexture == null) {
					return;
				}
				var data = barCodeReader.Decode (webcamTexture.GetPixels32 (), webcamTexture.width, webcamTexture.height);
				if (data != null) {
					// QRCode detected.
					//Debug.Log ("Team Met: " + data.Text);
					//TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Identifying this team...");
					//QRCodeManager.instance._referenceDisplayManager.DisplaySystemMessage ("Person Met: " + data.Text);
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;

					StartCoroutine(JoinTeam(data.Text));
				}

			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}
	}

	private IEnumerator JoinTeam (string team)
	{
		queryInExecutionBeforeExit = true;
		bool previouslyHadTeam = MasterManager.instance.playerHasTeam;

		string this_teamID = "";
		string this_teamName = "";
		string this_teamRefIcon = "";
		string[] temporaryArray = team.Split ('_');
		foreach(string s in temporaryArray)
		{
			Debug.Log(s);
		}
		if (temporaryArray.Length < 3) {
			//TeamMenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("Not valid QR Code.");
			UpdateSelectedOption (0);
			MasterManager.instance.playerHasTeam = false; // should be true
			MasterManager.instance.teamID = "";
			MasterManager.instance.teamName = "";
			MasterManager.instance.teamRefIcon = "";
			//CloseCanvas ();
			//GameManager.instance._referenceDisplayManager.DisplayErrorMessage ("Not a valid QR Code of a team. Try again...");
		
		} else {
		
			this_teamID = temporaryArray [0];
			this_teamName = temporaryArray [1];
			this_teamRefIcon = temporaryArray [2];

			MasterManager.instance.playerHasTeam = true; // should be true
			MasterManager.instance.teamID = this_teamID;
			MasterManager.instance.teamName = this_teamName;
			MasterManager.instance.teamRefIcon = this_teamRefIcon;

			//txt25
			string[] displayMessage4 = {"Identifying this QR ...", 
				"Deze QR identificeren ...", 
				"A identificar este código QR ..." };

			TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage4[MasterManager.language]);

			if (!previouslyHadTeam) {
				//txt23
				string[] displayMessage2 = {"Loading multiplayer challenges from Server ...", 
					"Bezig met laden van multiplayer-uitdagingen van Server ...", 
					"A carregar desafios Multiplayer do servidor ..." };
				OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[MasterManager.language], true);
				TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage2[MasterManager.language]);
				yield return GameManager.instance.LoadChallengesFromTheServer();
				GameManager.instance.reloadChallengesFromCloud = true;
			}



			// save in file
			yield return SettingsPanel.UpdateTeamOfPlayerAsync(this_teamID, this_teamName, this_teamRefIcon);

			//txt24
			string[] displayMessage3 = {"Wrapping up", 
				"Afsluiten", 
				"A terminar" };
			TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage3[MasterManager.language]);

			yield return RecordTeamOfPlayerInServer (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);

			TeamMenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);


			queryInExecutionBeforeExit = false;

			// add player to server!

			UpdateSelectedOption (0);
		
		}


		yield break;
	}


	//public bool playerHasTeam = false;
	//public string teamID = "";
	//public string teamName="";
	//public string teamRefIcon = "";

	private  IEnumerator UpdateSelectedOptionThread (int index)
	{
		if (!queryInExecutionBeforeExit) {
			// set every panel to false, so that we only show the one we want
			for (int i = 0; i < panels.Count; i++) {
				panels [i].SetActive (false);
			}
			for (int i = 0; i < buttons.Count; i++) {
				buttons [i].color = Color.white;
			}


			switch (index) {
			case 0:	// Your QR Code Wallet

				if (webcamTexture != null) {
					if (webcamTexture.isPlaying) {
						webcamTexture.Stop ();
						QRCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
					}
				}


				string[] displayMessage2 = {"Your Team", 
					"Jouw team", 
					"A tua equipa" };
				titleText.text = displayMessage2[MasterManager.language];

				if (MasterManager.instance.playerHasTeam) {
					if (webcamTexture != null) {
						if (webcamTexture.isPlaying) {
							webcamTexture.Stop ();
							QRCameraRawImage.gameObject.SetActive (false);
							cameraInitialized = false;
						}
					}

					// Sprite img = imageToSpriteConverter.LoadNewSprite ("TeamIcons/2 - challenge_wolf", 100.0f, SpriteMeshType.Tight);

					Texture2D texture = generateYourQRID (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);

					// generate the QR image for the player
					Rect rec = new Rect (0, 0, texture.width, texture.height);
					//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);
					yourTeamQRIDImage.sprite = Sprite.Create (texture, rec, new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

					teamImage.sprite = Resources.Load<Sprite>(MasterManager.instance.teamRefIcon);
					teamName.text = MasterManager.instance.teamName;

					// *************************************************
					// Handle the click of buttons on the left panel
					// *************************************************
					// set colours of buttons (which one's active, which one's not)
					buttons [0].color = Color.yellow;

					panels [0].SetActive (true);
				} else {
					buttons [0].color = Color.yellow;
					panels [1].SetActive (true);

				}
					


				break;
			case 1:	// QR Code Reader

				teamFoundLabel.gameObject.SetActive (false);
				teamFoundName.gameObject.SetActive (false);
				titleText.text = "Join a Team";

			// set up the QRcode reader
				barCodeReader = new BarcodeReader ();
				webcamTexture = new WebCamTexture ();
				if (webcamTexture != null) {


					QRCameraRawImage.texture = webcamTexture;
					QRCameraRawImage.material.mainTexture = webcamTexture;

					baseRotation = transform.rotation;
					QRCameraRawImage.gameObject.SetActive (true);
					QRCameraRawImage.gameObject.transform.localScale = new Vector3 (1, 1, 1);

					webcamTexture.Play ();
				}



				cameraInitialized = true;


			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
				buttons [1].color = Color.yellow;

				panels [2].SetActive (true);
				break;
			
			case 2:	// QR Code Reader
				if (webcamTexture != null) {
					if (webcamTexture.isPlaying) {
						webcamTexture.Stop ();
						QRCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
					}
				}

				string[] displayMessage3 = {"Create Your Team", 
					"Creëer uw team", 
					"Cria a tua equipa" };

				titleText.text = displayMessage3[MasterManager.language];


		// *************************************************
		// Handle the click of buttons on the left panel
		// *************************************************
		// set colours of buttons (which one's active, which one's not)
				buttons [2].color = Color.yellow;

				panels [3].SetActive (true);
				break;
			}
		}

		yield break;
	}

	// Generates the texture with the QR Code of the player
	// http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x001c00_201811091313274_bla 6_TeamIcons/54
	public Texture2D generateYourQRID(string teamID , string teamName, string iconReference) {

		var encoded = new Texture2D (256, 256);
		var color32 = Encode(MasterManager.serverURL + "/HandleQRCode/_0x001c00_" + teamID + "_" + teamName + "_" + iconReference, encoded.width, encoded.height);
		encoded.SetPixels32(color32);
		encoded.Apply();
		return encoded;
	}
	private static Color32[] Encode(string textForEncoding, 
		int width, int height) {
		var writer = new BarcodeWriter {
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions {
				Height = height,
				Width = width
			}
		};
		return writer.Write(textForEncoding);
	}

	public void CloseCanvas()
	{

		if (!queryInExecutionBeforeExit) {
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			// deactivate this canvas at the beginning of the game
			teamsCanvas.gameObject.SetActive (false);
			gamePlayCanvas.gameObject.SetActive (true);



			/*
			// clean all challenge objects in 3D world
			for (int i = 0; i <GameManager.instance.objectsToDestroyOnLoadScene.Count; i++) {
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
			}*/
		}

		GameManager.instance.isAnyWindowOpen = false;
		//GameManager.instance.LoadObjectsFromGameScene ();
		GameManager.instance.RespawnChallengeObjects ();

	}
	public void OpenTeamsCanvas()
	{
		// deactivate this canvas at the beginning of the game
		UpdateSelectedOption (0);
		teamsCanvas.gameObject.SetActive (true);
		gamePlayCanvas.gameObject.SetActive (false);
	}

	public void CreateTeam()
	{
		StartCoroutine (CreateTeamProcess());

	}
	public IEnumerator CreateTeamProcess()
	{
		bool previouslyHadTeam = MasterManager.instance.playerHasTeam;
		// "asdasd", "My Furst Team", "1-icon ref"

		if (string.Compare (createTeamName.text, string.Empty) == 0) {

			//txt26
			string[] displayMessage4 = {"You should provide a name for your team :)", 
				"Verzin een naam voor het team :)", 
				"Deves de dar um nome à tua equipa :)" };
		
			TeamMenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage4[MasterManager.language]);
			feedbackMessagePanel.transform.SetAsLastSibling ();

		} else {
		
			// generate unique ID
			// get team name
			// randomly choose an image for the team
			// from 1 to 80
			int imgRef = UnityEngine.Random.Range(1,81);
			//Sprite img = imageToSpriteConverter.LoadNewSprite ("TeamIcons/2 - challenge_wolf", 100.0f, SpriteMeshType.Tight);

			// save it in the file?

			// record this in the server

			// and update GameManager
			MasterManager.instance.playerHasTeam = true; // should be true
			MasterManager.instance.teamID = DateTime.Now.ToString("yyyyMMddHHmmssf");
			MasterManager.instance.teamName = createTeamName.text;
			MasterManager.instance.teamRefIcon = "TeamIcons/"+imgRef.ToString();

			Debug.Log ("Team Created. ID:" + MasterManager.instance.teamID + " , Name: " + MasterManager.instance.teamName + " , ImageRef: " + MasterManager.instance.teamRefIcon);
			TeamMenuManager.instance._referenceDisplayManager.statusPanel.transform.SetAsLastSibling ();

			//txt27
			string[] displayMessage5 = {"Creating your team ...", 
				"Team aanmaken ...", 
				"A criar a tua equipa ..." };
			TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage5[MasterManager.language]);

			if (!previouslyHadTeam) {
				//txt23
				string[] displayMessage2 = {"Loading multiplayer challenges from Server ...", 
					"Bezig met laden van multiplayer-uitdagingen van Server ...", 
					"A carregar desafios Multiplayer do servidor ..." };

				TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessagePermanent (displayMessage2[MasterManager.language]);
				//yield return GameManager.instance.LoadMultiplayerChallengesFromTheServer ();
				GameManager.instance.reloadChallengesFromCloud = true;
				yield return GameManager.instance.LoadChallengesFromTheServer();

				// yield return ChallengesCanvasMenuManager.instance.LoadChallengesMenuAsync ();
			}

			//txt24
			string[] displayMessage3 = {"Wrapping up", 
				"Afsluiten", 
				"A terminar" };
			TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessagePermanent (displayMessage3[MasterManager.language]);

			yield return RecordTeamOfPlayerInServer (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);


			// save in file
			yield return SettingsPanel.UpdateTeamOfPlayerAsync(MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);


			TeamMenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);


			// show newly created team
			UpdateSelectedOption (0);
		}

		yield break;
	}

	public IEnumerator RecordTeamOfPlayerInServer(string teamID, string teamName, string teamRefIcon)
	{
		
		string url;
		teamName.Replace (' ', '_');
		url = MasterManager.serverURL + "/api/teams?PlayerPlayFabID=" + MasterManager.activePlayFabId
			+ "&TeamID=" + teamID
			+ "&TeamName=" + teamName
			+ "&TeamRefIcon=" + teamRefIcon;

		//Debug.LogError ("[GameManager CollectPlayerDataGPSRegisterEventInDB] URL with event: " + url);
		//Debug.LogError ("Arrived here. URL: " + url);
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
			if (failed || !string.IsNullOrEmpty(www.error))
			{
				www.Dispose();
				yield break;
			}

			//yield return www;  // Wait for download to complete
			www.Dispose ();
		}

		yield return null;
	}

	public void resetTeam () {

		//if (MasterManager.instance.playerHasTeam) {
			MasterManager.instance.playerHasTeam = false;

			MasterManager.instance.teamID = null;
			MasterManager.instance.teamName = null;
			MasterManager.instance.teamRefIcon = null;


			StartCoroutine (SettingsPanel.RemoveTeamFromPlayerAsync(MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon));


		//} 

		UpdateSelectedOption (0);
	}
}
