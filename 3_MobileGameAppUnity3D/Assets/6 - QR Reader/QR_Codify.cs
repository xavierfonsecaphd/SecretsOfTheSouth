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
//using UnityEditor;

using PlayFab;
using PlayFab.ClientModels;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using PlayFab.Json;
using UnityEngine.Networking;


/**
 *  This class interprets the codes read in a QR code, and reacts to them
 * 
 * Public link to my SurfDrive folder: https://surfdrive.surf.nl/files/index.php/s/azdwV1PXT1q3zsu
 */
public class QR_Codify : MonoBehaviour {

	[HideInInspector]
	public static QR_Codify instance { set; get ; }

	// This is a singleton
	[HideInInspector]
	private static QR_Codify singleton;

	[HideInInspector]
	public bool queryInExecutionBeforeExit = false;
	private List<string> recognizableQrCodes;

	[HideInInspector]
	public byte[] eventualPictureTaken;

	// Use this for initialization
	void Awake () {
		if (singleton == null) {
			singleton = this;

			eventualPictureTaken = null;

			//Sets this to not be destroyed when reloading scene
			instance = QR_Codify.singleton;


			// in here, please specify the codes that should appear after the complete website url
			recognizableQrCodes = new List<string> (3);
			recognizableQrCodes.Add ("0x001b00");	// 0x001b00 -> PlayerID
			recognizableQrCodes.Add ("0x001c00");	// 0x001c00 -> TeamID
			recognizableQrCodes.Add ("0x001d00");	// 0x001d00 -> foreigner person (non player to the game)
			recognizableQrCodes.Add ("0x001a00");	// 0x001a00 -> Challenge, Hunter, Picture or Text
			recognizableQrCodes.Add ("0x002a00");	// 0x002a00 -> Challenge, Voting

		
		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}


	/*
	 * The format of the QR Codes is as following (shifting 1 letter for privacy?)
	 * 
	 * http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_[code ]_[content]
	 *  0x001b00 -> PlayerID
	 *  0x001c00 -> TeamID
	 *  0x001d00 -> foreigner person (non player to the game)
	 *  0x001a00 -> Challenge, Hunter
	 * 
	 * In case of hunter challenge codes, the scheme is:
	 *  [code]_[id of challenge to mark as solved]
	 * example:
	 * http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x001a00_mbysampX5jpTmJKEi
	 * 
	 * 
	 * 
	 * In case PlayerIDs, the scheme is:
	 *  [code]_[playerID]_[playerName]
	 * 
	 * In case teamIDs, the scheme is:
	 *  [code]_[teamID]_[team name]_[team icon]
	 * 
	 * In case of Foreign person, the scheme is:
	 *  [code]_[foreignPlayerID]
	 * 
	 */
	public IEnumerator HandleQRCode (string qrContent)
	{
		// replace GUI buttons in the menu
		// activate the exit button always
		QRReaderManager.instance.buttons[2].gameObject.SetActive(true);
		// deactivate the cancel Reading QRCode button
		QRReaderManager.instance.buttons[4].gameObject.SetActive(false);
		int countCallBack = 20;

		queryInExecutionBeforeExit = true;

		string[] temporaryArray = qrContent.Split ('_');
		Debug.Log("[QR_Codify] Found " + temporaryArray.Length + " word count to handle. These are:");
		foreach(string s in temporaryArray)
		{
			Debug.Log(s);
		}


		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";

		if (string.Compare(temporaryArray [0], urlToCompare) == 0 ) {

			// Is this a valid QR code?
			bool validCode = false;
			int index = -1;
			for (int i = 0; i < recognizableQrCodes.Count; i++) {
				if (string.Compare (temporaryArray [1], recognizableQrCodes [i]) == 0) {
					validCode = true;
					index = i;
					break;
				}
			}

			if (validCode) {
				// perfect, this QR code has a valid codification which is it?

				switch (index) {
				case 0: // 0x001b00 -> PlayerID
					// [website]_[code]_[playerID]_[playerName]
					string otherRegisteredPlayFabID = temporaryArray [2];

					HandlePersonMetPlayFab playfab = new HandlePersonMetPlayFab ();
					// this is a registered person always.
					yield return playfab.HandleTwoPeopleMeetingUp (MasterManager.activePlayFabId, otherRegisteredPlayFabID, 1);
					countCallBack = 20;
					while ((!playfab.callbackReturned) && (countCallBack > 0)) {
						yield return new WaitForSeconds (1);
						countCallBack--;
					}
					if (countCallBack <= 0) {throw new UnityException ("[QR_Codify] timeout countCallBack");}
					List<string> callbackResult = playfab.callbackResult;
					if (string.Compare (callbackResult [0], "HandleTwoPeopleMeetingUpSuccess") == 0) {
						MasterManager.LogEventInServer (MasterManager.activePlayerName + " scanned a QR code of another Player.");
						StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Player_Met_->_"+otherRegisteredPlayFabID));

						string[] displayMessage1 = {"Congratulations, you just met a Player! If he is a new friend, you will get a reward. Go check it out in awhile ...", 
													"Gefeliciteerd, je hebt net een speler ontmoet! Als hij een nieuwe vriend is, krijg je een beloning. Ga het even bekijken ...", 
													"Parabéns, acabaste de conhecer outro jogador! Se ele é um novo contacto, irás receber pontos por isso. Podes confirmar isso dentro de uns momentos ..."};
						QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage1[MasterManager.language], false);

						Debug.Log ("[QR_Codify] Congratulations, you just made a new friend! :)");
					} else {
						string[] displayMessage2 = {"Could not add this player to your list of friends.", 
							"Kan deze speler niet toevoegen aan je lijst met vrienden.", 
							"Não foi possível adicionar este jogador à tua lista de amigos."};
						QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage2[MasterManager.language], true);
						Debug.LogError ("[QR_Codify] Could not add this player to your list of friends. Check the cloud script at HandlePersonMetPlayFab");
					}

					queryInExecutionBeforeExit = false;
					yield return QRReaderManager.instance.HandleScannedPlayerIDOrForeignerIDQRCode();

					break;
				case 1: // 0x001c00 -> TeamID
					// [website]_[code]_[teamID]_[team name]_[team icon]
					// http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x001c00_201811091313274_bla 6_TeamIcons/54

					queryInExecutionBeforeExit = false;
					string[] displayMessage3 = {"You scanned a Team's QR code. Let's join them! :)", 
						"Je hebt de QR-code van een team gescand. Laten we met hen meedoen! :)", 
						"Tu leste um código QR duma equipa. Vamo-nos lá juntar a esta equipa! :)"};
					QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage3[MasterManager.language], false);

					MasterManager.LogEventInServer (MasterManager.activePlayerName + " scanned a QR code of another Team, and joined it.");

					string argument = "" + temporaryArray[2] +"_" + temporaryArray[3] + "_" + temporaryArray[4];
					Debug.Log ("[QR_Codify] Handling a Team");
					yield return QRReaderManager.instance.HandleJoinTeamQRCode(argument);
					
					break;
				case 2: // 0x001d00 -> foreigner person (non player to the game)
					// [website]_[code]_[foreignPlayerID]
					string foreignPlayFabID = temporaryArray [2];

					HandlePersonMetPlayFab foreignPlayfab = new HandlePersonMetPlayFab ();
					// this is NOT a registered person. Count these people as well.

					yield return foreignPlayfab.HandleTwoPeopleMeetingUp (MasterManager.activePlayFabId, foreignPlayFabID, 0);
					countCallBack = 20;
					while ((!foreignPlayfab.callbackReturned) && (countCallBack > 0)) {
						yield return new WaitForSeconds (1);
						countCallBack--;
					}
					if (countCallBack <= 0) {throw new UnityException ("[QR_codify] countCallBack timeout case 2");}
					List<string> callbackResult2 = foreignPlayfab.callbackResult;
					if (string.Compare (callbackResult2 [0], "HandleTwoPeopleMeetingUpSuccess") == 0) {
						MasterManager.LogEventInServer (MasterManager.activePlayerName + " scanned a QR code of a random person.");

						StartCoroutine(GameManager.instance.CollectPlayerDataGPSRegisterEventInDB ("Person_Met_->_"+foreignPlayFabID));
						string[] displayMessage4 = {"Congratulations, you just met a random person! If he is a new friend, you will get a reward. Go check it out in awhile...", 
							"Gefeliciteerd, je hebt zojuist een willekeurige persoon ontmoet! Als hij een nieuwe vriend is, krijg je een beloning. Ga het even bekijken ...", 
							"Boa, acabaste de conhecer uma pessoa desconhecida! Se essa pessoa é uma nova amizade, receberás pontos por isso. Podes verificar daqui a um bocado ..."};
						QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage4[MasterManager.language], false);

						Debug.Log ("[QR_Codify] Congratulations, you just made a new friend with a foreigner! :)");
					} else {
						string[] displayMessage5 = {"Could not add this person to your list of friends.", 
							"Kan deze persoon niet toevoegen aan je lijst met vrienden.", 
							"Não foi possível adicionar esta pessoa à tua lista de amigos."};
						QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage5[MasterManager.language], true);
						Debug.LogError ("[QR_Codify] Could not add this person to your list of friends. Check the cloud script at HandlePersonMetPlayFab");
					}

