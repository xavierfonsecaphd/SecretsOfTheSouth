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



public class QRCodeManager : MonoBehaviour {

	public static QRCodeManager instance { set; get ; }
	// This is a singleton
	private static QRCodeManager singleton;
	public GameObject feedbackMessagePanel;
	public Text selectedOptionText;
	public DisplayManager_Menu_Scene_5 _referenceDisplayManager;

	// holders for the data you collect from the QR Reader
	public Text idReadContainer;
	public Text registeredAsContainer;
	public Text registeredAsLabel;
	public Text idReadLabel;
	public Text personFoundLabel;

	public List<GameObject> panels;
	// 0 - Your QR Code panel
	// 1 - QR Code Reader panel
	public List<Image> buttons;		// images of buttons to play with colours
	// 0 - Your QR Code button
	// 1 - QR Code Reader button

	private WebCamTexture camTexture;
	public Image yourQRIDImage;
	public RawImage QRCameraRawImage;
	private BarcodeReader barCodeReader;
	private bool cameraInitialized = false;
	private WebCamTexture webcamTexture;
	public Quaternion baseRotation;

	private bool queryInExecutionBeforeExit = false;

	void Start () {

		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = QRCodeManager.singleton;



			// in case you did not do the mistake of firing up this scene only
			if (GameManager.instance != null) 
			{
				// clean previous scene after loading this one
				GameManager.instance.UnloadObjectsFromGameScene ();


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
			QRCodeManager.instance._referenceDisplayManager.DisplayErrorMessage ("Could not use this functionality. Please enable the use of the Webcam and try again...");
		}
	}

