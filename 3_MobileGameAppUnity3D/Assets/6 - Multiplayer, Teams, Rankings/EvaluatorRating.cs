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
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using PlayFab.Json;
using UnityEngine.Networking;

public class EvaluatorRating : MonoBehaviour {

	public static EvaluatorRating instance { set; get ; }
	// This is a singleton
	private static EvaluatorRating singleton;

	public Sprite blackStar, star;
	public List<Image> buttonsToRateFun = new List<Image>();
	public List<Image> buttonsToRateCollaboration = new List<Image>();
	public List<Image> buttonsToRateParticipation = new List<Image>();
	private bool ratedFun, ratedCollaboration, ratedParticipation;
	private int valueRatedFun, valueRatedCollaboration, valueRatedParticipation;
	private float weightForFun = 3.3f, weightForCollaboration = 3.3f, weightForParticipation = 3.4f; // max rating allowed is 10, so I have to spread the weights per criteria
	public int numberOfTeamsThatSolvedTheChallenge = 0;
	public bool evaluationSuccessfullyDone = false;

	[HideInInspector]
	private int numberOfRatingsDone;

	// Use this for initialization
	void Start () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = EvaluatorRating.singleton;
			if (blackStar == null) throw new System.Exception ("[EvaluatorRating] blackStar is null!");
			if (star == null) throw new System.Exception ("[EvaluatorRating] star is null!");

			evaluationSuccessfullyDone = false;
			ratedFun = false;
			ratedCollaboration = false;
			ratedParticipation = false;
			valueRatedFun = 0;
			valueRatedCollaboration = 0;
			valueRatedParticipation = 0;
		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void DefaultRatings()
	{
		valueRatedFun = 0;
		ratedFun = false;
		valueRatedCollaboration = 0;
		ratedCollaboration = false;
		valueRatedParticipation = 0;
		ratedParticipation = false;

		buttonsToRateFun [0].sprite = blackStar;
		buttonsToRateFun [1].sprite = blackStar;
		buttonsToRateFun [2].sprite = blackStar;
		buttonsToRateFun [3].sprite = blackStar;
		buttonsToRateFun [4].sprite = blackStar;

		buttonsToRateCollaboration [0].sprite = blackStar;
		buttonsToRateCollaboration [1].sprite = blackStar;
		buttonsToRateCollaboration [2].sprite = blackStar;
		buttonsToRateCollaboration [3].sprite = blackStar;
		buttonsToRateCollaboration [4].sprite = blackStar;

		buttonsToRateParticipation [0].sprite = blackStar;
		buttonsToRateParticipation [1].sprite = blackStar;
		buttonsToRateParticipation [2].sprite = blackStar;
		buttonsToRateParticipation [3].sprite = blackStar;
		buttonsToRateParticipation [4].sprite = blackStar;
	}

