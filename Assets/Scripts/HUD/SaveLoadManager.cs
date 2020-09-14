using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [Serializable]
    private struct SaveFormat
    {
        public int Money;

        public int GreenHeroLevel;
        public int BlueHeroLevel;
        public int YellowHeroLevel;
    }

    private const int DefaultMoney = 10;
    private const int DefaultLevel = 1;

    private const string SaveDir = "saves";
    private const string SaveFilename = "save"; 

    [SerializeField]
    private PlayerState _playerState = null;

    protected void Awake()
    {
        LoadPlayerState();
    }

    protected void OnDestroy()
    {
        SavePlayerState();
    }

    private void SavePlayerState()
    {
        string fullSaveDir = $"{Application.persistentDataPath}/{SaveDir}";
        Directory.CreateDirectory(fullSaveDir);
 
        using (var stream = new StreamWriter($"{fullSaveDir}/{SaveFilename}"))
        {
            var json = JsonUtility.ToJson(new SaveFormat()
            {
                Money = _playerState.Money,
                GreenHeroLevel = _playerState.GreenHero.Level,
                BlueHeroLevel = _playerState.BlueHero.Level,
                YellowHeroLevel = _playerState.YellowHero.Level
            }); 
            
            stream.Write(json);
        }
    }

    private void LoadPlayerState()
    {
        string saveFile = $"{Application.persistentDataPath}/{SaveDir}/{SaveFilename}";

        if (File.Exists(saveFile))
        {
            using (var stream = new StreamReader(saveFile))
            {
                string json = stream.ReadToEnd();
                SaveFormat save = JsonUtility.FromJson<SaveFormat>(json);
                _playerState.Money = save.Money;
                _playerState.GreenHero.Level = save.GreenHeroLevel;
                _playerState.BlueHero.Level = save.BlueHeroLevel;
                _playerState.YellowHero.Level = save.YellowHeroLevel;
            }
        }
        else
        {
            _playerState.Money = DefaultMoney;
            _playerState.GreenHero.Level = _playerState.BlueHero.Level = _playerState.YellowHero.Level = DefaultLevel;
        }
    }
}
