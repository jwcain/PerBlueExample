using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a button in the level select screen
/// </summary>
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button button;
    [SerializeField] TMPro.TMP_Text textObj;
    [SerializeField] UnityEngine.UI.Image lockedImage;
    [SerializeField] UnityEngine.UI.Image starredImage;

    string levelName = "";

    /// <summary>
    /// Adds a listener to the Unity Event
    /// </summary>
    /// <param name="e"></param>
    public void RegisterEvent(UnityEngine.Events.UnityAction e)
    {
        button.onClick.AddListener(e);
    }

    /// <summary>
    /// Sets the name of the game object and stores the level name for access later
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        levelName = name;
        this.gameObject.name = name;
    }

    /// <summary>
    /// Updates te visual elements based on a level completion status
    /// </summary>
    /// <param name="completionStatus"></param>
    internal void UpdateByStatus(LevelObject.CompletionStatus completionStatus)
    {
        switch (completionStatus)
        {
            default:
            case LevelObject.CompletionStatus.Locked:
                lockedImage.gameObject.SetActive(true);
                starredImage.gameObject.SetActive(false);
                textObj.text = "???"; // Hide the level name
                break;
            case LevelObject.CompletionStatus.Unlocked:
                lockedImage.gameObject.SetActive(false);
                starredImage.gameObject.SetActive(false);
                textObj.text = levelName;
                break;
            case LevelObject.CompletionStatus.Starred:
                lockedImage.gameObject.SetActive(false);
                starredImage.gameObject.SetActive(true);
                textObj.text = levelName;
                break;
        }
    }
}