	// fun_3
	// collaboration_1
	// participation_5
	public void ChangeIconsWhenThouched(string data)
	{
		string[] temporaryArray = data.Split ('_');
		Debug.Log("[EvaluatorRating] found " + temporaryArray.Length + " word count to handle. These are:");
		foreach(string s in temporaryArray)
		{
			Debug.Log(s);
		}

		// ratingsClicked
		if (string.Compare (temporaryArray [0], "fun") == 0) {
			Debug.Log ("Evaluator rated the criterion FUN.");
			ratedFun = true;

			// change stars
			switch (int.Parse(temporaryArray [1])) {
			case 1:
				buttonsToRateFun [0].sprite = star;
				buttonsToRateFun [1].sprite = blackStar;
				buttonsToRateFun [2].sprite = blackStar;
				buttonsToRateFun [3].sprite = blackStar;
				buttonsToRateFun [4].sprite = blackStar;
				valueRatedFun = 1;
				break;
			case 2:
				buttonsToRateFun [0].sprite = star;
				buttonsToRateFun [1].sprite = star;
				buttonsToRateFun [2].sprite = blackStar;
				buttonsToRateFun [3].sprite = blackStar;
				buttonsToRateFun [4].sprite = blackStar;
				valueRatedFun = 2;
				break;
			case 3:
				buttonsToRateFun [0].sprite = star;
				buttonsToRateFun [1].sprite = star;
				buttonsToRateFun [2].sprite = star;
				buttonsToRateFun [3].sprite = blackStar;
				buttonsToRateFun [4].sprite = blackStar;
				valueRatedFun = 3;
				break;
			case 4:
				buttonsToRateFun [0].sprite = star;
				buttonsToRateFun [1].sprite = star;
				buttonsToRateFun [2].sprite = star;
				buttonsToRateFun [3].sprite = star;
				buttonsToRateFun [4].sprite = blackStar;
				valueRatedFun = 4;
				break;
			case 5:
				buttonsToRateFun [0].sprite = star;
				buttonsToRateFun [1].sprite = star;
				buttonsToRateFun [2].sprite = star;
				buttonsToRateFun [3].sprite = star;
				buttonsToRateFun [4].sprite = star;
				valueRatedFun = 5;
				break;
			default:
				valueRatedFun = 0;
				ratedFun = false;
				Debug.LogError ("Evaluator rated the criterion FUN with an invalid value");
				break;
			}
		}
		else if (string.Compare (temporaryArray [0], "collaboration") == 0) {
			Debug.Log ("Evaluator rated the criterion COLLABORATION.");
			ratedCollaboration = true;

			// change stars
			switch (int.Parse(temporaryArray [1])) {
			case 1:
				buttonsToRateCollaboration [0].sprite = star;
				buttonsToRateCollaboration [1].sprite = blackStar;
				buttonsToRateCollaboration [2].sprite = blackStar;
				buttonsToRateCollaboration [3].sprite = blackStar;
				buttonsToRateCollaboration [4].sprite = blackStar;
				valueRatedCollaboration = 1;
				break;
			case 2:
				buttonsToRateCollaboration [0].sprite = star;
				buttonsToRateCollaboration [1].sprite = star;
				buttonsToRateCollaboration [2].sprite = blackStar;
				buttonsToRateCollaboration [3].sprite = blackStar;
				buttonsToRateCollaboration [4].sprite = blackStar;
				valueRatedCollaboration = 2;
				break;
			case 3:
				buttonsToRateCollaboration [0].sprite = star;
				buttonsToRateCollaboration [1].sprite = star;
				buttonsToRateCollaboration [2].sprite = star;
				buttonsToRateCollaboration [3].sprite = blackStar;
				buttonsToRateCollaboration [4].sprite = blackStar;
				valueRatedCollaboration = 3;
				break;
			case 4:
				buttonsToRateCollaboration [0].sprite = star;
				buttonsToRateCollaboration [1].sprite = star;
				buttonsToRateCollaboration [2].sprite = star;
				buttonsToRateCollaboration [3].sprite = star;
				buttonsToRateCollaboration [4].sprite = blackStar;
				valueRatedCollaboration = 4;
				break;
			case 5:
				buttonsToRateCollaboration [0].sprite = star;
				buttonsToRateCollaboration [1].sprite = star;
				buttonsToRateCollaboration [2].sprite = star;
				buttonsToRateCollaboration [3].sprite = star;
				buttonsToRateCollaboration [4].sprite = star;
				valueRatedCollaboration = 5;
				break;
			default:
				valueRatedCollaboration = 0;
				ratedCollaboration = false;
				Debug.LogError ("Evaluator rated the criterion COLLABORATION with an invalid value");
				break;
			}
		}
		else if (string.Compare (temporaryArray [0], "participation") == 0) {
			Debug.Log ("Evaluator rated the criterion PARTICIPATION.");
			ratedParticipation = true;

			// change stars
			switch (int.Parse(temporaryArray [1])) {
			case 1:
				buttonsToRateParticipation [0].sprite = star;
				buttonsToRateParticipation [1].sprite = blackStar;
				buttonsToRateParticipation [2].sprite = blackStar;
				buttonsToRateParticipation [3].sprite = blackStar;
				buttonsToRateParticipation [4].sprite = blackStar;
				valueRatedParticipation = 1;
				break;
			case 2:
				buttonsToRateParticipation [0].sprite = star;
				buttonsToRateParticipation [1].sprite = star;
				buttonsToRateParticipation [2].sprite = blackStar;
				buttonsToRateParticipation [3].sprite = blackStar;
				buttonsToRateParticipation [4].sprite = blackStar;
				valueRatedParticipation = 2;
				break;
			case 3:
				buttonsToRateParticipation [0].sprite = star;
				buttonsToRateParticipation [1].sprite = star;
				buttonsToRateParticipation [2].sprite = star;
				buttonsToRateParticipation [3].sprite = blackStar;
				buttonsToRateParticipation [4].sprite = blackStar;
				valueRatedParticipation = 3;
				break;
			case 4:
				buttonsToRateParticipation [0].sprite = star;
				buttonsToRateParticipation [1].sprite = star;
				buttonsToRateParticipation [2].sprite = star;
				buttonsToRateParticipation [3].sprite = star;
				buttonsToRateParticipation [4].sprite = blackStar;
				valueRatedParticipation = 4;
				break;
			case 5:
				buttonsToRateParticipation [0].sprite = star;
				buttonsToRateParticipation [1].sprite = star;
				buttonsToRateParticipation [2].sprite = star;
				buttonsToRateParticipation [3].sprite = star;
				buttonsToRateParticipation [4].sprite = star;
				valueRatedParticipation = 5;
				break;
			default:
				valueRatedParticipation = 0;
				ratedParticipation = false;
				Debug.LogError ("Evaluator rated the criterion COLLABORATION with an invalid value");
				break;
			}
		}
	}

