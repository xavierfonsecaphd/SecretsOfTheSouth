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
using System;


[Serializable]
public class ChallengeDBMeteorFormat_JSON {
	
	public string _id;
	//[HideInInspector]
	public string name;
	public string description;
	public string ownerPlayFabID;
	public int typeOfChallengeIndex;
	public double latitude;
	public double longitude;
	public string question;
	public string answer;
	public string imageURL;
	public bool validated;
	public int route;
}

// ***************************************************************
// This down here is legacy. should disappear in the future
// ***************************************************************

[Serializable]
public class ChallengeDBFormat_JSON {

	public long challengeID;
	public string challenge_Name;
	public double challenge_Latitude;
	public double challenge_Longitude;
	public string ownerPlayFabID;
	public int typeOfChallengeIndex;
	public int challenge_NumberOfPeopleToMeet;
	public object refChallenge_Quiz;	// always null
	public object refChallenge_AR;		// always null
}

[Serializable]
public class Challenge_QuizDBFormat_JSON {

	public long challengeID;
	public string quiz_Description;
	public string quiz_ImageURL;
	public string quiz_Question;
	public long quiz_Answer;
	public object head_Challenge;		// always null
}


[Serializable]
public class Challenge_ARDBFormat_JSON {

	public long challengeID;
	public string aR_Description;
	public object head_Challenge;		// always null
}