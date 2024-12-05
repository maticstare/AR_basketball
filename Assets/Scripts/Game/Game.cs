using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    public static ScoreHandler ScoreHandler;
    public static SkinHandler SkinHandler;
    public static GameData data;
    public static bool skinChanged = false;
    [SerializeField] GameObject _ball;
    [SerializeField] GameObject _ballIndicator;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
    }

    public static void initialize() {
        // Loads saved data
        data = SaveHandler.sh_LoadGameData();

        // Initializes the score handler
        ScoreHandler = new ScoreHandler(data.maxScore);
        ScoreHandler.SetMaxScoreText(ScoreHandler.GetMaxScore().ToString());

        // Loads ball meta data
        SkinHandler = new SkinHandler(data.ballColor, data.unlockedSkins);
    }

    void Update() {
        //GameObject _ball = GameObject.Find("Ball");
        //GameObject _ballIndicator = GameObject.Find("BallIndicator");
        if(!skinChanged && _ball != null && _ballIndicator != null) {
            Debug.Log("Here");
            MeshRenderer _ballMeshRenderer = _ball.GetComponent<MeshRenderer>();
            MeshRenderer _ballIndicatorMeshRenderer = _ballIndicator.GetComponent<MeshRenderer>();
            
            string _skinName = SkinHandler.GetBallSkin() + "Basketball";
            Material _material = Resources.Load("Materials/BasketballTextures/" + _skinName, typeof(Material)) as Material;

            _ballMeshRenderer.material = _material;
            _ballIndicatorMeshRenderer.material = _material;
 
            skinChanged = true;
        }
    }

    // Save high score before game exit    
	private void OnApplicationQuit() {
		SaveHandler.sh_SaveGameData();
	}
}
