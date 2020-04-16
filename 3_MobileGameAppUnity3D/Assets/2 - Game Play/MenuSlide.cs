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

public class MenuSlide : MonoBehaviour {


	public static MenuSlide instance { set; get ; }
	// This is a singleton
	private static MenuSlide singleton;

	public GameObject messagePanelToDisable;

	public GameObject pauseMenuPanel;
	//animator reference
	private Animator anim;
	//variable for checking if the game is paused 
	//private bool isPaused = false;

	void Awake()
	{
		if (singleton == null) 
		{
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			instance = MenuSlide.singleton;


		} 
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}
	}




	// Use this for initialization
	void Start () {
		//unpause the game on start
		Time.timeScale = 1;
		//get the animator component
		anim = pauseMenuPanel.GetComponent<Animator>();
		//disable it on start to stop it from playing the default animation
		anim.enabled = false;
	}

	public void PauseGame()
	{
		messagePanelToDisable.SetActive (false);

		// enable the component animator
		anim.enabled = true;
		// play the slide in animation
		anim.Play("1 - MainMenuPullIn");
		//set the isPaused flag to true to indicate that the game is paused
		//isPaused = true;
		//freeze the timescale
		Time.timeScale = 0;
	}

	public void UnpauseGame()
	{
		messagePanelToDisable.SetActive (true);
		//set the isPaused flag to false to indicate that the game is not paused
		//isPaused = false;
		//play the SlideOut animation
		anim.Play("1 - MainMenuPullOut");
		//set back the time scale to normal time scale
		Time.timeScale = 1;
	}

	public void ResetAnimation()
	{
		anim.Play("1 - MainMenuPullOut", -1, 0f);
	}
}
