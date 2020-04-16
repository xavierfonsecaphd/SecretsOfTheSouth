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
using System.Text;


public class ManageVotingChallenge : MonoBehaviour {

	public List<GameObject> panels;
	// 0 - TakePicture Panel
	// 1 - Active Take Picture panel
	// 2 - Result of Taking Pic Panel
	// 3 - What name panel
	// 4 - Uploading panel
	// 5 - Show Take Picture Panel 2 (initial button to take picture, should only appear when you didn't solve the challenge)
	// 6 - Show All Pictures Panel 4 (all pictures, should only appear when you solved the challenge)
	// 7 - Solutions Panel
	// 8 - Vote Panel
	// 9 - Give Stars Panel

	public List<GameObject> butons;
	// 0 - Go Back Button (when adding a picture and a name to the author)
	// 1 - Vote Button (hide when clicking in the picture of the same player)
	// 2 - Star 1
	// 3 - Star 1 Color
	// 4 - Star 2
	// 5 - Star 2 Color
	// 6 - Star 3
	// 7 - Star 3 color
	// 8 - Star 4
	// 9 - Star 4 Color
	// 10 - Star 5
	// 11 - Star 5 Color

	public DisplayManager_FeedBack_VotingWindow_Scene_2 _referenceDisplayManager;
	public Text taskHolder, descriptionHolder, titleHolder;
	public Image imageHolder;
	public ScrollRect descriptionScrollViewPanel; // this is to put the text of the description always up
	//private bool updatePictureOfThisPlayer = false;// this variable will be signalled when the player uploads a picture. Then, while verifying if we need to download pictures, if there is an existent file rendered locally, this will be used to delete it and load the new one
	public GameObject feedbackMessagePanel;

	private bool cameraActive = false;
	private string urlPrefix;
	//private bool cameraInitialized = false;
	private WebCamTexture webcamTexture;
	[HideInInspector]
	public Quaternion baseRotation;
	// variables to take a picture
	public RawImage rearPictureCameraRawImage, frontPictureCameraRawImage;
	//public GameObject rearPictureContainerPanel, frontPictureContainerPanel;
	//private int indexOFCameraUsed; // 0 for QR reader; 1 for Take Picture; 2 for QR reader after Taking Picture; 
	public Image photoTaken;
	public byte[] eventualPictureTaken;
	private bool rotated = false;
	public InputField nameOfAuthorPhotoTaken;
	public Text uploadingLabel;
	private bool animateUploadingLabel = false;
	[HideInInspector]
	public VotingChallengeInfo currentVotingChallengeBeingSolved;
	private PictureDataToVote pictureBeingVotedFor;
	[HideInInspector]
	public List<Sprite> imageSolutions;
	[HideInInspector]
	public List<string> authorsOfSolutions;
	public List<string> votingChallengeIDsOfSolutions;

	public Button invertCameraIcon;
	private bool isFrontCamera;
	private string nameOfCameraFeedUsed;
	//public Image temporaryImageHolder;
	public GameObject containerForIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject iconPrefab;	// this is the whole icon being generated
	public List <GameObject> listOfIcons;
	private List<string> listOfChallengesAlreadyVotedFor;
	public int voteGivenToPicture;

	// ************************************ 
	// Vote panel variables

	public Image imageHolderVotePanel;
	// Label: NumberOfVotes
	// Label: AverageRating 
	public Text nameOfAuthor, numberOfVotes, averageRating;

	//************************************

	// Use this for initialization
	void Start () {
		imageSolutions = new List<Sprite> ();
		authorsOfSolutions = new List<string> ();
		votingChallengeIDsOfSolutions = new List<string> ();

		listOfChallengesAlreadyVotedFor = new List<string> ();
		voteGivenToPicture = -1;

		//GameManager.instance.isAnyWindowOpen = true;
		urlPrefix = MasterManager.serverURL;
		//urlPrefix = "localhost:3000";
		//StartCoroutine(PrepareToShowSolutionsOfVotingChallengeInBackground());

		#if UNITY_EDITOR
			invertCameraIcon.gameObject.SetActive(false);
		#else 
		// if you have more than one camera on your phone, then activate the invert camera button. Otherwise, don't
		if (WebCamTexture.devices.Length > 1) {
			invertCameraIcon.gameObject.SetActive(true);
		}
		else {
			invertCameraIcon.gameObject.SetActive(false);
		}
		#endif


		StartCoroutine (AskForCameraPermission());
	}

	public void ActivateWindow()
	{
		GameManager.instance.isAnyWindowOpen = true;

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Voting Challenge \"" + 
			currentVotingChallengeBeingSolved.name + "\" ["+currentVotingChallengeBeingSolved._id+"]");

		// show original voting challenge window
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}

		// activate all required buttons
		for (int i = 0; i < 2; i++) {
			butons [i].SetActive (true);
		}
			
		GameManager.instance.votingChallengeWindow.SetActive(true);

		// and now, show the different buttons, to allow for the players to see other players' photos
		// 5 - Show Take Picture Panel 2 (initial button to take picture, should only appear when you didn't solve the challenge)
		// 6 - Show All Pictures Panel 4 (all pictures, should only appear when you solved the
		if (currentVotingChallengeBeingSolved.solved) {
			Debug.Log ("[ManageVotingChallenge] Challenge is solved.");
			panels[5].SetActive(false);
			panels[6].SetActive(true);
		} else {
			Debug.Log ("[ManageVotingChallenge] Challenge is not solved.");
			panels[5].SetActive(true);
			panels[6].SetActive(false);
		}


		Debug.Log ("[ManageVotingChallenge] Challenge ID:  " + currentVotingChallengeBeingSolved._id);

		nameOfAuthorPhotoTaken.text = MasterManager.activePlayerName;

		// this is simply to get the most recent voting of all the pictures in the challenge
		StartCoroutine (RefreshCurrentChallengeBeingSolved());
		// if you are not already taking care of such request, then do this
		//if (!loadingSolutionsToShowInProgress) {
			
