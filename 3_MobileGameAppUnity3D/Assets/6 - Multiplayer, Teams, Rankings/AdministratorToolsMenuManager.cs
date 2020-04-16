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

using PlayFab;
using PlayFab.ClientModels;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using PlayFab.Json;
using UnityEngine.Networking;

public class AdministratorToolsMenuManager : MonoBehaviour {
	public static AdministratorToolsMenuManager instance { set; get ; }
	private static AdministratorToolsMenuManager singleton;
	// All the references for the icons of the challenges being generated in runtime
	public GameObject containerForPermissionsListIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject permissionsListIcon;	// this is the whole icon being generated
	public List<GameObject> generatedIconsOnLeaderboard;
	private WebCamTexture webcamTexture;
	public RawImage QRCameraRawImage;
	private bool cameraInitialized = false;
	private BarcodeReader barCodeReader;
	public Quaternion baseRotation;
	public Text playerNameLabel;
	public InputField newPermissions;

	[HideInInspector]
	public string foreignPlayFabID;

	public Text selectedOptionText;
	public List<GameObject> panels;
	// 0 - Manage Permissions Panel 1
	// 1 - Manage Permissions Panel 2
	// 2 - Manage Permissions Panel 3
	// 3 - Manage Permissions List Panel 1
	public List<Image> buttons;		// images of buttons to play with colours
	// 0 - Manage Permissions button
	// 1 - Adm Permissions list Button

