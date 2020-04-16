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
using Mapbox.Unity.Utilities;
using Mapbox.Map;
using Mapbox.Utils;

using UnityEngine.SceneManagement;


public class GPSLocationProvider_Xavier : MonoBehaviour
{
	public GameToolkit.Localization.LocalizedText restartPopup;

	//public double latitude = 0.0f, longitude = 0.0f;
	[HideInInspector]
	public Vector2d latlong;

	[HideInInspector]
	public static GPSLocationProvider_Xavier instance { set; get ; }

	private static GPSLocationProvider_Xavier singleton;	// This is a singleton

	public bool initialized = false;


	[HideInInspector]
	public bool gotGPSLocation = false;

	// get the GPS from my Phone.
	// 1) I need the phone's permission, I have no Idea how to get that
	// 2) I need to send my phone a request to start using the GPS. This process will take time, 
	//			I cannot do it all in the Start(), so I'll need a co-routine
	void Awake()
	{
		if (singleton == null) 
		{
			singleton = this;
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

			instance = GPSLocationProvider_Xavier.singleton;

			latlong = new Vector2d (-1.0f, -1.0f);

			//initiateGPS ();
		} 
		else if (singleton != this) 
		{
			throw new UnityException ("[GPSlocationprovider] there was another creation of this object! verify!");
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			//Destroy (gameObject);    
		}
	}

	void Start()
	{
		initialized = true;
	}
		
	public void initiateGPS()
	{
		// First, check if user has location service enabled
		#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		if (Input.location.isEnabledByUser) {

		StartCoroutine(StartGPSLocationServiceOnThePhone());
		}
		else 
		{	// the player only plays when he has got the positioning permission enabled.
		throw new UnityException ("[GPSLocationProvider_Xavier] Location was not enabled by the player. Quitting.");
		}
		#endif

