using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecretBall : MonoBehaviour
{
    [SerializeField] GameObject PurpleParticleSystem;
    [SerializeField] TextMeshProUGUI scoreText;

    void Start() {
        // Removes the SecretBall object form scene if the purple ball is already unlocked
        if(Game.SkinHandler.IsSkinUnlocked("Purple")) {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if(other.name=="Ball"){
            scoreText.text = "";
            BallCollider.SetWasCollided(true);
            // Removes the secret ball object form scene
            this.gameObject.SetActive(false);

            Game.SkinHandler.UnlockSkin("Purple");
        }
    }
}
