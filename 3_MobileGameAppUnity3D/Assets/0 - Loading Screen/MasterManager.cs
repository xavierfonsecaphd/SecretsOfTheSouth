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


using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SotS;
using UnityEngine.Networking;

// Mapbox APIs
using Mapbox.Unity.Utilities;
using Mapbox.Map;
using Mapbox.Utils;
using Mapbox.Unity.Map;

using Facebook.Unity;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine.SceneManagement;

public class MasterManager : MonoBehaviour {

	public static ConcurrentQueue<string> logEventsInServer;

	public static MasterManager instance { set; get ; }
	// This is a singleton
	private static MasterManager singleton;

	// Have here one reference for every game object you wish to control.
	//private GPSLocationProvider_Xavier _referenceGPSLocationProvider_Xavier = null;
	public GameObject _referenceMap;
	public Camera _referenceCamera;
	// This manages the status messages appearing on top of the game
	public DisplayManager _referenceDisplayManager;
	public GameObject [] objectsToManageOnLoadScene;

	// I want this variable here to only handle objects in this scene (scene 0) after others have been properly loaded. 
	// Only then I want to disable the objects from this scene
	public static bool properlyUnloaded = true; 

	public bool playerHasTeam = false;
	public string teamID;
	public string teamName;
	public string teamRefIcon;

	// this is the PlayFabId of the user playing the game (regardless of login method). with this, I can always get his info online
	public static string activePlayFabId;	
	public static string activePlayerName;	
	public static AccessToken facebookAccessToken = null;
	public static string activePlayerAvatarURL;
	public static string activePlayerEmail;
	public static string activePlayerPassword;
	public static double desiredMaxDistanceOfChallenges = 2.0;
	//public static List<ChallengeData> challengesFromServer; // Legacy. Delete
	public static List<ChallengeInfo> challengesFromServerMeteor;
	public static List<MultiplayerChallengeInfo> multiplayerChallengesFromServerMeteor;
	public static List<HunterChallengeInfo> hunterChallengesFromServerMeteor;
	public static List<VotingChallengeInfo> votingChallengesFromServerMeteor;
	public static List<TimedTaskChallengeInfo> timedTaskChallengesFromServerMeteor;
	public static List<OpenQuizChallengeInfo> openQuizChallengesFromServerMeteor;
	public static string serverURL = "http://secretsofthesouth.tbm.tudelft.nl";
	public static int language = 0;	// 0 - English; 1 - Netherlands; 2 - Português 
	public static int route = 0;	// 0 - No Route; 1 - Route 1; 2 - Route 2; 3 - Route 3 
	public bool gameIsLoading = true;	// at the beginning of the game, while the player is entering the game, this will be loading. Otherwise will be set to false

	//[Header("Localization")]
	//public LocalizedText loadingText;
	//public LocalizedText initalizingText;

	// esta variável global permite ao jogador clicar e entrar no challenge mal veja o icone 3D no mapa, em vez de caminhar até estar 
	// perto dele (cagaNaDistanciaDosIcones3DAteOsAtivares)
	public bool developmentMode = false;	// desativa para fazeres o deploy para produção

