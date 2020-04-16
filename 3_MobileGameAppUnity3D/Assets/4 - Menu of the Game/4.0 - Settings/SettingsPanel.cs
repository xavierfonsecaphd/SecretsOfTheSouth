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

// this is for Save and Load methods
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class SettingsPanel : MonoBehaviour {

	public InputField updateDisplayNameInputField;
	public InputField updateAvatarURLInputField;
	public InputField updateDistanceChallenges;

	public static SettingsPanel instance { set; get ; }
	private static SettingsPanel singleton;

	void Awake()
	{
		if (singleton == null) 
		{
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = SettingsPanel.singleton;

		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	void Start()
	{
		// put the url we're using, to be easier to change
		updateAvatarURLInputField.text = MasterManager.activePlayerAvatarURL;
		updateDisplayNameInputField.text = MasterManager.activePlayerName;
		updateDistanceChallenges.text = MasterManager.desiredMaxDistanceOfChallenges.ToString();
	}

	// This will delete the stored credentials of the player on the phone. This will maintain the player's profile.
	public void ResetPlayerButton()
	{
		if (File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			File.Delete (Application.persistentDataPath + "/SotSPlayerInfo.dat");
			//txt33
			string [] displayMessage1 = {"Profile erased from the smartphone. Changes will take effect next time you launch the game.",
				"Profiel gewist van de smartphone. Wijzigingen worden van kracht de volgende keer dat u het spel start.",
				"Perfil apagado do telemóvel. As alterações terão efeito a partir da próxima vez que abrires o jogo."};
			MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage1[MasterManager.language]);
		} else {
			//txt34
			string [] displayMessage2 = {"No profile available to delete.",
				"Geen profiel beschikbaar om te verwijderen.",
				"O perfil já foi apagado."};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage2[MasterManager.language]);
		}
	}

	public void resetTeam ()
	{
		if (MasterManager.instance.playerHasTeam) {
			MasterManager.instance.playerHasTeam = false;

			MasterManager.instance.teamID = null;
			MasterManager.instance.teamName = "";
			MasterManager.instance.teamRefIcon = null;


			StartCoroutine (RemoveTeamFromPlayerAsync(MasterManager.instance.teamID, MasterManager.instance.teamName, MasterManager.instance.teamRefIcon));

			//txt35
			string [] displayMessage3 = {"You were removed from the team you had.",
				"Je bent verwijderd uit het team dat je had.",
				"Retiraste-te da equipa que tinhas."};
			MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage3[MasterManager.language]);
		} else {
			//txt36
			string [] displayMessage4 = {"You do not have a team. Nothing done.",
				"Je hebt geen team. Niets gedaan.",
				"Tu não estás inserido numa equipa. Não aconteceu nada."};
			MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage4[MasterManager.language]);
		}
	}

	public static IEnumerator RemoveTeamFromPlayerAsync(string this_teamID, string this_teamName, string this_teamRefImage)
	{
		
			string email;
			string password;
			bool isFacebookAccessToken = false;
			double distance;
			// first, collect existent data from player
			// playerDataStored
			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();

				email = data.playFabEmail;
				password = data.playFabPassword;
				isFacebookAccessToken = data.isFacebookAccessToken;
				distance = data.desiredMax_DistanceOfChallenges;

				// then, delete the file with the previous name
				DeleteFileStatic ();

				// then, create a new file, and re-store all the info in it
				bf = new BinaryFormatter ();

				if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
					file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
					file.Close ();
				}

				file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

				data = new SotSPlayerInfo ();
				data.isFacebookAccessToken = isFacebookAccessToken;
				data.playFabEmail = email;
				data.playFabPassword = password;
				data.activePlayerName = MasterManager.activePlayerName;
				data.activePlayFabId = MasterManager.activePlayFabId;
				data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
				data.desiredMax_DistanceOfChallenges = distance;
				data.teamID = this_teamID;
				data.teamName = this_teamName;
				data.teamRefIcon = this_teamRefImage;
				data.route = MasterManager.route;
				data.language = MasterManager.language;

				bf.Serialize (file, data);
				file.Close ();


			} else {
				Debug.LogError ("[LoginMenuManager] Distance could not be updated. File does not seem to exist.");
			//txt37
			string [] displayMessage5 = {"Could not update the Distance. Try it later.",
				"Kan de afstand niet bijwerken. Probeer het later nog eens.",
				"Não foi possível atualizar a distância. Tenta de novo mais tarde."};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage5[MasterManager.language]);
			}


		yield return null;
	}


	public void DeleteFile()
	{
		if (File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			File.Delete (Application.persistentDataPath + "/SotSPlayerInfo.dat");

		}
	}

	public static void DeleteFileStatic()
	{
		if (File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			File.Delete (Application.persistentDataPath + "/SotSPlayerInfo.dat");

		}
	}

	public void UpdateDisplayNameOfPlayer()
	{
		string newName = updateDisplayNameInputField.text;
		// between 3 and 25 characters
		// update playfabid display name, or attempt to, because the email might be bigger than the allowed number of characters
		UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest();
		request.DisplayName = newName;
		PlayFabClientAPI.UpdateUserTitleDisplayName (request, UpdateDisplayNameOfPlayerSuccess, UpdateDisplayNameOfPlayerFail);
	}
	private void UpdateDisplayNameOfPlayerSuccess(UpdateUserTitleDisplayNameResult result)
	{
		MasterManager.activePlayerName = result.DisplayName;
		Debug.Log ("[SettingsPanel] User Display Name was successfully updated.");

		//txt38
		string [] displayMessage5 = {"User Display Name was successfully updated to ",
			"User Display Name is succesvol bijgewerkt naar ",
			"O teu nome de jogador foi atualizado com sucesso para "};
		MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage5[MasterManager.language] + MasterManager.activePlayerName);

		// after you did the Change, regenerate a new file in the smartphone's storage system
		RegenerateStoredFileWithNewInfo ();
	}
	private void UpdateDisplayNameOfPlayerFail(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;

		//txt39
		string [] displayMessage6 = {"Could not update display name of the player: ",
			"De weergavenaam van de speler kan niet worden bijgewerkt: ",
			"Não foi possível atualizar o teu nome de jogador para: "};
		MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage6[MasterManager.language]+errorcode);

		Debug.LogError ("[SettingsPanel] Could not update display name of the player. I got this code: "+errorcode);
		Debug.LogError ("[SettingsPanel] Full report: "+error.GenerateErrorReport());
	}


	// update the URL of the player's Avatar in PlayFab
	public void UpdateAvatarURLOfPlayer()
	{
		
		if (string.Compare (updateAvatarURLInputField.text, string.Empty) == 0) {
			//txt40
			string [] displayMessage7 = {"Please give an URL of an image. Example: www.website.com/avatar.png",
				"Geef een URL van een afbeelding op. Voorbeeld: www.website.com/avatar.png",
				"Introduz por favor um endereço URL duma imagem válido. Exemplo: www.website.com/avatar.png"};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage7[MasterManager.language]);
		}
		else
		{
			UpdateAvatarUrlRequest request = new UpdateAvatarUrlRequest();
			request.ImageUrl = updateAvatarURLInputField.text;//newURL;
			PlayFabClientAPI.UpdateAvatarUrl (request, UpdateAvatarURLOfPlayerSuccess, UpdateAvatarURLOfPlayerFail);
			//PlayFabClientAPI.UpdateUserTitleDisplayName (request, UpdateDisplayNameOfPlayerSuccess, UpdateDisplayNameOfPlayerFail);
		}
	}
	private void UpdateAvatarURLOfPlayerSuccess(EmptyResult result)
	{
		Debug.Log ("[SettingsPanel] URL of the Avatar Updated successfully.");
		string newURL = updateAvatarURLInputField.text;
		MasterManager.activePlayerAvatarURL = newURL;
		updateAvatarURLInputField.text = newURL;	// clear it, just for feedback purposes

		// then, delete the file with the previous Avatar URL
		//txt41
		string [] displayMessage7 = {"URL of the Avatar Updated successfully to: ",
			"URL van de Avatar succesvol bijgewerkt naar: ",
			"URL do Avatar foi atualizado com sucesso para: "};
		MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage7[MasterManager.language] + MasterManager.activePlayerAvatarURL);

		// after you did the Change, regenerate a new file in the smartphone's storage system
		RegenerateStoredFileWithNewInfo ();

		StartCoroutine(MenuManager.instance.LoadsAvatarImage(MasterManager.activePlayerAvatarURL, MenuManager.instance.buttons[6]));
	}
	private void UpdateAvatarURLOfPlayerFail(PlayFabError error)
	{
		PlayFabErrorCode errorcode = error.Error;
		//txt42
		string [] displayMessage8 = {"Could not update the URL of the Avatar: ",
			"Kon de URL van de avatar niet bijwerken: ",
			"Não foi possível atualizar o endereço URL do Avatar: "};
		MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage8[MasterManager.language]+errorcode);

		Debug.LogError ("[SettingsPanel] Could not update the URL of the Avatar of the player. I got this code: "+errorcode);
		Debug.LogError ("[SettingsPanel] Full report: "+error.GenerateErrorReport());
	}

	public static void UpdateTeamOfPlayer(string this_teamID, string this_teamName, string this_teamRefImage)
	{
		if ((string.Compare (this_teamID, string.Empty) != 0) && (string.Compare (this_teamName, string.Empty) != 0) && (string.Compare (this_teamRefImage, string.Empty) != 0)) {
			string email;
			string password;
			bool isFacebookAccessToken = false;
			double distance;
			// first, collect existent data from player
			// playerDataStored
			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();

				email = data.playFabEmail;
				password = data.playFabPassword;
				isFacebookAccessToken = data.isFacebookAccessToken;
				distance = data.desiredMax_DistanceOfChallenges;

				// then, delete the file with the previous name
				DeleteFileStatic ();

				// then, create a new file, and re-store all the info in it
				bf = new BinaryFormatter ();

				if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
					file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
					file.Close ();
				}

				file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

				data = new SotSPlayerInfo ();
				data.isFacebookAccessToken = isFacebookAccessToken;
				data.playFabEmail = email;
				data.playFabPassword = password;
				data.activePlayerName = MasterManager.activePlayerName;
				data.activePlayFabId = MasterManager.activePlayFabId;
				data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
				data.desiredMax_DistanceOfChallenges = distance;
				data.teamID = this_teamID;
				data.teamName = this_teamName;
				data.teamRefIcon = this_teamRefImage;
				data.route = MasterManager.route;
				data.language = MasterManager.language;

				bf.Serialize (file, data);
				file.Close ();


			} else {
				Debug.LogError ("[LoginMenuManager] Distance could not be updated. File does not seem to exist.");
				//txt37
				string [] displayMessage8 = {"Could not update the Distance. Try it later.",
					"Kan de afstand niet bijwerken. Probeer het later nog eens.",
					"Não foi possível atualizar a distância. Tenta de novo mais tarde."};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage8[MasterManager.language]);
			}
		} else {
			Debug.LogError ("Error trying to save the data of the team in the file. Some of the data is empty.");
		}


	}
	public static IEnumerator UpdateTeamOfPlayerAsync(string this_teamID, string this_teamName, string this_teamRefImage)
	{
		if ((string.Compare (this_teamID, string.Empty) != 0) && (string.Compare (this_teamName, string.Empty) != 0) && (string.Compare (this_teamRefImage, string.Empty) != 0)) {
			string email;
			string password;
			bool isFacebookAccessToken = false;
			double distance;
			// first, collect existent data from player
			// playerDataStored
			if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
				file.Close ();

				email = data.playFabEmail;
				password = data.playFabPassword;
				isFacebookAccessToken = data.isFacebookAccessToken;
				distance = data.desiredMax_DistanceOfChallenges;

				// then, delete the file with the previous name
				DeleteFileStatic ();

				// then, create a new file, and re-store all the info in it
				bf = new BinaryFormatter ();

				if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
					file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
					file.Close ();
				}

				file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
				Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

				data = new SotSPlayerInfo ();
				data.isFacebookAccessToken = isFacebookAccessToken;
				data.playFabEmail = email;
				data.playFabPassword = password;
				data.activePlayerName = MasterManager.activePlayerName;
				data.activePlayFabId = MasterManager.activePlayFabId;
				data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
				data.desiredMax_DistanceOfChallenges = distance;
				data.teamID = this_teamID;
				data.teamName = this_teamName;
				data.teamRefIcon = this_teamRefImage;
				data.route = MasterManager.route;
				data.language = MasterManager.language;

				bf.Serialize (file, data);
				file.Close ();


			} else {
				Debug.LogError ("[LoginMenuManager] Distance could not be updated. File does not seem to exist.");
				//txt37
				string [] displayMessage9 = {"Could not update the Distance. Try it later.",
					"Kan de afstand niet bijwerken. Probeer het later nog eens.",
					"Não foi possível atualizar a distância. Tenta de novo mais tarde."};
				MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage9[MasterManager.language]);
			}
		} else {
			Debug.LogError ("Error trying to save the data of the team in the file. Some of the data is empty.");
		}

		yield return null;
	}

	public void UpdateMaxDistance()
	{
		
		string email;
		string password;
		string newDistance = updateDistanceChallenges.text; 
		bool isFacebookAccessToken = false;

		// first, collect existent data from player
		// playerDataStored
		if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
			SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
			file.Close ();

			email = data.playFabEmail;
			password = data.playFabPassword;
			isFacebookAccessToken = data.isFacebookAccessToken;

			// then, delete the file with the previous name
			DeleteFile ();

			// then, create a new file, and re-store all the info in it
			bf = new BinaryFormatter ();

			if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
				file.Close ();
			}

			file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
			Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

			data = new SotSPlayerInfo ();
			data.isFacebookAccessToken = isFacebookAccessToken;
			data.playFabEmail = email;
			data.playFabPassword = password;
			data.activePlayerName = MasterManager.activePlayerName;
			data.activePlayFabId = MasterManager.activePlayFabId;
			data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
			data.desiredMax_DistanceOfChallenges = Convert.ToDouble (newDistance);
			data.teamID = MasterManager.instance.teamID;
			data.teamName = MasterManager.instance.teamName;
			data.teamRefIcon = MasterManager.instance.teamRefIcon;
			data.route = MasterManager.route;
			data.language = MasterManager.language;
			bf.Serialize (file, data);
			file.Close ();





			MasterManager.desiredMaxDistanceOfChallenges = Convert.ToDouble (newDistance);
			//StartCoroutine (ChallengesCanvasMenuManager.instance.LoadChallengesFromTheServer());
			GameManager.instance.reloadChallengesFromCloud = true;
			//txt43
			string [] displayMessage11 = {"Distance updated. From now on, you will only see challenges up to ",
				"Afstand bijgewerkt. Vanaf nu zie je alleen uitdagingen tot ",
				"A distância foi atualizada. A partir de agora, só irás ver os desafios até à distância de "};
			MenuManager.instance._referenceDisplayManager.DisplaySystemMessage (displayMessage11[MasterManager.language] + newDistance + " Kms.");


			//StartCoroutine (GameManager.instance.LoadChallengesFromTheCloud(
			//	MasterManager.desiredMaxDistanceOfChallenges, GPSLocationProvider_Xavier.instance.latlong));


		} else {
			Debug.LogError ("[LoginMenuManager] Distance could not be updated. File does not seem to exist.");
			//txt37
			string [] displayMessage10 = {"Could not update the Distance. Try it later.",
				"Kan de afstand niet bijwerken. Probeer het later nog eens.",
				"Não foi possível atualizar a distância. Tenta de novo mais tarde."};
			MenuManager.instance._referenceDisplayManager.DisplayErrorMessage (displayMessage10[MasterManager.language]);
		}

	}


	private void RegenerateStoredFileWithNewInfo()
	{
		string email;
		string password;
		bool isFacebookAccessToken = false;

		// first, collect existent data from player
		// playerDataStored
		if (System.IO.File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
			SotSPlayerInfo data = (SotSPlayerInfo)bf.Deserialize (file); // we are creating an object, pulling the object out of a file (generic), and have to cast it
			file.Close ();

			email = data.playFabEmail;
			password = data.playFabPassword;
			isFacebookAccessToken = data.isFacebookAccessToken;

			// then, delete the file with the previous name
			DeleteFile ();

			// then, create a new file, and re-store all the info in it
			bf = new BinaryFormatter ();

			if (!File.Exists (Application.persistentDataPath + "/SotSPlayerInfo.dat")) {
				file = File.Create (Application.persistentDataPath + "/SotSPlayerInfo.dat");
				file.Close ();
			}

			file = File.Open (Application.persistentDataPath + "/SotSPlayerInfo.dat", FileMode.Open);
			Debug.Log ("[SotSHandler] The saving path for the SotS player info is: " + Application.persistentDataPath + "/SotSPlayerInfo.dat");

			data = new SotSPlayerInfo ();
			data.isFacebookAccessToken = isFacebookAccessToken;
			data.playFabEmail = email;
			data.playFabPassword = password;
			data.activePlayerName = MasterManager.activePlayerName;
			data.activePlayFabId = MasterManager.activePlayFabId;
			data.playFabPlayerAvatarURL = MasterManager.activePlayerAvatarURL;
			data.desiredMax_DistanceOfChallenges = MasterManager.desiredMaxDistanceOfChallenges;
			data.teamID = "";
			data.teamName = "";
			data.teamRefIcon = "";
			MasterManager.activePlayerEmail = email;
			MasterManager.activePlayerPassword = password;
			data.route = MasterManager.route;
			data.language = MasterManager.language;

			bf.Serialize (file, data);
			file.Close ();

		} else {
			Debug.LogError ("[LoginMenuManager] file does not seem to exist.");
		}
	}
}
