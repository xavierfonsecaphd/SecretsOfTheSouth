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
using UnityEngine.Events;

public class AR_ModalWindow : MonoBehaviour {
/**
 * 
 * THIS IS NOT IMPLEMENTED. start from zero on AR
 * 
 * 
	[HideInInspector]
	public bool popedUp = false;
	public Text text, titleText;
	public Image iconImage;
	public Button yesButton, challengeButton, closeButton;
	//public GameObject modalPanelObject;
	public ScrollRect scrollViewPanel;
	//public RectTransform contentWindow; 

	private static AR_ModalWindow ar_modalPanel;

	public static AR_ModalWindow Instance()
	{
		
		if (!ar_modalPanel) {
			ar_modalPanel = GameManager.instance._referenceAR_ModalPanel;
			if (!ar_modalPanel) {
				// find all the objects on the game of this type. there has to be at least one
				ar_modalPanel = FindObjectOfType (typeof(AR_ModalWindow)) as AR_ModalWindow;
			}
			if (!ar_modalPanel)
				throw new UnityException ("[ModalWindowManager] There needs to be one active ModalWindowManager script on a game object in your scene.");
		}

		return ar_modalPanel;
	}

	public IEnumerator Choice (Sprite image, string titleOfChallenge, string textChallenge, UnityAction yesEvent, UnityAction answerChallengeEvent, UnityAction closeEvent)
	{
		ar_modalPanel.gameObject.SetActive (true);

		Debug.Log ("[AR_ModalWindow] AR choice Fired up.");

		// lets just clean any listeners that the button might have defined
		yesButton.onClick.RemoveAllListeners();
		yesButton.onClick.AddListener (yesEvent);
		yesButton.onClick.AddListener (ClosePanel);


		// lets just clean any listeners that the button might have defined
		challengeButton.onClick.RemoveAllListeners();
		challengeButton.onClick.AddListener (answerChallengeEvent);
		challengeButton.onClick.AddListener (ClosePanel);

		// lets just clean any listeners that the button might have defined
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener (closeEvent);
		closeButton.onClick.AddListener (ClosePanel);

		// dealing with all the contents of the modal window (image, text, buttons)
		this.text.text = textChallenge;
		this.titleText.text = titleOfChallenge;
		//this.iconImage.gameObject.SetActive (false);
		this.iconImage.sprite = image;
		yesButton.gameObject.SetActive (true);			
		challengeButton.gameObject.SetActive (true);
		closeButton.gameObject.SetActive (true);

		// snap window text to top, so that we always have the content on top
		//contentWindow.position = new Vector3(contentWindow.position.x, 0, contentWindow.position.z);

		// guarantee that this window is always on top of everything else
		ar_modalPanel.gameObject.transform.SetAsLastSibling ();

		popedUp = true;

		yield return null;
	}

	void ClosePanel()
	{
		popedUp = false;

		ar_modalPanel.gameObject.SetActive (false);
	}
	*/
}
