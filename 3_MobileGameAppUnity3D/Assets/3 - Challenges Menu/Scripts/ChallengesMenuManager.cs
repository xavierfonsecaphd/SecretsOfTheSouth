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

using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity;

public class ChallengesMenuManager : MonoBehaviour {


	// All the references for the icons of the challenges being generated in runtime
	public GameObject containerForChallengeIcons;	// it is here that I will attach all the challenge icons as a child
	public GameObject challengeIcon;	// this is the whole icon being generated


	//public ChallengeData temporaryChallengeToFollow;
	public List <GameObject> challengeIcons;
	public ChallengeIconModalPanel challengeIconModalPanel;
	public Camera _referenceCamera;
	private bool initiated = false;


	void Start()
	{
		//StartCoroutine (setUpChallenges ());	
		if (GameManager.instance != null) {
			// clean previous scene after loading this one
			GameManager.instance.UnloadObjectsFromGameScene ();



			challengeIconModalPanel = ChallengeIconModalPanel.Instance();

			if (GameManager.instance.challengesOnScreenMeteor.Count > 0) 
			{

				// items = items.OrderBy(w => w.startPos).ToList();
				for (int j = 0; j < GameManager.instance.challengesOnScreenMeteor.Count; j++) 
				{
					GameManager.instance.challengesOnScreenMeteor.Sort(delegate(ChallengeInfo x, ChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});
				}

				GameObject genericChallengeIcon;
				for (int i = 0; i < GameManager.instance.challengesOnScreenMeteor.Count; i++) 
				{

					genericChallengeIcon = Instantiate(challengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					ChallengeIconGUIScript scriptOfTheObject = genericChallengeIcon.GetComponent<ChallengeIconGUIScript> ();
					//Texture2D SpriteTexture;


					scriptOfTheObject.challengeTitle.text = GameManager.instance.challengesOnScreenMeteor [i].challenge_name;
					scriptOfTheObject.challengeDistance.text = "" + GameManager.instance.challengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheObject.challenge = GameManager.instance.challengesOnScreenMeteor [i];
					scriptOfTheObject.challengeImage.sprite = GameManager.challengeImages[i];

					// for the compass auto rotation
					//genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().challenge = 
					//	GameManager.instance.challengesOnScreen [i];
					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.challengesOnScreenMeteor [i]);

					challengeIcons.Add (genericChallengeIcon);
				}

				initiated = true;
			}
		}
	}
	public object getChallengeDetails(long id, int typeOfChallenge)
	{
		if (typeOfChallenge == 0)
		{
			foreach (Challenge_QuizData q in GameManager.instance.quizChallengesOnScreen) {
				if (q.challengeID == id) {
					return q;
				}
			}
		}	
		if (typeOfChallenge == 1)
		{
			foreach (Challenge_ARData a in GameManager.instance.arChallengesOnScreen) {
				if (a.challengeID == id) {
					return a;
				}
			}
		}

		throw new UnityException("[CompassIconRotation] unrecognizable type of challenge details to attach to the 3D object.");
	}


	private IEnumerator setUpChallenges()
	{
		// in case you did not do the mistake of firing up this scene only
		while (!GameManager.challengeImageSpritesLoaded) {
			yield return new WaitForSeconds (1);
		}

		if (GameManager.instance != null) {
			// clean previous scene after loading this one
			GameManager.instance.UnloadObjectsFromGameScene ();



			challengeIconModalPanel = ChallengeIconModalPanel.Instance();

			if (GameManager.instance.challengesOnScreenMeteor.Count > 0) 
			{

				// items = items.OrderBy(w => w.startPos).ToList();
				for (int j = 0; j < GameManager.instance.challengesOnScreenMeteor.Count; j++) 
				{
					GameManager.instance.challengesOnScreenMeteor.Sort(delegate(ChallengeInfo x, ChallengeInfo y) {
						return x.distanceToPlayer.CompareTo(y.distanceToPlayer);
					});
				}

				GameObject genericChallengeIcon;
				for (int i = 0; i < GameManager.instance.challengesOnScreenMeteor.Count; i++) 
				{

					genericChallengeIcon = Instantiate(challengeIcon, Vector3.zero, Quaternion.identity) as GameObject;
					genericChallengeIcon.SetActive (true);
					genericChallengeIcon.transform.SetParent (containerForChallengeIcons.transform);
					genericChallengeIcon.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
					genericChallengeIcon.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					ChallengeIconGUIScript scriptOfTheObject = genericChallengeIcon.GetComponent<ChallengeIconGUIScript> ();
					//Texture2D SpriteTexture;

				
					scriptOfTheObject.challengeTitle.text = GameManager.instance.challengesOnScreenMeteor [i].challenge_name;
					scriptOfTheObject.challengeDistance.text = "" + GameManager.instance.challengesOnScreenMeteor[i].DistanceToString();
					scriptOfTheObject.challenge = GameManager.instance.challengesOnScreenMeteor [i];
					scriptOfTheObject.challengeImage.sprite = GameManager.challengeImages[i];

					// for the compass auto rotation
					//genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().challenge = 
					//	GameManager.instance.challengesOnScreen [i];
					genericChallengeIcon.GetComponentInChildren<CompassIconRotation> ().FollowChallenge (GameManager.instance.challengesOnScreenMeteor [i]);

					challengeIcons.Add (genericChallengeIcon);
				}

				initiated = true;
				yield return null;

			}
		}
		yield break;
	}

	void Update()
	{
		if (initiated) {
			// This recalculates the distance of the icons to the player at GameManager 
			// (because the GPS is still changing while in this menu, and then re-display the distance per icon)
			for (int i = 0; i < GameManager.instance.challengesOnScreenMeteor.Count; i++) {
				GameManager.instance.challengesOnScreenMeteor [i].distanceToPlayer = 
				GameManager.instance.challengesOnScreenMeteor [i].DistanceToPlayer (
					new Vector2d (
						GameManager.instance.challengesOnScreenMeteor [i].latitude, 
						GameManager.instance.challengesOnScreenMeteor [i].longitude),	
					GPSLocationProvider_Xavier.instance.latlong);
			
				challengeIcons [i].GetComponent<ChallengeIconGUIScript> ().challengeDistance.text = 
				GameManager.instance.challengesOnScreenMeteor [i].DistanceToString ();
			}
		}
	}


	// Update is called once per frame
	public void ReturnToGameGUI()
	{
		for (int i = 0; i < challengeIcons.Count; i++) {
			Destroy (challengeIcons[i]);
		}
		SceneManager.UnloadSceneAsync (3);
		//DestroyObjectsFromScene ();
		GameManager.instance.LoadObjectsFromGameSceneMeteor ();
	}		
}
