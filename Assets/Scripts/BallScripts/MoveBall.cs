using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveBall : MonoBehaviour
{

    protected float Animation;
    Rigidbody rigid;
    public static bool ThrowBegin = false;
    public static Vector3 swipeDirection;
    Vector3 currentPosition;
    Vector3 endPosition;
    public static float CameraRotation;
    GameObject hoop;
    public static bool HoopCollision = false;

    bool firstUpdate = false;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(ThrowBegin){
            if (!firstUpdate){
                firstUpdate = true;
                RotateSwipeDirection();
            }
            Animation += Time.deltaTime;
            if(Animation<=1.5f && !HoopCollision){
                currentPosition = GameObject.Find("ARCamera").transform.position;
                hoop = GameObject.Find("hoop");
                rigid.isKinematic=true;
                CreateParabolaAnimation();
            }
            if((Animation>1.5f && Animation<4f) || HoopCollision){
                rigid.isKinematic=false;
                transform.position = transform.position;
                if(!HoopCollision)GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,15));
            }
            if(Animation>4f){
                /*
                // Updating max score if current score if higher
                if (BallCollider.score > BallCollider.maxScoreText) {
                    BallCollider.maxScoreText = BallCollider.score;
                    maxScoreText.text = BallCollider.maxScoreText.ToString();
                }

                // Code to be executed when you miss
                if (BallCollider.missed) {
                    status.text = "\n Your max score is: " + BallCollider.maxScoreText;

                    BallCollider.score = 0;
                    currentScoreText.text = "0";
                }

                // Resets the missed flag
                BallCollider.missed = true;*/
                
                Game.ScoreHandler.UpdateMaxScore();

                // Case if the ball does not enter the hoop
                if(!BallCollider.GetWasCollided()) {
                    Game.ScoreHandler.SetCurrentScore(0);
                    Game.ScoreHandler.SetCurrentScoreText("X");
                }
                BallCollider.SetWasCollided(false);

                ThrowBegin = false;
                Animation = 0f;
                GameObject.Find("BallIndicator").GetComponent<MeshRenderer>().enabled = true;
                GetComponent<MeshRenderer>().enabled = false;
                HoopCollision = false;
                firstUpdate = false;
                rigid.isKinematic=true;
            }
        }
    }
    void CreateParabolaAnimation(){
        Vector3 hoopPosition = hoop.transform.position;
        float hoopBallDistance = Vector2.Distance(new Vector2(hoopPosition.x,hoopPosition.z),new Vector2(currentPosition.x,currentPosition.z));
        float radianRotation = (float)(Math.PI / 180) * CameraRotation;
        float xCoordinate = (float)(hoopBallDistance*Math.Sin(radianRotation))+currentPosition.x;
        float zCoordinate = (float)(hoopBallDistance*Math.Cos(radianRotation))+currentPosition.z;
        Vector3 facingOffset = new Vector3(xCoordinate,0,zCoordinate) - swipeDirection/(2650 / hoopBallDistance);
        rigid.isKinematic = false;
        transform.position = MathParabola.Parabola(currentPosition,facingOffset+new Vector3(0,0.3f,-0.1f),0.7f,Animation/1.5f);
        //transform.Rotate(-3,0,0,Space.Self);
    }
    void RotateSwipeDirection(){
        swipeDirection = Quaternion.Euler(0, CameraRotation, 0) * swipeDirection;
    }
}