	void Awake()
	{
		// keep smartphone awake
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

		if (singleton == null) 
		{
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = MasterManager.singleton;


			// put all the items of the game inactive, so that they can only be seen after a proper initialization of the game
			// GPSLocationProvider_Xavier

			// put all the items of the game inactive, so that they can only be seen after a proper initialization of the game
			// GPSLocationProvider_Xavier
			//if (loadingCanvas == null) throw new System.Exception("Loading Canvas from MasterManager is null!");

			// Camera
			if (_referenceCamera == null) throw new System.Exception("_reference Camera is null!");
			//DontDestroyOnLoad (_referenceCamera);


			// Display manager of the MasterManager (loading screen)
			if (_referenceDisplayManager == null) {
				throw new UnityException ("[GameManager] Display Manager is still null.");
			}

			#if UNITY_EDITOR
			//QualitySettings.vSyncCount = 0;  // VSync must be disabled
			Application.targetFrameRate = 300;
			#endif


			activePlayerName = string.Empty;
			activePlayFabId = string.Empty;
			//challengesFromServer = new List<ChallengeData> ();
			challengesFromServerMeteor = new List<ChallengeInfo> ();

			LoadPreviousGameData ();

			// popup
			// card
			// translations
			// loading

			//List <SystemLanguage> availableLanguages = LocalizationSettings.Availa
			//Localization.Instance.CurrentLanguage = SystemLanguage.English;
			// txt1
			string [] displayMessage1 = {"Initializing Game ...",
				"Game initialiseren ...",
				"A inicializar o jogo ..."};


			OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[language]);
			// i need to create the corresponding asset in the scene for this to work.



			//This is to initiate the Queue of messages to send to the server
			logEventsInServer = ConcurrentQueue<string>.InitFromArray (null);

			Application.runInBackground = true;
		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	// this is to initiate services that are asynchronous or take some time to start (like displayManager or GPSprovider)
	void Start()
	{
		StartCoroutine (SetUpRoutine());	
			
	}


	public void LoadPreviousGameData()
	{
		
		Debug.Log ("[MasterManager] Loading previous game data. Reading the file:  " + 
			Application.persistentDataPath + "/SotSPlayerInfo.dat");

		// does this file exist?
		if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {

			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
			SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
			file.Close ();

			// re-login with the last login method used
			Debug.Log("[MasterManager] Email: " + data.playFabEmail + "   and password:   " + data.playFabPassword);
			//SotSHandler.instance.LoginToContinueGame (data.playFabEmail, data.playFabPassword);

			MasterManager.activePlayerName = data.activePlayerName;
			MasterManager.activePlayFabId = data.activePlayFabId;
			MasterManager.activePlayerAvatarURL = data.playFabPlayerAvatarURL;
			MasterManager.desiredMaxDistanceOfChallenges = data.desiredMax_DistanceOfChallenges;
			MasterManager.activePlayerEmail = data.playFabEmail;
			MasterManager.activePlayerPassword = data.playFabPassword;
			MasterManager.language = data.language;
			MasterManager.route = data.route;


			if ((string.Compare (data.teamID, string.Empty) == 0) || (string.Compare (data.teamName, string.Empty) == 0) || (string.Compare (data.teamRefIcon, string.Empty) == 0) 
				|| (string.IsNullOrEmpty(data.teamID))|| (string.IsNullOrEmpty(data.teamName))|| (string.IsNullOrEmpty(data.teamRefIcon))) {
				MasterManager.instance.playerHasTeam = false;
				MasterManager.instance.teamID = "";
				MasterManager.instance.teamName = "";
				MasterManager.instance.teamRefIcon = "";
			} else {
				MasterManager.instance.playerHasTeam = true;

				//Debug.Log("1 MasterManager.teamID = " + MasterManager.instance.teamID + " -> with character count: " + MasterManager.instance.teamID.Length );

				MasterManager.instance.teamID = data.teamID;
				MasterManager.instance.teamName = data.teamName;
				MasterManager.instance.teamRefIcon = data.teamRefIcon;

				//Debug.Log("2 MasterManager.teamID = " + MasterManager.instance.teamID + " -> with character count: " + MasterManager.instance.teamID.Length );
			}

			switch (language)
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
	}




	public void OnApplicationPause(bool paused) {
		if (paused) {
			// Game is paused, start service to get notifications
		} else {
			// Game is unpaused, stop service notifications. 
		}
	}

	private IEnumerator SetUpRoutine()
	{
		// Let's set up the screen to send messages to the player
		int count = 10;
		while (!_referenceDisplayManager.isProperlyInitialized && count >= 0)
		{
			count--;
			yield return new WaitForSeconds (1);
		}
		if (count < 0) {
			// Txt2
			string [] displayMessage1 = {"Display did not initiate properly",
				"Display is niet correct gestart",
				"O ecran não conseguiu inicializar de forma correta"};
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[language], true);
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Display Manager did not initiate properly" );
			throw new UnityException ("[MasterManager] Display Manager did not initiate properly");
		}
			
		count = 10;


		//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Initializing game..." );

		while (!GPSLocationProvider_Xavier.instance.initialized && count >= 0)
		{
			count--;
			yield return new WaitForSeconds (1);
		}
		if (count < 0) {
			// txt3
			string [] displayMessage2 = {"Could Not Find Your GPS Location. Please Go Outside and Try the Game Again.",
				"Kon uw GPS-locatie niet vinden. Ga alsjeblieft naar buiten en probeer opnieuw.",
				"Não foi possível encontrar a tua localização GPS. Por favor desloca-te para o exterior e tenta jogar de novo."};
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[language], true);
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "GPS location did not initiate properly. Please guarantee that you have your GPS ON." );
			throw new UnityException ("[MasterManager] GPSLocationProvider_Xavier did not initiate properly, in order to initiate the GPS");
		}
		// ************************************************************************
		// **************************** Initiating the GPS ************************
		// ************************************************************************
		// First, check if user has location service enabled
		#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		if (Input.location.isEnabledByUser) {