					queryInExecutionBeforeExit = false;

					yield return QRReaderManager.instance.HandleScannedPlayerIDOrForeignerIDQRCode();


					break;
				case 3: // 0x001a00 -> Challenge, Hunter
					// 0x001a00_mbysampX5jpTmJKEi

					// first you read the challenge id
					// /api/challenges_hunter?id=mbysampX5jpTmJKEi
					string url;
					UnityWebRequest request;


					// **************************************************
					// first, get the information of this Hunter challenge
					// **************************************************
					url = MasterManager.serverURL + "/api/challenges_hunter?id=" + temporaryArray [2];
					request = UnityWebRequest.Get (url);
					request.timeout = 10;
					yield return request.SendWebRequest();


					if (request.isNetworkError) {
						Debug.LogError ("[QR_Codify] Error While Sending: " + request.error);
						Debug.LogError ("[QR_Codify] URL: " + url);
					} else {
						Debug.Log("[QR_Codify] Request with: " + url);
						Debug.Log("[QR_Codify] Received: " + request.downloadHandler.text);

						HunterChallengeDBMeteorFormat_JSON tmpResult=
							JsonWrapper.DeserializeObject<HunterChallengeDBMeteorFormat_JSON>(request.downloadHandler.text);

						if (string.Compare(tmpResult.content_picture, string.Empty) != 0) // this is picture 
						{
							// then, load picture first
							Debug.Log("[QR_Codify] Picture found.");
							Texture2D texture;
							using (WWW www = new WWW(tmpResult.content_picture))
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
									if ((www.texture == null) || (www.texture.width == 0) || (www.texture.height == 0)) {
										WWW wwwDefault = new WWW ("https://cdn3.iconfinder.com/data/icons/lineato-basic-business/94/lineato_torn_document-512.png");

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

											Debug.Log("[QR_Codify] could not load the picture. loading a standard one instead");
										}

										//yield return wwwDefault;

									} else {
										// assign texture
										texture = www.texture;
										Debug.Log("[QR_Codify] Picture loaded ok.");
									}
								}

