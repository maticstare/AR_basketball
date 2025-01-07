using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveHandler : MonoBehaviour
{

    // persistentDataPath -> directory path where you can store data that you want to be kept between runs
    // iOS and Android, persistentDataPath points to a public directory on the device 
    private static string path = Application.persistentDataPath + "/maxscore.abd";

    public static void sh_SaveGameData () {
        Debug.Log("Saving game data . . . ");

        BinaryFormatter _formatter = new BinaryFormatter();

        // provedes a stream for a file { read, write operations }
        // path -> string reference to the file location
        // FileMode -> Specifies how the operating system should open a file

        FileStream _stream = new FileStream(path, FileMode.Create);

        int _maxScore = Game.ScoreHandler.GetMaxScore();
        string _ballColor = Game.SkinHandler.GetBallSkin();
        string[] _unlockedSkins = Game.SkinHandler.GetUnlockedSkins();

        GameData _data = new GameData(_maxScore, _ballColor, _unlockedSkins);
        _formatter.Serialize(_stream, _data);
        _stream.Close();
    }

    public static GameData sh_LoadGameData () {
        if(!File.Exists(path)) {
            Debug.LogError("No game data found in " + path + "\n Returning default game data!");
            return GameData.defaultData;
        } 
    
        BinaryFormatter _formatter = new BinaryFormatter();

        // provedes a stream for a file { read, write operations }
        // path -> string reference to the file location
        // FileMode -> Specifies how the operating system should open a file
        FileStream _stream = new FileStream(path, FileMode.Open);

        if(_stream.Length == 0) {
            return GameData.defaultData;
        }

        GameData data = (GameData)_formatter.Deserialize(_stream);
        Debug.Log("Loaded data with " + data.maxScore + " max score and " + data.ballColor + " ballColor!");
        _stream.Close();
        return data;
    }

}
