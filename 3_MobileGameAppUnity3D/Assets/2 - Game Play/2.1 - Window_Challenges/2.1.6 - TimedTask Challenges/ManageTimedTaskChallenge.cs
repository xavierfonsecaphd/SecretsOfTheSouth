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

public class ManageTimedTaskChallenge : MonoBehaviour {

	public List<GameObject> panels;
	// 0 - Content Panel
	// 1 - Show Task Panel
	// 2 - Timer panel
	// 3 - HowMany panel
	// 4 - feedback panel

	public List<GameObject> butons;
	// 0 - Go Back Button (when adding a picture and a name to the author)
	// 1 - Vote Button (hide when clicking in the picture of the same player)

	public DisplayManager_FeedBack_TimedTaskWindow_Scene_2 _referenceDisplayManager;
	public Text taskHolder, descriptionHolder, titleHolder, timerCountdownHolder, titleQuestionHowManyHolder;
	public Image imageHolder;
	public InputField answerHowMany;
	public ScrollRect descriptionScrollViewPanel; // this is to put the text of the description always up
	public GameObject feedbackMessagePanel;
	[HideInInspector]
	public TimedTaskChallengeInfo currentTimedTaskChallengeBeingSolved;


	/**
	 * 	olha para o ficheiro ManageVotingChallenge para escreveres este
	 */

	public void ActivateWindow()
	{
		GameManager.instance.isAnyWindowOpen = true;

		MasterManager.LogEventInServer (MasterManager.activePlayerName + " opened the Timed Task Challenge \"" + 
			currentTimedTaskChallengeBeingSolved.name + "\" ["+currentTimedTaskChallengeBeingSolved._id+"].");

		// show original TimedTask challenge window
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		panels [0].SetActive (true);	// set the content panel as active

		// activate all required buttons
		for (int i = 0; i < butons.Count; i++) {
			butons [i].SetActive (true);
		}

		GameManager.instance.timedTaskChallengeWindow.SetActive(true);

		// and now, show the different buttons, to allow for the players to see other players' photos
		// 5 - Show Take Picture Panel 2 (initial button to take picture, should only appear when you didn't solve the challenge)
		// 6 - Show All Pictures Panel 4 (all pictures, should only appear when you solved the
		if (currentTimedTaskChallengeBeingSolved.solved) {
			Debug.Log ("[ManageTimedTaskChallenge] Challenge is solved.");
			//panels[5].SetActive(false);
			//panels[6].SetActive(true);
		} else {
			Debug.Log ("[ManageTimedTaskChallenge] Challenge is not solved.");
			//panels[5].SetActive(true);
			//panels[6].SetActive(false);
		}


		Debug.Log ("[ManageTimedTaskChallenge] Challenge ID:  " + currentTimedTaskChallengeBeingSolved._id);



	}

	private bool interruptClock = false;
	public void InterruptClockCountdown () {
		interruptClock = true;
	}

	public void InitiateTimerCountdown () {
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " started the timer countdown of the Timed Task Challenge \"" + 
			currentTimedTaskChallengeBeingSolved.name + "\" ["+currentTimedTaskChallengeBeingSolved._id+"].");

