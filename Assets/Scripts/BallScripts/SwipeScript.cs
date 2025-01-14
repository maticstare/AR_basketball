using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine.TextCore.Text;
//using System.Numerics;

public class SwipeScript : MonoBehaviour
{
    Vector2 startPos, endPos; // touch start position, touch end position, swipe direction
    float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to control throw force in Z direction

    Rigidbody rb;

    static bool begin = false;
    [SerializeField] GameObject arcamera;
    [SerializeField] GameObject ballIndicator;
    [SerializeField] private HandLandmarkerRunner handLandmarkerRunner;
    [SerializeField] TMP_Text text;

    float swipeDistance;

    // New Input System variables
    private Touchscreen touchscreen;
    private Vector3 swipeDirection;

    private Vector2 prevPos;
    float stationaryStartTime = 0;
    float stationaryThreshold = 1f;
    float stationaryThresholdEnd = 0.15f;
    bool startPositionSet = false;
    bool endPositionSet = true;
    bool isStationary = false;
    float throwTimeoutEndTime = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize the touchscreen (for mobile) or other input device
        touchscreen = Touchscreen.current;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector2 indexTip = handLandmarkerRunner.indexTipPosition;

        if (Time.time < throwTimeoutEndTime)
        {
            text.text = "";
            return;
        }

        // Check if the indexTip position is valid
        if (indexTip != default)
        {
            if(startPositionSet == true){
                text.text ="Go!";
            }
            else{
                text.text = "";
            }
            
            // Check if the indexTip is stationary (within the threshold)
            if (Vector2.Distance(indexTip, prevPos) < 0.05f)
            {
                // If the stationary timer is not already started, start it
                if (!isStationary)
                {
                    stationaryStartTime = Time.time;
                    isStationary = true;
                }

                // If it's been stationary for at least the threshold time, set startPos or endPos
                if (Time.time - stationaryStartTime > stationaryThreshold && !startPositionSet){
                    startPos = indexTip;
                    startPositionSet = true;
                    endPositionSet = false;
                }
                else if (!endPositionSet && Time.time - stationaryStartTime > stationaryThresholdEnd && Vector2.Distance(indexTip,startPos) > 0.15f){
                    endPos = indexTip;
                    endPositionSet = true;
                }
            }
            else
            {
                // Reset stationary tracking if the indexTip moves
                isStationary = false;
                stationaryStartTime = 0;
            }

            // Update prevPos for the next frame
            prevPos = indexTip;

            // Reset positions if both start and end positions are set
            if (startPositionSet && endPositionSet)
            {
                swipeDistance = Vector2.Distance(startPos, endPos);
                if (swipeDistance > 0.2f && !MoveBall.ThrowBegin){
                    // Calculate the swipe direction (X axis movement here)
                    MoveBall.swipeDirection = new Vector3(- ((startPos.y - endPos.y) * Screen.width), 0, 0);
                    startPositionSet = false;
                    ballRepositionAndVisibility(arcamera);
                    MoveBall.CameraRotation = CameraAngle(arcamera);
                    MoveBall.ThrowBegin = true; // Start the ball throw logic
                    throwTimeoutEndTime = Time.time + 4.5f;
                }
            }
        }



        // If swipe begins, show the mesh renderer
        if (MoveBall.ThrowBegin)
            GetComponent<MeshRenderer>().enabled = true;
        else
            GetComponent<MeshRenderer>().enabled = false;

        if (touchscreen != null)
        {
            var touch = touchscreen.primaryTouch;

            // 1. Start the swipe (touch begins)
            if (touch.press.isPressed && !begin)
            {
                // Getting touch position and marking time when you touch the screen
                touchTimeStart = Time.time;
                startPos = touch.position.ReadValue();
                begin = true; // Mark that the touch has started
            }

            // 2. End of swipe (touch ends)
            if (begin && !touch.press.isPressed) // When touch is released (touch ended)
            {
                touchTimeFinish = Time.time; // Marking time when you release it
                timeInterval = touchTimeFinish - touchTimeStart; // Calculate swipe time interval
                endPos = touch.position.ReadValue(); // Get the release touch position

                // Calculate the swipe direction and send info to MoveBall script
                swipeDistance = Vector2.Distance(startPos, endPos);

                // If the swipe distance is greater than a threshold, consider it a valid swipe
                if (swipeDistance > 250f && !MoveBall.ThrowBegin)
                {
                    // Calculate the swipe direction (X axis movement here)
                    MoveBall.swipeDirection = new Vector3(startPos.x - endPos.x, 0, 0);
                    Debug.Log(MoveBall.swipeDirection);
                    
                    ballRepositionAndVisibility(arcamera);
                    MoveBall.CameraRotation = CameraAngle(arcamera);
                    MoveBall.ThrowBegin = true; // Start the ball throw logic
                }

                begin = false; // Reset the touch flag after processing the swipe
            }
        }
    }

    // Reposition the ball and hide the indicator
    void ballRepositionAndVisibility(GameObject arcamera)
    {
        transform.position = ballIndicator.transform.position;
        transform.rotation = arcamera.transform.rotation;
        rb.isKinematic = false;
        ballIndicator.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshRenderer>().enabled = true;
    }

    // Returns the angle where the camera is facing
    float CameraAngle(GameObject arcamera)
    {
        float angle = arcamera.transform.localEulerAngles.y;
        if (angle < 0)
        {
            angle = 360 + angle;
        }
        return angle;
    }
}
