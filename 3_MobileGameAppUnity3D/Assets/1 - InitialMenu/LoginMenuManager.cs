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
using GameToolkit.Localization;

// this is for Save and Load methods
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using PlayFab.Json;
using UnityEngine.Networking;


public class LoginMenuManager : MonoBehaviour {
	public static LoginMenuManager instance { set; get ; }
	// This is a singleton
	private static LoginMenuManager singleton;

	public Dropdown languageSelected, routeSelected;


	public GameObject _referenceCamera;
	public GameObject [] objectsToManageOnLoadScene;
	public DisplayManager_Menu_Scene_1 _referenceDisplayManager;

	// I want this variable here to only handle objects in this scene (scene 0) after others have been properly loaded. 
	// Only then I want to disable the objects from this scene
	public static bool properlyUnloaded = true; 

	public Button newGameButton;
	public Button continueGameButton;
	public bool processingLogin = false; // simple bool variable to disable multiple clicks on certain buttons that might take more time to respond. Pure GUI thing
	public Text versionNumber;
	public GameObject [] objectsToManageWhileLoadingGame;

	private WebCamTexture webcamTexture;
	public RawImage QRCameraRawImage;
	private bool cameraInitialized = false;
	private BarcodeReader barCodeReader;
	public Quaternion baseRotation;
	public GameObject AboutPanelRef;
	public GameObject NewGamePanelRef;
	public InputField displayNameOfPlayer;

	void Awake()
	{
		if (singleton == null) 
		{
			singleton = this;
		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}

		instance = LoginMenuManager.singleton;


	}

	// Use this for initialization
	void Start () {
		if (Camera.allCameras.Length > 1) {
			Debug.Log ("I got " + Camera.allCameras.Length +"  cameras. Disabling them.");
			for (int i = 0; i < Camera.allCameras.Length; i++) {
				Camera.allCameras [i].enabled = false;
				//Camera.allCameras [i].gameObject.SetActive (false);
			}

			//MasterManager.instance._referenceCamera.GetComponent<Camera> ().enabled = false;
			_referenceCamera.GetComponent<Camera> ().enabled = true;
			_referenceCamera.GetComponent<Camera> ().gameObject.SetActive (true);
		}

		if (!MasterManager.properlyUnloaded) {
			MasterManager.instance.UnloadObjectsFromScene ();
		}

		if (newGameButton == null)
			throw new UnityException ("[LoginMenuManager] Please link the new Game Button!");
		if (continueGameButton == null)
			throw new UnityException ("[LoginMenuManager] Please link the continue Game Button!");
		if (versionNumber == null)
			throw new UnityException ("[LoginMenuManager] Please link the versionNumber Text!");
		if (displayNameOfPlayer == null)
			throw new UnityException ("[LoginMenuManager] Please link the displayNameOfPlayer Text!");

		OGLoadingOverlay.StopAllLoaders ();
		AboutPanelRef.SetActive (true);
		NewGamePanelRef.SetActive (false);
		//txt13
		string [] displayMessage1 = {"Version ",
			"Versie ",
			"Versão "};

		versionNumber.text = displayMessage1[MasterManager.language] + Application.version;

		StartCoroutine (StartTheDisplayManagerProperly());

		StartCoroutine (AskForCameraPermission());
	}

