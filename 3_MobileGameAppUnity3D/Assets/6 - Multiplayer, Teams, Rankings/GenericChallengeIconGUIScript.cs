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
using UnityEngine.SceneManagement;

public class GenericChallengeIconGUIScript : MonoBehaviour {

	public GameObject challengeIcon;	// this is the whole icon being generated
	public Image challengeImage;
	public Text challengeTitle;
	public Text challengeDistance;
	public int typeOfChallenge;			// 1 - Quiz; 2 - Multiplayer; 3 - Hunter; 4 - Voting; 5 - TimedTask

	// depending on the type, only one of these objects will have data
	public ChallengeInfo challenge;
	public MultiplayerChallengeInfo multiplayerChallenge;
	public HunterChallengeInfo hunterChallenge;
	public VotingChallengeInfo votingChallenge;
	public TimedTaskChallengeInfo timedTaskChallenge;
	public OpenQuizChallengeInfo openQuizChallenge;

	public void NavigateToChallengeInChallengesCanvasMenu()
	{
		ChallengesCanvasMenuManager.instance.CloseCanvas ();

		switch (typeOfChallenge) {
		case 1:

			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] Quiz challenge " + challengeTitle.text + ", with ID: " + challenge._id);

			GameManager.instance.FollowChallenge (challenge._id, true);
			break;
		case 2:
			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] Multiplayer challenge " + challengeTitle.text + ", with ID: " + multiplayerChallenge._id);
			GameManager.instance.FollowMultiplayerChallenge (multiplayerChallenge._id, true);
			break;
		case 3:
			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] Hunter challenge " + challengeTitle.text + ", with ID: " + hunterChallenge._id);
			GameManager.instance.FollowHunterChallenge (hunterChallenge._id, true);
			break;
		case 4:
			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] Voting challenge " + challengeTitle.text + ", with ID: " + votingChallenge._id);
			GameManager.instance.FollowVotingChallenge (votingChallenge._id, true);
			break;
		case 5:
			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] TimedTask challenge " + challengeTitle.text + ", with ID: " + timedTaskChallenge._id);
			GameManager.instance.FollowTimedTaskChallenge (timedTaskChallenge._id, true);
			break;
		case 6:
			Debug.Log ("[NavigateToChallengeInChallengesCanvasMenu] OpenQuiz challenge " + challengeTitle.text + ", with ID: " + openQuizChallenge._id);
			GameManager.instance.FollowOpenQuizChallenge (openQuizChallenge._id, true);
			break;
		default:
			Debug.LogError ("[GenericChallengeIconGUIScript] could not start following the challenge because I do not know its type");
			break;
		}

	}
}
