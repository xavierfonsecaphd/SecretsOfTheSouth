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

public class ManageOpenQuizChallenge : MonoBehaviour {

	public List<GameObject> panels;
	// 0 - Content Panel
	// 1 - Show Question Panel
	// 2 - Show Answer Panel
	// 3 - feedback panel

	//public List<GameObject> butons;
	// 0 - Go Back Button (when adding a picture and a name to the author)
	// 1 - Vote Button (hide when clicking in the picture of the same player)

	public DisplayManager_FeedBack_OpenQuizWindow_Scene_2 _referenceDisplayManager;
	public Text questionHolder, descriptionHolder, titleHolder;
	public Image imageHolder;
	public InputField answer;
	public ScrollRect descriptionScrollViewPanel; // this is to put the text of the description always up
	public GameObject feedbackMessagePanel;
	[HideInInspector]
	public OpenQuizChallengeInfo currentOpenQuizChallengeBeingSolved;


	/**
	 * 	olha para o ficheiro ManageVotingChallenge para escreveres este
	 */

	public void ActivateWindow()
	{
		GameManager.instance.isAnyWindowOpen = true;

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Open Quiz Challenge \"" + 
			currentOpenQuizChallengeBeingSolved.name + "\" ["+currentOpenQuizChallengeBeingSolved._id+"]");

		// show original Open Quiz challenge window
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		panels [0].SetActive (true);	// set the content panel as active

		// activate all required buttons
		/*for (int i = 0; i < butons.Count; i++) {
			butons [i].SetActive (true);
		}*/

		GameManager.instance.openQuizChallengeWindow.SetActive(true);

		// and now, show the different buttons, to allow for the players to see other players' photos
		// 5 - Show Take Picture Panel 2 (initial button to take picture, should only appear when you didn't solve the challenge)
		// 6 - Show All Pictures Panel 4 (all pictures, should only appear when you solved the
		if (currentOpenQuizChallengeBeingSolved.solved) {
			Debug.Log ("[ManageOpenQuizChallenge] Open Quiz Challenge is solved.");
			//panels[5].SetActive(false);
			//panels[6].SetActive(true);
		} else {
			Debug.Log ("[ManageOpenQuizChallenge] Open Quiz Challenge is not solved.");
			//panels[5].SetActive(true);
			//panels[6].SetActive(false);
		}