	public void ContinueGameButton() {
	



		// if the player already logged into the game, go straight into the game. Otherwise, make him/her log in.
		// if there's a persistence file, load it, and show continue button
		if (File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			//txt14
			string [] displayMessage1 = {"Continuing previous game ...",
				"Doorgaan met vorige game ...",
				"A continuar o jogo anterior ..."};

			OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
			Debug.Log ("[Entered here after the continue button : continuing previous game]");
			LoadPlayer ();

			//newGameButton.gameObject.SetActive (false);
			//continueGameButton.gameObject.SetActive (true);
		} else {// if not, activate the new game button
			newGameButton.gameObject.SetActive (true);
			continueGameButton.gameObject.SetActive (false);
			NewGamePanelRef.SetActive (true);
			AboutPanelRef.SetActive (false);

			//OGLoadingOverlay.StopAllLoaders ();
		}

		//AboutPanelRef.SetActive (false);
		//NewGamePanelRef.SetActive (true);
	}

	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("_reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		while (!_referenceDisplayManager.isProperlyInitialized)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[MenuManager] the display manager was properly initialized");

	}
	private IEnumerator AskForCameraPermission()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);


		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {

			// LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("Error: " + error.GenerateErrorReport ());

		} else {
			string [] displayMessage = {"Could not use this functionality. Please enable the use of the Webcam and try again...",
				"Kon deze functionaliteit niet gebruiken. Schakel het gebruik van de webcam in en probeer het opnieuw ...",
				"Não foi possível usar esta funcionalidade. Por favor ativa o uso da camara nas definições e tenta de novo..."};
			LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage[MasterManager.language]);
		}




	}
	// Update is called once per frame
	public void LoadSceneAndManageObjectsOnLoadScene()
	{
		Debug.Log ("[Entered here after the continue button]");
		properlyUnloaded = false;

		for (int i = 0; i < objectsToManageOnLoadScene.Length; i++) {
			objectsToManageOnLoadScene [i].SetActive (false);
		}

		for (int i = 0; i < objectsToManageWhileLoadingGame.Length; i++) {
			objectsToManageWhileLoadingGame [i].SetActive (true);
		}

		StopQRCamera ();

		SceneManager.LoadScene (2,LoadSceneMode.Additive);
		//SceneManager.UnloadSceneAsync (1);
	}

	// deactivate all the objects that are still active in this scene
	public void UnloadObjectsFromScene()
	{
		SceneManager.UnloadSceneAsync (1);

		// guarantee that you only have one camera active and enabled
		if (Camera.allCameras.Length > 1) {
			Debug.Log ("I got " + Camera.allCameras.Length +"  cameras. Disabling them.");
			for (int i = 0; i < Camera.allCameras.Length; i++) {
				// only disable the cameras that are still active and are not the one linked to this game object
				if (Camera.allCameras [i] != GameManager.instance._referenceCamera.GetComponent<Camera> ()) {
					Camera.allCameras [i].enabled = false;
				}
			}

		}

		// reactivate main map in MasterManager, so that we can start putting objects on it
		MasterManager.instance._referenceMap.SetActive (true);

		GameManager.instance._referenceCamera.GetComponent<Camera> ().enabled = true;
		StopQRCamera ();

		properlyUnloaded = true;
	}

	public void LoadPlayer()
	{
		Debug.Log ("[Continue button loadPlayer]");

		//txt15
		string [] displayMessage1 = {"Loading Player",
			"Speler laden",
			"A carregar jogador"};

		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);

		SotSHandler.instance.LoginToContinueGame (MasterManager.activePlayerEmail, MasterManager.activePlayerPassword);
		/*
		if (!processingLogin) {
			processingLogin = true;

			Debug.Log ("[LoadingMenuManager] Reading the file:  " + Application.persistentDataPath + "/SotSPlayerInfo.dat");
			// does this file exist?
			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {

				//_referenceDisplayManager.DisplaySystemMessage ("Loading Player");
				OGLoadingOverlay.ShowFullcoverLoading("Loading Player", true);

				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();
				/*if (data.isFacebookAccessToken) {
					// re-login with the last login method used
					FacebookHandler.instance.LoginViaContinueButton ();	// login again through facebook, just because

					MasterManager.activePlayerName = data.activePlayerName;
					MasterManager.activePlayFabId = data.activePlayFabId;
					MasterManager.activePlayerAvatarURL = data.playFabPlayerAvatarURL;
					MasterManager.desiredMaxDistanceOfChallenges = data.desiredMax_DistanceOfChallenges;
					MasterManager.activePlayerEmail = data.playFabEmail;
					MasterManager.activePlayerPassword = data.playFabPassword;

					if (string.Compare (data.teamName, string.Empty) == 0) {
						MasterManager.instance.playerHasTeam = false;
					} else {
						MasterManager.instance.playerHasTeam = true;

						MasterManager.instance.teamID = data.teamID;
						MasterManager.instance.teamName = data.teamName;
						MasterManager.instance.teamRefIcon = data.teamRefIcon;
					}

				} else *//*{
					// re-login with the last login method used
					Debug.Log("[LoginMenuManager] Email: " + data.playFabEmail + "   and password:   " + data.playFabPassword);
					SotSHandler.instance.LoginToContinueGame (data.playFabEmail, data.playFabPassword);

					MasterManager.activePlayerName = data.activePlayerName;
					MasterManager.activePlayFabId = data.activePlayFabId;
					MasterManager.activePlayerAvatarURL = data.playFabPlayerAvatarURL;
					MasterManager.desiredMaxDistanceOfChallenges = data.desiredMax_DistanceOfChallenges;
					MasterManager.activePlayerEmail = data.playFabEmail;
					MasterManager.activePlayerPassword = data.playFabPassword;
					MasterManager.language = data.language;
					//Debug.LogError("[LoginMenuManager] Language Read: " + MasterManager.language);


					if ((string.Compare (data.teamID, string.Empty) == 0) || (string.Compare (data.teamName, string.Empty) == 0) || (string.Compare (data.teamRefIcon, string.Empty) == 0) 
						|| (string.IsNullOrEmpty(data.teamID))|| (string.IsNullOrEmpty(data.teamName))|| (string.IsNullOrEmpty(data.teamRefIcon))) {
						MasterManager.instance.playerHasTeam = false;
						MasterManager.instance.teamID = "";
						MasterManager.instance.teamName = "";
						MasterManager.instance.teamRefIcon = "";
					} else {
						MasterManager.instance.playerHasTeam = true;

						Debug.Log("1 MasterManager.teamID = " + MasterManager.instance.teamID + " -> with character count: " + MasterManager.instance.teamID.Length );

						MasterManager.instance.teamID = data.teamID;
						MasterManager.instance.teamName = data.teamName;
						MasterManager.instance.teamRefIcon = data.teamRefIcon;

						Debug.Log("2 MasterManager.teamID = " + MasterManager.instance.teamID + " -> with character count: " + MasterManager.instance.teamID.Length );
					}
				}

			} else {
				Debug.LogError ("[LoginMenuManager] file does not seem to exist.");
			}
		}*/
			
	}

	public void VisitSecretsOfTheSouthPage()
	{
		//StopQRCamera ();
		Application.OpenURL (MasterManager.serverURL);
	}

