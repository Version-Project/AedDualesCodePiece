using UnityEngine;


/// <summary>
/// This manager is used to update the sounds volume of the game each time a player's volume settings has been modified.
/// </summary>
public class SoundManager : MonoBehaviour
{
    [SerializeField] private float musicBase;
    [SerializeField] private float sfxBase;

    private SaveLoadManager slManager;
    private AudioSource backgroundMusic;
    private float[] sfxVolume;

    private void Start()
    {
        slManager = GameObject.FindGameObjectWithTag(nameof(SaveLoadManager)).GetComponent<SaveLoadManager>();
        backgroundMusic = GameObject.Find("/BackgroundMusic").GetComponent<AudioSource>();
        sfxVolume = new float[Constants.NbPlayers];
        UpdateVolume();
    }

    /// <summary>
    /// Get back the "virtual" volume settings value for each player (from 1 to 10), and calculate the new background music volume 
    /// and the new SFX volume balance between Player 1's stereo side and Player 2's, according to these settings.
    /// Default value is when the two players have a volume settings value of 5. In this case, background music volume is 0.25f,
    /// and SFX volume is 0.75f for each player.
    /// 
    /// Example: if Player 1 has a volume settings of 2 and Player 2 a volume settings of 6, the background music volume will be 0.2f 
    /// (lower than the default value of 0.25f, because 2 + 6 < 5 + 5), Player 1's SFX volume will be 0.3f, and Player 2's will be 0.775f.
    /// </summary>
    public void UpdateVolume()
    {
        float[] volume = new float[Constants.NbPlayers];
        volume[Constants.Player1] = slManager.psds1.volume;
        volume[Constants.Player2] = slManager.psds2.volume;

        float volumeFraction = musicBase > 0.5f ? (1 - musicBase) / 10 : musicBase / 10;
        backgroundMusic.panStereo = (volume[Constants.Player2] - volume[Constants.Player1]) / 10;
        backgroundMusic.volume = musicBase + (volume[Constants.Player1] - Constants.DefaultSettingsVolume
            + volume[Constants.Player2] - Constants.DefaultSettingsVolume) * volumeFraction;

        for (int i = 0; i < Constants.NbPlayers; ++i)
        {
            if (volume[i] > Constants.DefaultSettingsVolume)
                volumeFraction = (1 - sfxBase) / 10;
            else
                volumeFraction = sfxBase / Constants.DefaultSettingsVolume;
            sfxVolume[i] = sfxBase + (volume[i] - Constants.DefaultSettingsVolume) * volumeFraction;
        }
    }

    /// <summary>
    /// Get the SFX volume of a specific player.
    /// </summary>
    /// <param name="playerId">Id of the player we want to get SFX volume.</param>
    public float GetSFXVolume(int playerId) { return sfxVolume[playerId]; }
}
