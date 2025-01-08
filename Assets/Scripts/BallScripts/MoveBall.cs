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
        hoop = GameObject.Find("hoop");
    }

    void Update()
    {   
        if(ThrowBegin){
            if (!firstUpdate){
                firstUpdate = true;
                RotateSwipeDirection();
            }
            
            Animation += Time.deltaTime;
            
            // ustavi animacijo in dodaj gravitacijo, ce zoga zadane kos
            if(Animation<=1.6f && Animation>=1.4f && HoopCollision){
                rigid.isKinematic=false;
            }

            // izvajaj animacijo dokler zoga ne zadane kosa
            if(!HoopCollision){
                currentPosition = GameObject.Find("Main Camera").transform.position;
                rigid.isKinematic=true;
                CreateParabolaAnimation();
            }

            // obmocje po koncani animaciji
            if(Animation>4f){
                rigid.isKinematic = true;
                
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
        transform.position = MathParabola.Parabola(currentPosition,facingOffset+new Vector3(0,0.3f,-0.1f),0.7f,Animation/1.5f);
        //transform.Rotate(-3,0,0,Space.Self);
    }
    void RotateSwipeDirection(){
        swipeDirection = Quaternion.Euler(0, CameraRotation, 0) * swipeDirection;
    }
}