	// Use this for initialization
	void Start () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = AdministratorToolsMenuManager.singleton;
			StartCoroutine (AskForCameraPermission());
			foreignPlayFabID = "";

		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	public void UpdateSelectedOption (int index)
	{
		StartCoroutine (UpdateSelectedOptionAsync (index));
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

	public IEnumerator UpdateSelectedOptionAsync (int index)
	{
		MenuManager.instance._referenceDisplayManager.statusPanel.SetActive (false);
		StopQRCamera ();


		// set every panel to false, so that we only show the one we want
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		for (int i = 0; i < buttons.Count; i++) {
			buttons [i].color = Color.white;
		}

		switch (index) {
		case 0:	// Manage Permissions

			string[] displayMessage1 = {"Manage Permissions of Player",
				"Beheer machtigingen van speler",
			"Gere as Permissões dos Jogadores"};
			selectedOptionText.text = displayMessage1[MasterManager.language];


			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [0].color = Color.yellow;

			panels [0].SetActive (true);
			break;
		case 1:	// Manage Permissions

			string[] displayMessage2 = {"List of Permissions of all Players",
				"Lijst met machtigingen van alle spelers",
				"Lista das Permissões de Todos os Jogadores"};
			selectedOptionText.text = displayMessage2[MasterManager.language];

			// how many permissions you have? you have to ask
			StartCoroutine (HandlePermissionsListGeneration ());

			// *************************************************
			// Handle the click of buttons on the left panel
			// *************************************************
			// set colours of buttons (which one's active, which one's not)
			buttons [1].color = Color.yellow;

			panels [3].SetActive (true);
			break;
		
		}
		yield return null;
	}

	public IEnumerator HandlePermissionsListGeneration ()
	{
		string url;
		UnityWebRequest request;
		url = MasterManager.serverURL + "/api/allListOfPermissions";
		request = UnityWebRequest.Get (url);
		yield return request.SendWebRequest ();
		if (request.isNetworkError) {
			Debug.LogError ("[MenuManager] Error While Sending: " + request.error);
			Debug.LogError ("[MenuManager] URL: " + url);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Error: " + request.error);
			yield return new WaitForSeconds (3);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
		} else {
			List<ListOfPermissionsJSON> results = 
				JsonWrapper.DeserializeObject<List<ListOfPermissionsJSON>>(request.downloadHandler.text);


			for (int i = 0; i < results.Count; i++) {
				generatedIconsOnLeaderboard.Add(Instantiate<GameObject> (permissionsListIcon, Vector3.zero, Quaternion.identity));
				generatedIconsOnLeaderboard[i].SetActive (true);
				generatedIconsOnLeaderboard[i].transform.SetParent (containerForPermissionsListIcons.transform);
				generatedIconsOnLeaderboard[i].transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
				generatedIconsOnLeaderboard[i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

				generatedIconsOnLeaderboard [i].GetComponent<PermissionsListIcon> ().playerName.text = results [i].PlayerPlayFabID;
				generatedIconsOnLeaderboard [i].GetComponent<PermissionsListIcon> ().playerPermissionsContainer.text = results [i].Permission.ToString();
			}

		}





		yield break;
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
				
					StartCoroutine(HandlePlayer(data.Text));
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}    
	private IEnumerator AskForCameraPermission()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);


		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {

			MenuManager.instance.feedbackMessagePanel.SetActive (false);
			// set default option
			UpdateSelectedOption (0);

		} else {
			string[] displayMessage = {"Could not use this functionality. Please enable the use of the Webcam and try again...",
				"Kon deze functionaliteit niet gebruiken. Schakel het gebruik van de webcam in en probeer het opnieuw ...",
				"Não é possível usar esta funcionalidade. Por favor ativa o uso da camara nas definições e tenta de novo..."};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage[MasterManager.language]);
		}
	}

	public void ActivateQRScan () {
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
	}

	public IEnumerator HandlePlayer(string anonymousPlayer)
	{

		string[] theTemporaryArray = anonymousPlayer.Split ('_');
		Debug.Log("[AdministratorToolsMenuManager] Found " + theTemporaryArray.Length + " word count to handle. These are:");
		foreach(string s in theTemporaryArray)
		{
			Debug.Log(s);
		}
		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";

		if (string.Compare (theTemporaryArray [0], urlToCompare) == 0) {
			if (string.Compare (theTemporaryArray [1], "0x001b00") == 0) {
				// [website]_[code]_[playerID]_[playerName]

				if (string.Compare(theTemporaryArray [2], "E30E387C456145E0") == 0) {
					// E30E387C456145E0
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
					string[] displayMessage1 = {"This is the creator of the game. You cannot alter his permissions.",
						"Dit is de maker van het spel. U kunt zijn rechten niet wijzigen.",
						"Este é o criador do jogo. Tu não podes alterar as permissões dele."};
					MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage1[MasterManager.language]);
					yield return new WaitForSeconds (3);
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
				}
				else {
					foreignPlayFabID = theTemporaryArray [2];
					//foreignDisplayName = temporaryArray [3];

					panels [2].gameObject.SetActive (true);
					if (theTemporaryArray.Length == 4) {
						string[] displayMessage2 = {"Player [" + theTemporaryArray [2] + "]: " + theTemporaryArray [3],
							"Speler [" + theTemporaryArray [2] + "]: " + theTemporaryArray [3],
							"Jogador [" + theTemporaryArray [2] + "]: " + theTemporaryArray [3]};
						playerNameLabel.text = displayMessage2[MasterManager.language];
					} else {
						string[] displayMessage3 = {"Player: " + theTemporaryArray [2] ,
							"Speler: " + theTemporaryArray [2] ,
							"Jogador: " + theTemporaryArray [2] };
						playerNameLabel.text = displayMessage3[MasterManager.language];
					}

				}


			
			}
			else {
				Debug.Log ("[AdministratorToolsMenuManager] the QR code read is not from a SotS registered Player: " + anonymousPlayer);

				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
				string[] displayMessage4 = {"This is not a player's QR code.",
					"Dit is geen QR-code van een speler.",
					"Este não é um código QR dum jogador."};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage4[MasterManager.language]);
				yield return new WaitForSeconds (3);
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
				UpdateSelectedOption (0);
			}


		}
		else {
			//queryInExecutionBeforeExit = false;
			Debug.Log ("[AdministratorToolsMenuManager] the QR code does not appear to have the website prefix ok. What was read was: " + anonymousPlayer);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			string[] displayMessage5 = {"Not valid QR code to manage. Is it a legacy QR code?",
				"Geen geldige QR-code om te beheren. Is het een oude QR-code?",
				"Não é um código QR válido. Será que é um código antigo?"};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage5[MasterManager.language]);
			yield return new WaitForSeconds (3);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
			UpdateSelectedOption (0);
		}



		yield return null;



	}
	public void SubmitNewPermissions()
	{
		StartCoroutine (SubmitNewPermissionsAsync ());
	}
	public IEnumerator SubmitNewPermissionsAsync()
	{
		
		int val = int.Parse (newPermissions.text);
		Debug.Log ("[AdministratorToolsMenuManager] New Permissions requested: " + val);

		if (val < 0 || val > 7) {
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			string[] displayMessage1 = {"Not a Valid Permission. Choose from 0 to 7.",
				"Geen geldige toestemming. Kies uit 0 tot 7.",
				"Não é uma permissão válida. Escolhe uma de 0 a 7."};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage1[MasterManager.language]);
			yield return new WaitForSeconds (3);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
		} else {

			// extra validation before submitting: is the player still an admin?
			yield return GameManager.instance.LoadGamePermissionsForPlayer();

			if (MenuManager.instance.PermissionAsAdministrator ()) {
				// http://localhost:3000/api/entergamePermissions?PlayerID=E30E387C456145E0&DesiredPermission=1
				if (string.Compare(foreignPlayFabID, "E30E387C456145E0") != 0)
				{
					string url;
					UnityWebRequest request;
					url = MasterManager.serverURL + "/api/entergamePermissions?PlayerID=" + foreignPlayFabID + "&DesiredPermission=" + val.ToString ();
					request = UnityWebRequest.Get (url);
					yield return request.SendWebRequest ();
					if (request.isNetworkError) {
						Debug.LogError ("[MenuManager] Error While Sending: " + request.error);
						Debug.LogError ("[MenuManager] URL: " + url);
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
						MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Error: " + request.error);
						yield return new WaitForSeconds (3);
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
					} else {
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
						string[] displayMessage2 = {"OK",
							"OK",
							"OKAY"};
						MenuManager.instance._referenceDisplayManager.DisplayGameMessageNonFading (displayMessage2[MasterManager.language]);
						yield return new WaitForSeconds (3);
						MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
					}
				}
				else {
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
					string[] displayMessage3 = {"You cannot alter the permissions of the creator of the game.",
						"Je kunt de rechten van de maker van het spel niet wijzigen.",
						"Não podes alterar as permissões do criador do jogo."};
					MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage3[MasterManager.language]);
					yield return new WaitForSeconds (3);
					UpdateSelectedOption (0);
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
				}

			} else {
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
				string[] displayMessage4 = {"Your permissions as Administrator were revoked.",
					"Uw machtigingen als beheerder zijn ingetrokken.",
					"As tuas permissões de administrador foram removidas."};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading (displayMessage4[MasterManager.language]);
				yield return new WaitForSeconds (3);
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
				MenuManager.instance.UpdateSelectedOption (0);
			}
		}
	}
	public void VerifyInputFieldOfPermissionsToSubmit() {
		int val = int.Parse (newPermissions.text);
		if (val < 0 || val > 7) {
			newPermissions.text = "";
		}
	}
}
