using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwipeScript : MonoBehaviour {

	Vector2 startPos, endPos; // touch start position, touch end position, swipe direction
	float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to sontrol throw force in Z direction

	Rigidbody rb;

	static bool begin=false;
	[SerializeField] GameObject arcamera;
	[SerializeField] GameObject ballIndicator;

    float swipeDistance;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		begin=true;
	}

	// Update is called once per frame
	void Update () {
		// if you touch the screen
		if(MoveBall.ThrowBegin) GetComponent<MeshRenderer>().enabled = true;
		else GetComponent<MeshRenderer>().enabled = false;
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {

			// getting touch position and marking time when you touch the screen
			touchTimeStart = Time.time;
			startPos = Input.GetTouch (0).position;
		}

		if(begin){
		// if you release your finger
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {

				// marking time when you release it
				touchTimeFinish = Time.time;

				// calculate swipe time interval 
				timeInterval = touchTimeFinish - touchTimeStart;

				// getting release finger position
				endPos = Input.GetTouch (0).position;

				// calculating swipe direction in 2D space and sends info to MoveBall script
				swipeDistance=Vector2.Distance(startPos,endPos);
				if(swipeDistance>250f && !MoveBall.ThrowBegin){
					MoveBall.swipeDirection = new Vector3(startPos.x - endPos.x,0,0);
					ballRepositionAndVisibility(arcamera);
					MoveBall.CameraRotation = CameraAngle(arcamera);
					MoveBall.ThrowBegin = true;
				}
			}

			void ballRepositionAndVisibility(GameObject arcamera){
				transform.position = ballIndicator.transform.position;
				transform.rotation = arcamera.transform.rotation;
				rb.isKinematic = false;
				ballIndicator.GetComponent<MeshRenderer>().enabled = false;
				GetComponent<MeshRenderer>().enabled = true;
			}


			float CameraAngle(GameObject arcamera){
				//Returns the angle where the camera is facing
				float angle = arcamera.transform.localEulerAngles.y;
				if(angle < 0){
					angle = 360+angle;
				}
				return angle;
			}
		}		
	}
}
