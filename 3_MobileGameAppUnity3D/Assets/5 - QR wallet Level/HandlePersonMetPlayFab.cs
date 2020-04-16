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

using PlayFab;
using PlayFab.ClientModels;

//using FullSerializer;
using PlayFab.Json;

using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class HandlePersonMetPlayFab : MonoBehaviour {

	public bool callbackReturned = true;
	public List<string> callbackResult;


	// Use this for initialization
	void Start () {
		callbackResult = new List<string> ();

	}

	public void GetTitleDataByKey_CloudScript(string searcheableKeyOnServer) {
		callbackReturned = false;
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
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done, searching for the list of peopleMet for " + searcheableKeyOnServer);
				if(success.FunctionResult != null) {

					Debug.Log(JsonWrapper.SerializeObject(success.FunctionResult));
					JsonObject jsonResult = (JsonObject)success.FunctionResult;

					ICollection<object> values = jsonResult.Values;

					int i = 0;
					foreach (object s in jsonResult.Values)
					{
						i ++;
						//Debug.Log("[HandlePersonMetPlayFab] value in jsonResult found. [" + i + "]: " + s.ToString());

						Dictionary<string,string> list = JsonWrapper.DeserializeObject<Dictionary<string,string>>(s.ToString());

						//Debug.Log("[HandlePersonMetPlayFab] Result of CloudScript already as dictionary: " + list.ToStringFull());

						string valuesInJSON = "";
						list.TryGetValue(searcheableKeyOnServer, out valuesInJSON);

						//Debug.Log("[HandlePersonMetPlayFab] List of challenges in JSON: " + valuesInJSON); 

						Dictionary<string,List<string>> listOfValues = JsonWrapper.DeserializeObject<Dictionary<string,List<string>>>(valuesInJSON);

						List<string> challenges;
						if (listOfValues != null)
						{
						Debug.Log("[HandlePersonMetPlayFab] Dictionary count: " + listOfValues.Count);
						listOfValues.TryGetValue(searcheableKeyOnServer,out challenges);
						}
						else
						{
							 challenges = null;
							Debug.Log("[HandlePersonMetPlayFab] NO INFORMATION found!");
						}

						callbackResult = challenges;
						callbackReturned = true;
					}
				}
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}


	// ServerHandleTwoPeopleMeetingUp
	public IEnumerator HandleTwoPeopleMeetingUp(string idThisPlayer, string id2, int isID2Registered) {
		callbackReturned = false;
		//JSON_TitleData_Challenge
		JSON_HandlePeopleMet argObject = new JSON_HandlePeopleMet();
		argObject.ID1 = idThisPlayer;
		argObject.ID2 = id2;
		argObject.isRegistered = isID2Registered;


		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerHandleTwoPeopleMeetingUp", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done on Create an EMPTY List Of People Met In FabServer");
				//if(success.FunctionResult != null) {
				//	Debug.Log("[HandlePersonMetPlayFab] : " + success.FunctionResult);
				//}

				callbackResult = new List<string>();
				callbackResult.Add("HandleTwoPeopleMeetingUpSuccess");
				callbackReturned = true;
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function on Creating List Of People Met In FabServer:" + error.ErrorDetails + "\n" + error.ErrorMessage);

				callbackResult = new List<string>();
				callbackResult.Add("HandleTwoPeopleMeetingUpFailure");
				callbackReturned = true;
			});

		yield return null;
	}


	// with this, I initiate the list in the server of people that this player met so far
	public IEnumerator CreateListOfPeopleMetInFabServer(string playerIDToAddInTheServerPlayFab) {
		callbackReturned = false;
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = playerIDToAddInTheServerPlayFab;
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
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done on Create an EMPTY List Of People Met In FabServer");
				//if(success.FunctionResult != null) {
				//	Debug.Log("[HandlePersonMetPlayFab] : " + success.FunctionResult);
				//}
				callbackReturned = true;
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function on Creating List Of People Met In FabServer:" + error.ErrorDetails + "\n" + error.ErrorMessage);
				callbackReturned = true;
			});

		yield return null;
	}

	public IEnumerator CreateListOfPeopleMetInFabServer(string playerIDToAddInTheServerPlayFab, List<string> foreignPlayer) {
		callbackReturned = false;
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = playerIDToAddInTheServerPlayFab;
		argObject.players = foreignPlayer;
		//argObject.players.Add (""); // log this player in this challenge

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done on Create an EMPTY List Of People Met In FabServer");
				//if(success.FunctionResult != null) {
				//	Debug.Log("[HandlePersonMetPlayFab] : " + success.FunctionResult);
				//}
				callbackReturned = true;
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function on Creating List Of People Met In FabServer:" + error.ErrorDetails + "\n" + error.ErrorMessage);
				callbackReturned = true;
			});

		yield return null;
	}

	public IEnumerator AddPersonToPeopleMetPlayFab(string person, List<string> players) {
		callbackReturned = false;
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = person;
		argObject.players = players;

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done. Player added to the list of players that he knows: " + person + ".");
				//if(success.FunctionResult != null) {
				//	Debug.Log("[HandlePersonMetPlayFab] : " + success.FunctionResult);
				//}
				callbackReturned = true;
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function AddPersonToPeopleMetPlayFab:" + error.ErrorDetails + "\n" + error.ErrorMessage);
				callbackReturned = true;
			});

		yield return null;
	}


	public IEnumerator ServerUpdateStatisticsNumberOfPeopleMetOfGivenPlayer(string person) {
		callbackReturned = false;
		//JSON_TitleData_Challenge
		JSON_TitleData_RegisteredPlayerStatisticsUpdate argObject = new JSON_TitleData_RegisteredPlayerStatisticsUpdate();
		argObject.playfabID = person;

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerUpdateStatisticsNumberOfPeopleMetOfGivenPlayer", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[HandlePersonMetPlayFab] CloudScript Done. This other Registered player got updated in the PeopleMet statistics: " + person + ".");
				//if(success.FunctionResult != null) {
				//	Debug.Log("[HandlePersonMetPlayFab] : " + success.FunctionResult);
				//}
				callbackReturned = true;
			}, 
			error => {
				Debug.Log("[HandlePersonMetPlayFab] There was error in the Cloud Script function ServerUpdateStatisticsNumberOfPeopleMetOfGivenPlayer:" + error.ErrorDetails + "\n" + error.ErrorMessage);
				callbackReturned = true;
			});

		yield return null;
	}


	public IEnumerator IncrementNumberOfPeopleMetStatisticsPlayFab()
	{
		callbackReturned = false;
		// ******************************************************************************************************************
		// Then, ask for the statistics and add to them in the success callback
		// ******************************************************************************************************************
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest ();
		List<string> statisticNames = new List<string> (1);
		statisticNames.Add ("PeopleMet");
		request.StatisticNames = statisticNames;	// you are going to attribute Gold per each challenge solved

		PlayFabClientAPI.GetPlayerStatistics (request, success => {
			//Debug.Log ("[HandlePersonMetPlayFab] Quantity of stats: " + success.Statistics.Count);
			if (success.Statistics.Count > 0) {
				UpdatePlayerStatisticsRequest updateRequest = new UpdatePlayerStatisticsRequest ();
				updateRequest.Statistics = new List<StatisticUpdate> (success.Statistics.Count);	// create an update for all the statistics


				foreach (StatisticValue stat in success.Statistics) {
					if (string.Compare (stat.StatisticName, "PeopleMet") == 0) {
						//gold = true;	// the statistic already exist on the server; great

						StatisticUpdate statToAdd = new StatisticUpdate ();

						statToAdd.StatisticName = "PeopleMet";
						statToAdd.Value = stat.Value + 1;	// add a known person to the pool of people you met so far
						statToAdd.Version = stat.Version; // the existent version of the value

						updateRequest.Statistics.Add (statToAdd);
					}

					// possibly increment other stats like badges
				}
				// ******************************************************************************************************************
				// Do the actual update with the statistic defined
				// ******************************************************************************************************************
				PlayFabClientAPI.UpdatePlayerStatistics (updateRequest, PlayerAttributeRewardsChallengeSolvedSuccess => {
					Debug.Log ("[HandlePersonMetPlayFab] Success in attributing rewards to the player. ");

					callbackReturned = true;

				}, PlayerAttributeRewardsChallengeSolvedFail => {
					PlayFabErrorCode errorcode = PlayerAttributeRewardsChallengeSolvedFail.Error;
					Debug.LogError ("[HandlePersonMetPlayFab] Updating Statistics of the Player error: " + errorcode);
					Debug.LogError ("[HandlePersonMetPlayFab] Full report: " + PlayerAttributeRewardsChallengeSolvedFail.GenerateErrorReport ());
					callbackReturned = true;
				});
			}
		}, getPlayerStatisticsError => {
			PlayFabErrorCode errorcode = getPlayerStatisticsError.Error;
			Debug.LogError ("[PlayerStatisticsServer] Updating Statistic -> error: " + errorcode);
			Debug.LogError ("[PlayerStatisticsServer] Full report: " + getPlayerStatisticsError.GenerateErrorReport ());
			callbackReturned = true;
		});
		yield return null;
	}
}
