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
using System;

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



public class GameManager : MonoBehaviour {

	public GameToolkit.Localization.LocalizedText dismissPopup;


	// Have here one reference for every game object you wish to control.
	//public GPSLocationProvider_Xavier _referenceGPSLocationProvider_Xavier = null;
	//public GameObject _referenceMap;
	//[HideInInspector]
	public Camera _referenceCamera;
	public GameObject _referencePlayer;
	public DisplayManager _referenceDisplayManager;
	//public ModalWindowManager _referenceModalPanelOfChallenges;	// This is the generic dialogue window appearing when clicking on each challenge object
	public Quiz_ModalWindow _referenceQuiz_ModalPanel;

	/**
	 * Open Quiz Challenges related variables
	 * 
	 * 
	 */
	public ManageOpenQuizChallenge manageOpenQuizChallenge;
	public GameObject openQuizChallengeWindow;
	public GameObject openQuizChallengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject openQuizChallengesSolvedPrefab;
	public List <OpenQuizChallengeInfo> openQuizChallengesOnScreenMeteor = new List<OpenQuizChallengeInfo> (0);
	public static List < Sprite> openQuizChallengeImages = new List<Sprite>(0);

	/**
	 * Timed Task Challenges related variables
	 */
	public ManageTimedTaskChallenge manageTimedTaskChallenge;
	public GameObject timedTaskChallengeWindow;
	public GameObject timedTaskChallengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject timedTaskChallengesSolvedPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public List <TimedTaskChallengeInfo> timedTaskChallengesOnScreenMeteor = new List<TimedTaskChallengeInfo> (0);
	public static List < Sprite> timedTaskChallengeImages = new List<Sprite>(0);

	/**
	 * Voting Challenges related variables
	 */

	/**
	 * Hunter Challenges related variables
	 */
	public GameObject hunterChallengeWindow;
	public Hunter_window_manager hunterChallengeWindowManager;
	public ManageVotingChallenge manageVotingChallenge;
	public GameObject hunterChallengeQRFoundWindow;
	public Hunter_window_QRFound_manager hunterChallengeQRFoundWindowManager;
	public Multiplayer_ModalWindow _referenceMultiplayer_ModalPanel;
	//public AR_ModalWindow _referenceAR_ModalPanel;
	public bool isAnyWindowOpen = false; // this concerns the modal windows reacting to the 3D icons on the map. 
										// I want to guarantee that, when I am seeing a window, that my touches do not
										// interfere with the game for as much as possible
	public GameObject votingChallengeWindow;
	//public Voting_window_manager votingChallengeWindowManager;
	//public GameObject votingChallengeQRFoundWindow;
	//public Voting_window_QRFound_manager votingChallengeQRFoundWindowManager;
	public GameObject votingChallengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject votingChallengesSolvedPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public List <VotingChallengeInfo> votingChallengesOnScreenMeteor = new List<VotingChallengeInfo> (0);
	public static List < Sprite> votingChallengeImages = new List<Sprite>(0);



	public GameObject buttonWalkingNotWalking;
	public GameObject challengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject challengesSolvedPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject challengesHalfSolvedPrefab;
	public GameObject multiplayerChallengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject hunterChallengesPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject hunterChallengesSolvedPrefab; 		// this is to be able to dynamically instantiate challenges in code
	public GameObject messsagePanel;
	public Button menuButton;	// this is to call the 3rd scene with the list of all the challenges. I need this to deactivate and set auto compass
	public Button cancelAutoCompassRotation;	// this is to cancel the automatic rotation of the compass.

	// this is the script to attach to all the instantiated challenge assets
	public HandleTouchedObject challengeScriptHandler;
	// This will disappear. Just to simulate walking with a finger
	public Text debugingButtonWalkinigNotWalking;
	// objects to disable when loading other scenes
	public List <ChallengeInfo> challengesOnScreenMeteor = new List<ChallengeInfo> (0);
	public static bool challengeImageSpritesLoaded = false;
	public List <MultiplayerChallengeInfo> multiplayerChallengesOnScreenMeteor = new List<MultiplayerChallengeInfo> (0);
	public List <HunterChallengeInfo> hunterChallengesOnScreenMeteor = new List<HunterChallengeInfo> (0);

	public List <Challenge_ARData> arChallengesOnScreen = new List<Challenge_ARData>(0);	// legacy. Delete
	public List <Challenge_QuizData> quizChallengesOnScreen = new List<Challenge_QuizData>(0);// legacy. Delete

	public List <GameObject> objectsToSetActiveOnLoadScene = new List<GameObject>(0);
	public List <GameObject> objectsToDestroyOnLoadScene = new List<GameObject>(0);

	public static GameManager instance { set; get ; }
	private static GameManager singleton;	// This is a singleton
	public bool productionMode = false;
	[HideInInspector]
	public bool gameProperlyInitialized = false;



	public static List < Sprite> challengeImages = new List<Sprite>(0);
	public static List < Sprite> multiplayerChallengeImages = new List<Sprite>(0);
	public static List < Sprite> hunterChallengeImages = new List<Sprite>(0);


	//public Image imageFromModelPanel;

	// this is a bool to help commute from one scene to another (for instance, only disable cameras if everything is set up first)
	[HideInInspector]
	public bool isGameManagerLoaded = false;
	[HideInInspector]
	public bool isCameraInitialized = false;
	[HideInInspector]
	public bool isPlayerInitialized = false;
	[HideInInspector]
	public bool reloadChallengesFromCloud = false;
	[HideInInspector]
	public bool loadedImageFromServerForMenu = false;
	[HideInInspector]
	public Sprite playerImageForMenu;
	[HideInInspector]
	public int playerGamePermissions;
	[HideInInspector]
	public bool loadedGamePermissionsFromServer = false;

	void Start ()
	{
		if (singleton == null) 
		{
			singleton = this;
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = GameManager.singleton;

			// Production: do not consider the first one, because that is the cheating mode button



			// shall we remove the cheating button? if so, enable productionMode [DEPRECATED]
			productionMode = true;
			for (int i = 0; i < objectsToSetActiveOnLoadScene.Count; i++) {
				objectsToSetActiveOnLoadScene [i].gameObject.SetActive (false);
			}

			StartCoroutine (StartTheDisplayManagerProperly());

			// collect GPS Data about the Player
			//StartCoroutine (AndroidServiceSetup ());

			StartCoroutine (LoadGamePermissionsForPlayer());
		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}

		_referenceCamera.GetComponent<Camera> ().enabled = false;
		_referenceCamera.GetComponent<Camera> ().gameObject.SetActive (true);

		Time.timeScale = 1;
	}

