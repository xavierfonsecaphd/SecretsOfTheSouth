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
using System;

public class Quiz_ModalWindow : MonoBehaviour {

	[HideInInspector]
	public bool popedUp = false;
	public Text text, titleText;
	public Image iconImage;
	public Button yesButton, challengeButton, closeButton, answerButton;
	public InputField answerBox;
	public DisplayManager_FeedBack_QuizWindow_Scene_2 _referenceDisplayManager;
	[HideInInspector]
	public string answerOfTheChallenge;
	public GameObject popupQuestionPanel;
	public GameObject popupAnswerPanel;
	//public GameObject feedbackPanel;
	public Text questionContainer;
	public ScrollRect scrollViewPanel;
	//public RectTransform contentWindow; 

	private static Quiz_ModalWindow quiz_modalPanel;

	public static Quiz_ModalWindow Instance()
	{
		if (!quiz_modalPanel) {
			quiz_modalPanel = GameManager.instance._referenceQuiz_ModalPanel;
			if (!quiz_modalPanel) {
				// find all the objects on the game of this type. there has to be at least one
				quiz_modalPanel = FindObjectOfType (typeof(Quiz_ModalWindow)) as Quiz_ModalWindow;
			}
			if (!quiz_modalPanel)
				throw new UnityException ("[ModalWindowManager] There needs to be one active ModalWindowManager script on a game object in your scene.");
		}


		return quiz_modalPanel;
	}

	public IEnumerator Choice (Sprite image, string titleOfChallenge, string textChallenge, string challengeAnswer, UnityAction yesEvent, UnityAction answerChallengeEvent, UnityAction closeEvent)
	{
		quiz_modalPanel.gameObject.SetActive (true);
		GameManager.instance.isAnyWindowOpen = true;
		popupQuestionPanel.SetActive (false);
		popupAnswerPanel.SetActive (false);
		answerOfTheChallenge = "-1000000";
		_referenceDisplayManager.statusPanel.SetActive (false);
		//feedbackPanel.SetActive (false);

		Debug.Log ("[Quiz_ModalWindow] Quiz choice Fired up.");

		// lets just clean any listeners that the button might have defined
		yesButton.onClick.RemoveAllListeners();
		yesButton.onClick.AddListener (yesEvent);
		yesButton.onClick.AddListener (PopUpQuestionPanel);


		challengeButton.onClick.RemoveAllListeners();
		//challengeButton.onClick.AddListener (answerChallengeEvent);
		challengeButton.onClick.AddListener (PopUpAnswerPanel);


		answerButton.onClick.RemoveAllListeners ();
		answerButton.onClick.AddListener (answerChallengeEvent);
		answerOfTheChallenge = challengeAnswer;

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
		quiz_modalPanel.gameObject.transform.SetAsLastSibling ();

		popedUp = true;

		yield return null;
	}

	void PopUpQuestionPanel()
	{
		Debug.Log ("[Quiz_modalWindow] PopUpQuestionPanel fired up!");
		popupQuestionPanel.SetActive (true);
		popupQuestionPanel.transform.SetAsLastSibling ();

		//quiz_modalPanel.gameObject.SetActive (false);
	}
	public void ClosePopUpQuestionPanel()
	{
		popupQuestionPanel.SetActive (false);
		//popupPanel.transform.SetAsLastSibling ();

		//quiz_modalPanel.gameObject.SetActive (false);
	}
	public void ClosePopUpAnswerPanel()
	{
		popupAnswerPanel.SetActive (false);
		//popupPanel.transform.SetAsLastSibling ();

		//quiz_modalPanel.gameObject.SetActive (false);
	}
	void PopUpAnswerPanel()
	{
		Debug.Log ("[Quiz_modalWindow] PopUpAnswerPanel fired up!");
		popupAnswerPanel.SetActive (true);
		popupAnswerPanel.transform.SetAsLastSibling ();

		//quiz_modalPanel.gameObject.SetActive (false);
	}
	void ClosePanel()
	{
		popedUp = false;
		quiz_modalPanel.gameObject.SetActive (false);
		GameManager.instance.isAnyWindowOpen = false;
	}
}
