using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{

    float width;
    float height;
    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    public void SetText(string text) {
        gameObject.GetComponent<TMP_Text>().text = Game.ScoreHandler.GetCurrentScore().ToString();
    }
    // Update is called once per frame
    void Update()
    {
        // DEPRICATED: score text should only update when score in increased
        // if(score>0) GameObject.Find("ScoreText").GetComponent<TMP_Text>().text = score.ToString();
    }
}
