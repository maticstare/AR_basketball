using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public static GameData defaultData = new GameData(0, "Classic", new string[1]{"Classic"});
    
    public int maxScore;
    public string ballColor;
    public string[] unlockedSkins;

    public GameData(int ca_score, string ca_ballColor, string[] ca_unlockedSkins) {
        this.maxScore = ca_score;
        this.ballColor = ca_ballColor;
        this.unlockedSkins = ca_unlockedSkins;
    }
}
