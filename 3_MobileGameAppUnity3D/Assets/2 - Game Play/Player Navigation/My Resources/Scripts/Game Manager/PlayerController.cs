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
using UnityEngine.UI;

using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Map;
using Mapbox.Utils;



// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
public class PlayerController : MonoBehaviour {

	//private GameObject _referenceCamera;

	public bool areYouWalking;
	public float speed;
	public bool walkingNotWalkingcommuteDone = false;


	private Quaternion calibration;
	private const int EarthRadius = 6378137; //no seams with globe example
	private const double OriginShift = 2 * Math.PI * EarthRadius / 2;
	public Vector3 previousLatLonPosition ;
	private bool farEnoughToUpdateHeading = false;

	//public Text text;


	// Use this for initialization
	void Start () 
	{
		
		/*
		#if !UNITY_EDITOR
		areYouWalking = true;
		GameManager.instance.buttonWalkingNotWalking.GetComponentInChildren<Text> ().text = "Walking";
		#endif
*/
		GameManager.instance.isPlayerInitialized = true;


	}

	// this is to reposition the player for the first time in the correct coordinates. otherwise, it will be put in the coordinates (0,0) on the map, which are not exactly his coordinates,
	// but the center of the map
	public void SetPlayer()
	{
		if (GPSLocationProvider_Xavier.instance.gotGPSLocation) {
			Debug.Log ("[PlayerController] Coordinates: [" +
			GPSLocationProvider_Xavier.instance.latlong.x + "  -  " +
			GPSLocationProvider_Xavier.instance.latlong.y + "]... ");
			transform.MoveToGeocoordinate (GPSLocationProvider_Xavier.instance.latlong.x, GPSLocationProvider_Xavier.instance.latlong.y, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap> ().CenterMercator, 
				MasterManager.instance._referenceMap.GetComponent<MyBasicMap> ().WorldRelativeScale);
		
			//transform.position = new Vector3 (transform.position.x,5.0f, transform.position.z);

			//#if UNITY_EDITOR
			transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
			previousLatLonPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

			//HandlingHeadingDirection ();
			//#endif
			// Update camera position, because I moved the player
			//SetPlayer ();
			GameManager.instance.GetComponent<CameraController> ().updateCameraPosition ();
		} else {
			Debug.LogError ("[PlayerController] this shit is not properly located!!!");
		}
	}


	// Update is called once per frame
	void LateUpdate () 
	{
		if ((GameManager.instance.isGameManagerLoaded) )
		{
			
			if (farEnoughToUpdateHeading) {
				previousLatLonPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				farEnoughToUpdateHeading = false;
			}

			// First, handle the player movement
			//#if UNITY_ANDROID || UNITY_IOS
			//if (!areYouWalking) {
			//	NotWalking ();

			//} else {
				Walking ();
			//}

			//#endif

			// Handle direction, when player moved
			AjustPlayerHeadingWhileWalking();
		//	RotatePlayerBasedOnCameraDirection();
			// Update camera position, because I moved the player
			GameManager.instance.GetComponent<CameraController> ().updateCameraPosition ();

		}
	}

	private void RotatePlayerBasedOnCameraDirection()
	{
		float distance, deltaX, deltaY, deltaZ;

		//Vector3 headingPreviousPosition = transform.position - previousLatLonPosition;
		Quaternion previousRotation = transform.rotation;
		deltaX = transform.position.x - previousLatLonPosition.x;
		deltaY = transform.position.y - previousLatLonPosition.y;
		deltaZ = transform.position.z - previousLatLonPosition.z;

		distance = Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);


		if (distance > 2) {
			// this is the target position - player position
			Vector3 heading = GameManager.instance._referencePlayer.transform.position - GameManager.instance._referenceCamera.transform.position;
			heading.y = 0;	// this is because I do not want to point to the ground (as the camera does)

			GameManager.instance._referencePlayer.transform.rotation = Quaternion.LookRotation (heading);
		}
	}

	private void AjustPlayerHeadingWhileWalking()
	{

		float distance, deltaX, deltaY, deltaZ;

		Vector3 heading = transform.position - previousLatLonPosition;
		Quaternion previousRotation = transform.rotation;
		deltaX = transform.position.x - previousLatLonPosition.x;
		deltaY = transform.position.y - previousLatLonPosition.y;
		deltaZ = transform.position.z - previousLatLonPosition.z;

		distance = Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

		//text.text = distance + "  meters.";

		if (distance > 2) {
			farEnoughToUpdateHeading = true;
			Quaternion rotation = Quaternion.LookRotation (heading);
			transform.rotation = rotation;

			Quaternion previousPlayerRotation = transform.rotation;

			// get a "forward vector" for each rotation
			Vector3 forwardA = previousRotation * Vector3.forward;
			Vector3 forwardB =  transform.rotation * Vector3.forward;


			// get a numeric angle for each vector, on the X-Z plane (relative to world forward)
			float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
			float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

			// get the signed difference in these angles
			float angleDiff = Mathf.DeltaAngle( angleA, angleB );
			GameManager.instance.GetComponent<CameraController>().cumulativeAutomaticHeadingRotated += -angleDiff;
		}
	}




	private void NotWalking()
	{
		//Debug.Log ("[PlayerController] ModalWindowManager.Instance().popedUp:  " + ModalWindowManager.Instance().popedUp.ToString());
		// move the player with the finger. 
		if ((Input.touchCount == 1) ) { 
			Vector2 direction;

			if (Input.touches [0].phase == TouchPhase.Moved) {//Check if Touch has moved.
				direction = Input.touches [0].deltaPosition.normalized;  //Unit Vector of change in position

				transform.Translate (direction.x * speed, 0.0f, direction.y * speed, Space.World);
			} 
		}
	}

	private void Walking()
	{
		transform.MoveToGeocoordinate(
			GPSLocationProvider_Xavier.instance.latlong.x, 
			GPSLocationProvider_Xavier.instance.latlong.y, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().CenterMercator, 
			MasterManager.instance._referenceMap.GetComponent<MyBasicMap>().WorldRelativeScale);
	}

}
