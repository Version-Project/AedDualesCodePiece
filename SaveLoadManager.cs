using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Manager used to save Serializable data classes that we called "DataStruct" into binary files, 
/// or to load these data by reading the files in which they are stored.
/// Three kinds of DataStructs exist:
///     — "GameDataStruct" contains all the game data: players positions, game state, lists of collectibles and switches...
///     — "SettingsDataStruct" is used to store the global settings data: autosave time interval and game language.
///     — "PlayerSettingsDataStruct" is used to store the settings data specific to each player: HUD displaying and player's volume.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    private string savePath, settingsPath, psettingsPath1, psettingsPath2;
    public GameDataStruct ds;
    public SettingsDataStruct sds;
    public PlayerSettingsDataStruct psds1, psds2;
    public GameManager gameManager;

    private void Awake()
    {
        if (FindObjectsOfType<SaveLoadManager>().Length != 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

        savePath = Application.dataPath + "/save";
        settingsPath = Application.dataPath + "/settings";
        psettingsPath1 = Application.dataPath + "/psettings1";
        psettingsPath2 = Application.dataPath + "/psettings2";

        ds = new GameDataStruct();
        sds = new SettingsDataStruct();
        psds1 = new PlayerSettingsDataStruct();
        psds2 = new PlayerSettingsDataStruct();

        if (File.Exists(settingsPath))
        {
            LoadSettings();
        }
        else
        {
            sds.language = Language.Default;
            sds.autosave = Autosave.TwoMinutes;
            sds.autosaveInterval = Constants.TwoMinutesAutosave;
            SaveSettings();
        }
    }


    /// <summary>
    /// This function is called when a player confirms that he wants to save the game.
    /// A snapshot of the game manager state is created and copied into the GameDataStruct (which is serializable).
    /// The GameDataStruct is then stored into a binary file.
    /// </summary>
    public void Save()
    {
        if (gameManager)
        {
            gameManager.SaveGame();
        }
        else
        {
            ds = new GameDataStruct();
        }
        FileStream fs = File.Open(savePath, FileMode.OpenOrCreate);
        BinaryFormatter bin = new BinaryFormatter();
        bin.Serialize(fs, ds);
        fs.Close();
    }


    /// <summary>
    /// This function is called when Player 1 apply global settings changes, after the game manager has 
    /// copied these settings data to the SettingsDataStruct.
    /// The SettingsDataStruct is then stored into a binary file.
    /// </summary>
    public void SaveSettings()
    {
        FileStream fs = File.Open(settingsPath, FileMode.OpenOrCreate);
        BinaryFormatter bin = new BinaryFormatter();
        bin.Serialize(fs, sds);
        fs.Close();
    }


    /// <summary>
    /// This function is called when a player apply personal settings changes, after the game manager has 
    /// copied these settings data to the specific PlayerSettingsDataStruct.
    /// The PlayerSettingsDataStruct is then stored into a binary file, according to the player id.
    /// </summary>
    /// <param name="playerId">Id of the player we want to save personal settings.</param>
    public void SavePlayerSettings(int playerId)
    {
        string psettingsPath = playerId == Constants.Player1 ? psettingsPath1 : psettingsPath2;
        FileStream fs = File.Open(psettingsPath, FileMode.OpenOrCreate);
        BinaryFormatter bin = new BinaryFormatter();
        object psds = playerId == Constants.Player1 ? psds1 : psds2;
        bin.Serialize(fs, psds);
        fs.Close();
    }


    /// <summary>
    /// This function is called when a game is loaded from the screentitle menu.
    /// The GameDataStruct binary file is deserialized, and the data it contains are stored 
    /// in the GameDataStruct created at the game scene loading.
    /// </summary>
    public void Load()
    {
        FileStream fs = File.Open(savePath, FileMode.Open);
        BinaryFormatter bin = new BinaryFormatter();
        ds = (GameDataStruct)bin.Deserialize(fs);
        ds.hasBeenLoaded = true;
        fs.Close();
    }


    /// <summary>
    /// cf. Load() function.
    /// </summary>
    public void LoadSettings()
    {
        FileStream fs = File.Open(settingsPath, FileMode.Open);
        BinaryFormatter bin = new BinaryFormatter();
        sds = (SettingsDataStruct)bin.Deserialize(fs);
        fs.Close();
    }


    /// <summary>
    /// cf. Load() function.
    /// </summary>
    /// <param name="playerId">Id of the player we want to load personal settings.</param>
    public void LoadPlayerSettings(int playerId)
    {
        string psettingsPath = playerId == Constants.Player1 ? psettingsPath1 : psettingsPath2;
        FileStream fs = File.Open(psettingsPath, FileMode.Open);
        BinaryFormatter bin = new BinaryFormatter();
        if (playerId == Constants.Player1)
            psds1 = (PlayerSettingsDataStruct)bin.Deserialize(fs);
        else
            psds2 = (PlayerSettingsDataStruct)bin.Deserialize(fs);
        fs.Close();
    }


    /// <summary>
    /// Check if a GameDataStruct binary file already exists at the location "savePath".
    /// </summary>
    public bool CheckSaveFileExists()
    {
        return File.Exists(savePath);
    }
}
