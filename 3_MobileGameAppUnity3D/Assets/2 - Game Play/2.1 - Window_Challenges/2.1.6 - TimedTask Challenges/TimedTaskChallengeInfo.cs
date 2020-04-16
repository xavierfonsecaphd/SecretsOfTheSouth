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

using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity;

public class TimedTaskChallengeInfo : MonoBehaviour {

	//[HideInInspector]
	public string _id;
	//[HideInInspector]
	public new string name;
	public string description;
	public string ownerPlayFabID;
	public int typeOfChallengeIndex;
	public double latitude;
	public double longitude;
	public string task;
	public string imageURL;
	public string questionHowMany;
	public int 	timer;
	public bool validated;
	public int route;

	public double distanceToPlayer;
	public bool solved = false;				// only generate challenges on screen if these haven't been solved.


	public void setData (
		string id, 
		string pname, 
		string desc, 
		string ownerOfTheChallenge,
		int typeOfTheChallenge, 
		double lat, 
		double lon, 
		string taskp,
		string img,
		string tquestionHowMany,
		int ttimer,
		bool isvalid,
		int routep,
		bool isItSolved)
	{
		this._id = id;
		this.name = pname;
		this.description = desc;
		this.ownerPlayFabID = ownerOfTheChallenge;
		this.typeOfChallengeIndex = typeOfTheChallenge;
		this.latitude = lat;
		this.longitude = lon;
		this.task = taskp;
		this.imageURL = img;
		this.questionHowMany = tquestionHowMany;
		this.timer = ttimer;
		this.validated = isvalid;
		this.route = routep;

		this.distanceToPlayer = DistanceToPlayer (new Vector2d (this.latitude, this.longitude),	GPSLocationProvider_Xavier.instance.latlong);
		this.solved = isItSolved;
	}
	
	// Update is called once per frame
	void Update () {
		this.distanceToPlayer = DistanceToPlayer (new Vector2d (this.latitude, this.longitude),	GPSLocationProvider_Xavier.instance.latlong);
	}

	// this function calculates the distance between two LatLonG GPS points on the map. 
	public double DistanceToPlayer(Vector2d latLongChallenge, Vector2d latlongPlayer)
	{
		double circumference = 40075.0; // Earth's circumference at the equator in km
		double distance = 0.0;



		//Calculate radians
		double latitude1Rad = latLongChallenge.x * Mathd.PI / 180.0;
		double longitude1Rad = latLongChallenge.y * Mathd.PI / 180.0;
		double latititude2Rad = latlongPlayer.x * Mathd.PI / 180.0;
		double longitude2Rad = latlongPlayer.y * Mathd.PI / 180.0;

		double logitudeDiff = Mathd.Abs(longitude1Rad - longitude2Rad);
		if (logitudeDiff > Mathd.PI)
		{
			logitudeDiff = 2.0 * Mathd.PI - logitudeDiff;
		}

		double angleCalculation =
			Mathd.Acos(
				Mathd.Sin(latititude2Rad) * Mathd.Sin(latitude1Rad) +
				Mathd.Cos(latititude2Rad) * Mathd.Cos(latitude1Rad) * Mathd.Cos(logitudeDiff));

		distance = circumference * angleCalculation / (2.0 * Mathd.PI);

		return distance;
	}

	// this pr
	public string DistanceToString()
	{
		double distance = distanceToPlayer;

		if (distance < 1) {
			distance *= 1000.0; // remove the 0, ...
			distance = System.Math.Round (distance, 0);

			string[] displayMessage1 = {"" + distance.ToString () + " meters", 
				"" + distance.ToString () + " meter", 
				"" + distance.ToString () + " metros"};
			return displayMessage1[MasterManager.language];
		} else {
			int km = (int) System.Math.Round (Mathd.Floor(distance), 0); 

			double floor = distance % 1.0;
			if (floor > 1.0) {
				floor -= 1.0;
			}
			floor *= 100.0; // remove the 0, but work with only 2 decimals after
			floor = System.Math.Round (floor, 0);
			if (floor >= 10)
			{
				string[] displayMessage2 = {"" + km.ToString () + "." + floor + " km", 
					"" + km.ToString () + "." + floor + " km", 
					"" + km.ToString () + "." + floor + " kms"};
				return displayMessage2[MasterManager.language];
			}
			else {
				string[] displayMessage3 = {"" + km.ToString () + ".0" + floor + " km", 
					"" + km.ToString () + ".0" + floor + " km", 
					"" + km.ToString () + ".0" + floor + " kms"};
				return displayMessage3[MasterManager.language];
			}
		}
	}
}