	/*private IEnumerator InitializeCamera()
	{

		// initiate the camera of the device
		webcamTexture = new WebCamTexture();
		if (webcamTexture != null) {
			QRCameraRawImage.texture = webcamTexture;
			QRCameraRawImage.material.mainTexture = webcamTexture;
	
			baseRotation = transform.rotation;
			QRCameraRawImage.gameObject.SetActive(true);
			QRCameraRawImage.gameObject.transform.localScale = new Vector3(1,1,1);
			webcamTexture.Play();
		}



		cameraInitialized = true;
		yield return null;
	}*/
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
					Debug.Log("Person Met: " + data.Text);
					QRCodeManager.instance._referenceDisplayManager.DisplaySystemMessage ("Identifying this person...");
					//QRCodeManager.instance._referenceDisplayManager.DisplaySystemMessage ("Person Met: " + data.Text);
					webcamTexture.Stop();
					QRCameraRawImage.gameObject.SetActive(false);
					cameraInitialized = false;
					StartCoroutine(HandlePersonMet(data.Text));
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}    

	// Generates the texture with the QR Code of the player
	// [website]_[code]_[playerID]_[playerName]
	public Texture2D generateYourQRID() {
		
		var encoded = new Texture2D (256, 256);
		var color32 = Encode(MasterManager.serverURL + "/HandleQRCode/_0x001b00_" + MasterManager.activePlayFabId + "_" + MasterManager.activePlayerName, encoded.width, encoded.height);
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

	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("[QRCodeManager] _reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		while (!_referenceDisplayManager.isProperlyInitialized)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[QRCodeManager] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}

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
				//txt32
				string[] displayMessage1 = {"Your QR ID", "Je QR ID", "O teu código QR" };
				selectedOptionText.text = displayMessage1[MasterManager.language];

				if (webcamTexture != null) {
					if (webcamTexture.isPlaying) {
						webcamTexture.Stop ();
						QRCameraRawImage.gameObject.SetActive (false);
						cameraInitialized = false;
					}
				}

				Texture2D texture = generateYourQRID ();
				// generate the QR image for the player
				Rect rec = new Rect (0, 0, texture.width, texture.height);
				//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);
				yourQRIDImage.sprite = Sprite.Create (texture, rec, new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);



				// *************************************************
				// Handle the click of buttons on the left panel
				// *************************************************
				// set colours of buttons (which one's active, which one's not)
				buttons [0].color = Color.yellow;

				panels [0].SetActive (true);
				break;
			case 1:	// QR Code Reader
				personFoundLabel.gameObject.SetActive (false);
				idReadLabel.gameObject.SetActive (false);
				idReadContainer.gameObject.SetActive (false);
				registeredAsLabel.gameObject.SetActive (false);
				registeredAsContainer.gameObject.SetActive (false);
				selectedOptionText.text = "QR Code Reader";

				// set up the QRcode reader
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


				// *************************************************
				// Handle the click of buttons on the left panel
				// *************************************************
				// set colours of buttons (which one's active, which one's not)
				buttons [1].color = Color.yellow;

				panels [1].SetActive (true);
				break;
			}
		}
		yield break;
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
		
		StartCoroutine (UpdateSelectedOptionThread(index));
	}


	public IEnumerator HandlePersonMet(string anonymousPerson)
	{
		queryInExecutionBeforeExit = true;

		string foreignPlayFabID = "";
		//string foreignDisplayName = "";
		bool registeredPersonInPlayfab = false;
		string[] temporaryArray = anonymousPerson.Split ('_');
		Debug.Log("[QRCodeManager] found " + temporaryArray.Length + " word count to handle. These are:");
		foreach(string s in temporaryArray)
		{
			Debug.Log(s);
		}

		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";
		//if (temporaryArray.Length < 2) {
		if (string.Compare(temporaryArray [0], urlToCompare) == 0 ) {
			foreignPlayFabID = temporaryArray [1];
		}
		else {
			// deprecate the foreign qr codes for the time being

			foreignPlayFabID = temporaryArray [0];
			//foreignDisplayName = temporaryArray [1];
			registeredPersonInPlayfab = true;
		}

		HandlePersonMetPlayFab playfab = new HandlePersonMetPlayFab();
		if (registeredPersonInPlayfab)
		{
			yield return playfab.HandleTwoPeopleMeetingUp (MasterManager.activePlayFabId, foreignPlayFabID, 1);
		}
		else
		{
			yield return playfab.HandleTwoPeopleMeetingUp (MasterManager.activePlayFabId, foreignPlayFabID, 0);
		}
		while (!playfab.callbackReturned) {
			yield return new WaitForSeconds (1);
		}
		List<string> callbackResult = playfab.callbackResult;
		if (string.Compare (callbackResult [0], "HandleTwoPeopleMeetingUpSuccess") == 0) {
			if (registeredPersonInPlayfab) {
				StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Player_Met_->_"+foreignPlayFabID));
				string[] displayMessage1 = {"Congratulations, you just met a Player! If he is a new friend, you will get a reward. Go check it out in awhile ...", 
					"Gefeliciteerd, je hebt net een speler ontmoet! Als hij een nieuwe vriend is, krijg je een beloning. Ga het even bekijken ...", 
					"Parabéns, acabaste de conhecer outro jogador! Se ele é um novo contacto, irás receber pontos por isso. Podes confirmar isso dentro de uns momentos ..."};
				QRCodeManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language]);
			} else {
				StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Person_Met_->_"+foreignPlayFabID));
				string[] displayMessage2 = {"Congratulations, you just met a random person! If he is a new friend, you will get a reward. Go check it out in awhile ...", 
					"Gefeliciteerd, je hebt zojuist een willekeurig persoon ontmoet! Als hij een nieuwe vriend is, krijg je een beloning. Ga het even bekijken ...", 
					"Parabéns, acabaste de conhecer uma pessoa desconhecida! Se esta pessoa é um novo contacto, irás receber pontos por isso. Podes confirmar isso dentro de uns momentos ..."};
				QRCodeManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage2[MasterManager.language]);
			}
			Debug.Log ("[QRCodeManager] Congratulations, you just made a new friend! :)");
		} else {
			string[] displayMessage3 = {"Could not add this player to your list of friends.", 
				"Kan deze speler niet toevoegen aan je lijst met vrienden.", 
				"Não foi possível adicionar este jogador à tua lista de amigos."};
			QRCodeManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage3[MasterManager.language]);
			Debug.LogError ("[QRCodeManager] Could not add this person to your list of friends. Check the cloud script at HandlePersonMetPlayFab");
		}

		while (QRCodeManager.instance._referenceDisplayManager.statusPanel.activeSelf) {
			yield return new WaitForSeconds (1);
		}

		queryInExecutionBeforeExit = false;

		yield return null;
	}



	/*public IEnumerator HandlePersonMetForRegisteredPlayer(string theOtherRegisteredPlayer)
	{

		// i have a list of the people I MET! If I do not have a list, create it first
		HandlePersonMetPlayFab playfab = new HandlePersonMetPlayFab();

		playfab.GetTitleDataByKey_CloudScript (theOtherRegisteredPlayer);
		while (!playfab.callbackReturned) {
			yield return new WaitForSeconds (1);
		}
		List<string> listOfPeopleYouMet = playfab.callbackResult;
		playfab.callbackResult = new List<string> ();	// reboot list for next call

		if ((listOfPeopleYouMet == null) || listOfPeopleYouMet.Count == 0) {
			// then in this case, you do not have a list of people you met yet. Let's fix that

			yield return playfab.CreateListOfPeopleMetInFabServer (theOtherRegisteredPlayer);
			while (!playfab.callbackReturned) {
				yield return new WaitForSeconds (1);
			}
			// in this case, he does not know anybody, then add this one directly to the list and submit it

			List<string> peopleToAdd = new List<string> (1);
			peopleToAdd.Add (MasterManager.activePlayFabId);
			yield return playfab.AddPersonToPeopleMetPlayFab (theOtherRegisteredPlayer, peopleToAdd);
			while (!playfab.callbackReturned) {
				yield return new WaitForSeconds (1);
			}

			// and then increment the statistics of people you met so far!
			// putas
			yield return playfab.ServerUpdateStatisticsNumberOfPeopleMetOfGivenPlayer (theOtherRegisteredPlayer);

			Debug.Log ("Congratulations, you updated the other player statistics as well! :)");

		} else {
			bool alreadyKnown = false;
			// then, see whether you already know this person
			for (int i = 0; i < listOfPeopleYouMet.Count; i++) {
				if (string.Compare (MasterManager.activePlayFabId, listOfPeopleYouMet [i]) == 0) {
					alreadyKnown = true;
					break;
				}
			}

			// if so, DO NOTHING
			if (!alreadyKnown) {
				// if not, first add this person to the list of known people
				listOfPeopleYouMet.Add (MasterManager.activePlayFabId);
				yield return playfab.AddPersonToPeopleMetPlayFab (theOtherRegisteredPlayer, listOfPeopleYouMet);
				while (!playfab.callbackReturned) {
					yield return new WaitForSeconds (1);
				}

				// and then increment the statistics of people you met so far!
				yield return playfab.ServerUpdateStatisticsNumberOfPeopleMetOfGivenPlayer (theOtherRegisteredPlayer);
				while (!playfab.callbackReturned) {
					yield return new WaitForSeconds (1);
				}

				Debug.Log ("Congratulations, you updated the other player statistics as well! :)");


			} else {
				Debug.Log ("The other registered player already knows this person.");
			}

		}
		queryInExecutionBeforeExit = false;

		yield return null;
	}*/


	// Update is called once per frame
	public void ReturnToGameGUI()
	{
		StartCoroutine (ExitScene());
		GameManager.instance.isAnyWindowOpen = false;
	}	

	private IEnumerator ExitScene()
	{
		if (!queryInExecutionBeforeExit) {
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					QRCameraRawImage.gameObject.SetActive (false);
					cameraInitialized = false;
				}
			}

			SceneManager.UnloadSceneAsync (4);
			//DestroyObjectsFromScene ();
			GameManager.instance.LoadObjectsFromGameSceneMeteor ();
		}

		yield break;
	}
		
}