		interruptClock = false;
		Debug.Log ("[ManageTimedTaskChallenge] Timer Value: " + currentTimedTaskChallengeBeingSolved.timer);
		StartCoroutine (InitiateTimerCountdownThread());
	}
	private IEnumerator InitiateTimerCountdownThread() {
		
		int minutes = currentTimedTaskChallengeBeingSolved.timer;
		int seconds = 0;

		timerCountdownHolder.text = minutes.ToString("00") + " : " + seconds.ToString("00");
		//Debug.Log ("[ManageTimedTaskChallenge] Timer String GUI before while: " + timerCountdownHolder.text + 
		//	" | Min: " + minutes + " - Sec: " + seconds);


		while ((minutes > 0) || (seconds > 0)) {
			// wait before decrementing timer
			yield return new WaitForSeconds (1f);

			// just for the sake the clock was stopped before time
			if (interruptClock) { yield break; }
			// do the math for the timer
			if (seconds > 0) {
				seconds = seconds - 1;
			} else if (minutes > 0) {
				minutes = minutes - 1;
				seconds = 59;
			}

			// update the timer
			timerCountdownHolder.text = minutes.ToString("00") + " : " + seconds.ToString("00");
		}

		// something automatic happens!
		// show The how many question window
		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		panels [3].SetActive (true);	// set the content panel as active


		yield return null;
	}

	public void SolveChallenge () {
		// mark the challenge as solved
		StartCoroutine (SolveChallengeThread (currentTimedTaskChallengeBeingSolved));	
	}
	private IEnumerator SolveChallengeThread (TimedTaskChallengeInfo challenge) {
		string[] displayMessage1 = {"Wait ...", "Wacht ...", "Aguarda um pouco ..."};

		_referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language]);
		feedbackMessagePanel.gameObject.SetActive (true);
		bool errorFlag = false;

		int value = -1;
		if (string.Compare (answerHowMany.text, string.Empty) != 0) {
			// this is validation for the case the player introduces jibberish!
			try {
				value = int.Parse (answerHowMany.text);	
			}
			catch (Exception e) {
				string[] displayMessage2 = {"Please introduce only numbers.", "Voer alleen cijfers in.", "Introduz apenas números, s.f.f.."};
				_referenceDisplayManager.DisplayErrorMessage (displayMessage2[MasterManager.language]);
				feedbackMessagePanel.gameObject.SetActive (true);
				errorFlag = true;
				Debug.Log ("[ManageTimedTaskChallenge] Error trying to read the number: " + e);
			}
			if (errorFlag) {
				yield return new WaitForSeconds (2);
				feedbackMessagePanel.gameObject.SetActive (false);
				yield break;
			}

			// only do something if the value is at least 0
			if (value >= 0) {

				// solve the challenge, and add the extra points the player earned!
				challenge.solved = true;

				for (int i = 0; i < GameManager.instance.timedTaskChallengesOnScreenMeteor.Count; i++) {
					if (string.Compare (GameManager.instance.timedTaskChallengesOnScreenMeteor [i]._id, challenge._id) == 0) {
						GameManager.instance.timedTaskChallengesOnScreenMeteor [i].solved = true;
						break;
					}
				}

				// ****************************************************************
				// attempt to remove the extra unsolved challenge icon on the map
				// TimedTaskChallenge  |   TimedTaskChallengeSolved
				// ****************************************************************
				int index = -1; bool found = false;
				for (int i = GameManager.instance.objectsToDestroyOnLoadScene.Count - 1; i >= 0; i--) {
					if (GameManager.instance.objectsToDestroyOnLoadScene [i].CompareTag ("TimedTaskChallenge")) {
						if (string.Compare (GameManager.instance.objectsToDestroyOnLoadScene [i].GetComponent<TimedTaskChallengeInfo> ()._id, challenge._id) == 0) {
							index = i;
							found = true;
							break;
						}
					}
				}
				if (index == -1) {
					throw new UnityException ("[ManageTimedTaskChallenge] The object of the selected TimedTask challenge was not found on the screen.");
				} else if (found) {
					Destroy (GameManager.instance.objectsToDestroyOnLoadScene[index]);
					GameManager.instance.objectsToDestroyOnLoadScene.RemoveAt (index);
				}

				// generate the new icon as solved on the map
				GameManager.instance.GenerateTimedTaskChallengeSolvedOnScreenMeteor (challenge);


				// attribute the points of solving the challenge
				StartCoroutine (PlayerStatisticsServer.instance.PlayerAttributeRewardsChallengeSolved (challenge._id, value));

				MasterManager.LogEventInServer (MasterManager.activePlayerName + " SOLVED the Timed Task Challenge \"" + 
					currentTimedTaskChallengeBeingSolved.name + "\" ["+currentTimedTaskChallengeBeingSolved._id+"]. In this How Many Question [" 
					+ currentTimedTaskChallengeBeingSolved.questionHowMany + "], the player answered: " + value);

				// show success to the player
				string[] displayMessage3 = {"Good Job, solving the challenge with a Timer! You just got + " + (10 + value) + " points", 
					"Goed gedaan, de uitdaging opgelost met een timer! Je hebt net +" + (10 + value) + "punten gekregen", 
					"Bom trabalho, resolveste o desafio com tempo! Acabaste de ganhar +" + (10 + value) + "pontos"};
				GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage3[MasterManager.language]);

				CloseWindow ();

				yield break;


			} else {
				string[] displayMessage4 = {"Please introduce a value at least 0", "Voer een waarde in van minimaal 0", "Por favor introduz um valor que seja pelo menos 0"};
				_referenceDisplayManager.DisplayErrorMessage (displayMessage4[MasterManager.language]);
				feedbackMessagePanel.gameObject.SetActive (true);
				yield return new WaitForSeconds (2);
				feedbackMessagePanel.gameObject.SetActive (false);
			}


		} else {
			string[] displayMessage5 = {"Please introduce only numbers.", "Voer alleen cijfers in.", "Por favor introduz apenas números."};
			_referenceDisplayManager.DisplayErrorMessage (displayMessage5[MasterManager.language]);
			feedbackMessagePanel.gameObject.SetActive (true);
			yield return new WaitForSeconds (2);
			feedbackMessagePanel.gameObject.SetActive (false);
		}
	}

	public void CloseWindow()
	{
		MasterManager.LogEventInServer (MasterManager.activePlayerName + " closed the Timed Task Challenge \"" + 
			currentTimedTaskChallengeBeingSolved.name + "\" ["+currentTimedTaskChallengeBeingSolved._id+"].");
		
		currentTimedTaskChallengeBeingSolved = null;

		for (int i = 0; i < panels.Count; i++) {
			panels [i].SetActive (false);
		}
		GameManager.instance.timedTaskChallengeWindow.SetActive(false);
		GameManager.instance.isAnyWindowOpen = false;


	}
}