	// wait the necessary time before using the display
	private IEnumerator StartTheDisplayManagerProperly()
	{
		
		// activate all elements on screen
		LoadObjectsFromGameSceneMeteor();

		if (MasterManager.instance != null) {
			Debug.Log ("[GameManager] Coordinates used for the set up of the objects are: [" +
				GPSLocationProvider_Xavier.instance.latlong.x + "  -  " +
				GPSLocationProvider_Xavier.instance.latlong.y + "]... ");
		} else {
			Debug.Log ("[GameManager] You started the scene only. You have no Map and no GPS provider.");
		}

		// Txt21
		string[] displayMessage1 = {"Loading challenges around you ...", "Laden van uitdagingen om je heen ...", "A carregar os desafios que estão à tua volta ..."};

		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		StartCoroutine (LoadChallengesFromTheServer());
		

		// panel for the messages
		if (_referenceDisplayManager == null) throw new System.Exception ("_reference display manager is null!");
		if (messsagePanel == null) throw new System.Exception ("_reference Canvas is null!");
		// Camera
		if (_referenceCamera == null) throw new System.Exception ("_reference Camera is null!");
		// Status Text
		if (challengeScriptHandler == null) throw new System.Exception ("[GameManager] reference to the challengeScriptHandler is null!");
		// modal window canvas
	//	if (_referenceAR_ModalPanel == null) throw new System.Exception ("[GameManager] reference to the AR Modal Panel is null!");

		if (_referenceQuiz_ModalPanel == null) throw new System.Exception ("[GameManager] reference to the Quiz Modal Panel is null!");

		if (_referenceMultiplayer_ModalPanel == null) throw new System.Exception ("[GameManager] reference to the Multiplayer Modal Panel is null!");

		//if (_referenceHunter_ModalPanel == null) throw new System.Exception ("[GameManager] reference to the Hunter Modal Panel is null!");

		if (hunterChallengeWindow == null) throw new System.Exception ("[GameManager] reference to the Hunter Window Canvas is null!");
		if (hunterChallengeWindowManager == null) throw new System.Exception ("[GameManager] reference to the Hunter Window manager inside the hunter canvas is null!");
		if (hunterChallengeQRFoundWindow == null) throw new System.Exception ("[GameManager] reference to the Hunter Window QR Found Canvas is null!");
		if (hunterChallengeQRFoundWindowManager == null) throw new System.Exception ("[GameManager] reference to the Hunter Window QR Found manager inside the hunter canvas is null!");

		if (votingChallengeWindow == null) throw new System.Exception ("[GameManager] reference to the Voting Window Canvas is null!");
		if (manageVotingChallenge == null) throw new System.Exception ("[GameManager] reference to the Manage Voting Window inside the voting canvas is null!");

		//if (timedTaskChallengeWindow == null) throw new System.Exception ("[GameManager] reference to the timedTask Window Canvas is null!");
		//if (manageTimedTaskChallenge == null) throw new System.Exception ("[GameManager] reference to the Manage timedTask Window inside the voting canvas is null!");

		// Walking / Not Walking Button
		//if (buttonWalkingNotWalking == null) {
		//	throw new System.Exception ("_reference button is null!");
		//}


		challengeScriptHandler.isProperlyInitialized = true;

		int count = 20;
		// guarantee you have the display manager before showing messages on screen
		while ((!_referenceDisplayManager.isProperlyInitialized) && (count > 0))
		{
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[GameManager] referenceDisplayManager is not Properly Initialized");}

		// maintain the order of initialization: 1) Player, 2) camera
		// guarantee you have the Camera Initialized before doing anything else
		count = 20;
		while ((!isPlayerInitialized) && (count > 0))
		{
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[GameManager] isPlayerInitialized is never true");}
		// guarantee you have the Camera Initialized before doing anything else
		count = 20;
		while ((!isCameraInitialized) && (count > 0))
		{
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[GameManager] isCameraInitialized is never true");}

		// set player in the correct coordinates, and update the camera with it.
		_referencePlayer.GetComponent<PlayerController> ().SetPlayer ();

		RangeAroundTransformTileProvider rangeTileAroundPlayer = 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap> ().GetComponent<RangeAroundTransformTileProvider>();
		rangeTileAroundPlayer._targetTransform = _referencePlayer.transform;

		// let other game objects that are dependent on having GameManager initialized to continue with their work
		isGameManagerLoaded = true;
		if (!MasterManager.properlyUnloaded) {
			MasterManager.instance.UnloadObjectsFromScene ();
		}
		if (!LoginMenuManager.properlyUnloaded) {
			LoginMenuManager.instance.UnloadObjectsFromScene ();
		}


		// Make sure you disable the (modal) panels of the challenges
		_referenceQuiz_ModalPanel.gameObject.SetActive(false);
		hunterChallengeWindow.SetActive(false);
		hunterChallengeQRFoundWindow.SetActive (false);
		votingChallengeWindow.SetActive(false);
		timedTaskChallengeWindow.SetActive(false);
		openQuizChallengeWindow.SetActive(false);



		//StartCoroutine(CollectPlayerDataGPS());

		Debug.Log ("[GameManager] GameManager is all set up for the player ID:  " + MasterManager.activePlayFabId);

		isAnyWindowOpen = false;

		gameProperlyInitialized = true;



		//OGPopup.MakeNotificationPopup ("This is the new popUp system", null);


		yield break;
	}
		
	private int frameCounter = 0, maxIterations = 2;
	void LateUpdate() {
	
		// each 30 frames, we verify whether we have extra objects we shouldn't have, and we repeat this for maxIterations
		if (maxIterations > 0)
		if (frameCounter < 30) {
			frameCounter++;
		} else {
			
			GameManager.instance.LoadObjectsFromGameSceneMeteor ();

			frameCounter = 0;
			maxIterations--;
		}
	}

	/**
	 * This is to create a WWW request timeout
	 *      IEnumerator DownloadFileWithTimeout(string URL)
     {
                         WWW www = new WWW(URL);
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
     }
	 */

	public IEnumerator LoadChallengesFromTheServer()
	{
		//OGLoadingOverlay.ShowFullcoverLoading("Loading challenges around you", true);

		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge Quiz Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.challengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.challengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// first, delete the list of challenges
		if (objectsToDestroyOnLoadScene.Count > 0) {
			for (int j = objectsToDestroyOnLoadScene.Count - 1; j >= 0; j--) {
				Destroy (objectsToDestroyOnLoadScene [j]);
			}
			objectsToDestroyOnLoadScene = new List<GameObject> ();

		}
		challengesOnScreenMeteor = new List <ChallengeInfo> ();
		challengeImages = new List<Sprite> ();

		string url = MasterManager.serverURL + "/api/challengesnearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		float timer = 0;
		float timeOut = 10;
		bool failed = false;


		request.timeout = 10;

		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<ChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<ChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);

			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
			MasterManager.challengesFromServerMeteor = new List<ChallengeInfo> (counterOfChallengesEnRoute);

			ChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					tmp = new ChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].question,
						results [i].answer,
						results [i].imageURL,
						results [i].validated,
						results [i].route,
						false);


					// Generate the challenge in the 3D world
					MasterManager.challengesFromServerMeteor.Add (tmp);
					challengesOnScreenMeteor.Insert (pointer, MasterManager.challengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.challengesFromServerMeteor [pointer].imageURL))
					{
						timer = 0;
						timeOut = 10;
						failed = false;

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
							timer = 0;
							timeOut = 10;
							failed = false;

							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
								while (!wwwDefault.isDone)
								{
									if (timer > timeOut) { failed = true; break; }
									timer += Time.deltaTime;
									yield return null;
								}
								//yield return wwwDefault;
								if (failed || !string.IsNullOrEmpty (wwwDefault.error)) {
									wwwDefault.Dispose ();
									yield break;
								} else {
									texture = wwwDefault.texture;
								}

							} else {
								// assign texture
								texture = www.texture;
							}

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							challengeImages.Insert (pointer, img);
						}
						// Wait for download to complete
						//yield return www;
					}
					challengeImageSpritesLoaded = true;

					Destroy (tmp);

					GenerateChallengeOnScreenMeteor (MasterManager.challengesFromServerMeteor [pointer]);
					pointer++;
				}

			}
		}

		if (MasterManager.instance.playerHasTeam) {
			yield return LoadMultiplayerChallengesFromTheServer ();
		} 

		yield return LoadHunterChallengesFromTheServer ();
		yield return LoadVotingChallengesFromTheServer ();
		yield return LoadTimedTaskChallengesFromTheServer ();
		yield return LoadOpenQuizChallengesFromTheServer ();

		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;
		//yield return null;



		// now, initiating the process of loading the player's avatar image 
		// StartCoroutine(LoadsAvatarImage(MasterManager.activePlayerAvatarURL, buttons[6]));


		Texture2D playerTexture;
		Debug.Log ("[GameManager] MasterManager.activePlayerAvatarURL is: " + MasterManager.activePlayerAvatarURL);

		if (MasterManager.activePlayerAvatarURL != null) {
		
			using (WWW www = new WWW (MasterManager.activePlayerAvatarURL)) {
				timer = 0;
				timeOut = 10;
				failed = false;

				while (!www.isDone) {
					if (timer > timeOut) {
						failed = true;
						break;
					}
					timer += Time.deltaTime;
					yield return null;
				}
				if (failed || !string.IsNullOrEmpty (www.error)) {
					www.Dispose ();
					yield break;
				} else {

					if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
						WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
						timer = 0;
						timeOut = 10;
						failed = false;

						while (!wwwDefault.isDone) {
							if (timer > timeOut) {
								failed = true;
								break;
							}
							timer += Time.deltaTime;
							yield return null;
						}
						if (failed || !string.IsNullOrEmpty (wwwDefault.error)) {
							wwwDefault.Dispose ();
							yield break;
						} else {
							playerTexture = wwwDefault.texture;
						}
						//yield return wwwDefault;

					} else {
						// assign texture
						playerTexture = www.texture;
					}
					www.Dispose ();

				}
				// Wait for download to complete
				//yield return www;


			}
		} else {
		
			WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
			timer = 0;
			timeOut = 10;
			failed = false;

			while (!wwwDefault.isDone) {
				if (timer > timeOut) {
					failed = true;
					break;
				}
				timer += Time.deltaTime;
				yield return null;
			}
			if (failed || !string.IsNullOrEmpty (wwwDefault.error)) {
				wwwDefault.Dispose ();
				yield break;
			} else {
				playerTexture = wwwDefault.texture;
			}
			wwwDefault.Dispose ();
		}


		Rect rec = new Rect(0, 0, playerTexture.width, playerTexture.height);
		//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);

		playerImageForMenu = Sprite.Create(playerTexture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);
		loadedImageFromServerForMenu = true;



		if (MasterManager.instance.gameIsLoading) {
			MasterManager.instance.gameIsLoading = false;

			//OGLoadingOverlay.StopAllLoaders();

			isAnyWindowOpen = true;

			if (string.IsNullOrEmpty (MasterManager.activePlayerName)) {
				string[] displayMessageTitle = {"Welcome to the Secrets of the South.", "Welkom in Secrets of the South.", "Bem vindo ao Secrets of the South."};
				string[] displayMessage1 = {"Finishing the setup for you ...", "De installatie wordt voor u voltooid ...", "A acabar de preparar o jogo para ti ..."};
				//_referenceDisplayManager.DisplaySystemMessage (displayMessage[MasterManager.language]);
				OGPopup.ShowNotificationPopup (displayMessageTitle[MasterManager.language],displayMessage1[MasterManager.language], ()=> {isAnyWindowOpen = false;});
			} else {
				string[] displayMessageTitle = {"Welcome to the Secrets of the South, " + MasterManager.activePlayerName + ".", 
					"Welkom in Secrets of the South, "  + MasterManager.activePlayerName + ".", 
					"Bem-vindo ao Secrets of the South, " + MasterManager.activePlayerName +"."};

				string[] displayMessage1 = {"Use your finger to move the camera around, and solve challenges to win points in your location.", 
					"Gebruik je vinger om de camera te verplaatsen en uitdagingen op te lossen om punten op je locatie te winnen.", 
					"Move a camara com o teu dedo, e soluciona desafios para ganhares pontos na tua localização."};
				//GameToolkit.Localization.LocalizedText close = new GameToolkit.Localization.LocalizedText ();

				//_referenceDisplayManager.DisplaySystemMessage (displayMessage[MasterManager.language]);
				OGPopup.GetInstance().closeButtonText = dismissPopup;
				OGPopup.ShowNotificationPopup (displayMessageTitle[MasterManager.language],displayMessage1[MasterManager.language], ()=> 
					{
						isAnyWindowOpen = false;

						MasterManager.LogEventInServer(MasterManager.activePlayerName + " started playing.");

						StartCoroutine(MasterManager.instance.StartRecordingEventsInServer());
					});
			}
		}

		OGLoadingOverlay.StopAllLoaders ();
		Resources.UnloadUnusedAssets ();

		// load as well the hunter challenges, as they can be solved by one person
		//yield return LoadHunterChallengesFromTheServer ();
		yield return null;
	}


	// /api/challenges1nearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
	public IEnumerator LoadMultiplayerChallengesFromTheServer()
	{
		//OGLoadingOverlay.ShowFullcoverLoading("Loading multiplayer challenges around you", true);

		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge Quiz Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.multiplayerChallengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.multiplayerChallengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// MasterManager.desiredMaxDistanceOfChallenges, GPSLocationProvider_Xavier.instance.latlong

		multiplayerChallengesOnScreenMeteor = new List <MultiplayerChallengeInfo> ();
		multiplayerChallengeImages = new List<Sprite> ();


		// ChallengeDBFormat_JSON
		//MasterManager.serverURL
		string url = MasterManager.serverURL + "/api/challenges1nearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);

		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] Multiplayer challenges Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<MultiplayerChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<MultiplayerChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);

			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
				
			MasterManager.multiplayerChallengesFromServerMeteor = new List<MultiplayerChallengeInfo> (counterOfChallengesEnRoute);

			MultiplayerChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					tmp = new MultiplayerChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].task,
						results [i].imageURL,
						results [i].validated,
						results [i].route,
						false);




					// Generate the challenge in the 3D world
					MasterManager.multiplayerChallengesFromServerMeteor.Add (tmp);
					multiplayerChallengesOnScreenMeteor.Insert (pointer, MasterManager.multiplayerChallengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.multiplayerChallengesFromServerMeteor [pointer].imageURL))
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
							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
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

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							multiplayerChallengeImages.Insert (pointer, img);
						}
						// Wait for download to complete
						//yield return www;


						challengeImageSpritesLoaded = true;
					}


					// now, create images once, for each received challenge


					Destroy (tmp);

					GenerateMultiplayerChallengeOnScreenMeteor (MasterManager.multiplayerChallengesFromServerMeteor [pointer]);
					pointer++;
				}

			}
		}

		//OGLoadingOverlay.StopAllLoaders ();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;
	}


	// /api/challenges1nearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
	public IEnumerator LoadHunterChallengesFromTheServer()
	{

		//OGLoadingOverlay.ShowFullcoverLoading("Loading hunter challenges around you", true);
		List<HunterChallengeInfo> tmpfilesSolvedInThisGameSession = new List<HunterChallengeInfo>();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge Hunter Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.hunterChallengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.hunterChallengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// keeping track on which challenges were solved or not
		bool copySolvedChallenges = false;
		if (hunterChallengesOnScreenMeteor != null) {
			if (hunterChallengesOnScreenMeteor.Count > 0) {
				copySolvedChallenges = true;
				//tmpfilesSolvedInThisGameSession = new List<HunterChallengeInfo> (hunterChallengesOnScreenMeteor.Count);		
				for (int i = 0; i < hunterChallengesOnScreenMeteor.Count; i++) {
					tmpfilesSolvedInThisGameSession.Add (hunterChallengesOnScreenMeteor [i]);
				}
			}
		}


		hunterChallengesOnScreenMeteor = new List <HunterChallengeInfo> ();
		hunterChallengeImages = new List<Sprite> ();


		// ChallengeDBFormat_JSON
		//MasterManager.serverURL
		string url = MasterManager.serverURL + "/api/challengeshunternearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending this, in asking for hunter challenges in server: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] Hunter challenges Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<HunterChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<HunterChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);


			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
			// should be ChallengeInfo
			MasterManager.hunterChallengesFromServerMeteor = new List<HunterChallengeInfo> (counterOfChallengesEnRoute);

			// should be ChallengeInfo
			HunterChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			bool isItSolved = false;
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					isItSolved = false;

					for (int k = 0; k < tmpfilesSolvedInThisGameSession.Count; k++) {

						if (string.Compare (tmpfilesSolvedInThisGameSession [k]._id, results [i]._id) == 0) {
							isItSolved = tmpfilesSolvedInThisGameSession [k].solved;
							break;
						}
					}


					tmp = new HunterChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].question,
						results [i].answer,
						results [i].imageURL,
						results [i].content_text,
						results [i].content_picture,
						results [i].validated,
						results [i].route,
						isItSolved);




					// Generate the challenge in the 3D world
					MasterManager.hunterChallengesFromServerMeteor.Add (tmp);
					hunterChallengesOnScreenMeteor.Insert (pointer, MasterManager.hunterChallengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.hunterChallengesFromServerMeteor [pointer].imageURL))
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
							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
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

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							hunterChallengeImages.Insert (pointer, img);

						}
						// Wait for download to complete
						//yield return www;


						challengeImageSpritesLoaded = true;
					}


					// now, create images once, for each received challenge

					Destroy (tmp);

					if (copySolvedChallenges) {
						if (MasterManager.hunterChallengesFromServerMeteor [pointer].solved) {
							GenerateHunterChallengeSolvedOnScreenMeteor (MasterManager.hunterChallengesFromServerMeteor [pointer]);
						} else {
							GenerateHunterChallengeOnScreenMeteor (MasterManager.hunterChallengesFromServerMeteor [pointer]);
						}


					} else {
						GenerateHunterChallengeOnScreenMeteor (MasterManager.hunterChallengesFromServerMeteor [pointer]);
					}
					pointer++;
				}


			}
		}

		//OGLoadingOverlay.StopAllLoaders ();


		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;

		// reboot LateUpdate iterations
		maxIterations = 2;
		frameCounter = 0;
	}

	// /api/challengesvotingnearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
	public IEnumerator LoadVotingChallengesFromTheServer()
	{
		//OGLoadingOverlay.ShowFullcoverLoading("Loading voting challenges around you", true);
		List<VotingChallengeInfo> tmpfilesSolvedInThisGameSession = new List<VotingChallengeInfo>();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge Voting Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.votingChallengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.votingChallengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// keeping track on which challenges were solved or not
		bool copySolvedChallenges = false;
		if (votingChallengesOnScreenMeteor != null) {
			if (votingChallengesOnScreenMeteor.Count > 0) {
				copySolvedChallenges = true;
				for (int i = 0; i < votingChallengesOnScreenMeteor.Count; i++) {
					tmpfilesSolvedInThisGameSession.Add (votingChallengesOnScreenMeteor [i]);
				}
			}
		}


		votingChallengesOnScreenMeteor = new List <VotingChallengeInfo> ();
		votingChallengeImages = new List<Sprite> ();


		// ChallengeDBFormat_JSON
		//MasterManager.serverURL
		string url = MasterManager.serverURL + "/api/challengesvotingnearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending this, in asking for voting challenges in server: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] Voting challenges Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<VotingChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<VotingChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);


			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
			// should be ChallengeInfo
			MasterManager.votingChallengesFromServerMeteor = new List<VotingChallengeInfo> (counterOfChallengesEnRoute);

			// should be ChallengeInfo
			VotingChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			bool isItSolved = false;
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					isItSolved = false;

					for (int k = 0; k < tmpfilesSolvedInThisGameSession.Count; k++) {

						if (string.Compare (tmpfilesSolvedInThisGameSession [k]._id, results [i]._id) == 0) {
							isItSolved = tmpfilesSolvedInThisGameSession [k].solved;
							break;
						}
					}


					tmp = new VotingChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].task,
						results [i].imageURL,
						results [i].listOfImagesAndVotes,
						results [i].validated,
						results [i].route,
						isItSolved);




					// Generate the challenge in the 3D world
					MasterManager.votingChallengesFromServerMeteor.Add (tmp);
					votingChallengesOnScreenMeteor.Insert (pointer, MasterManager.votingChallengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.votingChallengesFromServerMeteor [pointer].imageURL))
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

							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
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

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							votingChallengeImages.Insert (pointer, img);

							challengeImageSpritesLoaded = true;
						}
						// Wait for download to complete
						//yield return www;
					}


					// now, create images once, for each received challenge



					Destroy (tmp);

					if (copySolvedChallenges) {
						if (MasterManager.votingChallengesFromServerMeteor [pointer].solved) {
							GenerateVotingChallengeSolvedOnScreenMeteor (MasterManager.votingChallengesFromServerMeteor [pointer]);
						} else {
							GenerateVotingChallengeOnScreenMeteor (MasterManager.votingChallengesFromServerMeteor [pointer]);
						}


					} else {
						GenerateVotingChallengeOnScreenMeteor (MasterManager.votingChallengesFromServerMeteor [pointer]);
					}
					pointer++;
				}

			}
		}


		//OGLoadingOverlay.StopAllLoaders ();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;

		// reboot LateUpdate iterations
		maxIterations = 2;
		frameCounter = 0;
	}


	// /api/challengestimedtasknearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
	public IEnumerator LoadTimedTaskChallengesFromTheServer()
	{
		//OGLoadingOverlay.ShowFullcoverLoading("Loading timed challenges around you", true);
		List<TimedTaskChallengeInfo> tmpfilesSolvedInThisGameSession = new List<TimedTaskChallengeInfo>();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge timedTask Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.timedTaskChallengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.timedTaskChallengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// keeping track on which challenges were solved or not
		bool copySolvedChallenges = false;
		if (timedTaskChallengesOnScreenMeteor != null) {
			if (timedTaskChallengesOnScreenMeteor.Count > 0) {
				copySolvedChallenges = true;
				for (int i = 0; i < timedTaskChallengesOnScreenMeteor.Count; i++) {
					tmpfilesSolvedInThisGameSession.Add (timedTaskChallengesOnScreenMeteor [i]);
				}
			}
		}


		timedTaskChallengesOnScreenMeteor = new List <TimedTaskChallengeInfo> ();
		timedTaskChallengeImages = new List<Sprite> ();


		// ChallengeDBFormat_JSON
		//MasterManager.serverURL
		string url = MasterManager.serverURL + "/api/challengestimedtasknearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending this, in asking for timedTask challenges in server: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] TimedTask challenges Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<TimedTaskChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<TimedTaskChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);


			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
			// should be ChallengeInfo
			MasterManager.timedTaskChallengesFromServerMeteor = new List<TimedTaskChallengeInfo> (counterOfChallengesEnRoute);

			// should be ChallengeInfo
			TimedTaskChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			bool isItSolved = false;
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					isItSolved = false;

					for (int k = 0; k < tmpfilesSolvedInThisGameSession.Count; k++) {

						if (string.Compare (tmpfilesSolvedInThisGameSession [k]._id, results [i]._id) == 0) {
							isItSolved = tmpfilesSolvedInThisGameSession [k].solved;
							break;
						}
					}


					tmp = new TimedTaskChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].task,
						results [i].imageURL,
						results [i].questionHowMany,
						results [i].timer,
						results [i].validated,
						results [i].route,
						isItSolved);




					// Generate the challenge in the 3D world
					MasterManager.timedTaskChallengesFromServerMeteor.Add (tmp);
					timedTaskChallengesOnScreenMeteor.Insert (pointer, MasterManager.timedTaskChallengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.timedTaskChallengesFromServerMeteor [pointer].imageURL))
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
							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
								timer = 0;
								timeOut = 10;
								failed = false;
								//yield return wwwDefault;
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
							} else {
								// assign texture
								texture = www.texture;
							}

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							timedTaskChallengeImages.Insert (pointer, img);

							challengeImageSpritesLoaded = true;
						}

						// Wait for download to complete
						//	yield return www;
					}


					// now, create images once, for each received challenge



					Destroy (tmp);

					if (copySolvedChallenges) {
						if (MasterManager.timedTaskChallengesFromServerMeteor [pointer].solved) {
							GenerateTimedTaskChallengeSolvedOnScreenMeteor (MasterManager.timedTaskChallengesFromServerMeteor [pointer]);
						} else {
							GenerateTimedTaskChallengeOnScreenMeteor (MasterManager.timedTaskChallengesFromServerMeteor [pointer]);
						}


					} else {
						GenerateTimedTaskChallengeOnScreenMeteor (MasterManager.timedTaskChallengesFromServerMeteor [pointer]);
					}
					pointer++;
				}


			}
		}

		//OGLoadingOverlay.StopAllLoaders ();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;

		// reboot LateUpdate iterations
		maxIterations = 2;
		frameCounter = 0;
	}


	// /api/challengestimedtasknearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
	public IEnumerator LoadOpenQuizChallengesFromTheServer()
	{
		//OGLoadingOverlay.ShowFullcoverLoading("Loading timed challenges around you", true);
		List<OpenQuizChallengeInfo> tmpfilesSolvedInThisGameSession = new List<OpenQuizChallengeInfo>();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = false;

		// clean up the Challenge timedTask Icons everytime we reload challenges from server
		for (int i = 0; i < ChallengesCanvasMenuManager.instance.openQuizChallengeIcons.Count; i++) {
			Destroy (ChallengesCanvasMenuManager.instance.openQuizChallengeIcons[i]);
		}


		string[] imagePaths = new string[1];
		imagePaths[0] = "ChallengesImages/2 - challenge_wolf";
		//imagePaths[1] = "ChallengesImages/3 - challenge_witch";
		//imagePaths[2] = "ChallengesImages/4 - challenge_boogeyman";


		// keeping track on which challenges were solved or not
		bool copySolvedChallenges = false;
		if (openQuizChallengesOnScreenMeteor != null) {
			if (openQuizChallengesOnScreenMeteor.Count > 0) {
				copySolvedChallenges = true;
				for (int i = 0; i < openQuizChallengesOnScreenMeteor.Count; i++) {
					tmpfilesSolvedInThisGameSession.Add (openQuizChallengesOnScreenMeteor [i]);
				}
			}
		}


		openQuizChallengesOnScreenMeteor = new List <OpenQuizChallengeInfo> ();
		openQuizChallengeImages = new List<Sprite> ();


		// ChallengeDBFormat_JSON
		//MasterManager.serverURL
		// /api/challengesopenquiznearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
		string url = MasterManager.serverURL + "/api/challengesopenquiznearby?maxDistanceFromPlayer=" + 
			MasterManager.desiredMaxDistanceOfChallenges + "&playerLat=" +
			GPSLocationProvider_Xavier.instance.latlong.x+ "&playerLng=" +
			GPSLocationProvider_Xavier.instance.latlong.y;
		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending this, in asking for OpenQuiz challenges in server: " + request.error);
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] OpenQuiz challenges Request with: " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			List<OpenQuizChallengeDBMeteorFormat_JSON> results = 
				JsonWrapper.DeserializeObject<List<OpenQuizChallengeDBMeteorFormat_JSON>>(request.downloadHandler.text);


			int counterOfChallengesEnRoute = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					counterOfChallengesEnRoute++;
				}
			}
			// should be ChallengeInfo
			MasterManager.openQuizChallengesFromServerMeteor = new List<OpenQuizChallengeInfo> (counterOfChallengesEnRoute);

			// should be ChallengeInfo
			OpenQuizChallengeInfo tmp;

			// para cada desafio, gerá-lo no ecran e adicioná-lo a lista de desafios em 3D
			// in here, you have to take care of the images that the download gives you. download the
			// images and set them as sprites? download them into the filesystem?
			bool isItSolved = false;
			int pointer = 0;
			for (int i = 0; i < results.Count; i++) {
				if ((MasterManager.route == 0) || (MasterManager.route == results [i].route)) {
					isItSolved = false;

					for (int k = 0; k < tmpfilesSolvedInThisGameSession.Count; k++) {

						if (string.Compare (tmpfilesSolvedInThisGameSession [k]._id, results [i]._id) == 0) {
							isItSolved = tmpfilesSolvedInThisGameSession [k].solved;
							break;
						}
					}


					tmp = new OpenQuizChallengeInfo ();

					tmp.setData (results [i]._id,
						results [i].name,
						results [i].description,
						results [i].ownerPlayFabID,
						results [i].typeOfChallengeIndex,
						results [i].latitude,
						results [i].longitude,
						results [i].question,
						results [i].imageURL,
						results [i].validated,
						results [i].route,
						isItSolved);




					// Generate the challenge in the 3D world
					MasterManager.openQuizChallengesFromServerMeteor.Add (tmp);
					openQuizChallengesOnScreenMeteor.Insert (pointer, MasterManager.openQuizChallengesFromServerMeteor [pointer]);

					// challengeImages.Insert (i, ...);
					challengeImageSpritesLoaded = false;

					using (WWW www = new WWW(MasterManager.openQuizChallengesFromServerMeteor [pointer].imageURL))
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
							Texture2D texture;

							if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
								WWW wwwDefault = new WWW ("http://icons.iconarchive.com/icons/custom-icon-design/silky-line-user/256/user-man-invalid-icon.png");
								timer = 0;
								timeOut = 10;
								failed = false;
								//yield return wwwDefault;
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
							} else {
								// assign texture
								texture = www.texture;
							}

							Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

							openQuizChallengeImages.Insert (pointer, img);

							challengeImageSpritesLoaded = true;
						}

						// Wait for download to complete
						//	yield return www;
					}


					// now, create images once, for each received challenge



					Destroy (tmp);

					if (copySolvedChallenges) {
						if (MasterManager.openQuizChallengesFromServerMeteor [pointer].solved) {
							GenerateOpenQuizChallengeSolvedOnScreenMeteor (MasterManager.openQuizChallengesFromServerMeteor [pointer]);
						} else {
							GenerateOpenQuizChallengeOnScreenMeteor (MasterManager.openQuizChallengesFromServerMeteor [pointer]);
						}


					} else {
						GenerateOpenQuizChallengeOnScreenMeteor (MasterManager.openQuizChallengesFromServerMeteor [pointer]);
					}
					pointer++;
				}


			}
		}

		//OGLoadingOverlay.StopAllLoaders ();
		ChallengesCanvasMenuManager.instance.challengesLoadedFromServer = true;

		// reboot LateUpdate iterations
		maxIterations = 0;
		frameCounter = 0;
	}




	//ServerSaveChallengeAndPlayersWhoSolvedIt
	void GetTitleDataByKey_CloudScript(string searcheableKeyOnServer) {
		//JSON_TitleData_Challenge
		List<string> myKeys = new List<string>(1);
		myKeys.Add (searcheableKeyOnServer);

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{//GetTitleDataFromPlayFabKey
				FunctionName = "GetTitleDataFromPlayFabKey", FunctionParameter =  new { Keys = myKeys} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[MenuManager] CloudScript Done");
				if(success.FunctionResult != null) {

					Debug.Log(JsonWrapper.SerializeObject(success.FunctionResult));
					JsonObject jsonResult = (JsonObject)success.FunctionResult;

					ICollection<object> values = jsonResult.Values;

					int i = 0;
					foreach (object s in jsonResult.Values)
					{
						i ++;
						//Debug.Log("value in jsonResult found. [" + i + "]: " + s.ToString());

						Dictionary<string,string> list = JsonWrapper.DeserializeObject<Dictionary<string,string>>(s.ToString());

						//Debug.Log("Result of CloudScript already as dictionary: " + list.ToStringFull());

						string challengesInJSON = "";
						list.TryGetValue(searcheableKeyOnServer, out challengesInJSON);

						//Debug.Log("List of challenges in JSON: " + challengesInJSON); 

						Dictionary<string,List<string>> listOfChallenges = JsonWrapper.DeserializeObject<Dictionary<string,List<string>>>(challengesInJSON);

						//Debug.Log("Result of challenges already as dictionary: " + listOfChallenges.ToStringFull());
						//Debug.Log("Dictionary count: " + listOfChallenges.Count);
						//Debug.Log("Dictionary tostring: " + listOfChallenges.ToString());
						List<string> challenges;
						listOfChallenges.TryGetValue(searcheableKeyOnServer,out challenges);

						// print the results
						StartCoroutine(PrintTitleData(searcheableKeyOnServer,challenges));
					}
				}
			}, 
			error => {
				Debug.Log("There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}

	public IEnumerator PrintTitleData(string keyString, List<string> data)
	{

		if (data  != null) {
			Debug.Log("[GameManager] Count of data to print on the requested key " + keyString + ": " + data.Count);
			for (int j = 0; j < data.Count; j++)
			{
				Debug.Log("[GameManager] Result [" + j + "]: " + data [j]);
			}
		} else {
			throw new UnityException ("[GameManager] PrintTitleData with the requested key " + keyString + " returned null.");
		}

		yield return null;
	}

	public void GetTitleDataByKey (string keyword)
	{
		GetTitleDataRequest request = new GetTitleDataRequest();

		List<string> keys = new List<string> (1);
		keys.Add (keyword);
		request.Keys = keys;

		PlayFabClientAPI.GetTitleData (request, 
			response => 
			{
				// Dict<String, String>
				Dictionary<string, string> answer = ((GetTitleDataResult)response).Data;
				foreach(KeyValuePair<string,string> line in answer)
				{
					//Now you can access the key and value both separately from this attachStat as:
					Debug.Log("[GameManager] " + line.Key + "   ->    " + line.Value);
				}

			}, error =>
			{
				Debug.Log("[GameManager] Error getting the asked key. Reason: " + error.ErrorMessage);
			});

	}




	//ServerSaveChallengeAndPlayersWhoSolvedIt
	void AddChallengeToPlayFabServer(string cID) {
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = cID;
		argObject.players = new List<string> (0);
		//argObject.players.Add (""); // log this player in this challenge

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[MenuManager] CloudScript Done");
				if(success.FunctionResult != null) {
					Debug.Log("[MenuManager] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}

	// not tested. handle execute cloudscript returns
	private static void OnCloudHelloWorld(ExecuteCloudScriptResult result) {
		// Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
		Debug.Log(JsonUtility.ToJson(result.FunctionResult));
		Dictionary<string, object> jsonResult = JsonUtility.FromJson<Dictionary<string, object>>(JsonUtility.ToJson(result.FunctionResult));
		object messageValue;
		jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
		Debug.Log((string)messageValue);
	}
	// not tested. handle execute cloudscript returns
	private static void OnErrorShared(PlayFabError error)
	{
		Debug.Log(error.GenerateErrorReport());
	}



	public void EnableCompassNavigation(bool enable)
	{
		// if it is to enable the compass navigation
		if (!enable) {
			// disable compass rotation (by setting the object to follow by NULL) and hide it from view
			CompassAutoRotation.instance.SetObjectToFollow(null);

			// enable menu button
			menuButton.gameObject.SetActive(true);
			// disable stop auto compass rotation button
			cancelAutoCompassRotation.gameObject.SetActive(false);
		}
	}
		


	public void GenerateChallengeOnScreenMeteor(ChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge;

			for (int i = 0; i < MasterManager.challengesFromServerMeteor.Count; i++) {
				if (string.Compare (MasterManager.challengesFromServerMeteor [i]._id, challenge._id) == 0) {
					challenge.solved = MasterManager.challengesFromServerMeteor [i].solved; // copy the status of this challenge
					break;
				}
			}

			if (challenge.solved) {
				genericChallenge = Instantiate(challengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			} else {
				genericChallenge = Instantiate(challengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			}

			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<ChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);



			if (challenge.solved) {
				genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
					genericChallenge.transform.position.y + 10, genericChallenge.transform.position.z);
			} else {
				genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
					genericChallenge.transform.position.y, genericChallenge.transform.position.z);
			}


			objectsToDestroyOnLoadScene.Add (genericChallenge);	
		}

	}

	public void GenerateMultiplayerChallengeOnScreenMeteor(MultiplayerChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(multiplayerChallengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<MultiplayerChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);


			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);
			//		Debug.Log ("Closed Chess Challenge set with the ID " + genericChallenge.GetComponent<ChallengeData>().challenge_ID + " and the name: "+genericChallenge.GetComponent<ChallengeData>().challenge_Name);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}

	// hunterChallengesPrefab
	public void GenerateHunterChallengeOnScreenMeteor(HunterChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(hunterChallengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<HunterChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);


			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}

	public void GenerateHunterChallengeSolvedOnScreenMeteor(HunterChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(hunterChallengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<HunterChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);	
		}

	}

	// votingChallengesPrefab
	public void GenerateVotingChallengeOnScreenMeteor(VotingChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(votingChallengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<VotingChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, false);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}

	public void GenerateVotingChallengeSolvedOnScreenMeteor(VotingChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(votingChallengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<VotingChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, true);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);


			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}



	}

	// votingChallengesPrefab
	public void GenerateTimedTaskChallengeOnScreenMeteor(TimedTaskChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(timedTaskChallengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<TimedTaskChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, false);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}

	public void GenerateTimedTaskChallengeSolvedOnScreenMeteor(TimedTaskChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(timedTaskChallengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<TimedTaskChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, true);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}

	// OpenQuizChallengesPrefab
	public void GenerateOpenQuizChallengeOnScreenMeteor(OpenQuizChallengeInfo challenge)
	{
		
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(openQuizChallengesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<OpenQuizChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, false);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);
		}

	}
	public void GenerateOpenQuizChallengeSolvedOnScreenMeteor(OpenQuizChallengeInfo challenge)
	{
		//if ((MasterManager.route == 0) || (MasterManager.route == challenge.route)) // If No specific route to do, or if route is the one player wants to do
		{
			GameObject genericChallenge = Instantiate(openQuizChallengesSolvedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad (genericChallenge);

			genericChallenge.AddComponent<OpenQuizChallengeInfo> ();

			// ver esta função
			AttachChallengeToGameObjectMeteor (genericChallenge, challenge, true);
			// set challenge at the given location
			genericChallenge.transform.MoveToGeocoordinate(challenge.latitude,challenge.longitude, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);

			genericChallenge.transform.position = new Vector3 (genericChallenge.transform.position.x, 
				genericChallenge.transform.position.y, genericChallenge.transform.position.z);

			objectsToDestroyOnLoadScene.Add (genericChallenge);	
		}

	}


	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, ChallengeInfo challenge)
	{
		challengeInstange.GetComponent<ChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<ChallengeInfo> ().challenge_name = challenge.challenge_name;
		challengeInstange.GetComponent<ChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<ChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<ChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<ChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<ChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<ChallengeInfo> ().question = challenge.question;
		challengeInstange.GetComponent<ChallengeInfo> ().answer = challenge.answer;
		challengeInstange.GetComponent<ChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<ChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<ChallengeInfo> ().solved = challenge.solved;
		challengeInstange.GetComponent<ChallengeInfo> ().route = challenge.route;
	}
	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, MultiplayerChallengeInfo challenge)
	{
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().challenge_name = challenge.challenge_name;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().task = challenge.task;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().solved = challenge.solved;
		challengeInstange.GetComponent<MultiplayerChallengeInfo> ().route = challenge.route;

	}
	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, HunterChallengeInfo challenge)
	{
		challengeInstange.GetComponent<HunterChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<HunterChallengeInfo> ().name = challenge.name;
		challengeInstange.GetComponent<HunterChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<HunterChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<HunterChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<HunterChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<HunterChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<HunterChallengeInfo> ().question = challenge.question;
		challengeInstange.GetComponent<HunterChallengeInfo> ().answer = challenge.answer;
		challengeInstange.GetComponent<HunterChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<HunterChallengeInfo> ().content_text = challenge.content_text;
		challengeInstange.GetComponent<HunterChallengeInfo> ().content_picture = challenge.content_picture;
		challengeInstange.GetComponent<HunterChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<HunterChallengeInfo> ().solved = challenge.solved;
		challengeInstange.GetComponent<HunterChallengeInfo> ().route = challenge.route;
	}
	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, VotingChallengeInfo challenge, bool solved)
	{
		challengeInstange.GetComponent<VotingChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<VotingChallengeInfo> ().name = challenge.name;
		challengeInstange.GetComponent<VotingChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<VotingChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<VotingChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<VotingChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<VotingChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<VotingChallengeInfo> ().task = challenge.task;
		challengeInstange.GetComponent<VotingChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<VotingChallengeInfo> ().listOfImagesAndVotes = challenge.listOfImagesAndVotes;
		challengeInstange.GetComponent<VotingChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<VotingChallengeInfo> ().solved = solved;
		challengeInstange.GetComponent<VotingChallengeInfo> ().route = challenge.route;
	}
	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, TimedTaskChallengeInfo challenge, bool solved)
	{
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().name = challenge.name;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().task = challenge.task;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().questionHowMany = challenge.questionHowMany;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().timer = challenge.timer;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().solved = solved;
		challengeInstange.GetComponent<TimedTaskChallengeInfo> ().route = challenge.route;
	}
	public void AttachChallengeToGameObjectMeteor(GameObject challengeInstange, OpenQuizChallengeInfo challenge, bool solved)
	{
		//Debug.Log ("gamemanager AttachChallengeToGameObjectMeteor entered");
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ()._id = challenge._id;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().name = challenge.name;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().description = challenge.description;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().ownerPlayFabID = challenge.ownerPlayFabID;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().typeOfChallengeIndex = challenge.typeOfChallengeIndex;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().latitude = challenge.latitude;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().longitude = challenge.longitude;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().question = challenge.question;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().imageURL = challenge.imageURL;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().validated = challenge.validated;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().solved = solved;
		challengeInstange.GetComponent<OpenQuizChallengeInfo> ().route = challenge.route;
	}

	public void UnloadObjectsFromGameScene()
	{
		// activate all elements on screen
		for (int i = 0; i < objectsToSetActiveOnLoadScene.Count; i++) {
			objectsToSetActiveOnLoadScene [i].gameObject.SetActive (false);
		}/*
		if (productionMode) {
			for (int i = 1; i < objectsToSetActiveOnLoadScene.Count; i++) {
				objectsToSetActiveOnLoadScene [i].gameObject.SetActive (false);
			}
		} else {
			for (int i = 0; i < objectsToSetActiveOnLoadScene.Count; i++) {
				objectsToSetActiveOnLoadScene [i].SetActive (false);
			}
		}
*/

		// clean all challenge objects before calling another scene. the reason for this is that, when the scene returns, the objects 
		// do not rotate anymore
		for (int i = 0; i < objectsToDestroyOnLoadScene.Count; i++) {
			Destroy (objectsToDestroyOnLoadScene [i]);
		}
		// clean this list
		objectsToDestroyOnLoadScene = new List<GameObject>();
	}


	public void LoadObjectsFromGameSceneMeteor()
	{
		// activate all elements on screen
		for (int i = 0; i < objectsToSetActiveOnLoadScene.Count; i++) {
			objectsToSetActiveOnLoadScene [i].gameObject.SetActive (true);
		}

		// this is to make the main menu button disappear when loading the game
		//if (!gameProperlyInitialized) {
		//	objectsToSetActiveOnLoadScene [0].gameObject.SetActive (false);
		//}

		// this is to handle the cheating buttons
		/*#if UNITY_EDITOR
		if (productionMode) {
			buttonWalkingNotWalking.SetActive(false);
		} else {
			buttonWalkingNotWalking.SetActive(true);
		}

		#else
		if (productionMode) {
		buttonWalkingNotWalking.SetActive(false);
		} else {
		buttonWalkingNotWalking.SetActive(false);
		}
		#endif */

		// clean all challenge objects before calling another scene. the reason for this is that, when the scene returns, the objects 
		// do not rotate anymore
		for (int i = 0; i < objectsToDestroyOnLoadScene.Count; i++) {
			Destroy (objectsToDestroyOnLoadScene [i]);
		}
		// clean this list
		objectsToDestroyOnLoadScene = new List<GameObject>();


		// reload all challenges on map, rotating and all
		if (!reloadChallengesFromCloud) {

			for (int j = challengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				//if (!challengesOnScreenMeteor [j].solved) {
					GenerateChallengeOnScreenMeteor (challengesOnScreenMeteor [j]);
				//}
			}
			for (int j = multiplayerChallengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				if (!multiplayerChallengesOnScreenMeteor [j].solved) {
					//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
					GenerateMultiplayerChallengeOnScreenMeteor (multiplayerChallengesOnScreenMeteor [j]);
				}
			}
			for (int j = hunterChallengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				if (!hunterChallengesOnScreenMeteor [j].solved) {
					//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
					GenerateHunterChallengeOnScreenMeteor (hunterChallengesOnScreenMeteor [j]);
				} else {
					GenerateHunterChallengeSolvedOnScreenMeteor (hunterChallengesOnScreenMeteor [j]);
				}
			}
			for (int j = votingChallengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				if (!votingChallengesOnScreenMeteor [j].solved) {
					//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
					GenerateVotingChallengeOnScreenMeteor (votingChallengesOnScreenMeteor [j]);
				} else {
					GenerateVotingChallengeSolvedOnScreenMeteor (votingChallengesOnScreenMeteor [j]);
				}
			}
			for (int j = timedTaskChallengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				if (!timedTaskChallengesOnScreenMeteor [j].solved) {
					//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
					GenerateTimedTaskChallengeOnScreenMeteor (timedTaskChallengesOnScreenMeteor [j]);
				} else {
					GenerateTimedTaskChallengeSolvedOnScreenMeteor (timedTaskChallengesOnScreenMeteor [j]);
				}
			}
			for (int j = openQuizChallengesOnScreenMeteor.Count -1; j >= 0; j--) {
				// only generate items that have not been solved
				if (!openQuizChallengesOnScreenMeteor [j].solved) {
					//GenerateChallengeOnScreen (challengesOnScreen [j], getChallengeDetails(challengesOnScreen [j].challenge_ID, challengesOnScreen [j].typeOfChallengeIndex));
					GenerateOpenQuizChallengeOnScreenMeteor (openQuizChallengesOnScreenMeteor [j]);
				} else {
					GenerateOpenQuizChallengeSolvedOnScreenMeteor (openQuizChallengesOnScreenMeteor [j]);
				}
			}
		}
		else
		{
			reloadChallengesFromCloud = false;
			//StartCoroutine (LoadChallengesFromTheCloud(
			//	MasterManager.desiredMaxDistanceOfChallenges, GPSLocationProvider_Xavier.instance.latlong));
			StartCoroutine (LoadChallengesFromTheServer());
		}
		// everytime you jump from one menu to another, do you want to do this?
		//StartCoroutine (GameManager.instance.LoadChallengesFromTheCloud(
		//	MasterManager.desiredMaxDistanceOfChallenges, GPSLocationProvider_Xavier.instance.latlong));

	}



	public object getChallengeDetails(long id, int typeOfChallenge)
	{
		if (typeOfChallenge == 0)
		{
			foreach (Challenge_QuizData q in quizChallengesOnScreen) {
				if (q.challengeID == id) {
					return q;
				}
			}
		}	
		if (typeOfChallenge == 1)
		{
			foreach (Challenge_ARData a in arChallengesOnScreen) {
				if (a.challengeID == id) {
					return a;
				}
			}
		}

		throw new Exception("[GameManager] unrecognizable type of challenge details to attach to the 3D object.");
	}

	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowChallenge(string challengeToFollowID, bool pursuitEnabled = false)
	{

		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {
			//Debug.Log ("[GameManager] Follow: " + challengeToFollow.challenge_Name + "at " + challengeToFollow.DistanceToString());

			//if (GameManager.challengeToFollow. != null) {
			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				
				if (objectsToDestroyOnLoadScene [i].CompareTag ("ClosedChessChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<ChallengeInfo> ()._id, challengeToFollowID) == 0) {
						index = i;
						break;
					}
				}

			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected challenge was not found on the screen.");
			}


			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
		// Set compass to follow defined challenge
		CompassAutoRotation.instance.SetObjectToFollow (null);
		// enable menu button
		GameManager.instance.menuButton.gameObject.SetActive (true);
		// disable stop auto compass rotation button
		GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}

	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowMultiplayerChallenge(string multiplayerChallengeIDToFollow, bool pursuitEnabled = false)
	{
		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {
			Debug.Log ("[GameManager] possible objects on screen: " + objectsToDestroyOnLoadScene.Count);
			//Debug.Log ("[GameManager] Follow it at: " + challengeToFollow.DistanceToString());

			//if (GameManager.challengeToFollow. != null) {
			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				Debug.Log ("[GameManager] Index : " + i);

				if (objectsToDestroyOnLoadScene [i].CompareTag ("MultiplayerChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<MultiplayerChallengeInfo> ()._id, multiplayerChallengeIDToFollow) == 0) {
						index = i;
						break;
					}
				}
			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected Multiplayer challenge was not found on the screen.");
			}
				

			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (null);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (true);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}

	// FollowHunterChallenge
	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowHunterChallenge(string challengeToFollowID, bool pursuitEnabled = false)
	{
		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {
			//Debug.Log ("[GameManager] Follow: " + challengeToFollow.challenge_Name + "at " + challengeToFollow.DistanceToString());

			//if (GameManager.challengeToFollow. != null) {
			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {

				if (objectsToDestroyOnLoadScene [i].CompareTag ("HunterChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<HunterChallengeInfo> ()._id, challengeToFollowID) == 0) {
						index = i;
						break;
					}
				}

			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected Hunter challenge was not found on the screen.");
			}

			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (null);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (true);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}
		
	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowVotingChallenge(string challengeToFollowID, bool pursuitEnabled = false)
	{
		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {
			
			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				
				if (objectsToDestroyOnLoadScene [i].CompareTag ("VotingChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<VotingChallengeInfo> ()._id, challengeToFollowID) == 0) {
						index = i;
						break;
					}
				}

			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected Voting challenge was not found on the screen.");
			}

			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (null);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (true);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}


	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowTimedTaskChallenge(string challengeToFollowID, bool pursuitEnabled = false)
	{
		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {

			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {

				if (objectsToDestroyOnLoadScene [i].CompareTag ("TimedTaskChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<TimedTaskChallengeInfo> ()._id, challengeToFollowID) == 0) {
						index = i;
						break;
					}
				}

			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected TimedTask challenge was not found on the screen.");
			}

			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (null);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (true);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}

	// call this when you want to activate the compass. guarantee that the challenge object to follow is set up
	public void FollowOpenQuizChallenge(string challengeToFollowID, bool pursuitEnabled = false)
	{
		MenuSlide.instance.ResetAnimation ();

		if (pursuitEnabled) {

			int index = -1;
			for (int i = objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {

				if (objectsToDestroyOnLoadScene [i].CompareTag ("OpenQuizChallenge")) {
					if (string.Compare (objectsToDestroyOnLoadScene [i].GetComponent<OpenQuizChallengeInfo> ()._id, challengeToFollowID) == 0) {
						index = i;
						break;
					}
				}

			}
			if (index == -1) {
				throw new UnityException ("[GameManager] The object of the selected OpenQuiz challenge was not found on the screen.");
			}

			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (objectsToDestroyOnLoadScene [index]);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (false);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (true);
		}
		else {
			// Set compass to follow defined challenge
			CompassAutoRotation.instance.SetObjectToFollow (null);
			// enable menu button
			GameManager.instance.menuButton.gameObject.SetActive (true);
			// disable stop auto compass rotation button
			GameManager.instance.cancelAutoCompassRotation.gameObject.SetActive (false);
		}
		isAnyWindowOpen = false;
	}

	// this is to load the settings menu
	public void LoadChallengesMenu()
	{
		StartCoroutine (LoadChallengesMenuAsync());
	}

	private IEnumerator LoadChallengesMenuAsync()
	{
		// txt21
		string[] displayMessage = {"Loading challenges around you ...", "Laden van uitdagingen om je heen ...", "A carregar os desafios que estão à tua volta ..."};
		_referenceDisplayManager.DisplaySystemMessage (displayMessage[MasterManager.language]);
		//	MasterManager.desiredMaxDistanceOfChallenges, GPSLocationProvider_Xavier.instance.latlong));
		yield return StartCoroutine (LoadChallengesFromTheServer());

		int count = 20;
		while ((!GameManager.challengeImageSpritesLoaded) && (count > 0)) {
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[GameManager] In LoadChallengesMenuAsync, the game could not load the challenges again within 20 seconds!");}

		SceneManager.LoadScene (3, LoadSceneMode.Additive);
	}

	// this is to load the settings menu
	public void LoadMenu()
	{
		SceneManager.LoadScene (3, LoadSceneMode.Additive);
	}

	// this is to load the QR Wallet menu
	public void LoadQRWalletMenu()
	{
		SceneManager.LoadScene (4, LoadSceneMode.Additive);
	}
	// this is to load the QR Reader menu
	public void LoadQRReaderMenu()
	{
		SceneManager.LoadScene (5, LoadSceneMode.Additive);
	}

	// wait the necessary time before using the display
	private IEnumerator CollectPlayerDataGPS()
	{
		string url;

		//yield return CollectPlayerDataGPSBackGroundAndroidService ();

		yield return new WaitForSecondsRealtime (5);

		while (true) {
			url = MasterManager.serverURL + "/api/playerdata?ownerPlayFabID=" + MasterManager.activePlayFabId
				+ "&lat=" + GPSLocationProvider_Xavier.instance.latlong.x
				+ "&lng=" + GPSLocationProvider_Xavier.instance.latlong.y
				+ "&timestamp=" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")
				+ "&label=";
			
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
				} 

				//yield return www;  // Wait for download to complete
				www.Dispose ();
			}

			// alternative 
			//MasterManager.LogEventInServer("");
		
			yield return new WaitForSecondsRealtime (3);
		}

	}

	// deprecated?
	public IEnumerator CollectPlayerDataGPSRegisterEventInDB(string comment)
	{
		string url;
		Debug.Log ("Arrived.");

		url = MasterManager.serverURL + "/api/playerdata?ownerPlayFabID=" + MasterManager.activePlayFabId
			+ "&lat=" + GPSLocationProvider_Xavier.instance.latlong.x
			+ "&lng=" + GPSLocationProvider_Xavier.instance.latlong.y
			+ "&timestamp=" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")
			+ "&label=" + comment;

		//Debug.LogError ("[GameManager CollectPlayerDataGPSRegisterEventInDB] URL with event: " + url);

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
				//break;
			}
			if (failed || !string.IsNullOrEmpty (www.error)) {
				www.Dispose ();
				//break;
				yield break;
			} 
			//yield return www;  // Wait for download to complete
			www.Dispose ();
		}

		yield return null;
	}


	public IEnumerator LoadGamePermissionsForPlayer()
	{
		// 
		string url = MasterManager.serverURL + "/api/gamePermissions?PlayerID=" +MasterManager.activePlayFabId;

		// pedir os desafios ao servidor. Este é o URL
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.timeout = 10;
		yield return request.SendWebRequest();

		if (request.isNetworkError)
		{
			Debug.LogError("[GameManager] Error While Sending: " + request.error + " to LoadGamePermissionsForPlayer()");
			Debug.LogError ("[GameManager] URL: " + url);
		}
		else
		{
			Debug.Log("[GameManager] Request to LoadGamePermissionsForPlayer with : " + url);
			Debug.Log("[GameManager] Received: " + request.downloadHandler.text);


			playerGamePermissions = 
				JsonWrapper.DeserializeObject<int>(request.downloadHandler.text);

			loadedGamePermissionsFromServer = true;
		}

		if (MenuManager.instance != null) {
			yield return MenuManager.instance.CorrectIconsOnScreenBasedOnPermissions();
		}

		yield break;

	}



	public void AlertDidFinishWithResult(string selectedButtonTitle)
	{
		// Trigger event.
		_referenceDisplayManager.DisplaySystemMessage (selectedButtonTitle);
	}

	private void ensure_not_null(object mustBeNotNull, String message) {
		if (mustBeNotNull == null) {				
			to_log("NULL is detected: " + message);
		}
	}	
	private void to_log(String message) {
		Debug.Log("[GameManager] :" + message);
	}


	public void IsAnyWindowOpen(bool res){
		isAnyWindowOpen = res;
	}

	public void RespawnChallengeObjects() {

		// destroying it all
		GameManager.instance.objectsToDestroyOnLoadScene = new List<GameObject>();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("ClosedChessChallenge");  //returns GameObject[]
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("Challenge");  //returns GameObject[]
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
		}
		objs = GameObject.FindGameObjectsWithTag("VotingChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("VotingChallengeSolved");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("TimedTaskChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("TimedTaskChallengeSolved");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}

		objs = GameObject.FindGameObjectsWithTag("OpenQuizChallenge");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}

		objs = GameObject.FindGameObjectsWithTag("OpenQuizChallengeSolved");  //returns GameObject[]
		// kill the objects that should have been killed
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs[i]);
		}






		// generating quiz challenges again
		if (challengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < challengesOnScreenMeteor.Count; i++) {
				GenerateChallengeOnScreenMeteor(challengesOnScreenMeteor[i]);
			}
		}
		// generating multiplayer challenges again
		if (multiplayerChallengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < multiplayerChallengesOnScreenMeteor.Count; i++) {
				GenerateMultiplayerChallengeOnScreenMeteor(multiplayerChallengesOnScreenMeteor[i]);
			}
		}

		// generating hunter challenges again, either solved or unsolved
		if (hunterChallengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < hunterChallengesOnScreenMeteor.Count; i++) {
				// solved?
				if (!hunterChallengesOnScreenMeteor [i].solved) {
					GenerateHunterChallengeOnScreenMeteor(hunterChallengesOnScreenMeteor[i]);
				} else { // if so
					GenerateHunterChallengeSolvedOnScreenMeteor(hunterChallengesOnScreenMeteor[i]);
				}

			}
		}
		// generating voting challenges again, either solved or unsolved
		if (votingChallengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < votingChallengesOnScreenMeteor.Count; i++) {
				// solved?
				if (!votingChallengesOnScreenMeteor [i].solved) {
					GenerateVotingChallengeOnScreenMeteor(votingChallengesOnScreenMeteor[i]);
				} else { // if so
					GenerateVotingChallengeSolvedOnScreenMeteor(votingChallengesOnScreenMeteor[i]);
				}

			}
		}
		// generating timedTask challenges again, either solved or unsolved
		if (timedTaskChallengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < timedTaskChallengesOnScreenMeteor.Count; i++) {
				// solved?
				if (!timedTaskChallengesOnScreenMeteor [i].solved) {
					GenerateTimedTaskChallengeOnScreenMeteor(timedTaskChallengesOnScreenMeteor[i]);
				} else { // if so
					GenerateTimedTaskChallengeSolvedOnScreenMeteor(timedTaskChallengesOnScreenMeteor[i]);
				}

			}
		}

		// generating OpenQuiz challenges again, either solved or unsolved
		if (openQuizChallengesOnScreenMeteor.Count > 0) {
			for (int i = 0; i < openQuizChallengesOnScreenMeteor.Count; i++) {
				// solved?
				if (!openQuizChallengesOnScreenMeteor [i].solved) {
					GenerateOpenQuizChallengeOnScreenMeteor(openQuizChallengesOnScreenMeteor[i]);
				} else { // if so
					GenerateOpenQuizChallengeSolvedOnScreenMeteor(openQuizChallengesOnScreenMeteor[i]);
				}

			}
		}
	}

	private static int cw = 0;
	public void testEnQueue() {
	
		Debug.LogError ("[GameManager] TestEnQueue. Count: " + MasterManager.logEventsInServer.Count);
		cw++;
		MasterManager.logEventsInServer.Enqueue ("String " + cw);

	}
	public void testDeQueue() {

		Debug.LogError ("[GameManager] TestDeQueue. Count: " + MasterManager.logEventsInServer.Count);
		Debug.LogError ("[GameManager] Dequeue: " + MasterManager.logEventsInServer.Dequeue());
		Debug.LogError ("[GameManager] TestDeQueue after. Count: " + MasterManager.logEventsInServer.Count);

	}
}
 