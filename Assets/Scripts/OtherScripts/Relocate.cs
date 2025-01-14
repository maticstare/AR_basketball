using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity.Sample;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Relocate : MonoBehaviour
{
    public void LoadScene(string sceneName){
        
        if(SceneManager.GetActiveScene().name != "MenuScene") {
            SaveHandler.sh_SaveGameData();
        }
        
        Game.skinChanged = false;

        if(SceneManager.GetActiveScene().name == "Hand Landmark Detection") {
            CleanupSceneObjects();
        }
        

        SceneManager.LoadScene(sceneName);
        
        Game.data = SaveHandler.sh_LoadGameData();
    }

    private void CleanupSceneObjects()
    {
        // Find all root objects in the current scene
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var obj in rootObjects)
        {
            // Destroy each object
            Destroy(obj);
        }
        if (ImageSourceProvider.ImageSource != null)
        {
            ImageSourceProvider.ImageSource.Stop();
        }
    }

}