		Debug.Log ("[ManageOpenQuizChallenge] Challenge ID:  " + currentOpenQuizChallengeBeingSolved._id);



	}



	public void SolveChallenge () {
		// mark the challenge as solved
		StartCoroutine (SolveChallengeThread (currentOpenQuizChallengeBeingSolved));	
	}
	private IEnumerator SolveChallengeThread (OpenQuizChallengeInfo challenge) {
		string[] displayMessage1 = {"Wait ...", "Wacht ...", "Aguarda um momento ..."};

		string url;
		string urlPrefix = "";
		urlPrefix = MasterManager.serverURL;
		//urlPrefix = "localhost:3000";
		url = urlPrefix + "/api/emailopenquizanswer";

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Open Quiz Challenge \"" + 
			currentOpenQuizChallengeBeingSolved.name + "\" ["+currentOpenQuizChallengeBeingSolved._id+"]");
	
		string content = "The player " + MasterManager.activePlayerName + " [" + MasterManager.activePlayFabId + "] has solved the challenge "
			+ currentOpenQuizChallengeBeingSolved.name + " [" + currentOpenQuizChallengeBeingSolved._id + "]. \n\nHis/her answer to this challenge is: \n\n\n\n\n\n\n\n"
			+ answer.text;
	
		WWWForm form = new WWWForm ();
		form.AddField ("message", content);

		using (var w = UnityWebRequest.Post(url, form))
		{
			yield return w.SendWebRequest();
			if (w.isNetworkError || w.isHttpError) {
				print(w.error);
				string[] displayMessage3 = {"Something went wrong, and your message could not be sent. Can you please try again?", 
					"Er is iets misgegaan en uw bericht kan niet worden verzonden. Kun je alsjeblieft opnieuw proberen?", 
					"Passou-se alguma coisa de errado, e a tua resposta não foi enviado. Podes tentar de novo por favor?"};
				GameManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage3[MasterManager.language]);
				//yield break;
			}
			else {
				//print("Finished sending the email. Answer: " + w.downloadHandler.text);
				Debug.Log ("Finished sending the email. Answer: " + w.downloadHandler.text);

			}
		}

		/**
		 *  I want this piece of code to work, regardless of whether the email gets sent or not!
		 */
		_referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language]);
		feedbackMessagePanel.gameObject.SetActive (true);
		// solve the challenge, and add the extra points the player earned!
		challenge.solved = true;

		for (int i = 0; i < GameManager.instance.openQuizChallengesOnScreenMeteor.Count; i++) {
			if (string.Compare (GameManager.instance.openQuizChallengesOnScreenMeteor [i]._id, challenge._id) == 0) {
				GameManager.instance.openQuizChallengesOnScreenMeteor [i].solved = true;
				break;
			}
		}

		// ****************************************************************
		// attempt to remove the extra unsolved challenge icon on the map
		// TimedTaskChallenge  |   TimedTaskChallengeSolved
		// ****************************************************************
		int index = -1; bool found = false;
		List <int> indexesToRemove = new List<int> (0);
		try {
			for (int i = GameManager.instance.objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
				if (GameManager.instance.objectsToDestroyOnLoadScene [i] != null){
					if (GameManager.instance.objectsToDestroyOnLoadScene [i].CompareTag ("OpenQuizChallenge")) {
						if (string.Compare (GameManager.instance.objectsToDestroyOnLoadScene [i].GetComponent<OpenQuizChallengeInfo> ()._id, challenge._id) == 0) {
							index = i;
							indexesToRemove.Add (i);
							found = true;
							//break;
						}
					}
				}
			}
			if (index == -1) {
				throw new UnityException ("[ManageOpenQuizChallenge] The object of the selected OpenQuiz challenge was not found on the screen.");
			} else if (found) {
				for (int x = 0; x < indexesToRemove.Count; x++) {
					//GameManager.instance.objectsToDestroyOnLoadScene[indexesToRemove[x]].SetActive(false);
					Destroy (GameManager.instance.objectsToDestroyOnLoadScene[indexesToRemove[x]]);
					GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (indexesToRemove[x]);
				}
				//Destroy (GameManager.instance.objectsToDestroyOnLoadScene[index]);
				//GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (index);
			}
		}
		catch (Exception e) {
			Debug.LogError ("[ManageOpenQuizChallenge] Exception while trying to remove a 3D icon: " + e.Message);
		}




		// generate the new icon as solved on the map
		GameManager.instance.GenerateOpenQuizChallengeSolvedOnScreenMeteor (challenge);


		// attribute the points of solving the challenge
		StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge._id, 0));


		// show success to the player
		string[] displayMessage4 = {"Good Job, giving your answer to the challenge! You just got + " + 10 + " points", 
			"Goed gedaan, je antwoord geven op de uitdaging! Je hebt net +" + 10 + "punten gekregen", 
			"Bom trabalho, ao responder a este desafio! Acabaste de ganhar +" + 10 + "pontos"};
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage4[MasterManager.language]);

		CloseWindow ();



		yield break;
	}



	public void CloseWindow()
	{
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Open Quiz Challenge \"" + 
			currentOpenQuizChallengeBeingSolved.name + "\" ["+currentOpenQuizChallengeBeingSolved._id+"]");
		
		currentOpenQuizChallengeBeingSolved = null;

		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		GameManager.instance.openQuizChallengeWindow.SetActive(false);
		GameManager.instance.isAnyWindowOpen = false;


	}
}
