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


// this class is meant to deal with the playfab server, and interact with all statistics, leaderboards, players, and related content
public class PlayerStatisticsServer : MonoBehaviour {

	public static PlayerStatisticsServer instance { set; get ; }
	private static PlayerStatisticsServer singleton;	// This is a singleton

	//public int gold;	// player gold statistic
	[HideInInspector]
	public List<int> playerStats;	
	// 0 - gold; 
	// 1 - Challenges created; 
	// 2 - First to Solve Amount; 
	// 3 - Challenges Solved; 
	// 4 - People Met; 
	// 5 - Team Challenges Solved; 
			
	[HideInInspector]
	public List<Sprite> playersImages;	
	[HideInInspector]
	public List<string> playersIDsForTheImages;	
	[HideInInspector]
	public bool playersImagesReady = false;	


	public List<PlayerLeaderboardEntry> leaderboardPeopleMet;

	private List<string> statisticsOnline; // these are the statistics that all players need to have defined in their profile at PlayFab. At the beginning, the game will check whether these
											// statistics are even created for the player. If not, it will be created.

	public bool callbackReturned = true;
	public List<string> callbackResult;

	// Use this for initialization
	void Start () {
		if (singleton == null) 
		{
			singleton = this;
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = PlayerStatisticsServer.singleton;

			callbackResult = new List<string> ();

			playerStats = new List<int>();	
			playerStats.Add(0);	// gold
			playerStats.Add(0);	// Challenges created	(later on, you better initialize this with the server)
			playerStats.Add(0);	// First to Solve		
			playerStats.Add(0);	// Challenges solved	
			playerStats.Add(0);	// People Met			(later on, you better initialize this with the server)
			playerStats.Add(0);	// Team Challenges Solved

			statisticsOnline = new List<string> ();
			statisticsOnline.Add ("Gold");
			statisticsOnline.Add ("ChallengesCreated");
			statisticsOnline.Add ("FirstToSolve");
			statisticsOnline.Add ("ChallengesSolved");
			statisticsOnline.Add ("PeopleMet");
			statisticsOnline.Add ("TeamChallengesSolved");

			playersImages = new List<Sprite> ();
			playersIDsForTheImages = new List<string> ();

			LeaderboardGetPlayersWithChallengesSolved ();


			SetUpStatistics ();
		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	private void SetUpStatistics()
	{

		List<bool> statisticsFound = new List<bool> (statisticsOnline.Count);
		for (int i = 0; i < statisticsOnline.Count; i++) {
			statisticsFound.Add (false);
		}

		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest ();

		PlayFabClientAPI.GetPlayerStatistics (request, successResult => 
			{
				GetPlayerStatisticsResult result = (GetPlayerStatisticsResult) successResult;

				//Debug.Log ("[PlayerStatisticsServer] Quantity of stats initially found: " + result.Statistics.Count);
				int count = 0;
				foreach (StatisticValue stat in result.Statistics) {
					//Debug.Log ("[PlayerPanel] " + stat.StatisticName);
					//Debug.Log ("[PlayerPanel] Value: " + stat.Value);
					//goldAmountBox.text = stat.Value.ToString();
					//gold = stat.Value;

					for (int i = 0; i < statisticsOnline.Count; i++) {
						if (string.Compare (stat.StatisticName, statisticsOnline[i]) == 0) {
							statisticsFound[i] = true;
							count ++;
						}
					} 
				}

				if (count == statisticsOnline.Count)
				{
					Debug.Log("[PlayerStatisticsServer] Found all Statistics.");
				}
				else
				{
					Debug.Log("[PlayerStatisticsServer] Found " + count + " statistics out of " + statisticsOnline.Count + ". Setting up the lacking statistics for the player.");
					int quantityOfStatsToCreate = statisticsOnline.Count - count;
					UpdatePlayerStatisticsRequest setUpStatisticsRequest = new UpdatePlayerStatisticsRequest ();
					setUpStatisticsRequest.Statistics = new List<StatisticUpdate> (quantityOfStatsToCreate);
					StatisticUpdate stat;

					for (int j = 0; j < statisticsOnline.Count; j ++)
					{
						// if we didn't find this statistic, then we include it in the request the request 
						if (!statisticsFound[j])
						{
							Debug.Log("[PlayerStatisticsServer] setting up the stat: " + statisticsOnline[j]);
							stat = new StatisticUpdate ();
							stat.StatisticName = statisticsOnline[j];
							stat.Value = 0;
							setUpStatisticsRequest.Statistics.Add (stat);
						}
					}

					// request all the missing statistics.
					PlayFabClientAPI.UpdatePlayerStatistics(setUpStatisticsRequest, successSetUpStatistics => 
						{
							Debug.Log ("[PlayerStatisticsServer] Success in setting up " + quantityOfStatsToCreate + " statistics for the player.");
						}, errorSetUpStatistics => 
						{
							Debug.LogError ("[PlayerStatisticsServer] I could not set the lacking " + quantityOfStatsToCreate + " Statistics for the Player. I got this code: "+errorSetUpStatistics);
							Debug.LogError ("[PlayerStatisticsServer] Full report: "+errorSetUpStatistics.GenerateErrorReport());
						});
				}



				// set up the routine for the update of leaderboard

				// just to start the game, and be faster at loading the statistics of the player in the menu
				LoadPlayerStatistics ();

				// delete this
				//LeaderboardGetPlayersWithChallengesSolved ();

				StartCoroutine (UpdateLeaderboard ());


			}
			, error=> 
			{
				// PlayFabError error
				PlayFabErrorCode errorcode = error.Error;
				Debug.LogError ("[PlayerStatisticsServer] I could not get the existent Statistics of the Player. I got this code: "+errorcode);
				Debug.LogError ("[PlayerStatisticsServer] Full report: "+error.GenerateErrorReport());
			});

	}



	private IEnumerator UpdateLeaderboard()
	{
		Debug.Log ("[PlayerStatisticsServer] CoRoutine Running.");

		// update the leaderboard every minute
		while (true) {
			LeaderboardGetPlayersWithChallengesSolved ();
			yield return new WaitForSeconds (5);	// each 10 seconds, ask for an updated version of the leaderboard.

			// get player statistics from the server
			LoadPlayerStatistics ();

			yield return new WaitForSeconds (5);	// each 5 seconds, ask for an updated version of the player statistics.
		}
	}

	// Load the statistics of the player at the beginning of the game
	public void LoadPlayerStatistics()
	{
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest ();

		PlayFabClientAPI.GetPlayerStatistics (request, LoadPlayerStatisticsSuccess, LoadPlayerStatisticsFail);
	}
	private void LoadPlayerStatisticsSuccess(GetPlayerStatisticsResult result)
	{
		//Debug.Log ("[PlayerStatisticsServer] Quantity of stats: " + result.Statistics.Count);

		foreach (StatisticValue stat in result.Statistics) {
			//Debug.Log ("[PlayerPanel] " + stat.StatisticName);
			//Debug.Log ("[PlayerPanel] Value: " + stat.Value);
			//goldAmountBox.text = stat.Value.ToString();
			//gold = stat.Value;

			if (string.Compare (stat.StatisticName, "Gold") == 0) {
				playerStats[0] = stat.Value;	// set the gold
			} else if (string.Compare (stat.StatisticName, "ChallengesCreated") == 0) {
				playerStats[1] = stat.Value;	// set the gold
			} else if (string.Compare (stat.StatisticName, "ChallengesSolved") == 0) {
				playerStats[3] = stat.Value;	// set the challenges solved
			} else if (string.Compare (stat.StatisticName, "PeopleMet") == 0) {
				playerStats[4] = stat.Value;	// set the challenges solved
			}
			else if (string.Compare (stat.StatisticName, "FirstToSolve") == 0) {
				playerStats[2] = stat.Value;	// set the challenges solved
			}
			else if (string.Compare (stat.StatisticName, "TeamChallengesSolved") == 0) {
				playerStats[5] = stat.Value;	// set the challenges solved
			}

			//Debug.Log ("[PlayerPanel] V.: " + stat.Version);
		}
	}
	private void LoadPlayerStatisticsFail(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[PlayerStatisticsServer] I could not get the existent Statistics of the Player. I got this code: "+errorcode);
		Debug.LogError ("[PlayerStatisticsServer] Full report: "+error.GenerateErrorReport());
	}


	//ServerSaveChallengeAndPlayersWhoSolvedIt
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
				Debug.Log("[MenuManager] CloudScript Done");
				if(success.FunctionResult != null) {

					Debug.Log(JsonWrapper.SerializeObject(success.FunctionResult));
					JsonObject jsonResult = (JsonObject)success.FunctionResult;

					ICollection<object> values = jsonResult.Values;

					int i = 0;
					foreach (object s in jsonResult.Values)
					{
						i ++;
						Debug.Log("value in jsonResult found. [" + i + "]: " + s.ToString());

						Dictionary<string,string> list = JsonWrapper.DeserializeObject<Dictionary<string,string>>(s.ToString());

						Debug.Log("Result of CloudScript already as dictionary: " + list.ToString());

						string challengesInJSON = "";
						list.TryGetValue(searcheableKeyOnServer, out challengesInJSON);

						Debug.Log("List of challenges in JSON: " + challengesInJSON); 

						Dictionary<string,List<string>> listOfChallenges = JsonWrapper.DeserializeObject<Dictionary<string,List<string>>>(challengesInJSON);

						if (listOfChallenges != null)
						{
							Debug.Log("Dictionary count: " + listOfChallenges.Count);

							List<string> challenges;
							listOfChallenges.TryGetValue(searcheableKeyOnServer,out challenges);


							callbackResult = challenges;
							callbackReturned = true;
						}
						else {
							callbackResult = new List<string>(0);
							callbackReturned = true;
						}

					}
				}
			}, 
			error => {
				Debug.Log("There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}

	// attribute rewards when a player solves a challenge
	public IEnumerator PlayerAttributeRewardsChallengeSolved(string challengeID, int extraPoints = 0)
	{
		bool ftf = false;

		// ******************************************************************************************************************
		// First of all, let's search for this challenge: who solved it? Was this challenge solved already by this player?
		// ******************************************************************************************************************

		// after attributing these rewards, let's seee whether the challenge was solved alread. If not, let's attribute the reward "First to Solve"


		Debug.Log ("challenge ID used: " + challengeID);
		GetTitleDataByKey_CloudScript (challengeID);
		int count = 20;
		while ((!callbackReturned) && (count > 0)) {
			yield return new WaitForSeconds (1);
			count--;
		}
		if (count <= 0) {throw new UnityException ("[PlayerStatisticsServer] This shit crashed while waiting for callback!");}
		List<string> playersThatSolvedTheChallenge = callbackResult;
		callbackResult = new List<string> ();	// reboot list for next call

		bool alreadyThere = false;
		// I need to double check whether the player is already in this list
		for (int i = 0; i < playersThatSolvedTheChallenge.Count; i++) {
			if (playersThatSolvedTheChallenge [i] == MasterManager.activePlayFabId) {
				alreadyThere = true;
			}
		}
		Debug.Log ("is it already there? " + alreadyThere.ToString());

		// if the player is already in this list, do not double attribute rewards
		if (!alreadyThere) {
			

			// ******************************************************************************************************************
			// Then, ask for the statistics and add to them in the success callback
			// ******************************************************************************************************************
			GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest ();
			List<string> statisticNames = new List<string> (3);
			statisticNames.Add ("Gold");
			statisticNames.Add ("ChallengesSolved");
			//statisticNames.Add ("PeopleMet");
			statisticNames.Add ("FirstToSolve");
			request.StatisticNames = statisticNames;	// you are going to attribute Gold per each challenge solved


			PlayFabClientAPI.GetPlayerStatistics (request, success => 
				{
					//Debug.Log ("[PlayerStatisticsServer] Quantity of stats: " + success.Statistics.Count);
					if (success.Statistics.Count > 0) {
						UpdatePlayerStatisticsRequest updateRequest = new UpdatePlayerStatisticsRequest ();
						updateRequest.Statistics = new List<StatisticUpdate> (success.Statistics.Count);	// create an update for all the statistics


						foreach (StatisticValue stat in success.Statistics) {
							// increment the stat "Gold"
							if (string.Compare (stat.StatisticName, "Gold") == 0) {
								//gold = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "Gold";
								statToAdd.Value = stat.Value + 10 + extraPoints;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} else if (string.Compare (stat.StatisticName, "ChallengesSolved") == 0) {
								//challengesSolved = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "ChallengesSolved";
								statToAdd.Value = stat.Value + 1;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} 
							else if (string.Compare (stat.StatisticName, "FirstToSolve") == 0) {
	
								// this statistic sees whether there are other players that solved the challenge. If not, then attribute a first to find as well

								if ((playersThatSolvedTheChallenge.Count == 0)) {
									StatisticUpdate statToAdd = new StatisticUpdate ();

									statToAdd.StatisticName = "FirstToSolve";
									statToAdd.Value = stat.Value + 1;	// add 50 coins of gold to the user
									statToAdd.Version = stat.Version; // the existent version of the value
									updateRequest.Statistics.Add (statToAdd);
									ftf = true;
								}
							}  
							/*else if (string.Compare (stat.StatisticName, "PeopleMet") == 0) {
								//gold = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "PeopleMet";
								statToAdd.Value = stat.Value + numberOfPeopleFound;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} */

							// possibly increment other stats like badges
						}


						// ******************************************************************************************************************
						// Add the player to the list of players that solved that particular challenge
						// ******************************************************************************************************************
						playersThatSolvedTheChallenge.Add (MasterManager.activePlayFabId);
						AddPlayerToChallengePlayFabServer (challengeID, playersThatSolvedTheChallenge);
	
						if (ftf)
						{
							StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Challenge_Solved_First_To_Find_->_"+challengeID));

						}
						else
						{
							StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Challenge_Solved_->_"+challengeID));
						}
						// ******************************************************************************************************************
						// Do the actual update with all the statistics you defined
						// ******************************************************************************************************************
						PlayFabClientAPI.UpdatePlayerStatistics(updateRequest,PlayerAttributeRewardsChallengeSolvedSuccess =>
							{
								Debug.Log ("[PlayerStatisticsServer] Success in attributing rewards to the player. ");
								LoadPlayerStatistics ();	// reload all stats into the class
								LeaderboardGetPlayersWithChallengesSolved ();

							}, PlayerAttributeRewardsChallengeSolvedFail =>
							{
								PlayFabErrorCode errorcode = PlayerAttributeRewardsChallengeSolvedFail.Error;
								Debug.LogError ("[PlayerStatisticsServer] Updating Statistics of the Player error: "+errorcode);
								Debug.LogError ("[PlayerStatisticsServer] Full report: "+PlayerAttributeRewardsChallengeSolvedFail.GenerateErrorReport());
							});
					}
				}, getPlayerStatisticsError => {
					PlayFabErrorCode errorcode = getPlayerStatisticsError.Error;
					Debug.LogError ("[PlayerStatisticsServer] Updating Statistic -> error: " + errorcode);
					Debug.LogError ("[PlayerStatisticsServer] Full report: " + getPlayerStatisticsError.GenerateErrorReport ());
				});
		}
	}


	public IEnumerator PlayerAttributeHalfRewardsChallengeSolved(string challengeID)
	{
		bool ftf = false;

		// ******************************************************************************************************************
		// First of all, let's search for this challenge: who solved it? Was this challenge solved already by this player?
		// ******************************************************************************************************************

		// after attributing these rewards, let's seee whether the challenge was solved alread. If not, let's attribute the reward "First to Solve"


		Debug.Log ("challenge ID used: " + challengeID);
		GetTitleDataByKey_CloudScript (challengeID);
		while (!callbackReturned) {
			yield return new WaitForSeconds (1);
		}
		List<string> playersThatSolvedTheChallenge = callbackResult;
		callbackResult = new List<string> ();	// reboot list for next call

		bool alreadyThere = false;
		// I need to double check whether the player is already in this list
		for (int i = 0; i < playersThatSolvedTheChallenge.Count; i++) {
			if (playersThatSolvedTheChallenge [i] == MasterManager.activePlayFabId) {
				alreadyThere = true;
			}
		}
		Debug.Log ("is it already there? " + alreadyThere.ToString());

		// if the player is already in this list, do not double attribute rewards
		if (!alreadyThere) {


			// ******************************************************************************************************************
			// Then, ask for the statistics and add to them in the success callback
			// ******************************************************************************************************************
			GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest ();
			List<string> statisticNames = new List<string> (3);
			statisticNames.Add ("Gold");
			statisticNames.Add ("ChallengesSolved");
			//statisticNames.Add ("PeopleMet");
			statisticNames.Add ("FirstToSolve");
			request.StatisticNames = statisticNames;	// you are going to attribute Gold per each challenge solved


			PlayFabClientAPI.GetPlayerStatistics (request, success => 
				{
					//Debug.Log ("[PlayerStatisticsServer] Quantity of stats: " + success.Statistics.Count);
					if (success.Statistics.Count > 0) {
						UpdatePlayerStatisticsRequest updateRequest = new UpdatePlayerStatisticsRequest ();
						updateRequest.Statistics = new List<StatisticUpdate> (success.Statistics.Count);	// create an update for all the statistics


						foreach (StatisticValue stat in success.Statistics) {
							// increment the stat "Gold"
							if (string.Compare (stat.StatisticName, "Gold") == 0) {
								//gold = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "Gold";
								statToAdd.Value = stat.Value + 10;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} else if (string.Compare (stat.StatisticName, "ChallengesSolved") == 0) {
								//challengesSolved = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "ChallengesSolved";
								statToAdd.Value = stat.Value + 1;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} 
							else if (string.Compare (stat.StatisticName, "FirstToSolve") == 0) {

								// this statistic sees whether there are other players that solved the challenge. If not, then attribute a first to find as well

								if ((playersThatSolvedTheChallenge.Count == 0)) {
									StatisticUpdate statToAdd = new StatisticUpdate ();

									statToAdd.StatisticName = "FirstToSolve";
									statToAdd.Value = stat.Value + 1;	// add 50 coins of gold to the user
									statToAdd.Version = stat.Version; // the existent version of the value
									updateRequest.Statistics.Add (statToAdd);
									ftf = true;
								}
							}  
							/*else if (string.Compare (stat.StatisticName, "PeopleMet") == 0) {
								//gold = true;	// the statistic already exist on the server; great

								StatisticUpdate statToAdd = new StatisticUpdate ();

								statToAdd.StatisticName = "PeopleMet";
								statToAdd.Value = stat.Value + numberOfPeopleFound;	// add 50 coins of gold to the user
								statToAdd.Version = stat.Version; // the existent version of the value

								updateRequest.Statistics.Add (statToAdd);
							} */

							// possibly increment other stats like badges
						}


						// ******************************************************************************************************************
						// Add the player to the list of players that solved that particular challenge
						// ******************************************************************************************************************
						playersThatSolvedTheChallenge.Add (MasterManager.activePlayFabId);
						AddPlayerToChallengePlayFabServer (challengeID, playersThatSolvedTheChallenge);

						if (ftf)
						{
							StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Challenge_Solved_First_To_Find_->_"+challengeID));
						}
						else
						{
							StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Challenge_Solved_->_"+challengeID));
						}
						// ******************************************************************************************************************
						// Do the actual update with all the statistics you defined
						// ******************************************************************************************************************
						PlayFabClientAPI.UpdatePlayerStatistics(updateRequest,PlayerAttributeRewardsChallengeSolvedSuccess =>
							{
								Debug.Log ("[PlayerStatisticsServer] Success in attributing rewards to the player. ");
								LoadPlayerStatistics ();	// reload all stats into the class
								LeaderboardGetPlayersWithChallengesSolved ();

							}, PlayerAttributeRewardsChallengeSolvedFail =>
							{
								PlayFabErrorCode errorcode = PlayerAttributeRewardsChallengeSolvedFail.Error;
								Debug.LogError ("[PlayerStatisticsServer] Updating Statistics of the Player error: "+errorcode);
								Debug.LogError ("[PlayerStatisticsServer] Full report: "+PlayerAttributeRewardsChallengeSolvedFail.GenerateErrorReport());
							});
					}
				}, getPlayerStatisticsError => {
					PlayFabErrorCode errorcode = getPlayerStatisticsError.Error;
					Debug.LogError ("[PlayerStatisticsServer] Updating Statistic -> error: " + errorcode);
					Debug.LogError ("[PlayerStatisticsServer] Full report: " + getPlayerStatisticsError.GenerateErrorReport ());
				});
		}
	}

	// attribute rewards when a player solves a challenge
	public IEnumerator TeamAttributeRewardsChallengeSolved(string challengeID, List<string> teamplayers)
	{
		bool alreadyThere, firstToFind;
		string tmpString = "";
		List<string> playersToUpdateStatistics = new List<string>();
		int count = 0;



		// ******************************************************************************************************************
		// First of all, let's search for this challenge: who solved it? Was this challenge solved already by this player?
		// ******************************************************************************************************************
		Debug.Log ("challenge ID used: " + challengeID);
		GetTitleDataByKey_CloudScript (challengeID);
		int countCallback = 20;
		while ((!callbackReturned) && (countCallback > 0)) {
			yield return new WaitForSeconds (1);
			countCallback--;
		}
		if (countCallback <= 0) {throw new UnityException ("[PlayerStatisticsServer] another callback that timedOut");}

		List<string> playersThatSolvedTheChallenge = callbackResult;
		callbackResult = new List<string> ();	// reboot list for next call

		if (playersThatSolvedTheChallenge.Count > 0) {
			firstToFind = false;
		} else {
			firstToFind = true;
		}


		for (int x = 0; x < teamplayers.Count; x++) {
			alreadyThere = false;


			// let's seee whether the challenge was solved alread. If not, let's attribute the reward "First to Solve"
			// I need to double check whether the player is already in this list
			for (int i = 0; i < playersThatSolvedTheChallenge.Count; i++) {
				if (string.Compare(playersThatSolvedTheChallenge [i], teamplayers[x]) == 0) {
					alreadyThere = true;
				}
			}

			Debug.Log ("is " + teamplayers[x] +" already there? " + alreadyThere.ToString());


			// if the player is already in this list, do not double attribute rewards
			if (!alreadyThere) {
				
				playersToUpdateStatistics.Insert (count, teamplayers[x]);
				count++;

				playersThatSolvedTheChallenge.Insert(playersThatSolvedTheChallenge.Count, teamplayers[x]);
				AddPlayerToChallengePlayFabServer (challengeID, playersThatSolvedTheChallenge);
			}

		}

		if (playersToUpdateStatistics.Count > 0) {


			TeamSolvedMultiplayerChallengePlayFabServer (playersToUpdateStatistics, firstToFind);

			tmpString = "Multiplayer_Challenge_Solved_->_"+challengeID+"_->TeamID_->"+MasterManager.instance.teamID+"_->Team_Members_[";
			for (int z = 0; z < teamplayers.Count - 1; z++)
			{
				tmpString += teamplayers[z] + ",";
			}
			tmpString += teamplayers[teamplayers.Count - 1] + "]";
			StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB (tmpString));
		}

	}

	// attribute rewards when a player solves a challenge
	public IEnumerator TeamAttributeRewardsChallengeSolvedOnlyTeamIDs(string challengeID, List<string> teamplayers, int howManyTeamsSolvedTheChallenge)
	{
		bool alreadyThere, firstToFind;
		string tmpString = "";
		List<string> playersToUpdateStatistics = new List<string>();
		int count = 0;



		// ******************************************************************************************************************
		// First of all, let's search for this challenge: who solved it? Was this challenge solved already by this player?
		// ******************************************************************************************************************
		Debug.Log ("challenge ID used: " + challengeID);
		GetTitleDataByKey_CloudScript (challengeID);
		int countCallback = 20;
		while ((!callbackReturned) && (countCallback > 0)){
			yield return new WaitForSeconds (1);
			countCallback--;
		}
		if (countCallback <= 0) {throw new UnityException ("[PlayerStatisticsServer] teamattribtutes rewards callback timeout");}

		List<string> playersThatSolvedTheChallenge = callbackResult;
		callbackResult = new List<string> ();	// reboot list for next call

		if (howManyTeamsSolvedTheChallenge > 0) {
			firstToFind = false;
		} else {
			firstToFind = true;
		}


		Debug.Log ("[Player statistics server] teamplayers.Count = " + teamplayers.Count);
		for (int x = 0; x < teamplayers.Count; x++) {
			alreadyThere = false;


			// let's seee whether the challenge was solved alread. If not, let's attribute the reward "First to Solve"
			// I need to double check whether the player is already in this list
			for (int i = 0; i < playersThatSolvedTheChallenge.Count; i++) {
				if (string.Compare(playersThatSolvedTheChallenge [i], teamplayers[x]) == 0) {
					alreadyThere = true;
				}
			}

			Debug.Log ("is " + teamplayers[x] +" already there? " + alreadyThere.ToString());


			// if the player is already in this list, do not double attribute rewards
			if (!alreadyThere) {

				playersToUpdateStatistics.Insert (count, teamplayers[x]);
				count++;

				playersThatSolvedTheChallenge.Insert(playersThatSolvedTheChallenge.Count, teamplayers[x]);
				//AddPlayerToChallengePlayFabServerOnlyTeamIDs (challengeID, playersThatSolvedTheChallenge);
			}

		}

		if (playersToUpdateStatistics.Count > 0) {


			TeamSolvedMultiplayerChallengePlayFabServer (playersToUpdateStatistics, firstToFind);

			tmpString = "Multiplayer_Challenge_Solved_->_"+challengeID+"_->TeamID_->"+MasterManager.instance.teamID+"_->Team_Members_[";
			for (int z = 0; z < teamplayers.Count - 1; z++)
			{
				tmpString += teamplayers[z] + ",";
			}
			tmpString += teamplayers[teamplayers.Count - 1] + "]";
			StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB (tmpString));
		}

	}

	private void TeamSolvedMultiplayerChallengePlayFabServer(List<string> players, bool isThisTeamTheFtF) {
		//JSON_TitleData_Challenge
		JSON_TitleData_UpdateMultiplayerChallengeTeamStats argObject = new JSON_TitleData_UpdateMultiplayerChallengeTeamStats ();
		argObject.PlayersNumber = players.Count;
		argObject.TeamMembers = players;
		argObject.FirstToFind = isThisTeamTheFtF;	// true if it is to increment the FirstToFind statistic for all elements; false to otherwise

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerHandleTeamSolvedMultiplayerChallenge", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[PlayerStatisticsServer] CloudScript Done. Players' statistics of the whole team " + MasterManager.instance.teamName + "were updated.");
				if(success.FunctionResult != null) {
					Debug.Log("[PlayerStatisticsServer] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("[PlayerStatisticsServer] There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}




	public void AddPlayerToChallengePlayFabServer(string challengeID, List<string> players) {
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = challengeID;
		argObject.players = players;

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[PlayerStatisticsServer] CloudScript Done. Player added to the list of players who solved the challenge " + challengeID + ".");
				if(success.FunctionResult != null) {
					Debug.Log("[PlayerStatisticsServer] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("[PlayerStatisticsServer] There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}

	public void AddPlayerToChallengePlayFabServerOnlyTeamIDs(string challengeID, List<string> players) {
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = challengeID;
		argObject.players = players;

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[PlayerStatisticsServer] CloudScript Done. Player added to the list of players who solved the challenge " + challengeID + ".");
				if(success.FunctionResult != null) {
					Debug.Log("[PlayerStatisticsServer] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("[PlayerStatisticsServer] There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
	}

	public IEnumerator AddTeamToMultiplayerChallengePlayFabServerAsync(string challengeID, List<string> players) {
		//JSON_TitleData_Challenge
		JSON_TitleData_Challenge argObject = new JSON_TitleData_Challenge();
		argObject.challengeID = challengeID;
		argObject.players = players;

		PlayFabClientAPI.ExecuteCloudScript(
			new ExecuteCloudScriptRequest
			{
				FunctionName = "ServerSaveChallengeAndPlayersWhoSolvedIt", FunctionParameter =  argObject,//new { inputValue = JsonUtility.ToJson(argObject)} ,
				// handy for logs because the response will be duplicated on PlayStream
				GeneratePlayStreamEvent = true
			},
			success => {
				Debug.Log("[PlayerStatisticsServer] CloudScript Done. Player added to the list of players who solved the challenge " + challengeID + ".");
				if(success.FunctionResult != null) {
					Debug.Log("[PlayerStatisticsServer] : " + success.FunctionResult);
				}
			}, 
			error => {
				Debug.Log("[PlayerStatisticsServer] There was error in the Cloud Script function :" + error.ErrorDetails + "\n" + error.ErrorMessage);
			});
		yield return null;
	}


	public void LeaderboardGetPlayersWithChallengesSolved()
	{
		GetLeaderboardRequest request = new GetLeaderboardRequest ();
		request.MaxResultsCount = 10;
		request.StartPosition = 0;
		request.ProfileConstraints = new PlayerProfileViewConstraints ();
		request.ProfileConstraints.ShowAvatarUrl = true;
		request.ProfileConstraints.ShowDisplayName = true;
		request.StatisticName = "PeopleMet";

		PlayFabClientAPI.GetLeaderboard (request, 
			LeaderboardGetPlayersWithChallengesSolvedSuccess =>
			{
				// populate the leaderboard of the challenges solved variable with the result. this was asked at the beginning (scene 2)
				leaderboardPeopleMet = LeaderboardGetPlayersWithChallengesSolvedSuccess.Leaderboard;
				StartCoroutine(LoadAvatars());

			}, LeaderboardGetPlayersWithChallengesSolvedFail => 
			{
				PlayFabErrorCode errorcode = LeaderboardGetPlayersWithChallengesSolvedFail.Error;
				Debug.LogError ("[PlayerStatisticsServer] I could not get the existent leaderboard for the challenges solved statistic." +
					" I got this code: "+errorcode);
				Debug.LogError ("[PlayerStatisticsServer] Full report: "+LeaderboardGetPlayersWithChallengesSolvedFail.GenerateErrorReport());
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (LeaderboardGetPlayersWithChallengesSolvedFail.GenerateErrorReport());
			});
	}

	//  List<Sprite> playersImages;
	public IEnumerator LoadAvatars ()
	{
		playersImagesReady = false; 
		// ver se ha necessidade de carregar novos avatars
		// PlayerStatisticsServer.instance.leaderboardPeopleMet [i].Profile.AvatarUrl, 
		//generatedIconsOnLeaderboard[i].GetComponent<LeaderboardPlayerInfo> ().playerImage


		bool found = false;
		List <int> indexesToAdd = new List<int> ();
		int count = 0;

		// find the unique players that you did not generate their images yet
		for (int i = 0; i < PlayerStatisticsServer.instance.leaderboardPeopleMet.Count; i++) {
			found = false;
			for (int j = 0; j < playersIDsForTheImages.Count; j++) {
				if (string.Compare (playersIDsForTheImages [j], PlayerStatisticsServer.instance.leaderboardPeopleMet [i].DisplayName) == 0) { // então cria o sprite
					found = true;
					break;
				} 
			}
			if (!found) {
				indexesToAdd.Insert (count, i);
				count++;
			}
		}

		count = playersIDsForTheImages.Count;
		for (int i = 0; i < indexesToAdd.Count; i++) {

			Texture2D texture;

			using (WWW www = new WWW(PlayerStatisticsServer.instance.leaderboardPeopleMet [indexesToAdd[i]].Profile.AvatarUrl))
			{
				float timer = 0;
				float timeOut = 10;
				bool failed = false;
				// Wait for download to complete
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
				}
				//yield return www;


				www.Dispose ();
			}

			Rect rec = new Rect(0, 0, texture.width, texture.height);
			//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);
			Sprite img = Sprite.Create(texture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

			playersIDsForTheImages.Insert (count, PlayerStatisticsServer.instance.leaderboardPeopleMet [indexesToAdd[i]].DisplayName);
			playersImages.Insert (count, img);

			count++;
		}

		playersImagesReady = true; 
	
		yield return null;
	}

}
