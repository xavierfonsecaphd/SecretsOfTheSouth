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

using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;

// this is for Save and Load methods
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//http://srv779.tudelft.net/

public class SotSHandler : MonoBehaviour {

	public static SotSHandler instance { set; get ; }
	// This is a singleton
	private static SotSHandler singleton;

	public InputField usernameSignUp;
	public InputField emailSignUp;
	public InputField passwordSignUp;
	public InputField displayNameSignUp;
	public InputField emailLogin;
	public InputField passwordLogin;

	[HideInInspector]
	public string signUpEmail;
	[HideInInspector]
	public string signUpPassword;
	private string tmpPlayerID = "";

	void Awake()
	{
		if (singleton == null) 
		{
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = SotSHandler.singleton;

		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	public void LoginWithQRCode(string playerID)
	{
		// txt16
		string [] displayMessage1 = {"Connecting to SotS World ...",
			"Verbinden met SotS World ...",
			"A conectar-te ao mundo SotS ..."};
		
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();
		Debug.Log("Connecting to SotS World via QR Code..."); // logs the given message and displays it on the screen using OnGUI method
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Connecting to SotS World...");

		LoginWithEmailAddressRequest logintoPlayFabwithEmailrequest = new LoginWithEmailAddressRequest ();
		logintoPlayFabwithEmailrequest.Email = playerID + "@sots.nl";
		logintoPlayFabwithEmailrequest.TitleId = "CCF6";
		logintoPlayFabwithEmailrequest.Password = playerID;
		signUpEmail = playerID + "@sots.nl";
		signUpPassword = playerID;
		tmpPlayerID = playerID;

		// first login, then link accounts
		PlayFabClientAPI.LoginWithEmailAddress (logintoPlayFabwithEmailrequest, logintoExistentPlayFabAccountSuccessQR, logintoExistentPlayFabAccountError);
	}

	public void Login()
	{// txt16
		string [] displayMessage1 = {"Connecting to SotS World ...",
			"Verbinden met SotS World ...",
			"A conectar-te ao mundo SotS ..."};
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();
		Debug.Log("Connecting to SotS World..."); // logs the given message and displays it on the screen using OnGUI method
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Connecting to SotS World...");

		LoginWithEmailAddressRequest logintoPlayFabwithEmailrequest = new LoginWithEmailAddressRequest ();
		logintoPlayFabwithEmailrequest.Email = emailLogin.text;
		logintoPlayFabwithEmailrequest.TitleId = "CCF6";
		logintoPlayFabwithEmailrequest.Password = passwordLogin.text;
		signUpEmail = emailLogin.text;
		signUpPassword = passwordLogin.text;
		 
		// first login, then link accounts
		PlayFabClientAPI.LoginWithEmailAddress (logintoPlayFabwithEmailrequest, logintoExistentPlayFabAccountSuccess, logintoExistentPlayFabAccountError);
	}

	public void LoginToContinueGame(string email, string password)
	{
		// txt16
		string [] displayMessage1 = {"Connecting to SotS World ...",
			"Verbinden met SotS World ...",
			"A conectar-te ao mundo SotS ..."};
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Connecting again to SotS World...");
		
		Debug.Log("Connecting again to SotS World..."); // logs the given message and displays it on the screen using OnGUI method

		LoginWithEmailAddressRequest logintoPlayFabwithEmailrequest = new LoginWithEmailAddressRequest ();
		logintoPlayFabwithEmailrequest.Email = email;
		logintoPlayFabwithEmailrequest.TitleId = "CCF6";
		logintoPlayFabwithEmailrequest.Password = password;

		// first login, then link accounts
		PlayFabClientAPI.LoginWithEmailAddress (logintoPlayFabwithEmailrequest, logintoExistentPlayFabAccountSuccessViaContinueButton, logintoExistentPlayFabAccountError);
	}

	// just enter
	private void logintoExistentPlayFabAccountSuccessViaContinueButton(LoginResult result)
	{	//txt17
		string [] displayMessage1 = {"Logged in.",
			"Ingelogd.",
			"Conectado."};
//		Debug.Log ("[SotSHandler] Yey. I could login to PlayFab existent account "+emailLogin.text);
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Logged in.");

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}
	private void logintoExistentPlayFabAccountSuccess(LoginResult result)
	{//txt17
		string [] displayMessage1 = {"Logged in.",
			"Ingelogd.",
			"Conectado."};
		
		Debug.Log ("[SotSHandler] Yey. I could login to PlayFab existent account "+emailLogin.text);
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Logged in.");
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();

		GetPlayerProfileRequest requestURL = new GetPlayerProfileRequest ();
		MasterManager.activePlayFabId = result.PlayFabId;
		requestURL.PlayFabId = MasterManager.activePlayFabId;
		requestURL.ProfileConstraints = new PlayerProfileViewConstraints ();
		requestURL.ProfileConstraints.ShowAvatarUrl = true;

		PlayFabClientAPI.GetPlayerProfile (requestURL, 
			LogintoExistentPlayFabAccountSuccessRequestAvatarURLSuccess, LogintoExistentPlayFabAccountSuccessRequestAvatarURLFail);

		// get email address from playfab account
		GetAccountInfoRequest requestforPlayFabID = new GetAccountInfoRequest ();
		requestforPlayFabID.Email = emailLogin.text;
		PlayFabClientAPI.GetAccountInfo (requestforPlayFabID, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDSuccess, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDError);

	}
	private void logintoExistentPlayFabAccountSuccessQR(LoginResult result)
	{//txt17
		string [] displayMessage1 = {"Logged in.",
			"Ingelogd.",
			"Conectado."};
		//Debug.Log ("[SotSHandler] Yey. I could login to PlayFab existent account "+emailLogin.text);
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Logged in.");
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();

		GetPlayerProfileRequest requestURL = new GetPlayerProfileRequest ();
		MasterManager.activePlayFabId = result.PlayFabId;
		requestURL.PlayFabId = MasterManager.activePlayFabId;
		requestURL.ProfileConstraints = new PlayerProfileViewConstraints ();
		requestURL.ProfileConstraints.ShowAvatarUrl = true;

		PlayFabClientAPI.GetPlayerProfile (requestURL, 
			LogintoExistentPlayFabAccountSuccessRequestAvatarURLSuccess, LogintoExistentPlayFabAccountSuccessRequestAvatarURLFail);

		// get email address from playfab account
		GetAccountInfoRequest requestforPlayFabID = new GetAccountInfoRequest ();
		requestforPlayFabID.Email = tmpPlayerID + "@sots.nl";
		PlayFabClientAPI.GetAccountInfo (requestforPlayFabID, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDSuccess, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDError);

	}
	private void LogintoExistentPlayFabAccountSuccessRequestAvatarURLSuccess(GetPlayerProfileResult result)
	{
		MasterManager.activePlayerAvatarURL = result.PlayerProfile.AvatarUrl;
		Debug.Log ("[SotSHandler] Avatar of the Player is retrieved.");
	}
	private void LogintoExistentPlayFabAccountSuccessRequestAvatarURLFail(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[SotSHandler] I could not get the existent Avatar URL From the player." +
			" I got this code: "+errorcode);
		Debug.LogError ("[SotSHandler] Full report: "+error.GenerateErrorReport());
	}
	private void logintoExistentPlayFabAccountError(PlayFabError error)
	{
		//LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("Error: " + error.GenerateErrorReport ());
		OGLoadingOverlay.ShowFullcoverLoading("Error: " + error.GenerateErrorReport (), true);
		//OGLoadingOverlay.StopOldestActiveLoading();

		Debug.LogError ("[SotSHandler] Shit. I could not login into the existent PlayFab account, because: " + error.GenerateErrorReport ());
	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDSuccess (GetAccountInfoResult result)
	{
		MasterManager.activePlayFabId = result.AccountInfo.PlayFabId;

		// when login is successful
		//	MenuManager.instance.loginSotSPanel.SetActive (false);
		//MenuManager.instance.menuPanel.SetActive (false);
		//MenuManager.instance.isLoggedIn = true;
		//MasterManager.activePlayerName = result.AccountInfo.Username;
		Debug.Log ("[SotSHandler] Welcome to the Secrets of the South, " + result.AccountInfo.Username);
//		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + result.AccountInfo.Username);

		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDError (PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[SotSHandler] I could not get the existent PlayFabID from Facebook email. I got this code: "+errorcode);
		Debug.LogError ("[SotSHandler] Full report: "+error.GenerateErrorReport());
	}

	public void SignUp()
	{

		RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
		request.RequireBothUsernameAndEmail = false;
		request.TitleId = "CCF6";
		if (usernameSignUp.text != string.Empty) {
			request.Username = usernameSignUp.text;
		}
		request.Email = emailSignUp.text;
		request.Password = passwordSignUp.text;
		request.DisplayName = displayNameSignUp.text;

		signUpEmail = emailSignUp.text;
		signUpPassword = passwordSignUp.text;

		MasterManager.activePlayerName = displayNameSignUp.text;
		// this is a default image that all players start off with
		MasterManager.activePlayerAvatarURL = "http://images.hellokids.com/_uploads/_tiny_galerie/20120939/q73_how-to-draw-ninja-gir-tutorial-drawing.png";

		Debug.Log("Doing the player register request for "+request.DisplayName);
		PlayFabClientAPI.RegisterPlayFabUser(request,OnPlayFabRegisterSuccess,OnPlayFabRegisterError);


	}
	// register a user for the first time
	private void OnPlayFabRegisterSuccess(RegisterPlayFabUserResult result)
	{//txt18
		string [] displayMessage1 = {"Account created. Logging in ...",
			"Account aangemaakt. Inloggen ...",
			"Conta criada. A conectar-te ..."};
		
		//LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Account created. Logging in..."); 
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language], true);
		//OGLoadingOverlay.StopOldestActiveLoading();

		// record active player
		MasterManager.activePlayFabId = result.PlayFabId;
		//MenuManager.instance.isLoggedIn = true;
		Debug.Log ("[SotSHandler] Active PlayFabID recorded:  " + MasterManager.activePlayFabId);

		//MenuManager.instance.menuPanel.SetActive (false);
		//MenuManager.instance.signUpPanel.SetActive (false);
		//MenuManager.instance.isLoggedIn = true;
		//SceneManager.LoadSceneAsync("Secrets of the South V1", LoadSceneMode.Additive);
		//MasterManager.activePlayerName = result.Username;
		Debug.Log ("[SotSHandler] Welcome to the Secrets of the South, " + result.PlayFabId);
	//	GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + result.Username);
		UpdateAvatarURLOfPlayer ();
		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}
	// Fails to register a user for the first time
	private void OnPlayFabRegisterError(PlayFabError error)
	{
		//txt19
		string [] displayMessage1 = {"Error: ",
			"Fout: ",
			"Erro: "};
		//LoginMenuManager.instance._referenceDisplayManager.DisplayErrorMessage ("Error: " + error.GenerateErrorReport());
		OGLoadingOverlay.ShowFullcoverLoading(displayMessage1[MasterManager.language] + error.GenerateErrorReport(), true);
		//OGLoadingOverlay.StopOldestActiveLoading();
		Debug.LogError("[SotSHandler] CallBack error: " + error.GenerateErrorReport());
	}


	// this is outdated custom playfab login
	private void OnLoginSuccess(LoginResult result)
	{
		Debug.LogError("[SotSHandler] sots login not implemented");
	}
	// this is outdated custom playfab login
	private void OnLoginFailure(PlayFabError error)
	{
		Debug.LogError("[SotSHandler] CallBack error: " + error.GenerateErrorReport());
	}

	public void ResetPasswordFromPlayFabAccount(string email)
	{
		SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest ();
		request.Email = email;
		request.TitleId = "CCF6";

		PlayFabClientAPI.SendAccountRecoveryEmail (request, 
			ResetPasswordFromPlayFabAccountSuccess, ResetPasswordFromPlayFabAccountError);
	}

	private void ResetPasswordFromPlayFabAccountSuccess(SendAccountRecoveryEmailResult result)
	{
		Debug.Log ("[SotSHandler] An email was successfully sent to the address provided.");
	}
	private void ResetPasswordFromPlayFabAccountError(PlayFabError error)
	{
		Debug.Log ("[SotSHandler] The reconver email could NOT be sent: " + error.GenerateErrorReport ());
	}

	private void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
			file.Close ();
		}