								// Wait for download to complete
								//yield return www;


								www.Dispose ();
							}

							Rect rec = new Rect(0, 0, texture.width, texture.height);
							Sprite img = Sprite.Create(texture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);


							QRReaderManager.instance.HandleChallengeQRCodeFound(false,tmpResult.name ,tmpResult.content_picture, img);
							Debug.Log("[QR_Codify] Handled challenge QR code found ok.");
							// solve the challenge, attribute points, and manage the challenge's 3D icon on the game
							yield return QRReaderManager.instance.ManageHunterChallengeThroughQRFound(tmpResult);
							Debug.Log("[QR_Codify] Managed point attribution, and 3D icons ok.");
						}
						else if (string.Compare(tmpResult.content_text, string.Empty) != 0) // then this is text 
						{
							// then, you attempt to get the content for the QR code and render the code on the window
							QRReaderManager.instance.HandleChallengeQRCodeFound(true, tmpResult.name, tmpResult.content_text, null);
							Debug.Log("[QR_Codify] Handled challenge QR code found ok.");
							// solve the challenge, attribute points, and manage the challenge's 3D icon on the game
							yield return QRReaderManager.instance.ManageHunterChallengeThroughQRFound(tmpResult);
							Debug.Log("[QR_Codify] Managed point attribution, and 3D icons ok.");
						}
						else // the request failed to acquire the challenge 
						{
							Debug.Log ("[QR_Codify] the QR code appears to have the challenge ID ok in it, but I could not retrieve the challenge details. What was read was: " + qrContent + 
								" . Then, the challenge details were requested with the url: " + url);
							string[] displayMessage6 = {"QR code format ok, but could not retrieve the details of the challenge", 
								"QR-code formaat ok, maar kon de details van de uitdaging niet ophalen", 
								"O formato do código QR está bem, mas não foi possível descarregar os detalhes do desafio da internet"};
							QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage6[MasterManager.language], true);
						}
					}


					queryInExecutionBeforeExit = false;
					break;
				case 4:
					// 0x002a00 -> Challenge, Voting
					// /api/challenges_voting?id=CMCY8NAxEDRigaJ2f  para pedir informação sobre o desafio
					// in here I should be showing the challenge, and all the pictures of it, for you to vote.
					// http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x002a00_CMCY8NAxEDRigaJ2f

					// **************************************************
					// Basically, what we want to do here is get the information
					// of the challenge online, and generate a window so that the
					// player can interact with the challenge (i.e., take picture,
					// publish it in the challenge, and vote other pictures)
					// **************************************************

					url = MasterManager.serverURL + "/api/challenges_voting?id=" + temporaryArray [2];
					request = UnityWebRequest.Get (url);
					request.timeout = 10;
					yield return request.SendWebRequest ();

					if (request.isNetworkError) {
						Debug.LogError ("[QR_Codify] Error While Sending: " + request.error);
						Debug.LogError ("[QR_Codify] URL: " + url);
					} else { 
						Debug.Log("[QR_Codify] Request with: " + url);
						Debug.Log("[QR_Codify] Received: " + request.downloadHandler.text);

						VotingChallengeDBMeteorFormat_JSON tmpResult=
							JsonWrapper.DeserializeObject<VotingChallengeDBMeteorFormat_JSON>(request.downloadHandler.text);
					
						VotingChallengeInfo activeVotingChallenge = new VotingChallengeInfo();
						activeVotingChallenge._id 			= tmpResult._id;
						activeVotingChallenge.name 			= tmpResult.name;
						activeVotingChallenge.description	= tmpResult.description;
						activeVotingChallenge.ownerPlayFabID = tmpResult.ownerPlayFabID;
						activeVotingChallenge.typeOfChallengeIndex = tmpResult.typeOfChallengeIndex;
						activeVotingChallenge.latitude 		= tmpResult.latitude;
						activeVotingChallenge.longitude 	= tmpResult.longitude;
						activeVotingChallenge.task 			= tmpResult.task;
						activeVotingChallenge.imageURL 		= tmpResult.imageURL;
						activeVotingChallenge.listOfImagesAndVotes = tmpResult.listOfImagesAndVotes;
						activeVotingChallenge.validated 	= tmpResult.validated;
						activeVotingChallenge.solved 		= false;
						activeVotingChallenge.route = tmpResult.route;

						if (string.Compare (activeVotingChallenge.imageURL, string.Empty) != 0) {
							Debug.Log ("[QR_Codify] Handling Image URL: " + activeVotingChallenge.imageURL);
							Texture2D texture;

							using (WWW www = new WWW(activeVotingChallenge.imageURL))
							{
								// Wait for download to complete
								yield return www;

								texture = www.texture;
							}

							Sprite img = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0), 100.0f, 0, SpriteMeshType.Tight);
							GameManager.instance.manageVotingChallenge.titleHolder.text = activeVotingChallenge.name;
							GameManager.instance.manageVotingChallenge.descriptionHolder.text = activeVotingChallenge.description;
							GameManager.instance.manageVotingChallenge.taskHolder.text = activeVotingChallenge.task;
							GameManager.instance.manageVotingChallenge.imageHolder.sprite = img;
							GameManager.instance.manageVotingChallenge.descriptionScrollViewPanel.verticalNormalizedPosition = 1.0f;


							// understand whether the challenge scanned through the QR code is already solved in the game or not
							for (int i = 0; i < GameManager.instance.votingChallengesOnScreenMeteor.Count; i++) {
								if (string.Compare (GameManager.instance.votingChallengesOnScreenMeteor [i]._id, activeVotingChallenge._id) == 0) {
									if (GameManager.instance.votingChallengesOnScreenMeteor [i].solved) {
										activeVotingChallenge.solved = true;
									} else {
										activeVotingChallenge.solved = false;
									}
									break;
								}
							}

							// after getting all the information, pass the challenge to the managing window
							GameManager.instance.manageVotingChallenge.currentVotingChallengeBeingSolved = activeVotingChallenge;

							GameManager.instance.manageVotingChallenge.ActivateWindow ();

							SceneManager.UnloadSceneAsync (5);

							GameManager.instance.LoadObjectsFromGameSceneMeteor ();
						}

					}

					// for now, do nothing
					queryInExecutionBeforeExit = false;

					break;
				default: 
					queryInExecutionBeforeExit = false;
					MasterManager.LogEventInServer (MasterManager.activePlayerName + " tried to scan a QR code that was unrecognizable.");
					string[] displayMessage7 = {"QR code format not recognizable. Were the QR codes different before?", 
						"QR-code formaat niet herkenbaar. Waren de QR-codes eerder anders?", 
						"O formato do código QR não é reconhecível. Será que o formato entretanto mudou?"};
					QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage7[MasterManager.language], true);

					break;
				}

			
			} else {
				queryInExecutionBeforeExit = false;
				MasterManager.LogEventInServer (MasterManager.activePlayerName + " tried to scan a QR code that was unrecognizable.");
				Debug.Log ("[QR_Codify] the QR code appears to have the website prefix ok, but does not have a valid code with it. What was read was: " + qrContent);
				string[] displayMessage8 = {"QR code format not recognizable. Is this a legacy QR code?", 
					"QR-code formaat niet herkenbaar. Is dit een oudere QR-code?", 
					"O formato do código QR não é reconhecível. Será que o formato deste código QR é antigo?"};
				QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage8[MasterManager.language], true);


			}
		}
		else {
			queryInExecutionBeforeExit = false;
			MasterManager.LogEventInServer (MasterManager.activePlayerName + " tried to scan a QR code that was unrecognizable.");
			Debug.Log ("[QR_Codify] the QR code does not appear to have the website prefix ok. What was read was: " + qrContent);
			string[] displayMessage9 = {"QR code format not recognizable. Is this a legacy QR code?", 
				"QR-code formaat niet herkenbaar. Is dit een oudere QR-code?", 
				"O formato do código QR não é reconhecível. Será que o formato deste código QR é antigo?"};
			QRReaderManager.instance.ShowMessageAfterQRCodeScan (displayMessage9[MasterManager.language], true);

		}
			

		yield return null;
	}


	/*
	public IEnumerator HandleQRCodeAfterTakingPicture (string qrContent)
	{
		queryInExecutionBeforeExit = true;

		string[] temporaryArray = qrContent.Split ('_');
		Debug.Log ("[QR_Codify] Found " + temporaryArray.Length + " word count to handle. These are:");
		foreach (string s in temporaryArray) {
			Debug.Log (s);
		}


		string urlToCompare = MasterManager.serverURL + "/HandleQRCode/";

		if (string.Compare (temporaryArray [0], urlToCompare) == 0) {

			// Is this a valid QR code?
			bool validCode = false;
			int index = -1;
			for (int i = 0; i < recognizableQrCodes.Count; i++) {
				if (string.Compare (temporaryArray [1], recognizableQrCodes [i]) == 0) {
					validCode = true;
					index = i;
					break;
				}
			}

			if (validCode) {
				switch (index) {
				case 4: 
					// 0x002a00 -> Challenge, Voting
					// http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_0x002a00_mbysampX5blablabla


					// for now, do nothing


					**
					WWWForm form = new WWWForm();
					//form.AddField("frameCount", Time.frameCount.ToString());
					form.AddBinaryData("image", eventualPictureTaken, "avatar.jpg", "photo/png");
					// Upload to a cgi script
					using (var w = UnityWebRequest.Post(url + "/" + idUser + "/upload-image", form))
					{
						w.chunkedTransfer = false;
						yield return w.SendWebRequest();
						if (w.error != null)
						{
							print("error" + w.error);
						}
						else
						{
							print("Finished Uploading Picture into the challenge: " + w.responseCode);
						}
					}**

					// nulify the picture taken ; Not needed, but for the time being, ok.
					eventualPictureTaken = null;

					break;
				default: 
					queryInExecutionBeforeExit = false;
					QRReaderManager.instance.ShowMessageAfterQRCodeScan ("QR code format not recognizable. Were the QR codes different before?", true);
					break;
				}
			} else {
				queryInExecutionBeforeExit = false;
				Debug.Log ("[QR_Codify] the QR code appears to have the website prefix ok, but does not have a valid code with it. What was read was: " + qrContent);
				QRReaderManager.instance.ShowMessageAfterQRCodeScan ("QR code format not recognizable. Is this a legacy QR code?", true);
			}
		} else {
			queryInExecutionBeforeExit = false;
			Debug.Log ("[QR_Codify] the QR code does not appear to have the website prefix ok. What was read was: " + qrContent);
			QRReaderManager.instance.ShowMessageAfterQRCodeScan ("QR code format not recognizable. Is this a legacy QR code?", true);
		}

		queryInExecutionBeforeExit = false;
		yield break;
	} */
}
