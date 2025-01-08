using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Relocate : MonoBehaviour
{
    public void LoadScene(string sceneName){
        if(SceneManager.GetActiveScene().name == "MainScene") {
            SaveHandler.sh_SaveGameData();
        }
        Game.skinChanged = false;

        SceneManager.LoadScene(sceneName);
        
        Game.data = SaveHandler.sh_LoadGameData();
    }
}
