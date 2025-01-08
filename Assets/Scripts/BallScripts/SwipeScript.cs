using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SwipeScript : MonoBehaviour
{
    Vector2 startPos, endPos; // touch start position, touch end position, swipe direction
    float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to control throw force in Z direction

    Rigidbody rb;

    static bool begin = false;
    [SerializeField] GameObject arcamera;
    [SerializeField] GameObject ballIndicator;

    float swipeDistance;

    // New Input System variables
    private Touchscreen touchscreen;
    private Vector2 swipeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize the touchscreen (for mobile) or other input device
        touchscreen = Touchscreen.current;
    }

    // Update is called once per frame
    void Update()
    {   
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
