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
using UnityEngine.EventSystems;

// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
// http://wiki.unity3d.com/index.php/DetectTouchMovement

public class CameraController : MonoBehaviour
{
	public float hd = 0.0f;
	[HideInInspector]
	public float cumulativeAutomaticHeadingRotated = 0.0f;
	//public Toggle _referencetoggle;

	private float fingerManipulationAngle = 0.0f; 
	public static Vector3 offset; 
	private bool followPlayerRotation = false;



	// Use this for initialization
	void Start () 
	{
		// Check if the camera is in place!
		if (GameManager.instance._referenceCamera == null) throw new System.Exception("[CameraController] You must have a reference camera assigned!");
		//if (_referencetoggle == null) throw new System.Exception("[CameraController] You must have a reference toggle assigned!");

		// Launch a coroutine that waits for the player to be set up and ready, in order to finish the setup of the camera
		StartCoroutine(SetUpCameraAfterPlayerIsSetUp());
	}

	/** this is to finish this object set up.This guarantees that dependencies with other gameobjects that this gameobject might have
	* are properly taken care of before requesting them. We do not control the order of initialization of gameobjects, and this enforces a proper order of setting up
	*/
	private IEnumerator SetUpCameraAfterPlayerIsSetUp()
	{
		int waitCount = 20;
		while ((!GameManager.instance.isPlayerInitialized) && waitCount >= 0) {
			waitCount--;
			yield return new WaitForSeconds(1);
		}

		if (waitCount < 0) {
			throw new UnityException ("[CameraController] The player in GameManager did not initiate. Game should quit, because I need a Player to attach the player to!");
		}

		// throughout the game
		offset = GameManager.instance._referenceCamera.transform.position - GameManager.instance._referencePlayer.transform.position;

		GameManager.instance.isCameraInitialized = true;
		//Debug.Log("[CameraController] Camera was correctly set up around the player.");
		yield return null;
	}


	// Update is called once per frame. LateUpdate is the same, but GUARANTEES that everything was processed by now. In this case,
	// we know that when the camera moves, the player has moved before to that position!
	void LateUpdate () 
	{

		if ((GameManager.instance.isGameManagerLoaded) && (!GameManager.instance.isAnyWindowOpen))// (!GameManager.instance._referenceQuiz_ModalPanel.gameObject.activeSelf)  && 
			//(!GameManager.instance._referenceAR_ModalPanel.gameObject.activeSelf))	
		{
			//#if UNITY_ANDROID || UNITY_IOS

			// calculate the angle to turn, which is found in between the two fingers
			DetectTouchMovement.Calculate();
				
			// If the player changed the camera with the fingers
			if (Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0) 
			{ 
				offset = Quaternion.Euler (0.0f, -DetectTouchMovement.turnAngleDelta,0.0f) * offset;



				// place the camera at the right distance from the player by using the offset
				GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;

				GameManager.instance._referenceCamera.transform.eulerAngles = new Vector3(
					GameManager.instance._referenceCamera.transform.eulerAngles.x, 
					GameManager.instance._referenceCamera.transform.eulerAngles.y + (-DetectTouchMovement.turnAngleDelta), 
					GameManager.instance._referenceCamera.transform.eulerAngles.z);


				fingerManipulationAngle += DetectTouchMovement.turnAngleDelta;


				// automatically set toggle off
				//_referencetoggle.isOn = false;
				followPlayerRotation = false;
			} 

			// first, undo all the rotation done with the fingers and put the camera back on track
			if (followPlayerRotation) 
			{
				offset = Quaternion.Euler (0.0f, fingerManipulationAngle, 0.0f) * offset;
				//	// place the camera at the right distance from the player by using the offset
				GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
				// because it is not guaranteed that, after the offset, the camera is pointing at the player, then make the camera look to the player
				GameManager.instance._referenceCamera.transform.eulerAngles = new Vector3(
					GameManager.instance._referenceCamera.transform.eulerAngles.x, 
					GameManager.instance._referenceCamera.transform.eulerAngles.y + fingerManipulationAngle, 
					GameManager.instance._referenceCamera.transform.eulerAngles.z); // Standart Left-/Right Arrows and A & D Keys

				fingerManipulationAngle = 0.0f;

				// and after correcting the rotation from the fingers, adjust the automatic heading that the player might have taken in the meantime.
				if (cumulativeAutomaticHeadingRotated != 0.0f) {
					offset = Quaternion.Euler (0.0f, -cumulativeAutomaticHeadingRotated, 0.0f) * offset;

					// place the camera at the right distance from the player by using the offset
					GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;

					GameManager.instance._referenceCamera.transform.eulerAngles = new Vector3 (
						GameManager.instance._referenceCamera.transform.eulerAngles.x, 
						GameManager.instance._referenceCamera.transform.eulerAngles.y + (-cumulativeAutomaticHeadingRotated), 
						GameManager.instance._referenceCamera.transform.eulerAngles.z);
					cumulativeAutomaticHeadingRotated = 0.0f;
				}

				if (GameManager.instance._referenceCamera.transform.position.y != 35) {
					if (GameManager.instance._referenceCamera.transform.position.y < 35) {
						offset += GameManager.instance._referenceCamera.transform.forward * (- (35 - GameManager.instance._referenceCamera.transform.position.y));
						GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
					}
					if (GameManager.instance._referenceCamera.transform.position.y > 35) {
						offset += GameManager.instance._referenceCamera.transform.forward * ((GameManager.instance._referenceCamera.transform.position.y - 35));
						GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
					}
				}

			} 
		}
	}
	/*
	public void HandleButtonZoomOut()
	{
		if (GameManager.instance._referenceCamera.transform.position.y < 250)
		{
			offset += GameManager.instance._referenceCamera.transform.forward * (- 5) ;
			//dir = Quaternion.Euler (angles) * dir; // rotate it
			GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
		}
	}

	public void HandleButtonZoomIn()
	{
		if (GameManager.instance._referenceCamera.transform.position.y > 40)
		{
			offset += GameManager.instance._referenceCamera.transform.forward * ( 5) ;
			GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
		}
	}
	*/	
	/**
	* This function toggles between having the camera following the player's rotation on the scene or not
	*/
	public void HandleLocationToggleOnScreen(Toggle toggle)
	{
		if (toggle.isOn) {
			followPlayerRotation = true;
		} else {
			followPlayerRotation = false;
		}
	}

