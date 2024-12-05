using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler {

    private TextMeshProUGUI currentScoreText;
    private TextMeshPro maxScoreText;
    
    private int score;
    private int maxScore;

    public ScoreHandler(int ca_maxScore) {
        this.score = 0;
        this.maxScore = ca_maxScore;
        GameObject cst = GameObject.Find("ScoreText");
        GameObject mst = GameObject.Find("ScoreText");
        if(cst != null) {
            this.currentScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        }
        if(mst != null) {
            this.maxScoreText = GameObject.Find("MaxScoreText").GetComponent<TextMeshPro>();
        }
    }

       public int IncrementCurrentScore() {
        this.SetCurrentScore(this.GetCurrentScore() + 1);
        return this.score;
    }

    public int GetCurrentScore() {
        return this.score;
    }

    public int GetMaxScore() {
        return this.maxScore;
    }

    public int SetCurrentScore(int a_score) {
        this.score = a_score;
        SetCurrentScoreText(a_score.ToString());
        return this.score;
    }

    public int SetMaxScore(int a_maxScore) {
        this.maxScore = a_maxScore;
        SetMaxScoreText(a_maxScore.ToString());
        return this.maxScore;
    }

    public bool UpdateMaxScore() {
        if(this.GetMaxScore() > this.GetCurrentScore()) {
            return false;
        }
        this.SetMaxScore(Game.ScoreHandler.GetCurrentScore());
        return true;
    }

    public void SetCurrentScoreText(string text) {
        if(this.currentScoreText != null) {
            this.currentScoreText.text = text;
        }
    }

    public void SetMaxScoreText(string text) {
        if(this.maxScoreText != null) {
            this.maxScoreText.text = text;
        }
    }
}
