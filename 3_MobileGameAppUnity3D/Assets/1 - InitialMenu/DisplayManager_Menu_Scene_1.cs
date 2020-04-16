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


public class DisplayManager_Menu_Scene_1 : MonoBehaviour {

	public static bool readyForNewMessages = true;

	public GameObject statusPanel;
	public Text statusText;
	public float displayTime;
	public float fadeTime;

	//[HideInInspector]
	public Color colorSystemMessage;
	//[HideInInspector]
	public Color colorGameMessage;
	//[HideInInspector]
	public Color colorErrorMessage;
	public Color panelColor;
	public Color shadow;

	public bool isProperlyInitialized = false;

	private IEnumerator fadeAlpha;//, fadeAlphaPanel;

	/*private static DisplayManager displayManager;


	public static DisplayManager Instance () {
		if (displayManager == null) {
			displayManager = FindObjectOfType(typeof (DisplayManager)) as DisplayManager;
			if (displayManager == null) {
				Debug.LogError ("[DisplayManager] There needs to be one active DisplayManager script on a GameObject in your scene.");
			}

			// Status Panel
			//if (displayManager.statusPanel == null) throw new System.Exception("[DisplayManager] Reference to the status panel is null!");

			// Status Text
			//if (displayManager.statusText == null) throw new System.Exception("[DisplayManager] Status Text reference is null!");
			displayManager.statusText.text = "";

			//displayManager.colorSystemMessage = Color.white;
			//displayManager.colorGameMessage = new Color32 (0xCB, 0xFB, 0xB0, 0xFF); // CBFBB0FF    Lime
			//displayManager.colorErrorMessage = Color.red;
		}



		// panel 0xFFFFFF00 transparent
		// panel 0xFFFFFFFF opaque


		return displayManager;
	}
*/

	void Start () {


		// Status Panel
		if (statusPanel == null) throw new System.Exception("[DisplayManager] Reference to the status panel is null!");

		// Status Text
		if (statusText == null) throw new System.Exception("[DisplayManager] Status Text reference is null!");
		statusText.text = "";

		// colours
		//if (colorSystemMessage == null) throw new System.Exception("[DisplayManager] Colours are null!");
		//		if (colorGameMessage == null) throw new System.Exception("[DisplayManager] Colours are null!");
		//if (colorErrorMessage == null) throw new System.Exception("[DisplayManager] Colours are null!");
		//if (panelColor == null) throw new System.Exception("[DisplayManager] Colours are null!");
		//if (shadow == null) throw new System.Exception("[DisplayManager] Colours are null!");

		isProperlyInitialized = true;
		Debug.Log ("[DisplayManager] Display was properly initialized");
	}

	public void DisplayGameMessage (string message) {
		readyForNewMessages = false;
		// first, stop fading processes of other messages.
		if (fadeAlpha != null) {
			StopCoroutine (fadeAlpha);
		}
		//if (fadeAlphaPanel != null) {

		//	StopCoroutine (fadeAlphaPanel);
		//}

		// then, alter message
		statusText.color = colorGameMessage ;
		statusText.GetComponent<Shadow> ().effectColor = shadow;
		//statusPanel.GetComponent<Image>().color = new Color32 (0x00, 0x00, 0x00, 0x4A); // opaque Black
		statusText.text = message;

		// then, render it
		// fade text
		fadeAlpha = FadeAlphaText ();
		StartCoroutine (fadeAlpha);
		// fade panel behind the text as well
		//fadeAlphaPanel = FadeAlphaPanel ();
		//StartCoroutine (fadeAlphaPanel);
	}

	public void DisplaySystemMessage (string message) {
		statusPanel.SetActive (true);
		readyForNewMessages = false;
		// first, stop fading processes of other messages.
		if (fadeAlpha != null) {
			StopCoroutine (fadeAlpha);
		}
		//if (fadeAlphaPanel != null) {

		//	StopCoroutine (fadeAlphaPanel);
		//}

		// then, alter message
		statusText.color = colorSystemMessage;
		statusText.GetComponent<Shadow> ().effectColor = shadow;
		statusPanel.GetComponent<Image> ().color = new Color32(0xFF,0xFF,0xFF,0xD8); //FFFFFFD8
		statusText.text = message;

		// then, render it
		// fade text
		fadeAlpha = FadeAlphaText ();
		StartCoroutine (fadeAlpha);
		// fade panel behind the text as well
		//fadeAlphaPanel = FadeAlphaPanel ();
		//StartCoroutine (fadeAlphaPanel);
	}