		file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
		Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

		SotSPlayerInfo data = new SotSPlayerInfo ();
		data.isFacebookAccessToken = false;
		data.playFabEmail = signUpEmail;
		data.playFabPassword = signUpPassword;
		data.activePlayerName = MasterManager.activePlayerName;
		data.activePlayFabId = MasterManager.activePlayFabId;
		data.language = MasterManager.language;
		data.route = MasterManager.route;
		data.teamID = MasterManager.instance.teamID;
		data.teamName = MasterManager.instance.teamName;
		data.teamRefIcon = MasterManager.instance.teamRefIcon;

		if (MasterManager.activePlayerAvatarURL == string.Empty) {
			Debug.LogError ("The Player's Avatar URL was stored empty! Apparently, the callback returned later, or the image was not there the begin with.");
		}
		data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
		data.desiredMax_DistanceOfChallenges = MasterManager.desiredMaxDistanceOfChallenges;

		bf.Serialize (file, data);
		file.Close ();


		MasterManager.activePlayerEmail = signUpEmail;
		MasterManager.activePlayerPassword = signUpPassword;
	}

	// update the URL of the player's Avatar in PlayFab
	public void UpdateAvatarURLOfPlayer()
	{
		string newURL = MasterManager.activePlayerAvatarURL;
		UpdateAvatarUrlRequest request = new UpdateAvatarUrlRequest();
		request.ImageUrl = newURL;
		PlayFabClientAPI.UpdateAvatarUrl (request, UpdateAvatarURLOfPlayerSuccess, UpdateAvatarURLOfPlayerFail);
		//PlayFabClientAPI.UpdateUserTitleDisplayName (request, UpdateDisplayNameOfPlayerSuccess, UpdateDisplayNameOfPlayerFail);
	}
	private void UpdateAvatarURLOfPlayerSuccess(EmptyResult result)
	{
		Debug.Log ("[SettingsPanel] URL of the Avatar Updated successfully.");
	}
	private void UpdateAvatarURLOfPlayerFail(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[SettingsPanel] Could not update the URL of the Avatar of the player. I got this code: "+errorcode);
		Debug.LogError ("[SettingsPanel] Full report: "+error.GenerateErrorReport());
	}
}
