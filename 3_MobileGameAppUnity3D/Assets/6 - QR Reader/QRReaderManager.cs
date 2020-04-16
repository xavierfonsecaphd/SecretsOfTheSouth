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

using Mapbox.Unity.Utilities;
//using Mapbox.Map;
using Mapbox.Utils;
using Mapbox.Unity.Map;

// to store data about the challenges. later on, it is going to be my server doing this
using PlayFab;
using PlayFab.ClientModels;

//using FullSerializer;
using PlayFab.Json;
using UnityEngine.Networking;

// used to save a file
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class QRReaderManager : MonoBehaviour {

	[HideInInspector]
	public static QRReaderManager instance { set; get ; }

	// This is a singleton
	[HideInInspector]
	private static QRReaderManager singleton;

	public Text selectedOptionText;	// this is to change the text of what is selected in the game
	public Text outcomeTextInResultOfScanPanel;
	public DisplayManager_Menu_Scene_5 _referenceDisplayManager;
	public GameObject feedbackMessagePanel;


	public List<GameObject> panels;
	// 0 - Panel QR reader
	// 1 - Initial QR reader panel
	// 2 - Active QR reader panel
	// 3 - Result of Scanning panel
	// 4 - Panel Take picture
	// 5 - initial take picture panel
	// 6 - Active take picture panel
	// 7 - Result taking pic panel
	// 8 - What to do with the pic panel
	// 9 - Explain scan challenge panel
	// 10- Scan QR code of challenge panel

	public List<Image> buttons;		// images of buttons to play with colours
	// 0 - QR Reader
	// 1 - Take Picture
	// 2 - Exit button
	// 3 - Cancel Taking Picture button
	// 4 - Cancel Reading QRCode button

	// variables to read a QR code
	//private WebCamTexture camTexture;
	//public Image yourQRIDImage;
	public RawImage QRCameraRawImage;
	private BarcodeReader barCodeReader;
	private bool cameraInitialized = false;
	private WebCamTexture webcamTexture;
	[HideInInspector]
	public Quaternion baseRotation;

	// variables to take a picture
	public RawImage TakePictureCameraRawImage;
	private int indexOFCameraUsed; // 0 for QR reader; 1 for Take Picture; 2 for QR reader after Taking Picture; 
	public Image photoTaken;

	// variables to read a QR code after taking a picture, and wanting to attach it to a Voting Challenge
	public RawImage QRCameraTakePictureRawImage;


	void Start () {

		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = QRReaderManager.singleton;



			// in case you did not do the mistake of firing up this scene only
			if (GameManager.instance != null) 
			{
				// clean previous scene after loading this one
				GameManager.instance.UnloadObjectsFromGameScene ();

				feedbackMessagePanel.gameObject.SetActive(true);
				StartCoroutine (AskForCameraPermission());

				GameManager.instance.isAnyWindowOpen = true;




			}
		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	// 0 - QR Reader
	// 1 - Take Picture
	// 2 - Exit button
	// 3 - Cancel Taking Picture button
	// 4 - Cancel Reading QRCode button
	private  IEnumerator UpdateSelectedOptionThread (int index)
	{
		if (!QR_Codify.instance.queryInExecutionBeforeExit) {
			// set every panel to false, so that we only show the one we want
			for (int i = 0; i < panels.Count; i++) {
				panels [i].SetActive (false);
			}
			for (int i = 0; i < buttons.Count; i++) {
				buttons [i].color = Color.white;
			}

			// activate the exit button always
			buttons[2].gameObject.SetActive(true);
			// deactivate the cancel take picture button
			buttons[3].gameObject.SetActive(false);
			// deactivate the cancel Reading QRCode button
			buttons[4].gameObject.SetActive(false);

			switch (index) {
			case 0:	// Your QR Code Wallet
				//txt30
				string[] displayMessage1 = {"QR Reader", "QR Lezer", "Leitor Códigos QR" };
				selectedOptionText.text = displayMessage1[MasterManager.language];

				if (webcamTexture != null) {
					if (webcamTexture.isPlaying) {
						webcamTexture.Stop ();
						QRCameraRawImage.gameObject.SetActive (false);
						QRCameraTakePictureRawImage.gameObject.SetActive (false);
						TakePictureCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;

					}
					indexOFCameraUsed = -1;
				}
				// QRCameraTakePictureRawImage

				// *************************************************
				// Do something
				// *************************************************





				// *************************************************
				// Handle the click of buttons on the left panel
				// *************************************************
				// set colours of buttons (which one's active, which one's not)
				// buttons [0].color = Color.yellow;

				panels [0].SetActive (true);
				panels [1].SetActive (true);
				break;
			case 1:	// Take picture Button
				// 0 - Panel QR reader
				// 1 - Initial QR reader panel
				// 2 - Active QR reader panel
				// 3 - Result of Scanning panel
				// 4 - Panel Take picture
				// 5 - initial take picture panel

				// provide a title for the selected option
				//txt30
				string[] displayMessage2 = {"Take picture to attach to Challenge", 
					"Maak een foto om aan Challenge toe te voegen", 
					"Tira uma foto para anexares ao desafio" };
				selectedOptionText.text = displayMessage2[MasterManager.language];

				// guarantee the camera is turned off
				if (webcamTexture != null) {
					if (webcamTexture.isPlaying) {
						webcamTexture.Stop ();
						QRCameraRawImage.gameObject.SetActive (false);
						QRCameraTakePictureRawImage.gameObject.SetActive (false);
						TakePictureCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
					}
					indexOFCameraUsed = -1;
				}



				// enable panels
				panels [4].SetActive (true);
				panels [5].SetActive (true);
				
				break;
			}
		}
		yield break;
	}

	// Manage the windows selected
	// 0 - ...
	// 1 - ...
	// 2 - ...
	public void UpdateSelectedOption (int index)
	{
		indexOFCameraUsed = -1; // nothing being used

		StartCoroutine (UpdateSelectedOptionThread(index));
	}

	public void InitializeTakingPicture ()
	{
		StartCoroutine (InitializeCameraToTakePicture());
	}

	public void InitializeQRScan ()
	{
		StartCoroutine (InitializeCamera());
	}
	public void InitializeQRCameraAfterTakePicture ()
	{
		StartCoroutine (InitializeQRCameraAfterTakePictureThread ());
	}
	private IEnumerator InitializeCamera()
	{
		indexOFCameraUsed = 0;
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
		yield return null;
	}
	private IEnumerator InitializeCameraToTakePicture()
	{
		indexOFCameraUsed = 1;
		webcamTexture = new WebCamTexture ();
		if (webcamTexture != null) {
			
			TakePictureCameraRawImage.texture = webcamTexture;
			TakePictureCameraRawImage.material.mainTexture = webcamTexture;
			TakePictureCameraRawImage.gameObject.SetActive (true);
			TakePictureCameraRawImage.gameObject.transform.localScale = new Vector3 (1,1,1);

			baseRotation = transform.rotation;

			webcamTexture.Play ();
		}



		cameraInitialized = true;
		yield return null;
	}

	private IEnumerator InitializeQRCameraAfterTakePictureThread()
	{
		indexOFCameraUsed = 2;
		barCodeReader = new BarcodeReader ();
		webcamTexture = new WebCamTexture ();
		if (webcamTexture != null) {

			QRCameraTakePictureRawImage.texture = webcamTexture;
			QRCameraTakePictureRawImage.material.mainTexture = webcamTexture;
			QRCameraTakePictureRawImage.gameObject.SetActive (true);
			QRCameraTakePictureRawImage.gameObject.transform.localScale = new Vector3 (1,1,1);

			baseRotation = transform.rotation;

			webcamTexture.Play ();
		}



		cameraInitialized = true;
		yield return null;
	}


	private void Update()
	{

		if (cameraInitialized) {
			if (indexOFCameraUsed == 0) // we want to scan a generic QR code
			{
				transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);

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
						Debug.Log("[QRReaderManager] Code read: " + data.Text);


						webcamTexture.Stop();
						QRCameraRawImage.gameObject.SetActive(false);
						QRCameraTakePictureRawImage.gameObject.SetActive(false);
						TakePictureCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
						indexOFCameraUsed = -1;

						panels[2].gameObject.SetActive(false);
						panels[3].gameObject.SetActive(true);
						//txt31
						string[] displayMessage3 = {"Identifying QR code ...", 
							"Identificatie van QR-code ...", 
							"A identificar o código QR ..." };
						outcomeTextInResultOfScanPanel.text = displayMessage3[MasterManager.language];

						QRReaderManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage3[MasterManager.language]);
						// do something with it
						StartCoroutine(QR_Codify.instance.HandleQRCode(data.Text));
					}

				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			} 
			/*else if (indexOFCameraUsed == 2) // we want to read a voting challenge QR code to attach the picture
			{
				transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);

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
						Debug.Log("[QRReaderManager] Code read: " + data.Text);


						webcamTexture.Stop();
						QRCameraRawImage.gameObject.SetActive(false);
						QRCameraTakePictureRawImage.gameObject.SetActive(false);
						TakePictureCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
						indexOFCameraUsed = -1;

						panels[2].gameObject.SetActive(false);
						panels[3].gameObject.SetActive(true);
						outcomeTextInResultOfScanPanel.text = "Identifying QR code ...";

						QRReaderManager.instance._referenceDisplayManager.DisplaySystemMessage ("Identifying QR Code ...");
						// do something with it
						//StartCoroutine(QR_Codify.instance.HandleQRCodeAfterTakingPicture(data.Text));
					}

				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			} */
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
			QRReaderManager.instance._referenceDisplayManager.DisplayErrorMessage ("[QRReaderManager] Could not use this functionality. Please enable the use of the Webcam and try again...");
		}
	}
	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("[QRReaderManager] _reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		while (!_referenceDisplayManager.isProperlyInitialized)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[QRReaderManager] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}

	public void ShowMessageAfterQRCodeScan(string text, bool error)
	{
		// first disable all the panels
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		for (int i = 0; i < buttons.Count; i++) {
			buttons [i].color = Color.white;
		}

		// then enable the message panel
		panels[3].SetActive(true);
		feedbackMessagePanel.SetActive (true);
		outcomeTextInResultOfScanPanel.text = text;

		if (!error) {
			QRReaderManager.instance._referenceDisplayManager.DisplaySystemMessage (text);
		} else {
			QRReaderManager.instance._referenceDisplayManager.DisplayErrorMessage (text);
		}

		StartCoroutine (TimerToTurnOffFeedbackPanel (3));
	}

	private IEnumerator TimerToTurnOffFeedbackPanel(int number) {
	
		if (number > 0) {
		
			while (number > 0) {
				number --;
				yield return new WaitForSeconds (1);
			}

		}

		feedbackMessagePanel.SetActive (false);

		yield break;
	}


	// Update is called once per frame
	public void ReturnToGameGUI()
	{
		StartCoroutine (ExitScene());
		GameManager.instance.isAnyWindowOpen = false;
	}	

	private IEnumerator ExitScene()
	{
		if (!QR_Codify.instance.queryInExecutionBeforeExit) {
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					TakePictureCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			SceneManager.UnloadSceneAsync (5);

			GameManager.instance.LoadObjectsFromGameSceneMeteor ();
		}

		yield break;
	}

	public void CancelTakingPicture () {

		UpdateSelectedOption (1);
	}
	public void CancelReadingQRCodePicture () {

		UpdateSelectedOption (0);
	}

	public void HandleChallengeQRCodeFound(bool isText, string challengeName, string content, Sprite img) {
	

		Debug.Log("[QRReaderManager] Handling windows to show content.");

		GameManager.instance.isAnyWindowOpen = true;
		GameManager.instance.hunterChallengeQRFoundWindow.SetActive (true);

		if (isText) {

			Hunter_window_QRFound_manager.instance.textContentPanel.SetActive (true);
			Hunter_window_QRFound_manager.instance.imageContentPanel.SetActive (false);

			Hunter_window_QRFound_manager.instance.nameOfTheChallenge.text = challengeName;
			Hunter_window_QRFound_manager.instance.textHolder.text = content;
		} else { // is Image
			Hunter_window_QRFound_manager.instance.textContentPanel.SetActive (false);
			Hunter_window_QRFound_manager.instance.imageContentPanel.SetActive (true);

			Hunter_window_QRFound_manager.instance.nameOfTheChallenge.text = challengeName;
			Hunter_window_QRFound_manager.instance.imageHolder.sprite = img;
		}

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Hunter Challenge \"" + 
			challengeName + "\" by scanning QR Code.");

		SceneManager.UnloadSceneAsync (5);

		GameManager.instance.LoadObjectsFromGameSceneMeteor ();
		// loading objects from game scene



		//yield break;
	}

	public IEnumerator HandleJoinTeamQRCode(string arguments)
	{
		

		// first, join team
		TeamMenuManager.instance.queryInExecutionBeforeExit = true;
		bool previouslyHadTeam = MasterManager.instance.playerHasTeam;

		string this_teamID = "";
		string this_teamName = "";
		string this_teamRefIcon = "";
		string[] temporaryArray = arguments.Split ('_');
		foreach(string s in temporaryArray)
		{
			Debug.Log(s);
		}

		this_teamID = temporaryArray [0];
		this_teamName = temporaryArray [1];
		this_teamRefIcon = temporaryArray [2];

		MasterManager.instance.playerHasTeam = true; // should be true
		MasterManager.instance.teamID = this_teamID;
		MasterManager.instance.teamName = this_teamName;
		MasterManager.instance.teamRefIcon = this_teamRefIcon;
		//txt31
		string[] displayMessage4 = {"Identifying this QR code ...", 
			"Identificatie van deze QR-code ...", 
			"A identificar este código QR ..." };
		TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage4[MasterManager.language]);

		if (!previouslyHadTeam) {
			//txt23
			string[] displayMessage5 = {"Loading multiplayer challenges from Server ...", 
				"Bezig met laden van multiplayer-uitdagingen van Server ...", 
				"A carregar desafios Multiplayer do servidor ..." };
			TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage5[MasterManager.language]);
			yield return GameManager.instance.LoadChallengesFromTheServer();
			GameManager.instance.reloadChallengesFromCloud = true;
		}



		// save in file
		yield return SettingsPanel.UpdateTeamOfPlayerAsync(this_teamID, this_teamName, this_teamRefIcon);
		//txt24
		string[] displayMessage6 = {"Wrapping up", 
			"Afsluiten", 
			"A terminar" };
		TeamMenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage6[MasterManager.language]);

		yield return TeamMenuManager.instance.RecordTeamOfPlayerInServer (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);

		TeamMenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);


		TeamMenuManager.instance.queryInExecutionBeforeExit = false;


		// then, open scene 2 with team canvas open, and close current one
		if (webcamTexture != null) {
			if (webcamTexture.isPlaying) {
				webcamTexture.Stop ();
				QRCameraRawImage.gameObject.SetActive (false);
				TakePictureCameraRawImage.gameObject.SetActive (false);
				cameraInitialized = false;
			}
		}


		// handling scenes


		SceneManager.UnloadSceneAsync (5);

		GameManager.instance.LoadObjectsFromGameSceneMeteor ();

		TeamMenuManager.instance.OpenTeamsCanvas ();

		TeamMenuManager.instance.UpdateSelectedOption (0);

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " scanned a QR code of another Team, and joined it.");


		yield return null;
	}

	public IEnumerator HandleScannedPlayerIDOrForeignerIDQRCode()
	{
		int number = 4;
		while (number > 0) {
			number --;
			yield return new WaitForSeconds (1);
		}

		// in here, I want to show the screen of the player statistics
		SceneManager.UnloadSceneAsync (5);

		GameManager.instance.LoadObjectsFromGameSceneMeteor ();

		GameManager.instance.LoadMenu ();


		yield return null;
	}

	// HunterChallengeDBMeteorFormat_JSON

	public IEnumerator ManageHunterChallengeThroughQRFound(HunterChallengeDBMeteorFormat_JSON hunterChallenge) {

		HunterChallengeInfo challengeRef;
		Debug.Log ("[QRReaderManager] Good Job, solving the hunter challenge! :D");


		// first, search for the reference of the challenge, to handle
		int index = -1;
		for (int i = 0; i < GameManager.instance.hunterChallengesOnScreenMeteor.Count; i++) {
			if (string.Compare (GameManager.instance.hunterChallengesOnScreenMeteor [i].name, hunterChallenge.name) == 0) {
				index = i;
				break;
			}
		}
		 
		if (index < 0) {
			throw new UnityException ("[QRReaderManager] I was not able to get the reference in-game of the hunter challenge. Shit");
		}


		if (!GameManager.instance.hunterChallengesOnScreenMeteor [index].solved) {
			Debug.Log ("[QRReaderManager] Challenge is still not solved. marked the challenge as solved");

			GameManager.instance.hunterChallengesOnScreenMeteor [index].solved = true;
			challengeRef = GameManager.instance.hunterChallengesOnScreenMeteor [index];


			// ****************************************************************
			// attempt to remove all the challenge icons on the map that represent this challenge (should be one anyhow, but do till end)
			// ****************************************************************

			for (int i = GameManager.instance.objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				if (GameManager.instance.objectsToDestroyOnLoadScene [i].CompareTag ("HunterChallenge")) {
					if (string.Compare (GameManager.instance.objectsToDestroyOnLoadScene [i].GetComponent<HunterChallengeInfo> ()._id, challengeRef._id) == 0) {
						//Debug.Log ("[QRReaderManager] Found one object to destroy with the same hunter challenge ID");
						Destroy (GameManager.instance.objectsToDestroyOnLoadScene[i]);
						GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (i);
					}
				}
			}

			// generate the new icon as solved on the map
			GameManager.instance.GenerateHunterChallengeSolvedOnScreenMeteor (challengeRef);
			//Debug.Log ("[QRReaderManager] Generating a solved hunter challenge 3D icon on the map...");


			// attribute the points of solving the challenge
			//StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challengeRef._id));
			yield return PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challengeRef._id);
		} else {
			Debug.Log ("[QRReaderManager] Challenge is already solved. Doing nothing");

			yield break;
		}

	}

	public void TakePhoto() {
		StartCoroutine (TakePhotoThread ());
	}

	// 0 - Panel QR reader
	// 1 - Initial QR reader panel
	// 2 - Active QR reader panel
	// 3 - Result of Scanning panel
	// 4 - Panel Take picture
	// 5 - initial take picture panel
	// 6 - Active take picture panel
	// 7 - Result take picture panel
	private IEnumerator TakePhotoThread()
	{
		
		yield return new WaitForEndOfFrame(); 



		Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
		photo.SetPixels(webcamTexture.GetPixels());
		photo.Apply();



		try
		{
			if (!Directory.Exists(Application.persistentDataPath + "/SotSPhotos/"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/SotSPhotos/");
			}
				
			FileStream file;

			if (!File.Exists (Application.persistentDataPath + "/SotSPhotos/photo.png")) {
				file = File.Create (Application.persistentDataPath + "/SotSPhotos/photo.png");
				file.Close ();
			}
				
			Debug.Log ("[QRReaderManager] The saving path for the picture taken is: " + Application.persistentDataPath + "/SotSPhotos/photo.png");

			//Encode to a PNG and Write out the PNG
			QR_Codify.instance.eventualPictureTaken = photo.EncodeToPNG();

			File.WriteAllBytes(Application.persistentDataPath + "/SotSPhotos/photo.png", QR_Codify.instance.eventualPictureTaken);
			Debug.Log ("[QRReaderManager] File is saved. ");



			// Kill the webcam
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					TakePictureCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}


			// Image photoTaken
			photoTaken.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
			panels [6].SetActive (false);
			panels [7].SetActive (true);

			// deactivate the exit button
			buttons[2].gameObject.SetActive(false);
			// activate the cancel take picture button
			buttons[3].gameObject.SetActive(true);
			// deactivate the cancel Reading QRCode button
			buttons[4].gameObject.SetActive(false);


		}
		catch (IOException ex)
		{
			Debug.LogError (ex.Message);
		}
	}





}