//		}
		//StartCoroutine (PrepareToShowSolutionsOfVotingChallengeInBackground());	

	}
	// DEPRECATED
	private IEnumerator RefreshCurrentChallengeBeingSolved () {
		// CMCY8NAxEDRigaJ2f&
		//GameManager.instance.isAnyWindowOpen = true;
		Debug.Log("[ManageVotingChallenges] Refreshing Current Challenge votes");
		string url;
		UnityWebRequest request;


		urlPrefix = MasterManager.serverURL;
		url = urlPrefix+ "/api/challenges_voting?id=" + currentVotingChallengeBeingSolved._id;
		request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();
		if (request.isNetworkError) {
			Debug.LogError ("[ManageVotingChallenge] Error While Sending: " + request.error);
			Debug.LogError ("[ManageVotingChallenge] URL: " + url);
		} else {
			Debug.Log("[ManageVotingChallenge] Request with: " + url);
			Debug.Log("[ManageVotingChallenge] Received: " + request.downloadHandler.text);

			try {
				VotingChallengeDBMeteorFormat_JSON result = 
					JsonWrapper.DeserializeObject<VotingChallengeDBMeteorFormat_JSON>(request.downloadHandler.text);
				currentVotingChallengeBeingSolved.listOfImagesAndVotes = result.listOfImagesAndVotes;


				// Ok, now, in here you should use this opportunity to update all the voting values of the solution
				if (currentVotingChallengeBeingSolved.solved) {
					if (currentVotingChallengeBeingSolved.listOfImagesAndVotes != null) {
						PictureDataToVote script;
						Text[] texts;
						string[] tmpArray;
						float tmpAverage = 0;
						for (int k = 0; k < currentVotingChallengeBeingSolved.listOfImagesAndVotes.Count; k++) {
							for (int l = 0; l < listOfIcons.Count; l++) {
								// only show solutions of this challenge
								if (string.Compare(currentVotingChallengeBeingSolved._id, votingChallengeIDsOfSolutions[l]) != 0) {
									listOfIcons[l].SetActive(false);
								}
								else {
									listOfIcons[l].SetActive(true);

									script = listOfIcons [l].GetComponent<PictureDataToVote> ();	
									script.challengeBeingSolved = currentVotingChallengeBeingSolved;
									if (string.Compare (script.fileNameNoExtension.Split ('_') [0], currentVotingChallengeBeingSolved.listOfImagesAndVotes [k].Split ('_') [0]) == 0) {
										// then, we found a match between the downloaded info, and the objects generated.
										// update them.
										tmpArray = currentVotingChallengeBeingSolved.listOfImagesAndVotes [k].Split ('_');
										script.numberOfVotes = tmpArray[1];
										//Debug.LogError ("Value: " + tmpAverage);
										//Debug.LogError ("Number of Votes: " + script.numberOfVotes);
										if (int.Parse (script.numberOfVotes) > 0) {
											//	Debug.LogError ("Summing: " + (1 * int.Parse (tmpArray [5]))+" - " + (2 * int.Parse (tmpArray [7]))+" - " + (3 * int.Parse (tmpArray [9]))+" - " + (4 * int.Parse (tmpArray [11]))+" - " + (5 * int.Parse (tmpArray [13])));
											tmpAverage = (1 * int.Parse (tmpArray [5])) + (2 * int.Parse (tmpArray [7])) + (3 * int.Parse (tmpArray [9])) + (4 * int.Parse (tmpArray [11])) + (5 * int.Parse (tmpArray [13]));
											//Debug.LogError ("Value after sum: " + tmpAverage);
											tmpAverage = tmpAverage / int.Parse (script.numberOfVotes);
											//Debug.LogError ("Value after division: " + tmpAverage);
											tmpAverage = (float)System.Math.Round (tmpAverage, 1);
											//Debug.LogError ("Value after rounding: " + tmpAverage);
											script.averageRating = tmpAverage.ToString ();

										} else {
											script.numberOfVotes = 0.ToString ();
											script.averageRating = 0.ToString ();
										}



										texts = listOfIcons [l].gameObject.GetComponentsInChildren<Text> ();
										for (int z = 0; z < texts.Length; z++) {
											if (string.Compare (texts [z].tag, "NumberOfVotes") == 0) {
												texts [z].text = script.numberOfVotes;
											}
											if (string.Compare (texts [z].tag, "AverageRating") == 0) {
												texts [z].text = script.averageRating;
											}
										}

										break;
									}
								}


							}
						}
					}
					else {
						// neste caso, os votos na base de dados foram provavelmente apagados, e então deves fazer
						// o reset de todos os valores mostrados no ecrã para zero.
						PictureDataToVote script;
						Text[] texts;
						for (int l = 0; l < listOfIcons.Count; l++) { 
							script = listOfIcons [l].GetComponent<PictureDataToVote> ();	
							script.challengeBeingSolved = currentVotingChallengeBeingSolved;
							script.numberOfVotes = 0.ToString ();
							script.averageRating = 0.ToString ();

							texts = listOfIcons [l].gameObject.GetComponentsInChildren<Text> ();
							for (int z = 0; z < texts.Length; z++) {
								if (string.Compare (texts [z].tag, "NumberOfVotes") == 0) {
									texts [z].text = script.numberOfVotes;
								}
								if (string.Compare (texts [z].tag, "AverageRating") == 0) {
									texts [z].text = script.averageRating;
								}
							}
						}

						// in such case, I am allowing the player to vote again in the pictures
						listOfChallengesAlreadyVotedFor = new List<string> ();
					}

				}

			}
			catch (Exception e) {
				Debug.LogError ("[ManageVogintChallenge] skipped the rest of the function RefreshCurrentChallengeBeingSolved due to Poor JSON: " + e.Message);
			}
		}

		yield return VerifyPicturesAlreadyDownloaded();
	}
	public void CloseWindow()
	{
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Voting Challenge \"" + 
			currentVotingChallengeBeingSolved.name + "\" ["+currentVotingChallengeBeingSolved._id+"]");
		
		currentVotingChallengeBeingSolved = null;



		// if you voted, then reload the voting challenges votes.
		// anyhow, you should somehow respawn the info you have, because people might have voted already, and uploaded 
		// more pictures for this challenge
		StartCoroutine(GameManager.instance.LoadVotingChallengesFromTheServer());
		GameManager.instance.votingChallengeWindow.SetActive(false);
		GameManager.instance.isAnyWindowOpen = false;
	}
	public void SetVotingForPicture(int value) {
		voteGivenToPicture = value;
	}
	private IEnumerator AskForCameraPermission()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

		// start this, so that we can show messages in this scene
		yield return StartTheDisplayManagerProperly();


		if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {

			feedbackMessagePanel.SetActive (false);
			// set default option
			//UpdateSelectedOption (0);

			// guarantee the camera is turned off
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					//QRCameraRawImage.gameObject.SetActive (false);
					//QRCameraTakePictureRawImage.gameObject.SetActive (false);
					rearPictureCameraRawImage.gameObject.SetActive (false);
					frontPictureCameraRawImage.gameObject.SetActive (false);
				//	cameraInitialized = false;
					cameraActive = false;
				}
				//indexOFCameraUsed = -1;
			}


		} else {
			_referenceDisplayManager.DisplayErrorMessage ("[ManageVotingChallenge] Could not use this functionality. Please enable the use of the Webcam and try again...");
		}
	}
	private IEnumerator StartTheDisplayManagerProperly()
	{
		// this makes the message panel disappear
		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("[ManageVotingChallenge] _reference display manager is null!");
		// guarantee you have the display manager before showing messages on screen
		while (!_referenceDisplayManager.isProperlyInitialized)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[ManageVotingChallenge] the display manager was properly initialized");
		//_referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
	}
	public void ResetRotation () {
		if (rotated) {
			rotated = false;
			#if UNITY_EDITOR
				photoTaken.transform.Rotate (Vector3.forward * -90);
			#else 
				photoTaken.transform.Rotate (Vector3.forward * 180);
			#endif
		}
		nameOfCameraFeedUsed = "";
	}

	public void InitializeCamera() {
		StartCoroutine (InitializeCameraThread ());
		/*
		if (currentVotingChallengeBeingSolved != null) {
			StartCoroutine (InitializeCameraThread ());
		} else {
			Debug.LogError ("[ManageVotingChallenge] Camera will not initialize. The current voting challenge being solved is not known.");
		}*/
	}
	public void InitializeDualCamera() {
		if (rotated) {
			rotated = false;
			#if UNITY_EDITOR
				photoTaken.transform.Rotate (Vector3.forward * -90);
			#else 
				photoTaken.transform.Rotate (Vector3.forward * 180);
			#endif
		}

		if (string.IsNullOrEmpty(nameOfCameraFeedUsed)) {
			StartCoroutine (InitializeCameraThread ());
		}
		else {
			StartCoroutine (InitializeCameraThread(nameOfCameraFeedUsed, Screen.width, Screen.height));
		}
	}
	private IEnumerator InitializeCameraThread()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

		if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {
			nameOfCameraFeedUsed = WebCamTexture.devices [0].name;
			isFrontCamera = false;
			webcamTexture = new WebCamTexture ();
			if (webcamTexture != null) {
			
				rearPictureCameraRawImage.texture = webcamTexture;
				rearPictureCameraRawImage.material.mainTexture = webcamTexture;
				rearPictureCameraRawImage.gameObject.SetActive (true);

				#if UNITY_EDITOR
					rearPictureCameraRawImage.gameObject.transform.localScale = new Vector3 (1, -1, 1);
				#else 
					rearPictureCameraRawImage.gameObject.transform.localScale = new Vector3 (1, 1, 1);
				#endif

				baseRotation = transform.rotation;


				webcamTexture.Play ();
				cameraActive = true;
			}
		}

		//cameraInitialized = true;
		yield return null;
	}
	private IEnumerator InitializeCameraThread(string name, int width, int height)
	{

		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

		if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {//(UserAuthorization.WebCam | UserAuthorization.Microphone)) {

			nameOfCameraFeedUsed = name;
			webcamTexture = null;

			webcamTexture = new WebCamTexture (name, width, height);
			if (webcamTexture != null) {


				//takePictureContainer.transform.Rotate (new Vector3(0,180,0));
				if (!isFrontCamera) {
					rearPictureCameraRawImage.gameObject.SetActive (true);
					frontPictureCameraRawImage.gameObject.SetActive (false);

					rearPictureCameraRawImage.texture = webcamTexture;
					rearPictureCameraRawImage.material.mainTexture = webcamTexture;
					rearPictureCameraRawImage.gameObject.SetActive (true);

					rearPictureCameraRawImage.gameObject.transform.localScale = new Vector3 (1, 1, 1);
					baseRotation = transform.rotation;

				} else {
					rearPictureCameraRawImage.gameObject.SetActive (false);
					frontPictureCameraRawImage.gameObject.SetActive (true);


					frontPictureCameraRawImage.texture = webcamTexture;
					frontPictureCameraRawImage.material.mainTexture = webcamTexture;
					frontPictureCameraRawImage.gameObject.SetActive (true);


					frontPictureCameraRawImage.gameObject.transform.localScale = new Vector3 (1, -1, 1);
					baseRotation = transform.rotation;
				}

				// Quaternion.LookRotation( directionYouWantHimToFace, Vector3.up )


				webcamTexture.Play ();
				cameraActive = true;
			}
		}

		//cameraInitialized = true;
		yield return null;
	}
	public void StopCamera() {
		StartCoroutine (StopCameraThread ());
	}
	private IEnumerator StopCameraThread()
	{
		// guarantee the camera is turned off
		if (webcamTexture != null) {
			if (webcamTexture.isPlaying) {
				webcamTexture.Stop ();

				rearPictureCameraRawImage.gameObject.SetActive (false);
				frontPictureCameraRawImage.gameObject.SetActive (false);

				nameOfCameraFeedUsed = "";
				//cameraInitialized = false;
				cameraActive = false;
			}
			//indexOFCameraUsed = -1;

		}

		yield return null;
	}

	public void TakePhoto() {
		StartCoroutine (TakePhotoThread ());
	}
	private IEnumerator TakePhotoThread()
	{
		yield return new WaitForEndOfFrame(); 

		Resources.UnloadUnusedAssets ();

		Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
		photo.SetPixels(webcamTexture.GetPixels());
		photo.Apply();

		#if UNITY_EDITOR
		photoTaken.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
		photoTaken.transform.Rotate (Vector3.forward * 90);
		rotated = true;

		photo = RotateMatrix(photo, false);
		#else 
		// Image photoTaken
		if (!isFrontCamera) {
		photoTaken.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
		if (rotated) {
		// compensate rotation done for the front camera
		rotated = false;
		photoTaken.transform.Rotate (Vector3.forward * 180);
		}
		} 
		else {
		photoTaken.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
		//photoTaken.transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, new Vector3 (0,180,0));
		photoTaken.transform.Rotate (Vector3.forward * -180);
		rotated = true;

		photo = FlipTexture(photo, false);
		}
		#endif



		//Encode to a PNG and Write out the PNG
		eventualPictureTaken = photo.EncodeToPNG();

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

			Debug.Log ("[ManageVotingChallenge] The saving path for the picture taken is: " + Application.persistentDataPath + "/SotSPhotos/photo.png");



			File.WriteAllBytes(Application.persistentDataPath + "/SotSPhotos/photo.png", eventualPictureTaken);
			Debug.Log ("[ManageVotingChallenge] File is saved. ");

			// Kill the webcam
			if (webcamTexture != null) {
				if (webcamTexture.isPlaying) {
					webcamTexture.Stop ();
					rearPictureCameraRawImage.gameObject.SetActive (false);
					frontPictureCameraRawImage.gameObject.SetActive (false);
					//cameraInitialized = false;
					cameraActive = false;
				}
			}

		}
		catch (IOException ex)
		{
			Debug.LogError (ex.Message);
		}

		//Destroy (photo);
	}
	/*
	 * This function rotates a Texture2D in 180 Degrees (pass the second argument as false)
	*/
	private Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
	{

		Texture2D flipped = new Texture2D(original.width, original.height);

		int xN = original.width;
		int yN = original.height;


		for (int i = 0; i < xN; i++)
		{
			for (int j = 0; j < yN; j++)
			{
				if (upSideDown)
				{
					flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
				}
				else
				{
					flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
				}
			}
		}
		flipped.Apply();

		return flipped;
	}
	/*
	 * This function rotates a Texture2D in 90º
	 * 
	 * Color32[] pixels = photo.GetPixels32();
	 * pixels = RotateMatrix(pixels, photo.width);
	 * photo.SetPixels32(pixels);
	*/
	private Texture2D RotateMatrix(Texture2D originalTexture, bool clockwise)
	{
		Color32[] original = originalTexture.GetPixels32();
		Color32[] rotated = new Color32[original.Length];
		int w = originalTexture.width;
		int h = originalTexture.height;

		int iRotated, iOriginal;

		for (int j = 0; j < h; ++j)
		{
			for (int i = 0; i < w; ++i)
			{
				iRotated = (i + 1) * h - j - 1;
				iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
				rotated[iRotated] = original[iOriginal];
			}
		}

		Texture2D rotatedTexture = new Texture2D(h, w);
		rotatedTexture.SetPixels32(rotated);
		rotatedTexture.Apply();
		return rotatedTexture;
	}

	public void AttachPictureToChallenge() {

		if (rotated) {
			// compensate rotation done for the front camera
			rotated = false;

			#if UNITY_EDITOR
				photoTaken.transform.Rotate (Vector3.forward * -90);
			#else 
				photoTaken.transform.Rotate (Vector3.forward * 180);
			#endif

		}

		StartCoroutine (AttachPictureToChallengeThread());

	}
	private bool retry = false;
	private IEnumerator AttachPictureToChallengeThread() {
		if (retry) {
			yield return new WaitForSeconds (3);
		}

		if (string.IsNullOrEmpty (nameOfAuthorPhotoTaken.text)) {
			string[] displayMessage1 = {"Please introduce a name you want this picture to have", "Geef een naam op die u aan deze foto wilt geven", "Por favor dá um nome que queres que esta imagem tenha" };
			_referenceDisplayManager.DisplayErrorMessage (displayMessage1[MasterManager.language]);
			feedbackMessagePanel.gameObject.SetActive (true);
			yield return new WaitForSeconds (3);
			feedbackMessagePanel.gameObject.SetActive (false);

			panels [3].SetActive (true);
			panels [4].SetActive (false);
			animateUploadingLabel = false;
			yield break;
		} else {
			string[] displayMessage2 = {"Uploading . . .", "Uploaden . . .", "A carregar . . ." };
			uploadingLabel.text = displayMessage2[MasterManager.language];
			animateUploadingLabel = true;
			StartCoroutine (AnimateLoadingLabel());
			panels [3].SetActive (false);	// panel to attach the name to the photo
			panels [4].SetActive (true);	// panel to show the uploading stage
			butons [0].SetActive (false);	// hide the go back button

			// further code: https://answers.unity.com/questions/1470602/unable-to-upload-image-to-server-with-multipartfor.html
			Debug.Log ("[ManageVotingChallenge] Let us send the picture to the server.");

			if (eventualPictureTaken != null) {
				string nameOfFile = MasterManager.activePlayFabId+"_"+nameOfAuthorPhotoTaken.text+".png";



				// get picture from file
				FileStream file;

				if (!File.Exists (Application.persistentDataPath + "/SotSPhotos/photo.png")) {
					file = File.Create (Application.persistentDataPath + "/SotSPhotos/photo.png");
					file.Close ();
				}

				Debug.Log ("[ManageVotingChallenge] Getting Picture taken at: " + Application.persistentDataPath + "/SotSPhotos/photo.png");



				eventualPictureTaken = File.ReadAllBytes(Application.persistentDataPath + "/SotSPhotos/photo.png");



				List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
				formData.Add (new MultipartFormFileSection ("file", eventualPictureTaken, nameOfFile, "image/png"));

				string url;
				// urlPrefix
				url = urlPrefix + "/api/uploadphoto/" + currentVotingChallengeBeingSolved._id + 
					"_"+MasterManager.activePlayFabId;

				Debug.Log ("[ManageVotingChallenge] URL used to save image:  " + url);

				UnityWebRequest www = UnityWebRequest.Post(url, formData);
				www.timeout = 10;
				yield return www.SendWebRequest();

				if(!String.IsNullOrEmpty(www.error)) {
					if (retry) {
						Debug.LogError("[ManageVotingChallenge] Error: " + www.error);
						string[] displayMessage3 = {"Something went wrong. Please try again", "Er is iets fout gegaan. Probeer het opnieuw", "Passou-se algo que não devia. Por favor, tenta de novo" };
						_referenceDisplayManager.DisplayErrorMessage (displayMessage3[MasterManager.language]);
						feedbackMessagePanel.gameObject.SetActive (true);
						yield return new WaitForSeconds (3);
						feedbackMessagePanel.gameObject.SetActive (false);

						panels [3].SetActive (true);	// panel to attach the name to the photo
						panels [4].SetActive (false);	// panel to show the uploading stage
						butons [0].SetActive (true);	// hide the go back button
						// change the screen now
						animateUploadingLabel = false;
						retry = false;
					} else {
						retry = true;
						StartCoroutine (AttachPictureToChallengeThread());
						yield return null;
					}

				}
				else {
					eventualPictureTaken = null;

					Debug.Log ("[ManageVotingChallenge] Form upload complete! Results: " + www.downloadHandler.text);
					// [ManageVotingChallenge] Form upload complete! Results: true
					// show original voting challenge window
					for (int i = 0; i < panels.Count; i++) {
						panels [i].SetActive (false);
					}

					Resources.UnloadUnusedAssets ();

					// you uploaded the challenge with a picture. Now, reload all votes, and update the icons on the map
					StartCoroutine (RefreshCurrentChallengeBeingSolved());
					// Signal that you updated a new picture
					//updatePictureOfThisPlayer = true;
					//StartCoroutine(PrepareToShowSolutionsOfVotingChallengeInBackground());

					// and now, show the different buttons, to allow for the players to see other players' photos
					// 5 - Show Take Picture Panel 2 (initial button to take picture, should only appear when you didn't solve the challenge)
					// 6 - Show All Pictures Panel 4 (all pictures, should only appear when you solved the
					panels[5].SetActive(false);
					panels[6].SetActive(true);



					if (string.Compare (www.downloadHandler.text, "true") == 0) {
						string[] displayMessage4 = {"Uploaded.", "Geüpload.", "Pronto." };
						_referenceDisplayManager.DisplaySystemMessage (displayMessage4[MasterManager.language]);
						feedbackMessagePanel.gameObject.SetActive (true);
						yield return new WaitForSeconds (1);
						string[] displayMessage5 = {"You just uploaded your picture in this challenge!", "Je hebt zojuist je foto geüpload in deze uitdaging!", "Acabaste de carregar a tua imagem para este desafio!" };
						_referenceDisplayManager.DisplaySystemMessage (displayMessage5[MasterManager.language]);
						yield return new WaitForSeconds (3);
						string[] displayMessage6 = {"You can now vote pictures of other players :D", "U kunt nu op foto's van andere spelers stemmen: D", "Agora podes votar nas imagens de outros jogadores :D" };
						_referenceDisplayManager.DisplaySystemMessage (displayMessage6[MasterManager.language]);
						yield return new WaitForSeconds (3);
						feedbackMessagePanel.gameObject.SetActive (false);

						// change the screen now
						animateUploadingLabel = false;

						// mark the challenge as solved
						StartCoroutine(SolveChallenge(currentVotingChallengeBeingSolved));

						MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Voting Challenge \"" + 
							currentVotingChallengeBeingSolved.name + "\" ["+currentVotingChallengeBeingSolved._id+"] by uploading picture.");

					} else {
						string[] displayMessage7 = {"Something went wrong: " + www.downloadHandler.text, "Er is iets fout gegaan: "+ www.downloadHandler.text, "Alguma coisa correu mal: "+ www.downloadHandler.text};
						_referenceDisplayManager.DisplayErrorMessage (displayMessage7[MasterManager.language]);
						feedbackMessagePanel.gameObject.SetActive (true);
						yield return new WaitForSeconds (3);
						feedbackMessagePanel.gameObject.SetActive (false);

						panels [3].SetActive (true);	// panel to attach the name to the photo
						panels [4].SetActive (false);	// panel to show the uploading stage
						butons [0].SetActive (true);	// hide the go back button

						// change the screen now
						animateUploadingLabel = false;
					}
							/*StringBuilder sb = new StringBuilder();
					foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
					{
						sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
					}

					// Print Headers
					Debug.Log("sb.tostring: " + sb.ToString());

					// Print Body
					Debug.Log("downloadHandler.text:  " + www.downloadHandler.text);*/
				}
			}



			yield return null;
		}
	}

	public void InvertCamera () {

		//rotated = true;

		Debug.Log ("Number of Cameras: " + WebCamTexture.devices.Length);
		//int index;
		//bool orientationFront = false;
		// find the camera we are using from our list
		/*for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
			Debug.Log ("Camera name: " + WebCamTexture.devices[cameraIndex].name);
			if (string.Compare(WebCamTexture.devices[cameraIndex].name, nameOfCameraFeedUsed) == 0) {
		//		index = cameraIndex;
				// is our camera facing the screen direction?
				orientationFront = WebCamTexture.devices [cameraIndex].isFrontFacing;	
				break;
			}	
		}*/

		// if we found it, see if there are other ones you can use
		// do we have more cameras? 
		if ((WebCamTexture.devices.Length - 1) > 0) {
			// if you have more cameras, try to find the first one opposite to the one you are using

			for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
				Debug.Log ("Camera name: " + WebCamTexture.devices[cameraIndex].name);
				if ((string.Compare(WebCamTexture.devices[cameraIndex].name, nameOfCameraFeedUsed) != 0) && (WebCamTexture.devices [cameraIndex].isFrontFacing != isFrontCamera)) {
					// then, change for this one and break
					//WebCamTexture.devices[cameraIndex].name, Screen.width, Screen.height
					webcamTexture.Stop();
					cameraActive = false;
					isFrontCamera = WebCamTexture.devices [cameraIndex].isFrontFacing;

					/*
					if (isFrontCamera) {
						StartCoroutine (InitializeCameraThread (WebCamTexture.devices [cameraIndex].name, Screen.width, Screen.height));
					} else {
						StartCoroutine (InitializeCameraThread ());
					}*/
					StartCoroutine (InitializeCameraThread (WebCamTexture.devices [cameraIndex].name, Screen.width, Screen.height));

					break;
				}
			}

		} else {
			// if we do not have more cameras, then keep the one you have
			Debug.Log("[ManageVoting] There is no other camera to exchange for.");
		}

	}

	void Update() {
		if (cameraActive) {
			#if UNITY_EDITOR
			transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, new Vector3(0,1,0));
			#else 
			transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);
			#endif

		}
	}

	private IEnumerator AnimateLoadingLabel () {
		while (animateUploadingLabel) {
			if (string.Compare (uploadingLabel.text, "Uploading . . .") == 0) {
				string[] displayMessage1 = {"Uploading ", 
											"Uploaden ", 
											"A carregar "};
				uploadingLabel.text = displayMessage1[MasterManager.language];
				yield return new WaitForSeconds (0.5f);
			} else if ( (string.Compare (uploadingLabel.text, "Uploading ") == 0) || (string.Compare (uploadingLabel.text, "Uploaden ") == 0) || (string.Compare (uploadingLabel.text, "A carregar ") == 0) ) {
				string[] displayMessage2 = {"Uploading . ", 
					"Uploaden . ", 
					"A carregar . "};
				uploadingLabel.text = displayMessage2[MasterManager.language];
				yield return new WaitForSeconds (0.5f);
			} else if ( (string.Compare (uploadingLabel.text, "Uploading . ") == 0) || (string.Compare (uploadingLabel.text, "Uploaden . ") == 0) || (string.Compare (uploadingLabel.text, "A carregar . ") == 0) ) {
				string[] displayMessage3 = {"Uploading . . ", 
					"Uploaden . . ", 
					"A carregar . . "};
				uploadingLabel.text = displayMessage3[MasterManager.language];
				yield return new WaitForSeconds (0.5f);
			} else if ( (string.Compare (uploadingLabel.text, "Uploading . . ") == 0) || (string.Compare (uploadingLabel.text, "Uploaden . . ") == 0) || (string.Compare (uploadingLabel.text, "A carregar . . ") == 0) ){
				string[] displayMessage4 = {"Uploading . . .", 
					"Uploaden . . .", 
					"A carregar . . ."};
				uploadingLabel.text = displayMessage4[MasterManager.language];
				yield return new WaitForSeconds (0.5f);
			} else {
				string[] displayMessage5 = {"Uploading . ", 
					"Uploaden . ", 
					"A carregar . "};
				uploadingLabel.text = displayMessage5[MasterManager.language];
				yield return new WaitForSeconds (0.5f);
			}
		}

		yield break;
	}

	private IEnumerator SolveChallenge (VotingChallengeInfo challenge) {

		challenge.solved = true;

		for (int i = 0; i < GameManager.instance.votingChallengesOnScreenMeteor.Count; i++) {
			if (string.Compare (GameManager.instance.votingChallengesOnScreenMeteor [i]._id, challenge._id) == 0) {
				GameManager.instance.votingChallengesOnScreenMeteor [i].solved = true;
				break;
			}
		}

		// ****************************************************************
		// attempt to remove the extra unsolved challenge icon on the map
		// VotingChallenge  |   VotingChallengeSolved
		// ****************************************************************
		int index = -1; bool found = false;
		for (int i = GameManager.instance.objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
			if (GameManager.instance.objectsToDestroyOnLoadScene [i].CompareTag ("VotingChallenge")) {
				if (string.Compare (GameManager.instance.objectsToDestroyOnLoadScene [i].GetComponent<VotingChallengeInfo> ()._id, challenge._id) == 0) {
					index = i;
					found = true;
					break;
				}
			}
		}
		if (index == -1) {
			throw new UnityException ("[ManageVotingChallenge] The object of the selected Voting challenge was not found on the screen.");
		} else if (found) {
			Destroy (GameManager.instance.objectsToDestroyOnLoadScene[index]);
			GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (index);
		}

		Resources.UnloadUnusedAssets ();

		// generate the new icon as solved on the map
		GameManager.instance.GenerateVotingChallengeSolvedOnScreenMeteor (challenge);


		// attribute the points of solving the challenge
		StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge._id));

		yield break;
	}

	// DEPRECATED
	private IEnumerator VerifyPicturesAlreadyDownloaded() {

		Debug.Log ("Entered VerifyPicturesAlreadyDownloaded.");
		string url;
		bool found = true;
		// ask for the pictures stored in the server online
		url = urlPrefix + "/api/listphotosofchallenge/" + currentVotingChallengeBeingSolved._id;
		Debug.Log ("[ManageVotingChallenge] URL used to ask for all the images of this challenge:  " + url);

		//url = urlPrefix + "/api/listphotosofchallenge/" + currentVotingChallengeBeingSolved._id;
		//Debug.Log ("[ManageVotingChallenge] URL used to ask for all the images of this challenge:  " + url);

		UnityWebRequest www = UnityWebRequest.Get(url);
		www.timeout = 10;
		yield return www.SendWebRequest();
		if (!String.IsNullOrEmpty (www.error)) {
			Debug.Log ("[ManageVotingChallenge] Error while verifying whether the pictures already downloaded locally should be removed or not: " + www.error);
			//Resources.UnloadUnusedAssets ();

			//yield return null;
		} else {
			Debug.Log("[ManageVotingChallenge] Request with: " + url);
			Debug.Log("[ManageVotingChallenge] (VerifyPicturesAlreadyDownloaded) Received: " + www.downloadHandler.text);

			try {
				List<string> tmpResult=
					JsonWrapper.DeserializeObject<List<string>>(www.downloadHandler.text);
				string tmpFileNameNoExtension;

				List<int> indexesToRemove = new List<int> ();
				// for each file stored locally, you should have a match with a file stored in the server online
				for (int i = 0; i < authorsOfSolutions.Count; i++) {
					found = false;

					// for each file received from the server
					for (int j = 0; j < tmpResult.Count; j++) {
						// compare the playfabIDs of the pictures stored (for the sake of robustness)
						tmpFileNameNoExtension = tmpResult[j].Split('.')[0];
						Debug.Log ("Comparing " + authorsOfSolutions [i].Split('_')[0] + " with " + tmpFileNameNoExtension.Split('_')[0]);

						if (string.Compare (authorsOfSolutions [i].Split('_')[0], tmpFileNameNoExtension.Split('_')[0]) == 0) {
							found = true;
							break;
						}
					}
					if (!found) {
						// if the file registered locally is not in the server anymore, delete it
						indexesToRemove.Add (i);
						//Debug.LogError ("Remove File: " + authorsOfSolutions[i]);
					}

				}
				// remove pictures from list in an inverted order
				for (int k = indexesToRemove.Count -1; k >= 0; k--) {
					imageSolutions.RemoveAt (indexesToRemove[k]);
					authorsOfSolutions.RemoveAt (indexesToRemove[k]);
					votingChallengeIDsOfSolutions.RemoveAt(indexesToRemove[k]);
					// remove the icon generated in the menu of solutions
					Destroy (listOfIcons[indexesToRemove [k]]);
					listOfIcons.RemoveAt (indexesToRemove [k]);
				}
			}
			catch (Exception e) {
				Debug.LogError ("[ManageVotingChallenge] Skipped the rest of the VerifyPicturesAlreadyDownloaded function. Poor JSON: " + e.Message);
			}

		}

		Resources.UnloadUnusedAssets ();

		yield return null;
	}

	public void ShowSolutionsOfVotingChallenge()
	{
		// before it was this:
		StartCoroutine (ShowSolutionsOfVotingChallengeThread());

		// now, change this for a process in the background
		//StartCoroutine(ShowSolutionsVotingChallengeWindow());
	}

	private IEnumerator ShowSolutionsOfVotingChallengeThread() {

		string url;

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " started to look at the Voting Challenge \"" + 
			currentVotingChallengeBeingSolved.name + "\" ["+currentVotingChallengeBeingSolved._id+"] solutions.");

		url = urlPrefix + "/api/listphotosofchallenge/" + currentVotingChallengeBeingSolved._id;
		Debug.Log ("[ManageVotingChallenge] URL used to ask for all the images of this challenge:  " + url);

		UnityWebRequest www = UnityWebRequest.Get(url);
		www.timeout = 10;
		yield return www.SendWebRequest();

		if (!String.IsNullOrEmpty (www.error)) {
			Debug.LogError ("[ManageVotingChallenge] Error: " + www.error);
			string[] displayMessage1 = {"Something went wrong: " + www.error, 
				"Er is iets fout gegaan: " + www.error, 
				"Passou-se alguma coisa de errado: " + www.error};
			_referenceDisplayManager.DisplayErrorMessage (displayMessage1[MasterManager.language]);
			feedbackMessagePanel.gameObject.SetActive (true);
			yield return new WaitForSeconds (3);
			feedbackMessagePanel.gameObject.SetActive (false);
		} else {
		
			//Debug.Log("[ManageVotingChallenge] Received: " + www.downloadHandler.text);
			List<string> tmpResult=
				JsonWrapper.DeserializeObject<List<string>>(www.downloadHandler.text);

			//Debug.Log("[ManageVotingChallenge] Parsed: ");
			for (int i = 0; i < tmpResult.Count; i++) {
				Debug.Log(tmpResult[i]);	
			}

			// ************************************************************
			// Great, now you have the list of all the pictures taken for 
			// this challenge. Now, you need to see the pictures you 
			// already have stored locally, and download only the missing
			// ones. The first time the player clicks on seeing the 
			// solutions, you get all the images; after that, you get
			// only the missing ones
			// ************************************************************

			// ************************************************************
			// create a list of files you're going to ask the server to 
			// stream
			// ************************************************************
			string [] tmpFileNameNoExtension;
			//List <string> listOfFilesToRequestToServer = new List<string> (); // with extensions on purpose
			bool found;
			string tmpRequestedFile;
			bool interruptProcess = false;
			panels [7].SetActive (true);	// set the solutions panel active
			string [] displayMessage2 = {"Downloading 0/" + tmpResult.Count + " images",
				"0/" + tmpResult.Count + " Afbeeldingen downloaden",
				"A carregar 0/" + tmpResult.Count + " imagens"};
			_referenceDisplayManager.DisplaySystemMessage (displayMessage2[MasterManager.language]);
			feedbackMessagePanel.gameObject.SetActive (true);
			Texture2D texture;
			Rect rec;
			Sprite img;
			GameObject genericIconGenerated;
			Button tmpButtonToPutImage;
			Text buttonAuthorNameLabel;
			/*
				public List<Sprite> imageSolutions;
				public List<string> authorsOfSolutions;
			*/

			for (int i = 0; (i < tmpResult.Count) && (!interruptProcess); i++) {
				string [] displayMessage3 = {"Downloading " + i + "/" + tmpResult.Count + " images",
					"" + i + "/" + tmpResult.Count + " Afbeeldingen downloaden",
					"A carregar " + i + "/" + tmpResult.Count + " imagens"};
				_referenceDisplayManager.DisplaySystemMessage (displayMessage3[MasterManager.language]);

				tmpFileNameNoExtension = tmpResult [i].Split ('.');
				//Debug.Log ("[ManageVotingChallenge] Potential file to add: " + tmpFileNameNoExtension[0]);

				found = false;
				for (int j = 0; j < authorsOfSolutions.Count; j++) {
					if (string.Compare (tmpFileNameNoExtension [0], authorsOfSolutions [j]) == 0) {
						if (string.Compare (currentVotingChallengeBeingSolved._id, votingChallengeIDsOfSolutions [j]) == 0) {	// only mark as found if name of pic is same, and they belong to same challenge
							// the player might want to put the same name across all pictures
							//listOfIcons[j].SetActive(false);	// also disable the icon being shown
							found = true;
							break;
						}
					}
				}
				if (!found) {
					
					Debug.Log ("[ManageVotingChallenge] Downloading " + tmpResult [i] + " from the server...");
					//listOfFilesToRequestToServer.Add (tmpResult [i]); // add file name with extension, to ask it in server
					tmpRequestedFile = tmpResult [i].Replace (" ", "kzktsz");
					tmpRequestedFile = WWW.EscapeURL(tmpRequestedFile);

					// urlPrefix
					// example: http://secretsofthesouth.tbm.tudelft.nl/api/downloadimage/DzTiA3xP7bXsbgbpt_E9C1620AF8977C96_Hendrik.png
					url = urlPrefix + "/api/downloadimage/" + currentVotingChallengeBeingSolved._id + "_" + tmpRequestedFile;

					Debug.Log ("[ManageVotingChallenge] URL used to ask for the image " + tmpResult [i] +" is:  " + url);

					using (WWW wwwReq = new WWW(url))
					{
						float timer = 0;
						float timeOut = 10;
						bool failed = false;

						while (!wwwReq.isDone)
						{
							if (timer > timeOut) { failed = true; break; }
							timer += Time.deltaTime;
							yield return null;
						}
						if (failed || !string.IsNullOrEmpty (wwwReq.error)) {
							wwwReq.Dispose ();
							yield break;
						} else {
							if ((wwwReq.texture == null) || (wwwReq.texture.width == 0) || (wwwReq.texture.height == 0)) {
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
								} /*else {
									texture = wwwDefault.texture;
								}*/
								//yield return wwwDefault;

								texture = wwwDefault.texture;
								wwwDefault.Dispose ();

								Debug.Log("[ManageVoting] could not load the picture. loading a standard one instead");
							} else {
								// assign texture
								texture = wwwReq.texture;
								Debug.Log("[ManageVoting] Picture loaded ok.");

							}	
						}
						// Wait for download to complete
						//yield return wwwReq;


						wwwReq.Dispose ();
					}

					rec = new Rect(0, 0, texture.width, texture.height);
					//tmpImageToPutImageSprite = GetComponent<Image>();
					img = Sprite.Create(texture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
					//tmpImageToPutImageSprite.sprite = img;
					//temporaryImageHolder.sprite = img;

					// **********************************************************************
					// *********************** Generate Icon in the menu ********************
					// **********************************************************************
					// NumberOfVotes
					// AverageRating
					genericIconGenerated = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					genericIconGenerated.SetActive (true);
					genericIconGenerated.transform.SetParent (containerForIcons.transform);
					genericIconGenerated.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericIconGenerated.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					tmpButtonToPutImage = genericIconGenerated.GetComponentInChildren<Button> ();
					tmpButtonToPutImage.image.sprite = img;
					//GameObject.FindGameObjectsWithTag("Respawn");
					PictureDataToVote script = genericIconGenerated.GetComponent<PictureDataToVote> ();
					script.challengeBeingSolved = currentVotingChallengeBeingSolved;
					script.authorName = tmpFileNameNoExtension [0].Split ('_')[1];
					if (currentVotingChallengeBeingSolved.listOfImagesAndVotes != null) {
						int index = -1;
						for (int k = 0; k < currentVotingChallengeBeingSolved.listOfImagesAndVotes.Count; k++) {
							// tmpFileNameNoExtension [0]
							// playfabid_votes_...
							if (string.Compare (tmpFileNameNoExtension [0].Split ('_') [0], currentVotingChallengeBeingSolved.listOfImagesAndVotes [k].Split ('_') [0]) == 0) {
								index = k;
								break;
							}
						}
						string[] tmpArray;
						float tmpAverage = 0;
						if (index < 0) {
							script.numberOfVotes = 0.ToString();
							script.averageRating = 0.ToString();
						} else {
							tmpArray = currentVotingChallengeBeingSolved.listOfImagesAndVotes [index].Split ('_');
							script.numberOfVotes = tmpArray [1];

							if (int.Parse (script.numberOfVotes) > 0) {
								//Debug.LogError ("Value: " + tmpAverage);
								script.numberOfVotes = tmpArray [1];
								//Debug.LogError ("Number of Votes: " + script.numberOfVotes);
								//Debug.LogError ("Summing: " + (1 * int.Parse (tmpArray [5]))+" - " + (2 * int.Parse (tmpArray [7]))+" - " + (3 * int.Parse (tmpArray [9]))+" - " + (4 * int.Parse (tmpArray [11]))+" - " + (5 * int.Parse (tmpArray [13])));
								tmpAverage = (1 * int.Parse (tmpArray [5])) + (2 * int.Parse (tmpArray [7])) + (3 * int.Parse (tmpArray [9])) + (4 * int.Parse (tmpArray [11])) + (5 * int.Parse (tmpArray [13]));
								//Debug.LogError ("Value after sum: " + tmpAverage);
								tmpAverage = tmpAverage / int.Parse (script.numberOfVotes);
								//Debug.LogError ("Value after division: " + tmpAverage);
								tmpAverage = (float)System.Math.Round (tmpAverage, 1);
								//Debug.LogError ("Value after rounding: " + tmpAverage);
								script.averageRating = tmpAverage.ToString ();
							} else {
								script.numberOfVotes = 0.ToString();
								script.averageRating = 0.ToString();
							}
						}
					
					} else {
						script.numberOfVotes = 0.ToString();
						script.averageRating = 0.ToString();
					}



					script.img = img;
					script.fileNameNoExtension = tmpFileNameNoExtension [0];
					tmpButtonToPutImage.onClick.AddListener(() => ShowPictureBeforeVoting (script));

					buttonAuthorNameLabel = genericIconGenerated.GetComponentInChildren<Text> ();
					buttonAuthorNameLabel.text = tmpFileNameNoExtension [0].Split ('_')[1];

					Text [] texts = genericIconGenerated.gameObject.GetComponentsInChildren<Text> ();
					for (int z = 0; z < texts.Length; z++) {
						if (string.Compare (texts [z].tag, "NumberOfVotes") == 0) {
							texts [z].text = script.numberOfVotes;
						}
						if (string.Compare (texts [z].tag, "AverageRating") == 0) {
							texts [z].text = script.averageRating;
						}
					}
						

					/*
					ChallengeIconGUIScript scriptOfTheObject = genericChallengeIcon.GetComponent<ChallengeIconGUIScript> ();
					//Texture2D SpriteTexture;


					scriptOfTheObject.challengeTitle.text = GameManager.instance.challengesOnScreenMeteor [i].challenge_name;
					scriptOfTheObject.challengeDistance.text = "" + GameManager.instance.challengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheObject.challenge = GameManager.instance.challengesOnScreenMeteor [i];
					scriptOfTheObject.challengeImage.sprite = GameManager.challengeImages[i];
					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.challengesOnScreenMeteor [i]);*/

					listOfIcons.Add (genericIconGenerated);

					// Finaly, add this picture to the list of files downloaded plus their authors
					authorsOfSolutions.Add(tmpFileNameNoExtension [0]);
					imageSolutions.Add (img);
					votingChallengeIDsOfSolutions.Add (currentVotingChallengeBeingSolved._id);

				} else {Debug.Log ("[ManageVotingChallenge] File is already local: " + tmpResult[i]);}
			}
				
			feedbackMessagePanel.gameObject.SetActive (false);
		}

		// guarantee that you only see the icons relevant to this voting challenge
		for (int j = 0; j < authorsOfSolutions.Count; j++) {
			if (string.Compare (currentVotingChallengeBeingSolved._id, votingChallengeIDsOfSolutions [j]) != 0) {	// only mark as found if name of pic is same, and they belong to same challenge
				// the player might want to put the same name across all pictures
				listOfIcons [j].SetActive (false);	// also disable the icon being shown

			} else {
				listOfIcons [j].SetActive (true);	// also disable the icon being shown
			}
		}

		Resources.UnloadUnusedAssets ();

		yield break;
	}

	public void ShowPictureBeforeVoting (PictureDataToVote script){
		pictureBeingVotedFor = script;

		imageHolderVotePanel.sprite = script.img;
		nameOfAuthor.text = script.authorName;
		numberOfVotes.text = script.numberOfVotes;
		averageRating.text = script.averageRating;

		// Decide whether you want to allow the player to vote in this picture or not.
		// H/She should not be able to vote in his/her own picture
		//butons[1]
		// .Split ('_')[1];
		if (string.Compare (script.fileNameNoExtension.Split ('_') [0], MasterManager.activePlayFabId) == 0) {
			// then this picture belongs to the current player. Do not allow him/her to vote
			butons [1].SetActive (false);
		} else {

			// now, verify if you already voted for this picture in this challenge. You should only vote once per pic
			// listOfChallengesAlreadyVotedFor should hold the list of challenge_pictures already voted for.
			// e.g.:  CMCY8NAxEDRigaJ2f_F9C1620AF8977C96
			bool found = false;
			string tmp = currentVotingChallengeBeingSolved._id + "_" + script.fileNameNoExtension.Split('_')[0];
			for (int i = 0; i < listOfChallengesAlreadyVotedFor.Count; i++) {
				if (string.Compare (tmp, listOfChallengesAlreadyVotedFor[i]) == 0) {
					found = true;
					break;
				}
			}
			if (!found) {
				butons[1].SetActive(true);
			} else {
				butons[1].SetActive(false);
			}


		}

		panels[8].SetActive(true);

		Resources.UnloadUnusedAssets ();
	}

	// http://secretsofthesouth.tbm.tudelft.nl/api/voteonvotingchallenge?challengeid=CMCY8NAxEDRigaJ2f&playfabID=F9C1620AF8977C96&vote=5
	public void VoteChallenge() { 
		// the pictureBeingVotedFor is set with the function "ShowPictureBeforeVoting", so use this function beforehand
		StartCoroutine (VoteChallengeThread(pictureBeingVotedFor.fileNameNoExtension));
	}
	private IEnumerator VoteChallengeThread(string fileNameNoExtension) {
		


		string targetPlayFabID = fileNameNoExtension.Split ('_') [0];
		string tmpChallengeBeingVotedFor = currentVotingChallengeBeingSolved._id + "_" + targetPlayFabID;
		// now, make the actual vote
		string url;
		switch (voteGivenToPicture) {
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		default: 
			voteGivenToPicture = 0;
			break;
		}

		UnityWebRequest request;
		url = urlPrefix + "/api/voteonvotingchallenge?challengeid=" + currentVotingChallengeBeingSolved._id +
			"&playfabID=" + targetPlayFabID + "&vote=" + voteGivenToPicture;
		request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();
		if (request.isNetworkError) {
			Debug.LogError ("[ManageVotingChallenge] Error While trying to vote: " + request.error);
			Debug.LogError ("[ManageVotingChallenge] URL: " + url);
		} else {
			if (string.Compare (request.downloadHandler.text, "true") == 0) {
				listOfChallengesAlreadyVotedFor.Add (tmpChallengeBeingVotedFor);

				StartCoroutine (RefreshCurrentChallengeBeingSolved ());
				//StartCoroutine (PrepareToShowSolutionsOfVotingChallengeInBackground());
				//yield return RefreshCurrentChallengeBeingSolved ();

				MasterManager.LogEventInServer (MasterManager.activePlayerName + " voted for a picture in the Voting Challenge \"" + 
					currentVotingChallengeBeingSolved.name + "\" ["+currentVotingChallengeBeingSolved._id+"]. Picture: " + tmpChallengeBeingVotedFor);

				Debug.Log ("[ManageVotingChallenge] Successful vote.");
				string [] displayMessage1 = {"Successful vote.",
					"Succesvol gestemd.",
					"Voto enviado."};
				_referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language]);
				feedbackMessagePanel.gameObject.SetActive (true);
				yield return new WaitForSeconds (3);
				feedbackMessagePanel.gameObject.SetActive (false);

				panels [8].SetActive (false);
				panels [9].SetActive (false);
				// 2 - Star 1
				// 3 - Star 1 Color
				// 4 - Star 2
				// 5 - Star 2 Color
				// 6 - Star 3
				// 7 - Star 3 color
				// 8 - Star 4
				// 9 - Star 4 Color
				// 10 - Star 5
				// 11 - Star 5 Color
				// reset buttons active
				butons[2].SetActive(true);
				butons[4].SetActive(true);
				butons[6].SetActive(true);
				butons[8].SetActive(true);
				butons[10].SetActive(true);
				butons[3].SetActive(false);
				butons[5].SetActive(false);
				butons[7].SetActive(false);
				butons[9].SetActive(false);
				butons[11].SetActive(false);

			} else {
				Debug.LogError ("[ManageVotingChallenge] Something went wronte. Result: " + request.downloadHandler.text);
				Debug.LogError ("[ManageVotingChallenge] URL used: " + url);
				string [] displayMessage2 = {"Could not vote. Please try again.",
					"Kon niet stemmen. Probeer het opnieuw.",
					"Não foi possível votar. Por favor, tenta de novo."};
				_referenceDisplayManager.DisplayErrorMessage (displayMessage2[MasterManager.language]);
				feedbackMessagePanel.gameObject.SetActive (true);
				yield return new WaitForSeconds (3);
				feedbackMessagePanel.gameObject.SetActive (false);
			}
		}

		voteGivenToPicture = -1;

		Resources.UnloadUnusedAssets ();

		yield break;
	}
		

	[Serializable]
	public class ListOfFilesJSON { 
		[SerializeField]
		//public List<string> list;
		public string filename;
	}

	/* ************************************************************************
	List all pictures of a challenge
	http://localhost:3000/api/listphotosofchallenge/CMCY8NAxEDRigaJ2f
	
	Status: 200  (ok)
	[
		"A9C1620AF8977C96_great stuff, hein?.png",
		"E9C1620AF8977C96_great stuff, hein?.png",
		"I9C1620AF8977C96_Carl.png",
		"X9C1620AF8977C96_lolita.png"
	] 
	Status: 500  (Internal Server Error)
		Error: ENOENT: no such file or directory, .../Users/Xavier/...VotingChallenges/CMCY8NAxEDRigaJ2/&#39


	************************************************************************ */

	/* ************************************************************************
	I need to create a function that 
		-> sends all the pictures to the game
		-> requests only a few pictures, because the game has already a few

	************************************************************************ */

}


