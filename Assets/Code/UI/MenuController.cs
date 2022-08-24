using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : Bestagon.Behaviours.ProtectedSceneSingleton<MenuController>
{
    public GameObject menuOBJ;
    public GameObject gameUIOBJ;
    public GameObject levelSelectOBJ;
    public GameObject musicCreditsOBJ;
    public GameObject endScreenOBJ;

    public const string MusicVolumeString = "MusicVolume";
    public const string SoundEffectsVolumeString = "SoundEffectsVolume";

    public UnityEngine.UI.Slider musicSlider;
    public TMPro.TMP_InputField musicText;
    public UnityEngine.UI.Slider FXSlider;
    public TMPro.TMP_InputField FXText;

    public static bool MenuOpen { get {
            return !Instance.gameUIOBJ.activeSelf; 
        } }

    protected override void Destroy()
    {

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(MusicVolumeString) == false)
            PlayerPrefs.SetFloat(MusicVolumeString, 65f);
        if (PlayerPrefs.HasKey(SoundEffectsVolumeString) == false)
            PlayerPrefs.SetFloat(SoundEffectsVolumeString, 65f);
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeString));
        SetSoundEffectsVolume(PlayerPrefs.GetFloat(SoundEffectsVolumeString));
        Bestagon.Events.InputEvents.Register(KeyCode.Escape, Bestagon.Events.KeyEvent.Down, () => {
            if (MenuOpen)
                SetToGameUI();
            else
                SetToMenuUI();
            return null;
        });
        gameUIOBJ.SetActive(true);
    }

    public void SetMusicViaText(string value)
    {
        SetMusicVolume(float.Parse(value));
    }

    public void SetFXViaText(string value)
    {
        SetSoundEffectsVolume(float.Parse(value));
    }
    public void SetMusicVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 100f);
        PlayerPrefs.SetFloat(MusicVolumeString, val);
        musicSlider.SetValueWithoutNotify(val);
        musicText.SetTextWithoutNotify(val.ToString("00"));

    }
    public void SetSoundEffectsVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 100f);
        PlayerPrefs.SetFloat(SoundEffectsVolumeString, val);
        FXSlider.SetValueWithoutNotify(val);
        FXText.SetTextWithoutNotify(val.ToString("00"));
    }

    public void SetToMenuUI()
    {
        menuOBJ.SetActive(true);
        levelSelectOBJ.SetActive(false);
        gameUIOBJ.SetActive(false);
        musicCreditsOBJ.SetActive(false);
        endScreenOBJ.SetActive(false);
    }

    public void SetToGameUI()
    {
        menuOBJ.SetActive(false);
        levelSelectOBJ.SetActive(false);
        gameUIOBJ.SetActive(true);
        musicCreditsOBJ.SetActive(false);
        endScreenOBJ.SetActive(false);
    }

    public void SetToLevelSelectUI()
    {
        menuOBJ.SetActive(false);
        levelSelectOBJ.SetActive(true);
        gameUIOBJ.SetActive(false);
        musicCreditsOBJ.SetActive(false);
        endScreenOBJ.SetActive(false);
        LevelSelectController.Populate();

    }

    public void SetToMusicCreditsUI()
    {
        menuOBJ.SetActive(false);
        levelSelectOBJ.SetActive(false);
        gameUIOBJ.SetActive(false);
        musicCreditsOBJ.SetActive(true);
        endScreenOBJ.SetActive(false);
    }

    public void SetEndScreen()
    {
        menuOBJ.SetActive(false);
        levelSelectOBJ.SetActive(false);
        gameUIOBJ.SetActive(false);
        musicCreditsOBJ.SetActive(false);
        endScreenOBJ.SetActive(true);
    }

    public void ResetLevel()
    {
        LevelManager.Instance.ResetLevel();
        SetToGameUI();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("EXIT GAME");
        Debug.Break();
#else
        Application.Quit();
#endif
    }
}
