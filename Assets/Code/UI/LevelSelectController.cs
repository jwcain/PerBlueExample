using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelSelectController : Bestagon.Behaviours.ProtectedSceneSingleton<LevelSelectController>
{

    public GameObject confirmationMenu;
    public GameObject buttonPrefab;
    public RectTransform buttonHolder;

    Dictionary<string, LevelSelectButton> buttonMap = new Dictionary<string, LevelSelectButton>();

    /// <summary>
    /// Creates a button in the level select screen for each level
    /// </summary>
    public static void Populate()
    {
        foreach (var level in LevelManager.Levels)
        {
            if (Instance.buttonMap.ContainsKey(level.name) == false)
            {
                var newButton = CreateNewButton();
                newButton.SetName(level.name);
                newButton.RegisterEvent(() => {
                    if (level.GetCompletionStatus() != LevelObject.CompletionStatus.Locked)
                    {
                        MenuController.Instance.SetToGameUI();
                        LevelManager.Instance.LevelSelectLoad(level); 
                    }
                });
                newButton.UpdateByStatus(level.GetCompletionStatus());
                Instance.buttonMap.Add(level.name, newButton);
            }
            else
            {
                Instance.buttonMap[level.name].UpdateByStatus(level.GetCompletionStatus());
            }
        }
    }

    public void OpenConfirmationMenu()
    {
        confirmationMenu.SetActive(true);
    }

    public void CloseConfirmationMenu()
    {
        confirmationMenu.SetActive(false);
    }

    public void WhipeLevelData()
    {
        foreach (var level in LevelManager.Levels)
            level.ClearSavedData();
        LevelManager.Instance.LevelSelectLoad(LevelManager.Levels.First());
    }

    static LevelSelectButton CreateNewButton()
    {
        GameObject button = Instantiate(Instance.buttonPrefab, Instance.buttonHolder);
        return button.GetComponent<LevelSelectButton>();
    }

    protected override void Destroy()
    {
        
    }
}