			StartCoroutine(GPSLocationProvider_Xavier.instance.StartGPSLocationServiceOnThePhone());
		}
		else 
		{	// the player only plays when he has got the positioning permission enabled.
			throw new UnityException ("[GPSLocationProvider_Xavier] Location was not enabled by the player. Quitting.");
		}
		#endif

		#if UNITY_EDITOR
			StartCoroutine (GPSLocationProvider_Xavier.instance.InitUnityRemoteOnSmartphone ());
		#endif


		// Let's initiate the GPS
//		gameObject.AddComponent<GPSLocationProvider_Xavier>();
//		new GPSLocationProvider_Xavier();
		//_referenceGPSLocationProvider_Xavier.initiateGPS();
	}
		
	// Use this for initialization
	/*void Start () {
		if (!_referenceDisplayManager.isProperlyInitialized)
			throw new UnityException ("[MasterManager] DisplayManager is not ready!");
		
		MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Initializing game..." );
	}*/

	// it is public just to be able to be called in other coroutines
	// called by [GPSLocationProvider_Xavier] continue with the setting up of the game play 
	// (yes, this is an asynchronous approach)
	public void ContinueGameInitializationAfterGPS()
	{
		StartCoroutine (ContinueGameInitializationAfterGPSThread());
	}

	private IEnumerator CheckInternetConnection()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			Debug.Log ("Error. Check internet connection!");
			// txt4
			string [] displayMessage2 = {"No Internet Connection. Please enable Internet and GPS to play the game.",
				"Geen internet verbinding. Zet internet en GPS aan om het spel te spelen.",
				"Sem ligação à internet. Por favor ativa tanto a tua ligação à internet como a tua localização GPS."};
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[language], true);
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "No Internet Connection. Please enable Internet and GPS to play the game." );
		} else 
		{
			yield return ContinueGameInitializationAfterGPSThread();
		}
	}

	private IEnumerator ContinueGameInitializationAfterGPSThread()
	{
		int count = 10;

			
		// was the latitude/longitude properly initialized?
		if ((GPSLocationProvider_Xavier.instance.latlong.x == 0.0f) && 
			(GPSLocationProvider_Xavier.instance.latlong.y == 0.0f)) 
		{
			// txt5
			string [] displayMessage1 = {"You did not give permissions for this app to use GPS location. Enable this on settings to play.",
				"U heeft geen toestemming gegeven voor deze app om de GPS-locatie te gebruiken. Zet GPS aan in de instellingen.",
				"Tu não deste permissões a este jogo para usar a tua localização GPS. Por favor ativa a localização nas definições para poderes jogar."};
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[language], true);

			//_referenceDisplayManager.DisplayErrorMessage ( "You did not give permissions for this app to use GPS location. Enable this on settings to play." );
			//statusMessage.text = "You did not give permissions for this app to use GPS location. Enable this on settings to play." ;
			throw new UnityException ("The GPS coordinates should be here already. Killing Game!");
		}

		while (!GPSLocationProvider_Xavier.instance.gotGPSLocation && count >= 0)
		{
			yield return new WaitForSeconds (1);
		}
		if (count < 0)
			throw new UnityException ("[MasterManager] GPS provider did not initiate properly.");
		count = 10;

		Debug.Log ("[MasterManager] Location set.");



		// Then, we initialize the Map, as it takes time to load as well
		Debug.Log ("[MasterManager] Coordinates found are [" + 
			GPSLocationProvider_Xavier.instance.latlong.x + "  -  " + 
			GPSLocationProvider_Xavier.instance.latlong.y + "]... ");

		// start map with that location
		yield return StartCoroutine( LaunchMapAfterGPS() );
		_referenceMap.SetActive (false);



		// check internet connection, otherwise continue
		if ((Application.internetReachability == NetworkReachability.NotReachable)){// || (!Input.location.isEnabledByUser)) {
			// txt4
			string [] displayMessage2 = {"No Internet Connection. Please enable Internet and GPS to play the game.",
				"Geen internet verbinding. Schakel internet en GPS in om het spel te spelen.",
				"Sem ligação à internet. Por favor ativa tanto a tua ligação à internet como a tua localização GPS."};

			OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[language], true);
			Debug.Log ("No Internet Connection. Please enable Internet and GPS to play the game.");
			//MasterManager.instance._referenceDisplayManager.displayTime = 20;
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "No Internet Connection. Please enable Internet and GPS to play the game." );

			yield break;
		} else 
		{
			//txt6
			string [] displayMessage3 = {"All set!",
				"Helemaal klaar!",
				"Tudo a postos!"};
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage3[language], true);
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "All set!" );
			Debug.Log ("[MasterManager] All set up. Loading Scene");		// move the screen to the login screen

			// just wait for one second before you change screens
			//yield return new WaitForSeconds (1);

			SceneManager.LoadScene(1, LoadSceneMode.Additive);

			// this makes an assynchronous handling of the objects left active in the scene 0. Only when the next one is properly set up, you handle these
			properlyUnloaded = false;

			yield break;
		}
	}

	private IEnumerator LaunchMapAfterGPS()
	{
		yield return _referenceMap.GetComponent<MyBasicMap> ().CreateMap ();
	}
		
	// deactivate all the objects that are still active in this scene
	public void UnloadObjectsFromScene()
	{

		for (int i = 0; i < objectsToManageOnLoadScene.Length; i++) {
			objectsToManageOnLoadScene [i].SetActive (false);
		}

		properlyUnloaded = true;
	}

	void OnApplicationQuit()
	{
		Debug.Log("[MasterManager] Application ended properly after " + Time.time + " seconds.");

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else

		#endif

	}

	// primeiro, cria uma função para enviar mensagem e ponto final

	public static void LogEventInServer(string message) {
		logEventsInServer.Enqueue (message);
	}

	// depois, crias uma Thread que, de X em X segundos, vai acordar e tentar enviar mensagens que estejam por enviar!
	public IEnumerator StartRecordingEventsInServer() {

		Debug.Log ("Start the recording of Events of the game into the SotS Server.");
		string messageToSend;
		WWWForm form;
		string url;
		string urlPrefix;
		urlPrefix = MasterManager.serverURL;
		//urlPrefix = "localhost:3000";
		url = urlPrefix + "/api/logeventfromgame";
		int counter = 5;

		while (true) {
			counter--;

			// it always tries to log events
			if (MasterManager.logEventsInServer.Count > 0) {
				while (MasterManager.logEventsInServer.Count > 0) {
					// get next message to log in the server
					messageToSend = logEventsInServer.Dequeue ();

					// send dequeued message to server
					form = new WWWForm ();
					form.AddField ("name", MasterManager.activePlayerName);
					form.AddField ("playfabid", MasterManager.activePlayFabId);
					#if UNITY_EDITOR
					form.AddField ("latitude", "FAKE");
					form.AddField ("longitude", "FAKE");
					#else
						form.AddField ("latitude", GPSLocationProvider_Xavier.instance.latlong.x.ToString());
						form.AddField ("longitude", GPSLocationProvider_Xavier.instance.latlong.y.ToString());
					#endif

					form.AddField ("timestamp",  DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
					form.AddField ("message", messageToSend);


					using (var w = UnityWebRequest.Post(url, form))
					{
						yield return w.SendWebRequest();
						if (w.isNetworkError || w.isHttpError) {
							//print("[MasterManager] Message not recorded in server: " + messageToSend + " . Reason: " + w.error);
							Debug.LogError ("[MasterManager] Message not recorded in server: " + messageToSend + " . Reason: " + w.error);
						}
					}
				}
			}

			// then, each 5 seconds, it records its position
			if (counter < 0) {
				// record current GPS location
				form = new WWWForm ();
				form.AddField ("name", MasterManager.activePlayerName);
				form.AddField ("playfabid", MasterManager.activePlayFabId);
				#if UNITY_EDITOR
				form.AddField ("latitude", "FAKE");
				form.AddField ("longitude", "FAKE");
				#else
				form.AddField ("latitude", GPSLocationProvider_Xavier.instance.latlong.x.ToString());
				form.AddField ("longitude", GPSLocationProvider_Xavier.instance.latlong.y.ToString());
				#endif

				form.AddField ("timestamp",  DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
				form.AddField ("message", "");


				using (var w = UnityWebRequest.Post(url, form))
				{
					yield return w.SendWebRequest();
					if (w.isNetworkError || w.isHttpError) {
						//print("[MasterManager] Message not recorded in server: " + messageToSend + " . Reason: " + w.error);
						Debug.LogError ("[MasterManager] Position not recorded in server. Reason: " + w.error);

					} 
				}
			
				counter = 5;
			}

			yield return new WaitForSeconds(1);
		}

		//yield break;
	}
}
