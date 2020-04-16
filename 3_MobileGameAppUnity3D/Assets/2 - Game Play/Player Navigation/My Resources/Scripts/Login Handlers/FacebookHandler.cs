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
using System.Net.Mail;

using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;

// this is for Save and Load methods
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FacebookHandler : MonoBehaviour {

	public static FacebookHandler instance { set; get ; }
	// This is a singleton
	private static FacebookHandler singleton;

	[HideInInspector]
	public static string facebookGenericString;
	[HideInInspector]
	public static bool callbackReturned = false;
	[HideInInspector]
	public string facebookEmailToLinkToPlayFabID;
	[HideInInspector]
	public string facebookDisplayNameToDisplayAfterLinkingToPlayFabID;

	private string temporaryFacebookEmail;

	// Awake function from Unity's MonoBehavior
	void Awake ()
	{
		if (singleton == null) 
		{
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = FacebookHandler.singleton;

			if (!FB.IsInitialized) {
				// Initialize the Facebook SDK
				FB.Init(InitCallback, OnHideUnity);
			} else {
				// Already initialized, signal an app activation App Event
				FB.ActivateApp();
			}
			facebookGenericString = string.Empty;
			facebookEmailToLinkToPlayFabID = string.Empty;
			facebookDisplayNameToDisplayAfterLinkingToPlayFabID = string.Empty;
			temporaryFacebookEmail = string.Empty;

		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}


	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
			Debug.Log("[FacebookHandler] Facebook initialized :)"); // logs the given message and displays it on the screen using OnGUI method
		//	SceneManager.LoadSceneAsync("Secrets of the South V1", LoadSceneMode.Additive);
		} else {
			Debug.Log("[FacebookHandler] Failed to Initialize the Facebook SDK");
		}

	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			Debug.Log("[FacebookHandler] Game Paused...");
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			Debug.Log("[FacebookHandler] Game Resumed...");
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}




	// facebook login
	public void Login()
	{
		IEnumerable<string> perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}
	// facebook login
	public void LoginViaContinueButton()
	{
		LoginMenuManager.instance._referenceDisplayManager.DisplaySystemMessage ("Loggin in...");
		IEnumerable<string> perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthContinueCallback);
	}

	private void AuthCallback (ILoginResult result) {
		if (result == null || string.IsNullOrEmpty (result.Error)) {
			if (FB.IsLoggedIn) {
				
				// AccessToken class will have session details
				AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				MasterManager.facebookAccessToken = aToken;

				Debug.Log("[FacebookHandler] Facebook Auth Complete! Access Token: " + aToken + "   Logging into PlayFab...");

				// is the same email already as a PlayFab user? if so, see whether both accounts are linked already. if not, link and login. if so, login.


				LoginWithFacebookRequest loginFacebookRequest = new LoginWithFacebookRequest ();
				loginFacebookRequest.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
				loginFacebookRequest.CreateAccount = false;	// I have to link this account myself, only to already existent PlayFab accounts. If this fails, register

				PlayFabClientAPI.LoginWithFacebook(loginFacebookRequest, OnPlayfabFacebookAuthComplete, OnFacebookAuthPlayFabUserNotCreated);
				
				}
			else 
			{
				Debug.Log ("User cancelled login");
			}
		}
		else
		{
			// If Facebook authentication failed, we stop the cycle with the message
			Debug.LogError("[FacebookHandler] Facebook Auth Failed: " + result.Error + "    " + result.RawResult);
		}
	}
	private void AuthContinueCallback (ILoginResult result) {
		if (result == null || string.IsNullOrEmpty (result.Error)) {
			if (FB.IsLoggedIn) {

				// AccessToken class will have session details
				AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				MasterManager.facebookAccessToken = aToken;

				Debug.Log("[FacebookHandler] Facebook Auth Complete! Access Token: " + aToken + "   Logging into PlayFab...");

				// is the same email already as a PlayFab user? if so, see whether both accounts are linked already. if not, link and login. if so, login.


				LoginWithFacebookRequest loginFacebookRequest = new LoginWithFacebookRequest ();
				loginFacebookRequest.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
				loginFacebookRequest.CreateAccount = false;	// I have to link this account myself, only to already existent PlayFab accounts. If this fails, register

				PlayFabClientAPI.LoginWithFacebook(loginFacebookRequest, OnPlayfabFacebookAuthCompleteViaContinueButton, OnFacebookAuthPlayFabUserNotCreatedViaContinueButton);

			}
			else 
			{
				Debug.Log ("User cancelled login");
			}
		}
		else
		{
			// If Facebook authentication failed, we stop the cycle with the message
			Debug.LogError("[FacebookHandler] Facebook Auth Failed: " + result.Error + "    " + result.RawResult);
		}
	}

	private void OnFacebookAuthPlayFabUserNotCreated(PlayFabError error)
	{
		//PlayFabErrorCode code = error.Error;
		//Debug.LogError("[FacebookHandler] OnFacebookAuthPlayFabUserNotCreated CallBack error: " + code);

		if (string.Compare(error.GenerateErrorReport(), "User not found", true) == 0)
		{
			// instead of authenticating another area, for the user to fill in his email plus address, why dont you create that automatically for him?
			// because facebook IDs are unique, get the email for username, and get facebook name and put it as password (without any spaces)
			StartCoroutine (HandleUserLoginWithoutPlayFabAccount ());
			Debug.Log("[FacebookHandler] Facebook account not associated with PlayFab profile. Creating one, or linking to an existent one!");
		}
		else 
		{
			Debug.LogError("[FacebookHandler] OnFacebookAuthPlayFabUserNotCreated CallBack error: " + error.GenerateErrorReport());
		}

		// I might have to link accounts here: facebook, with an existent PlayFab email addresss

		callbackReturned = true;
	}

	private void OnFacebookAuthPlayFabUserNotCreatedViaContinueButton(PlayFabError error)
	{
		Debug.LogError("[FacebookHandler] OnFacebookAuthPlayFabUserNotCreated via continueButton CallBack error: " + error.GenerateErrorReport());

		//callbackReturned = true;
	}

	/**
	 * 	This method will use Facebook email and user name to automatically create a PlayFab account NOT FINISHED
	 */
	IEnumerator HandleUserLoginWithoutPlayFabAccount()
	{
		RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
		request.RequireBothUsernameAndEmail = false;
		request.TitleId = "CCF6";
	


		FB.API ("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		request.DisplayName = facebookGenericString;
		facebookGenericString = "";
		MasterManager.activePlayerName = request.DisplayName;


		FB.API ("me?fields=email", Facebook.Unity.HttpMethod.GET, GetFacebookEmail);
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		Debug.Log ("There is a facebook email: " + facebookGenericString);
		request.Email = facebookGenericString;
		MailAddress facebookEmail = new MailAddress(facebookGenericString);
		request.Username = null;
		request.Password = facebookEmail.User; // password is the same as the email from facebook, up to the @....com
		facebookGenericString = "";
	

		Debug.Log("[FacebookHandler] Doing the player register in PlayFab for " + request.DisplayName);
		PlayFabClientAPI.RegisterPlayFabUser(request,OnFacebookAuthDONEPlayFabRegisterSuccess,OnPlayFabRegisterErrorAfterFacebookLoginAccepted);



	}

	private void OnFacebookAuthDONEPlayFabRegisterSuccess(RegisterPlayFabUserResult result)
	{
		// record active player
		MasterManager.activePlayFabId = result.PlayFabId;
		Debug.Log ("[FacebookHandler] Active PlayFabID recorded:  " + MasterManager.activePlayFabId);



		// link this account to PlayFab account
		var request = new LinkFacebookAccountRequest
		{
			AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
			ForceLink = true
		};
		PlayFabClientAPI.LinkFacebookAccount(request, OnLinkFacebookAccountSuccess, CallBackError);


//		MenuManager.instance.menuPanel.SetActive (false);
		//MenuManager.instance.signUpPanel.SetActive (false);
		//MenuManager.instance.loginSotSPanel.SetActive (false);
		//MenuManager.instance.isLoggedIn = true;
		//SceneManager.LoadSceneAsync("Secrets of the South V1", LoadSceneMode.Additive);
//		MasterManager.activePlayerName = result.Username;
		Debug.Log ("[FacebookHandler] Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
		UpdateAvatarURLOfPlayer ();
		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}

	// Fails to register a user for the first time
	private void OnPlayFabRegisterErrorAfterFacebookLoginAccepted(PlayFabError error)
	{
		Debug.LogError("[FacebookHandler] Creation of PlayFab unique account after successful facebook login failed permanently: " + error.GenerateErrorReport());
	}

	/**
	 * 	This method will enter the game through Facebook
	 */
	IEnumerator HandleUserLoginWithPlayFabAccount()
	{
		string facebookName = "";
		// activePlayerAvatarURL
		string facebookID = "";

		FB.API ("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
		// change this shit
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		facebookName = facebookGenericString;
		facebookGenericString = "";

		FB.API ("me?fields=id", Facebook.Unity.HttpMethod.GET, GetFacebookID);
		// change this shit
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		facebookID = facebookGenericString;
		facebookGenericString = "";


		FB.API ("me?fields=email", Facebook.Unity.HttpMethod.GET, GetFacebookEmail);
		// change this shit
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		string temporaryFacebookEmail = facebookGenericString;
		facebookGenericString = "";

		MailAddress addr = new MailAddress (temporaryFacebookEmail);



		MasterManager.activePlayerEmail = temporaryFacebookEmail;
		MasterManager.activePlayerPassword = addr.User;

		// when login is successful
		//MenuManager.instance.menuPanel.SetActive (false);
		//MenuManager.instance.signUpPanel.SetActive (false);
		//MenuManager.instance.loginSotSPanel.SetActive (false);
		//MenuManager.instance.isLoggedIn = true;
		//SceneManager.LoadSceneAsync("Secrets of the South V1", LoadSceneMode.Additive);
		MasterManager.activePlayerName = facebookName;
		Debug.Log ("Welcome to the Secrets of the South, " + facebookName);
		MasterManager.activePlayerAvatarURL = "https://graph.facebook.com/" + facebookID + "/picture?type=large";
		Debug.Log ("... with Facebook ID: " + facebookID);
//		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + facebookName);
		UpdateAvatarURLOfPlayer ();
		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}
	IEnumerator HandleUserLoginWithPlayFabAccountViaContinueButton()
	{	
		Debug.Log ("[FacebookHandler] HandleUserLoginWithPlayFabAccountViaContinueButton SUCCESSFUL");
		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
		yield return null;
	}

	IEnumerator LoginWithFacebookAccountButInvalidPlayFabAccountID()
	{
		string facebookName = "";

		FB.API ("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
		// change this shit
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		facebookName = facebookGenericString;
		facebookGenericString = "";
		FB.API ("me?fields=email", Facebook.Unity.HttpMethod.GET, GetFacebookEmail);
		// change this shit
		while ((!callbackReturned) && (facebookGenericString == string.Empty)) {
			yield return new WaitForSeconds (0.1f);
		}
		callbackReturned = false;
		temporaryFacebookEmail = facebookGenericString;
		facebookGenericString = "";



		MasterManager.activePlayerName = facebookName;
		//Debug.Log ("Welcome to the Secrets of the South, " + facebookName);
		//		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + facebookName);

		MailAddress addr = new MailAddress (temporaryFacebookEmail);

	
		RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
		request.RequireBothUsernameAndEmail = false;
		request.TitleId = "CCF6";
		request.Username = null;
		request.Email = temporaryFacebookEmail;
		request.Password = addr.User;
		request.DisplayName = facebookName;
		MasterManager.activePlayerEmail = temporaryFacebookEmail;
		MasterManager.activePlayerPassword = addr.User;

		Debug.Log("[FacebookHandler] Doing the player register request for an existent facebook login, but no valid PlayFab account"+request.DisplayName);
		PlayFabClientAPI.RegisterPlayFabUser(request,OnPlayFabRegisterSuccess,OnPlayFabRegisterError);


	}

	// register a user for the first time
	private void OnPlayFabRegisterSuccess(RegisterPlayFabUserResult result)
	{
		// record active player
		MasterManager.activePlayFabId = result.PlayFabId;

		Debug.Log ("[FacebookHandler] Active PlayFabID recorded:  " + MasterManager.activePlayFabId);
		Debug.Log ("[FacebookHandler] Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + MasterManager.activePlayerName);
		UpdateAvatarURLOfPlayer ();
		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();
	}


	// facebook login
	// When processing both results, we just set the message, explaining what's going on.
	private void OnPlayfabFacebookAuthComplete(LoginResult result)
	{
		//Debug.Log("[FacebookHandler] PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
		// PlayFab user exists. record its unique login ID.
		MasterManager.activePlayFabId = result.PlayFabId;
		Debug.Log ("[FacebookHandler] Active PlayFabID recorded:  " + MasterManager.activePlayFabId);

		// this coroutine is called to handle login Through Facebook
		StartCoroutine (HandleUserLoginWithPlayFabAccount ());
	}
	// facebook login
	// When processing both results, we just set the message, explaining what's going on.
	private void OnPlayfabFacebookAuthCompleteViaContinueButton(LoginResult result)
	{

		StartCoroutine (HandleUserLoginWithPlayFabAccountViaContinueButton ());
	}

	// facebook login
	private void CallBackError(PlayFabError error)
	{
		Debug.LogError("[FacebookHandler] CallBack error: " + error.GenerateErrorReport());
		callbackReturned = true;
	}

	void GetFacebookName(Facebook.Unity.IGraphResult result)
	{
		facebookGenericString = result.ResultDictionary["name"].ToString();
		//Debug.Log ("Name: " + facebookGenericString);
		callbackReturned = true;
	}
	void GetFacebookEmail(Facebook.Unity.IGraphResult result)
	{
		facebookGenericString = result.ResultDictionary["email"].ToString();
		//Debug.Log ("Email: " + facebookGenericString);
		callbackReturned = true;
	}
	void GetFacebookID(Facebook.Unity.IGraphResult result)
	{
		facebookGenericString = result.ResultDictionary["id"].ToString();
		//Debug.Log ("ID: " + facebookGenericString);
		callbackReturned = true;
	}

	private void OnLinkFacebookAccountSuccess(LinkFacebookAccountResult result)
	{
		Debug.Log ("[FacebookHandler] Apparently, the Facebook user is linked to PlayFab.");
		callbackReturned = true;
	}

	// Fails to register a user for the first because email address likely already exists as another PlayFabID. Link Facebook with this PlayFabID.
	private void OnPlayFabRegisterError(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		if (errorcode == PlayFabErrorCode.EmailAddressNotAvailable) {
			// instead of authenticating another area, for the user to fill in his email plus address, why dont you create that automatically for him?
			// because facebook IDs are unique, get the email for username, and get facebook name and put it as password (without any spaces)
			Debug.Log ("[FacebookHandler] The email address is already in use. bummer! Now, link facebook to that existent email. Lets login to it.");


			LoginWithEmailAddressRequest logintoPlayFabwithEmailrequest = new LoginWithEmailAddressRequest ();
			logintoPlayFabwithEmailrequest.Email = facebookEmailToLinkToPlayFabID;
			logintoPlayFabwithEmailrequest.TitleId = "CCF6";
			MailAddress addr = new MailAddress (logintoPlayFabwithEmailrequest.Email);
			logintoPlayFabwithEmailrequest.Password = addr.User;
			MasterManager.activePlayerEmail = facebookEmailToLinkToPlayFabID;
			MasterManager.activePlayerPassword = addr.User;

			// first login, then link accounts
			PlayFabClientAPI.LoginWithEmailAddress (logintoPlayFabwithEmailrequest, logintoExistentPlayFabAccountSuccess, logintoExistentPlayFabAccountError);

		} else {
			Debug.LogError ("[FacebookHandler] CallBack error: " + error.GenerateErrorReport ());
			Debug.LogError ("[FacebookHandler] Letting the user login anyhow.");
			// this coroutine is called to handle login Through Facebook
			StartCoroutine (LoginWithFacebookAccountButInvalidPlayFabAccountID ());
		}
	}
	private void logintoExistentPlayFabAccountSuccess(LoginResult result)
	{
		Debug.Log ("[FacebookHandler] Yey. I could login to PlayFab existent account "+facebookEmailToLinkToPlayFabID+". Now, Let's link these accounts!");

		LinkFacebookAccountRequest linkfacebookaccountrequest = new LinkFacebookAccountRequest();
		linkfacebookaccountrequest.AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
		linkfacebookaccountrequest.ForceLink = true;

		PlayFabClientAPI.LinkFacebookAccount (linkfacebookaccountrequest, OnPlayFabRegisterLinkFacebookAccountSuccess, OnPlayFabRegisterLinkFacebookAccountError);

	}
	private void logintoExistentPlayFabAccountError(PlayFabError error)
	{
		Debug.LogError ("[FacebookHandler] Shit. I could not login into the existent PlayFab account, because: " + error.GenerateErrorReport ());
	}

	private void OnPlayFabRegisterLinkFacebookAccountSuccess(LinkFacebookAccountResult result)
	{
		// get email address from playfab account
		GetAccountInfoRequest requestforPlayFabID = new GetAccountInfoRequest ();
		requestforPlayFabID.Email = facebookEmailToLinkToPlayFabID;
		PlayFabClientAPI.GetAccountInfo (requestforPlayFabID, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDSuccess, OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDError);

		// finally, complete login by removing the login window 


	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDSuccess (GetAccountInfoResult result)
	{
		MasterManager.activePlayFabId = result.AccountInfo.PlayFabId;

		// when login is successful
		//MenuManager.instance.menuPanel.SetActive (false);
		//MenuManager.instance.signUpPanel.SetActive (false);
		//MenuManager.instance.loginSotSPanel.SetActive (false);
		//MenuManager.instance.isLoggedIn = true;
		//SceneManager.LoadSceneAsync("Secrets of the South V1", LoadSceneMode.Additive);
		MasterManager.activePlayerName = facebookDisplayNameToDisplayAfterLinkingToPlayFabID;
		Debug.Log ("Welcome to the Secrets of the South, " + facebookDisplayNameToDisplayAfterLinkingToPlayFabID);
		GameManager.instance._referenceDisplayManager.DisplaySystemMessage ("Welcome to the Secrets of the South, " + facebookDisplayNameToDisplayAfterLinkingToPlayFabID);

		// update playfabid display name, or attempt to, because the email might be bigger than the allowed number of characters
		UpdateUserTitleDisplayNameRequest displaynameupdate = new UpdateUserTitleDisplayNameRequest();
		displaynameupdate.DisplayName = facebookDisplayNameToDisplayAfterLinkingToPlayFabID;

		PlayFabClientAPI.UpdateUserTitleDisplayName (displaynameupdate, 
			OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDUpdateDisplayNameSuccess, 
			OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDUpdateDisplayNameError);

		UpdateAvatarURLOfPlayer ();
		// save player credentials (token, playfab id and display name)
		Save ();

		// Load the next scene, and handle this scene objects
		LoginMenuManager.instance.LoadSceneAndManageObjectsOnLoadScene ();

	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
	{
		Debug.Log ("[FacebookHandler] Aftter linking facebook account with the existent PlayFab email address, the Display Name was also updated with the one from facebook.");
	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDUpdateDisplayNameError(PlayFabError error)
	{
		Debug.Log ("[FacebookHandler] Aftter linking facebook account with the existent PlayFab email address, the Display Name was also updated with the one from facebook.");
		Debug.LogError ("[FacebookHandler] Reason: " + error.GenerateErrorReport ());
	}
	private void OnPlayFabRegisterLinkFacebookAccountGetPlayFabIDError (PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[FacebookHandler] Attempted to link facebook account to an existent PlayFabID, but I could not get the existent PlayFabID from Facebook email. I got this code: "+errorcode);
		Debug.LogError ("[FacebookHandler] Full report: "+error.GenerateErrorReport());
	}
	private void OnPlayFabRegisterLinkFacebookAccountError (PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		Debug.LogError ("[FacebookHandler] Attempted to link facebook account to an existent PlayFabID, and failed with the code: "+errorcode);
		Debug.LogError ("[FacebookHandler] Full report: "+error.GenerateErrorReport());
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
		Debug.Log ("[FacebookHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

		SotSPlayerInfo data = new SotSPlayerInfo ();
		data.isFacebookAccessToken = true;
		data.playFabEmail = MasterManager.activePlayerEmail;
		data.playFabPassword = MasterManager.activePlayerPassword;
		data.activePlayerName = MasterManager.activePlayerName;
		data.activePlayFabId = MasterManager.activePlayFabId;
		data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
		data.desiredMax_DistanceOfChallenges = 500.0;
		data.route = MasterManager.route;
		data.language = MasterManager.language;
		data.teamID = MasterManager.instance.teamID;
		data.teamName = MasterManager.instance.teamName;
		data.teamRefIcon = MasterManager.instance.teamRefIcon;

		bf.Serialize (file, data);
		file.Close ();
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
