using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the music credit screen. Will self populate on enable
/// </summary>
public class MusicCreditScreen : Bestagon.Behaviours.ProtectedSceneSingleton<MusicCreditScreen>
{

    public GameObject musicPrefab;
    public GameObject musicCreditHolder;

    bool populated = false;
    public void OnEnable()
    {
        //Only populate once
        if (Instance.populated)
            return;
        Instance.populated = true;

        //Populate an element per music credit
        foreach (var track in MusicHandler.MusicTracks)
        {
            string websitestring = "";
            for (int i = 0; i < track.websites.Length; i++)
            {
                websitestring += "\n\t" + track.websites[i];
            }
            CreateNewButton().text = $"{track.title} by {track.author}{websitestring}\n\t<i>Liscense: {track.liscense}";
        }
    }

    static TMPro.TMP_InputField CreateNewButton()
    {
        GameObject button = Instantiate(Instance.musicPrefab, Instance.musicCreditHolder.transform);
        return button.GetComponent<TMPro.TMP_InputField>();
    }

    protected override void Destroy()
    {

    }
}