		#if UNITY_EDITOR
		StartCoroutine (InitUnityRemoteOnSmartphone ());
		#endif
	}


	// Update is called once per frame
	void Update () 
	{	
		if (this.gotGPSLocation) {
			// I can only update these coordinates on the real device. If I do this inside UNITY Editor, latlong will become zero
			#if !UNITY_EDITOR
			//Scene currentScene = SceneManager.GetActiveScene ();

			// update the latitude and longitude values, for as long as the GPS on the smartphone is RUNNING, and for as long as the player is walking

			if (GameManager.instance != null)
			{
				//if (GameManager.instance._referencePlayer.GetComponent<PlayerController>().areYouWalking)
				//{
					latlong.x = Input.location.lastData.latitude;
					latlong.y = Input.location.lastData.longitude;
				
				//}
				
			}
			
			#endif
		} 


	}

	public IEnumerator StartGPSLocationServiceOnThePhone()
	{
		Input.location.Stop();

		Debug.Log ("[GPSLocationProvider_Xavier] Launching Coroutine outside UNITY_EDITOR - initiateGPSLocationService");
		yield return StartCoroutine("initiateGPSLocationService");


		Debug.Log("[GPSLocationProvider_Xavier]initiateGPSLocationService finished!");

		yield break;
	}
	private IEnumerator initiateGPSLocationService()
	{
		// txt7
		string [] displayMessage1 = {"Let's initiate the GPS ...",
			"Laten we de GPS starten ...",
			"A iniciar a localização GPS ..."};
		
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Let's initiate the GPS..." );
		Debug.Log ("[GPSLocationProvider_Xavier] Let's initiate the GPS...");


		// so, apparently we have the permission to start
		Input.location.Start (1.0f,0.1f);

		int maxWait = 20;

		Debug.Log ("[GPSLocationProvider_Xavier] GPS Status: " + Input.location.status.ToString());
		string [] displayMessage2 = {"Initiating the GPS and\n trying to locate you ... ",
			"De GPS activeren en\n jouw locatie vast stellen ... ",
			"A iniciar a localização GPS\n e a tentar localizar-te ... "};

		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {


			OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[MasterManager.language] + maxWait, true);
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Initiating the GPS and\n trying to locate you... " + maxWait);
			yield return new WaitForSeconds (1);
			maxWait--;
		}
		Debug.Log ("[GPSLocationProvider_Xavier] GPS Status: " + Input.location.status.ToString());
		if ((maxWait <= 0 ) || (Input.location.status == LocationServiceStatus.Failed)) {
			Debug.Log ("[GPSLocationProvider_Xavier] Waiting for the GPS location timed out. Unable to determine device locatio.");
			//txt9
			string [] displayMessage3 = {"Unable to determine device location, the GPS is not working. Try again!",
				"Kan de locatie niet bepalen, de GPS werkt niet. Probeer het opnieuw!",
				"Foi impossível descobrir a tua localização, o GPS não funcionou. Tenta de novo!"};
			
			OGLoadingOverlay.ShowFullcoverLoading(displayMessage3[MasterManager.language], true);
			// txt10
			string [] displayMessage4 = {"Unable to determine device location",
				"Kan de apparaatlocatie niet bepalen",
				"Foi impossível descobrir a tua localização"};
			string [] displayMessage5 = {"The game could not get your GPS location. Please enable the GPS location of the smartphone, and close this to try again.",
				"Het spel kan je GPS-locatie niet krijgen. Schakel de GPS-locatie van de smartphone in en sluit deze om het opnieuw te proberen.",
				"O jogo não conseguiu aceder à tua localização. Por favor ativa a localização GPS do teu telemóvel, e fecha esta janela para tentar de novo."};
			// closeButtonText
			OGPopup.GetInstance().closeButtonText = restartPopup;
			OGPopup.ShowNotificationPopup (displayMessage4[MasterManager.language],displayMessage5[MasterManager.language], ()=> {StartCoroutine(initiateGPSLocationService());});
			//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Unable to determine device location, the GPS is not working. You cannot play the game :(");
			yield break;
		}


		// in here, you do not advance until you have coordinates
		yield return StartCoroutine (awaitForServiceRunning());

		latlong.x = Input.location.lastData.latitude;
		latlong.y = Input.location.lastData.longitude;


		if ((latlong.x == 0.0f) &&
			(latlong.y == 0.0f)) {

			throw new UnityException ("[GPSLocationProvider_Xavier] you got coordinates, but these keep being zero?!");
		}

		//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Location found." );
		Debug.Log("Location found.");
		// Ask the game master to continue with the setting up of the game play (yes, this is an asynchronous approach)
		// This is so because initiating the GPS Location service on the Device really takes a long time

		MasterManager.instance.ContinueGameInitializationAfterGPS();

		gotGPSLocation = true;

		yield break;
	}


	private IEnumerator awaitForServiceRunning()
	{
		// txt11
		string [] displayMessage1 = {"Waiting for the GPS ...",
			"Wachten op de GPS ...",
			"À espera das coordenadas GPS ..."};
		
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Waiting for the GPS ..." );
		Debug.Log ("[GPSLocationProvider_Xavier] Waiting for the GPS ...");
		int count = 20;
		while (count > 0) {
			if (Input.location.status == LocationServiceStatus.Running) 
			{	
				Debug.Log ("[GPSLocationProvider_Xavier] GPS is RUNNING!");
				yield break;
			}

			count--;
				
			yield return new WaitForSeconds (1);
		}

		if (count < 0)
			throw new UnityException ("[GPSLocationProvider_Xavier] Timeout for getting GPS location service");
	}

	#if UNITY_EDITOR
	public IEnumerator InitUnityRemoteOnSmartphone()
	{
		
		//Wait until Unity connects to the Unity Remote
		//		Debug.Log("[GPSLocationProvider_Xavier] InitUnityRemoteOnSmartphone");
		//while (!UnityEditor.EditorApplication.isRemoteConnected)
		//{
		//	yield return new WaitForSeconds (1);
		//}

		//Debug.Log ("[GPSLocationProvider_Xavier] Location Status : " + Input.location.status);
		// in case I am using the Unity Remote on the smartphone, I need to wait until there is a connection to advance

		//Wait until Unity connects to the Unity Remote
		//while (UnityEditor.EditorApplication.isRemoteConnected)
		//{
		//		yield return new WaitForSeconds (1);
		//}

		//	Debug.Log ("[GPSLocationProvider_Xavier] Running in UNITY_EDITOR, therefore we are forcing coordinates (no GPS available from the smartphone)");
		latlong.x = 52.0023751398296; // 51.92442;
		latlong.y = 4.36814449999126; //4.477733;

		//txt12
		string [] displayMessage2 = {"Fake GPS used",
			"Fake GPS gebruikt",
			"Coordenadas GPS simuladas"};
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage2[MasterManager.language], true);
		//OGPopup.ShowNotificationPopup ("Unable to determine device location","The game could not get your GPS location. Please enable the GPS location of the smartphone, and close this to try again.", ()=> {StartCoroutine(InitUnityRemoteOnSmartphoneAfterPopUp());});
		//Debug.Log("[GPSLocationProvider_Xavier] Fake GPS used.");
		MasterManager.instance.ContinueGameInitializationAfterGPS();

		gotGPSLocation = true;
		yield return null;
	}

	#endif
}
