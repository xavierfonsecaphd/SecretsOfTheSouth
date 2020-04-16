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

public class Hunter_window_QRFound_manager : MonoBehaviour {

	public Text nameOfTheChallenge, textHolder;
	public Image imageHolder;
	public GameObject textContentPanel, imageContentPanel;

	[HideInInspector]
	public static Hunter_window_QRFound_manager instance { set; get ; }

	// This is a singleton
	[HideInInspector]
	private static Hunter_window_QRFound_manager singleton;

	// Use this for initialization
	void Awake () {
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			instance = Hunter_window_QRFound_manager.singleton;


		} else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}

	/* // not executed?
	public void ActivateWindow(bool isText, string content)
	{
		GameManager.instance.isAnyWindowOpen = true;
		GameManager.instance.hunterChallengeQRFoundWindow.SetActive (true);

		if (isText) {
			textContentPanel.SetActive (true);
			imageContentPanel.SetActive (false);

			textHolder.text = content;
		} else { // is Image
			textContentPanel.SetActive (true);
			imageContentPanel.SetActive (false);
			textHolder.text = "Loading image ...";
			StartCoroutine (ActivateWindowPictureThread( content));
		}


	}*/
		
	public IEnumerator ActivateWindowPictureThread(string content)
	{
		
		// generate sprite
		Texture2D texture;
		using (WWW www = new WWW(content))
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
					
					}
					//yield return wwwDefault;

				} else {
					// assign texture
					texture = www.texture;
				}
			}

			// Wait for download to complete
			//yield return www;


			www.Dispose ();
		}

		Rect rec = new Rect(0, 0, texture.width, texture.height);
		Sprite img = Sprite.Create(texture, rec, new Vector2(0, 0), 100.0f, 0 , SpriteMeshType.Tight);

		textContentPanel.SetActive (false);
		imageContentPanel.SetActive (true);

		imageHolder.sprite = img;

		yield break;
	}

	public void CloseWindow()
	{
		textContentPanel.SetActive (false);
		imageContentPanel.SetActive (false);

		GameManager.instance.hunterChallengeQRFoundWindow.SetActive(false);
		GameManager.instance.isAnyWindowOpen = false;
		//txt44
		string[] displayMessage4 = {"Congratulations, you just solved a Hunter challenge by finding the hidden QR code! Well done.", 
			"Gefeliciteerd, je hebt zojuist een Jager challenge opgelost door de QR code te vinden! Goed gedaan.",
			"Boa, acabaste de resolver um desafio caçador ao encontrares o seu respetivo código QR! Parabéns."};
		// Gefeliciteerd, je hebt zojuist een asdfgfhchallenge opgelost door de verborgen QR-code te vinden! Goed gedaan.
		GameManager.instance._referenceDisplayManager.DisplayGameMessage (displayMessage4[MasterManager.language]);
	}
}