	public void SubmitRating()
	{
		StartCoroutine (SubmitRatingAsync());
	}

	// StartCoroutine (PlayerStatisticsServer.instance.TeamAttributeRewardsChallengeSolved (challenge._id, teamplayers));
	public IEnumerator SubmitRatingAsync()
	{
		bool alreadyThere = false;
		string url;
		UnityWebRequest request;

		string foreignteamIconToEvaluate = MenuManager.instance.teamIconToEvaluate;
		string foreignteamNameToEvaluate = MenuManager.instance.teamNameToEvaluate;
		string foreignteamIDToEvaluate = MenuManager.instance.teamIDToEvaluate;

		MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
		MenuManager.instance._referenceDisplayManager.DisplaySystemMessageNonFading ("Please Wait.");
		MenuManager.instance._referenceDisplayManager.transform.SetAsLastSibling();

		// confirm that the evaluator filled in all the ratings
		if ((!ratedFun) || (!ratedCollaboration) || (!ratedParticipation)) {
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Please fill in all the ratings and resubmit.");
			yield return new WaitForSeconds (3);
			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);

			yield break;
		}

		// reconfirm that the evaluator still has the permission to evaluate
		yield return GameManager.instance.LoadGamePermissionsForPlayer();

		if (MenuManager.instance.PermissionAsEvaluator ()) {
			Debug.Log ("Evaluator still has permissions to submit rating. Proceeding");


			// Ok, now I need to get the challengeID this last team is being evaluated
			url = MasterManager.serverURL + "/api/evaluatorRateTeamLastChallengeSeen?TeamID=" + foreignteamIDToEvaluate;
			request = UnityWebRequest.Get(url);
			yield return request.SendWebRequest();

			if (request.isNetworkError) {
				Debug.LogError ("[MenuManager] Error While trying to find the last challenge this team saw: " + request.error);
				Debug.LogError ("[MenuManager] URL: " + url);

				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Error with the request. Try to form a new team and restart.");
				yield return new WaitForSeconds (3);
				MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);

				DefaultRatings ();
			}
			else {
				Debug.Log ("Request: " + url);

				string challengeID = JsonWrapper.DeserializeObject<string>(request.downloadHandler.text);
				Debug.Log ("Last challenge seen of this team: " + challengeID);

				/*if (string.Compare (challengeID, string.Empty) == 0) {
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
					MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("This team did not enter a challenge. You cannot rate a team that did not even read the challenge.");
					yield return new WaitForSeconds (3);
					MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);

					MenuManager.instance.UpdateSelectedOption (4);
					DefaultRatings ();
				} else*/ {
					
					// now, with it, I see whether this team already solved the challenge
					PlayerStatisticsServer.instance.GetTitleDataByKey_CloudScript (challengeID);
					while (!PlayerStatisticsServer.instance.callbackReturned) {
						yield return new WaitForSeconds (1);
					}
					List<string> teamsThatSolvedTheChallenge = PlayerStatisticsServer.instance.callbackResult;
					Debug.Log ("Teams that solved the chaLLenge already: ");
					PlayerStatisticsServer.instance.callbackResult = new List<string> ();	// reboot list for next call
					for (int i = 0; i < teamsThatSolvedTheChallenge.Count; i++) {
						Debug.Log (teamsThatSolvedTheChallenge[i]);
						if (string.Compare(teamsThatSolvedTheChallenge [i], foreignteamIDToEvaluate) == 0) {
							alreadyThere = true;
						}
					}
					Debug.Log ("This team already there? " + alreadyThere.ToString());
					numberOfTeamsThatSolvedTheChallenge = teamsThatSolvedTheChallenge.Count;

					// if no, then add this team to playfab server and attribute points to players
					if (!alreadyThere) {
						teamsThatSolvedTheChallenge.Insert (teamsThatSolvedTheChallenge.Count, foreignteamIDToEvaluate);

						yield return PlayerStatisticsServer.instance.AddTeamToMultiplayerChallengePlayFabServerAsync(challengeID, teamsThatSolvedTheChallenge);



						// attributing points to players, and submit the rating
						List<string> teamplayers = new List<string> ();

						// **************************************************
						// first, find out whether anybody joined this team
						// **************************************************

						// valueRatedFun, valueRatedCollaboration, valueRatedParticipation
						// weightForFun = 3.3f, weightForCollaboration = 3.3f, weightForParticipation = 3.4f;
						// I have 5 stars, so potential 5 points per criterion. Now, I need to give a rate from 1 to 10. So i need to normalize the values
						int rating = (int)Mathf.Round( weightForFun * (valueRatedFun/5.0f) + weightForCollaboration * (valueRatedCollaboration/5.0f) + weightForParticipation * (valueRatedParticipation/5.0f));
						url = MasterManager.serverURL + "/api/enterTeamRating?TeamID=" + foreignteamIDToEvaluate + "&TeamName=" + foreignteamNameToEvaluate + "&TeamRefIcon=" + foreignteamIconToEvaluate
							+ "&Rating=" + rating;
						request = UnityWebRequest.Get(url);
						yield return request.SendWebRequest();

						if (request.isNetworkError) {
							Debug.LogError ("[MenuManager] Error While trying to rate a team: " + request.error);
							Debug.LogError ("[MenuManager] URL: " + url);

							MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
							MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Error with the request.");
							yield return new WaitForSeconds (3);
							MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
							DefaultRatings ();
						} else {
							Debug.Log ("Request: " + url);


							// now, update all the members' statistics of this team
							teamplayers = new List<string> ();
							url = MasterManager.serverURL + "/api/numberofplayersinteam?TeamID=" + foreignteamIDToEvaluate;
							request = UnityWebRequest.Get (url);
							yield return request.SendWebRequest ();
							if (request.isNetworkError) {
								Debug.LogError ("[GameManager] Error While Sending: " + request.error);
								Debug.LogError ("[GameManager] URL: " + url);
							} else {
								MenuManager.instance.UpdateSelectedOption (4);

								Debug.Log ("[GameManager] Request with: " + url);
								Debug.Log ("[GameManager] Received: " + request.downloadHandler.text);

								teamplayers = 
									JsonWrapper.DeserializeObject<List<string>> (request.downloadHandler.text);

								evaluationSuccessfullyDone = true;

								yield return PlayerStatisticsServer.instance.TeamAttributeRewardsChallengeSolvedOnlyTeamIDs (challengeID, teamplayers, EvaluatorRating.instance.numberOfTeamsThatSolvedTheChallenge);
								MenuManager.instance.UpdateSelectedOption (4);
								DefaultRatings ();

								MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
								MenuManager.instance._referenceDisplayManager.DisplayGameMessageNonFading ("You rated the team. They can check their score in the leaderboard now. Individual Player statistics will also be updated.");
								yield return new WaitForSeconds (3);
								MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
							}




						}


					} else {
						// if yes, do nothing
						MenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("This team was already rated in the previous task. Let the team enter a new multiplayer challenge for you to rate it.");
						yield return new WaitForSeconds (5);
					}


				}
			
			}




		} else {
			DefaultRatings ();
			Debug.LogError ("Evaluator does not has permission to submit rating. Revoking option.");
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("Your permissions as Evaluator were revoked. Please clarify with the Administrator of the game.");
			yield return new WaitForSeconds (3);
			MenuManager.instance.UpdateSelectedOption (0);
		}
		yield break;
	}
}
