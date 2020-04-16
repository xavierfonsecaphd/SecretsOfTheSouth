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

public class GenerateTeamRanking : MonoBehaviour {
	public static GenerateTeamRanking instance { set; get ; }
	// This is a singleton
	private static GenerateTeamRanking singleton;

	public GameObject containerForLeaderboardTeamsIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject leaderboardTeamsIcon;	// this is the whole icon being generated


	public List<GameObject> generatedIconsOnLeaderboard;

	// Use this for initialization
	void Start () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = GenerateTeamRanking.singleton;
			//if (blackStar == null) throw new System.Exception ("[EvaluatorRating] blackStar is null!");
			//if (star == null) throw new System.Exception ("[EvaluatorRating] star is null!");



		} else if (singleton != this) 
		{
			Destroy (gameObject);    
		}
	}

	public void FetchTeamsRankings()
	{
		StartCoroutine (FetchTeamsRankingsAsync());
	}

	public IEnumerator FetchTeamsRankingsAsync()
	{
		string url;
		UnityWebRequest request;

		MenuManager.instance._referenceDisplayManager.transform.SetAsLastSibling();

		url = MasterManager.serverURL + "/api/teamRankings";
		request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();

		if (request.isNetworkError) {
			Debug.LogError ("[MenuManager] Error While trying to get teams' rankings from the server: " + request.error);
			Debug.LogError ("[MenuManager] URL: " + url);

			MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (true);
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessageNonFading ("Error with requesting the Rankings. Try again later.");
			yield return new WaitForSeconds (3);
		} else {

			List<TeamsRankingsJSON> results = 
				JsonWrapper.DeserializeObject<List<TeamsRankingsJSON>>(request.downloadHandler.text);
			
			// before generating the icons, you need to sort the teams by the highest ranking
			for (int j = 0; j < results.Count; j++) 
			{
				results.Sort(delegate(TeamsRankingsJSON x, TeamsRankingsJSON y) {
					return y.Rating.CompareTo(x.Rating);
				});
			}



			// now you generate the icons
			for (int i = 0; i < results.Count; i++) {
				generatedIconsOnLeaderboard.Add(Instantiate<GameObject> (leaderboardTeamsIcon, Vector3.zero, Quaternion.identity));
				generatedIconsOnLeaderboard[i].SetActive (true);
				generatedIconsOnLeaderboard[i].transform.SetParent (containerForLeaderboardTeamsIcons.transform);
				generatedIconsOnLeaderboard[i].transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
				generatedIconsOnLeaderboard[i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

				generatedIconsOnLeaderboard [i].GetComponent<TeamRankIcon> ().teamName.text = results [i].TeamName;
				generatedIconsOnLeaderboard[i].GetComponent<TeamRankIcon> ().teamQuantityChallengesSolved.text = results [i].NumberOfChallengesSolved.ToString();
				generatedIconsOnLeaderboard[i].GetComponent<TeamRankIcon> ().teamRanking.text = results [i].Rating.ToString();
				generatedIconsOnLeaderboard[i].GetComponent<TeamRankIcon> ().teamImage.sprite = Resources.Load<Sprite>(results[i].TeamRefIcon);


			}


			/*
Texture2D texture = generateYourQRID (MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon);

					// generate the QR image for the player
					Rect rec = new Rect (0, 0, texture.width, texture.height);
					//Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.0f,0.0f),100);
					yourTeamQRIDImage.sprite = Sprite.Create (texture, rec, new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);

					teamImage.sprite = Resources.Load<Sprite>(MasterManager.instance.teamRefIcon);
					teamName.text = MasterManager.instance.teamName;
			*/
		}

		MenuManager.instance._referenceDisplayManager.statusPanel.gameObject.SetActive (false);
	}
}