	public void DisplaySystemMessageNonFading (string message) {
		statusPanel.SetActive (true);
		//readyForNewMessages = false;
		// first, stop fading processes of other messages.
		if (fadeAlpha != null) {
			StopCoroutine (fadeAlpha);
		}

		statusText.color = colorSystemMessage;
		statusText.GetComponent<Shadow> ().effectColor = shadow;
		statusPanel.GetComponent<Image>().color = new Color32 (0x13, 0x41, 0x6D, 0xFF); // green -> 13416DFF this is blueish
		statusText.text = message;

		// then, render it
		// fade text
		fadeAlpha = FadeAlphaTextTimeless ();
		StartCoroutine (fadeAlpha);
		// fade panel behind the text as well
		//fadeAlphaPanel = FadeAlphaPanel ();
		//StartCoroutine (fadeAlphaPanel);
	}

	public void DisplayErrorMessage (string message) {
		statusPanel.SetActive (true);
		readyForNewMessages = false;
		// first, stop fading processes of other messages.
		if (fadeAlpha != null) {
			StopCoroutine (fadeAlpha);
		}
		//if (fadeAlphaPanel != null) {

		//	StopCoroutine (fadeAlphaPanel);
		//}

		// then, alter message
		statusText.color = colorErrorMessage;
		statusText.GetComponent<Shadow> ().effectColor = shadow;
		//statusPanel.GetComponent<Image>().color = new Color32 (0x00, 0x00, 0x00, 0x4A); // opaque Black
		statusText.text = message;
		statusPanel.GetComponent<Image> ().color = Color.red;

		// then, render it
		// fade text
		fadeAlpha = FadeAlphaText ();
		StartCoroutine (fadeAlpha);
		// fade panel behind the text as well
		//fadeAlphaPanel = FadeAlphaPanel ();
		//StartCoroutine (fadeAlphaPanel);
	}


	IEnumerator FadeAlphaText () {
		Color resetColor = statusText.color;
		resetColor.a = 1;
		statusText.color = resetColor;

		//Color resetColor = statusText.color;
		Color resetPanelColor = statusPanel.GetComponent<Image> ().color;
		resetColor.a = 1;
		//resetPanelColor.a = 0.4f;
		//statusText.color = resetColor;
		statusPanel.GetComponent<Image> ().color = resetPanelColor;

		yield return new WaitForSeconds (displayTime);

		while ((statusText.color.a > 0) || (statusPanel.GetComponent<Image> ().color.a > 0)) {

			// text
			if (statusText) {
				if (statusText.color.a > 0) {
					Color displayColor = statusText.color;
					displayColor.a -= Time.deltaTime / fadeTime;
					statusText.color = displayColor;
				}

				// panel
				if (statusPanel.GetComponent<Image> ().color.a > 0) {
					//Color displayColor = statusText.color;
					Color displayPanelColor = statusPanel.GetComponent<Image> ().color;
					//displayColor.a -= Time.deltaTime / fadeTime;
					displayPanelColor.a -= Time.deltaTime / fadeTime;
					//statusText.color = displayColor;
					statusPanel.GetComponent<Image> ().color = displayPanelColor;

				}
			} else {
				if (statusPanel) {
					statusPanel.SetActive (false);
				}

				readyForNewMessages = true;
				yield return null;
			}
				
			readyForNewMessages = true;
			if (statusPanel) {
				statusPanel.SetActive (false);
			}
			yield return null;
		}
		readyForNewMessages = true;
		yield return null;
	}
	IEnumerator FadeAlphaTextTimeless () {
		Color resetColor = statusText.color;
		resetColor.a = 1;
		statusText.color = resetColor;

		//Color resetColor = statusText.color;
		Color resetPanelColor = statusPanel.GetComponent<Image> ().color;
		resetColor.a = 1;
		//resetPanelColor.a = 0.4f;
		//statusText.color = resetColor;
		statusPanel.GetComponent<Image> ().color = resetPanelColor;

		//readyForNewMessages = true;
		yield return null;
	}
}
