using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class BallCollider : MonoBehaviour
{
    private static bool collided = false;

    void OnTriggerEnter(Collider other)
    {
        if(other.name=="Ball"){
            Game.ScoreHandler.IncrementCurrentScore();
            SetWasCollided(true);

            int _currentScore = Game.ScoreHandler.GetCurrentScore();
            if(Game.SkinHandler.CanUnlockNewSkin(_currentScore)){
                Game.SkinHandler.UnlockSkin(Game.SkinHandler.GetSkinFromScore(_currentScore));
            }
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        //Debug.Log("Object is inside the trigger");
    }
    
    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Object exited the trigger");
    }

    public static bool SetWasCollided(bool a_flag) {
        collided = a_flag;
        return collided;
    }

    public static bool GetWasCollided() {
        return collided;
    }
}