	// after moving the player, make the camera follow the player
	public void updateCameraPosition()
	{
		GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
	}

	#if UNITY_ANDROID || UNITY_IOS
	// This class handles touch on the smartphone
	public class DetectTouchMovement : MonoBehaviour 
	{
		const float pinchTurnRatio = Mathf.PI / 2;
		const float minTurnAngle = 0;

		const float pinchRatio = 1;
		const float minPinchDistance = 0;

		const float panRatio = 1;
		const float minPanDistance = 0;


		public static Vector2 direction;

		/// <summary>
		///   The delta of the angle between two touch points
		/// </summary>
		static public float turnAngleDelta;
		/// <summary>
		///   The angle between two touch points
		/// </summary>
		static public float turnAngle;

		/// <summary>
		///   The delta of the distance between two touch points that were distancing from each other
		/// </summary>
		static public float pinchDistanceDelta;
		/// <summary>
		///   The distance between two touch points that were distancing from each other
		/// </summary>
		static public float pinchDistance;

		/// <summary>
		///   Calculates Pinch and Turn - This should be used inside LateUpdate
		/// </summary>
		static public void Calculate () {
			pinchDistance = pinchDistanceDelta = 0;
			turnAngle = turnAngleDelta = 0;
			Vector2 startPos = new Vector2(), startPosDelta = new Vector2();

			// if two fingers are touching the screen at the same time ...
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);