public void SelectLanguage() {
	MasterManager.language = languageSelected.value;

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
}

public void SelectRoute() {
	MasterManager.route = routeSelected.value;

}


	public void ActivateQRScan () {
		// set up the QRcode reader
		barCodeReader = new BarcodeReader ();
		webcamTexture = new WebCamTexture ();
		if (webcamTexture != null) {


			QRCameraRawImage.texture = webcamTexture;
			//QRCameraRawImage.material.mainTexture = webcamTexture;

			baseRotation = transform.rotation;
			QRCameraRawImage.gameObject.SetActive (true);
			QRCameraRawImage.gameObject.transform.localScale = new Vector3(1,1,1);

			webcamTexture.Play ();
		}



		cameraInitialized = true;
	}
	public void StopQRCamera ()
	{
		if (webcamTexture != null) {
			if (webcamTexture.isPlaying) {
				webcamTexture.Stop ();
				QRCameraRawImage.gameObject.SetActive (false);
				cameraInitialized = false;
			}
		}
	}
	private void Update()
	{

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

					StartCoroutine(HandleQRCode(data.Text));
				}

			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}


	}    
	public IEnumerator HandleQRCode(string genericQRcodeData)
	{
		string[] theTemporaryArray = genericQRcodeData.Split ('_');
		Debug.Log("[LoginMenuManager] Found " + theTemporaryArray.Length + " word count to handle. These are:");
		foreach(string s in theTemporaryArray)
		{
			Debug.Log(s);
		}

		MasterManager.activePlayerName = displayNameOfPlayer.text;

		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";

		if (string.Compare (theTemporaryArray [0], urlToCompare) == 0) {
			if (string.Compare (theTemporaryArray [1], "0x001b00") == 0) {
				// [website]_[code]_[playerID]_[playerName]

				// attempt to login
				//private WebCamTexture webcamTexture;
				//public RawImage QRCameraRawImage;
				//private bool cameraInitialized = false;
				//private BarcodeReader barCodeReader;
				webcamTexture = null;
				QRCameraRawImage = null;
				barCodeReader = null;

				yield return CreateTeamProcess();

				SotSHandler.instance.LoginWithQRCode(theTemporaryArray[2]);
			}
			else {
				Debug.Log ("[LoginMenuManager] the QR code read is not from a SotS registered Player: " + genericQRcodeData);
				string [] displayMessage1 = {"This is not a registered SotS player's QR code.",
					"Dit is geen QR-code van een geregistreerde SotS-speler.",
					"Isto não é um código QR dum jogador registado no jogo."};
				LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage1[MasterManager.language]);
			}
		}
		else {
			//queryInExecutionBeforeExit = false;
			Debug.Log ("[LoginMenuManager] The QR code does not appear to have the website prefix ok. What was read was: " + genericQRcodeData);
			string [] displayMessage2 = {"Not valid QR code to manage. Is it a legacy QR code?",
				"Geen geldige QR-code om te beheren. Is het een oude QR-code?",
				"Não é um código QR válido para gerir. Será que se trata dum código QR antigo?"};
			LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage2[MasterManager.language]);
		}
			

		yield return null;
	}





public IEnumerator CreateTeamProcess()
{
	// randomly choose an image for the team
	// from 1 to 80
	int imgRef = UnityEngine.Random.Range(1,81);
	//Sprite img = imageToSpriteConverter.LoadNewSprite ("TeamIcons/2 - challenge_wolf", 100.0f, SpriteMeshType.Tight);

	// save it in the file?

	// record this in the server

	// and update GameManager
	MasterManager.instance.playerHasTeam = true; // should be true
	MasterManager.instance.teamID = DateTime.Now.ToString("yyyyMMddHHmmssf");
	MasterManager.instance.teamName = "Team " + imgRef;
	MasterManager.instance.teamRefIcon = "TeamIcons/"+imgRef.ToString();


	//txt27
	string[] displayMessage5 = {"Creating your team ...", 
		"Team aanmaken ...", 
		"A criar a tua equipa ..." };

	OGLoadingOverlay.ShowFullcoverLoading(displayMessage5[MasterManager.language], true);


	//txt23
	string[] displayMessage2 = {"Loading multiplayer challenges from Server ...", 
		"Bezig met laden van multiplayer-uitdagingen van Server ...", 
		"A carregar desafios Multiplayer do servidor ..." };

	OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[MasterManager.language], true);

	//txt24
	//string[] displayMessage3 = {"Wrapping up", "Afsluiten", "A terminar" };
	//TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessagePermanent (displayMessage3[MasterManager.language]);

	yield return RecordTeamOfPlayerInServer (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);

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
}