				switch (touch.phase) {
				//When a touch has first been detected, change the message and record the starting position
				case TouchPhase.Began:
					// Record initial touch position.
					startPos = touch.position;
					startPosDelta = touch.deltaPosition;

					pinchDistance = pinchDistanceDelta = 0;
					break;

					//Determine if the touch is a moving touch
				case TouchPhase.Moved:
					// ************************************
					// Finding the rotating angle
					// ************************************
					Vector2 initialPosition = touch.position;

					if (Input.touchCount > 1) {
						Touch touch1 = Input.touches[0];
						Touch touch2 = Input.touches[1];

						if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) {

							// ... check the delta distance between them ...
							pinchDistance = Vector2.Distance(touch1.position, touch2.position);
							float prevDistanceTwoFingers = Vector2.Distance(touch1.position - touch1.deltaPosition,
								touch2.position - touch2.deltaPosition);
							pinchDistanceDelta = pinchDistance - prevDistanceTwoFingers;

							// ... if it's greater than a minimum threshold, it's a pinch!
							if (Mathf.Abs (pinchDistanceDelta) > minPinchDistance) {
								pinchDistanceDelta *= pinchRatio;

								//Debug.Log ("[CameraController] pinchDistanceDelta: " + pinchDistanceDelta);

								if ((pinchDistanceDelta > 20) &&(GameManager.instance._referenceCamera.transform.position.y > 40)) {
									offset += GameManager.instance._referenceCamera.transform.forward * (10);
									//dir = Quaternion.Euler (angles) * dir; // rotate it
									GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
								}
								else if ((pinchDistanceDelta < -20) &&(GameManager.instance._referenceCamera.transform.position.y < 250)) {
									offset += GameManager.instance._referenceCamera.transform.forward * ( -10) ;
									GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
								}
							}

							turnAngleDelta = 0;

						}
					}
					else {

						DetectTouchMovement.direction = touch.position - startPos;


						pinchDistance = Vector2.Distance (touch.position, startPos);
						float prevDistance = Vector2.Distance (touch.position - touch.deltaPosition,
							startPos - startPosDelta);
						pinchDistanceDelta = pinchDistance - prevDistance;
						if (Mathf.Abs (pinchDistanceDelta) > minPinchDistance) {
							pinchDistanceDelta *= pinchRatio;
						} else {
							pinchDistance = pinchDistanceDelta = 0;
						}

						// ... or check the delta angle between them ...
						turnAngle = Angle (touch.position, startPos);
						float prevTurn = Angle (touch.position - touch.deltaPosition,
							startPos - startPosDelta);
						turnAngleDelta = Mathf.DeltaAngle (prevTurn, turnAngle);

						// ... if it's greater than a minimum threshold, it's a turn!
						if (Mathf.Abs (turnAngleDelta) > minTurnAngle) {

							// left or right hand is tricky to get to work without affecting the rotation 
							//if (initialPosition.x < Screen.width / 2) {
								turnAngleDelta *= pinchTurnRatio * 2;
							//} else {
							//	turnAngleDelta *= - pinchTurnRatio * 2;
							//}


						} else {
							turnAngle = turnAngleDelta = 0;
						}

						/*
						// ... check the delta distance between them ...
						pinchDistance = Vector2.Distance(touch.position, startPos);
						prevDistance = Vector2.Distance(touch.position - touch.deltaPosition,
							startPos - startPosDelta);
						pinchDistanceDelta = pinchDistance - prevDistance;

						// ... if it's greater than a minimum threshold, it's a pinch!
						if (Mathf.Abs (pinchDistanceDelta) > minPinchDistance) {
							pinchDistanceDelta *= 0;//pinchRatio;

							//Debug.Log ("[CameraController] pinchDistanceDelta: " + pinchDistanceDelta);

							if ((pinchDistanceDelta > 20) &&(GameManager.instance._referenceCamera.transform.position.y > 40)) {
								offset += GameManager.instance._referenceCamera.transform.forward * (10);
								//dir = Quaternion.Euler (angles) * dir; // rotate it
								GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
							}
							else if ((pinchDistanceDelta < -20) &&(GameManager.instance._referenceCamera.transform.position.y < 250)) {
								offset += GameManager.instance._referenceCamera.transform.forward * ( -10) ;
								GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
							}
						}*/
					}


					break;

				default:
					pinchDistance = pinchDistanceDelta = 0;
					break;


				}
				/*

				// ... if at least one of them moved ...
				if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
					// ... check the delta distance between them ...
					pinchDistance = Vector2.Distance(touch1.position, touch2.position);
					float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
						touch2.position - touch2.deltaPosition);
					pinchDistanceDelta = pinchDistance - prevDistance;

					// ... if it's greater than a minimum threshold, it's a pinch!
					if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
						pinchDistanceDelta *= pinchRatio;
					} else {
						pinchDistance = pinchDistanceDelta = 0;
					}

					// ... or check the delta angle between them ...
					turnAngle = Angle(touch1.position, touch2.position);
					float prevTurn = Angle(touch1.position - touch1.deltaPosition,
						touch2.position - touch2.deltaPosition);
					turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

					// ... if it's greater than a minimum threshold, it's a turn!
					if (Mathf.Abs(turnAngleDelta) > minTurnAngle) {
						turnAngleDelta *= pinchTurnRatio;
					} else {
						turnAngle = turnAngleDelta = 0;
					}
				}
				// if both moved, then deal with the zoom in/out
				if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) {
					// ... check the delta distance between them ...
					pinchDistance = Vector2.Distance(touch1.position, touch2.position);
					float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
						touch2.position - touch2.deltaPosition);
					pinchDistanceDelta = pinchDistance - prevDistance;

					// ... if it's greater than a minimum threshold, it's a pinch!
					if (Mathf.Abs (pinchDistanceDelta) > minPinchDistance) {
						pinchDistanceDelta *= pinchRatio;

						//Debug.Log ("[CameraController] pinchDistanceDelta: " + pinchDistanceDelta);

						if ((pinchDistanceDelta > 20) &&(GameManager.instance._referenceCamera.transform.position.y > 40)) {
							offset += GameManager.instance._referenceCamera.transform.forward * (10);
							//dir = Quaternion.Euler (angles) * dir; // rotate it
							GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
						}
						else if ((pinchDistanceDelta < -20) &&(GameManager.instance._referenceCamera.transform.position.y < 250)) {
							offset += GameManager.instance._referenceCamera.transform.forward * ( -10) ;
							GameManager.instance._referenceCamera.transform.position = GameManager.instance._referencePlayer.transform.position + offset;
						}
					}

					} else {
						pinchDistance = pinchDistanceDelta = 0;
				}

				*/
			}
		}

		static private float Angle (Vector2 pos1, Vector2 pos2) {
			Vector2 from = pos2 - pos1;
			Vector2 to = new Vector2(1, 0);

			float result = Vector2.Angle( from, to );
			Vector3 cross = Vector3.Cross( from, to );

			if (cross.z > 0) {
				result = 360f - result;
			}

			return result;
		}
	}
	#endif

}
