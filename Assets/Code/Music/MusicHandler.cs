using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the queing, playing, and waiting between music tracks
/// </summary>
public class MusicHandler : Bestagon.Behaviours.ProtectedSceneSingleton<MusicHandler>
{
    /// <summary>
    /// All available music tracks
    /// </summary>
    public static IEnumerable<AttributedMusic> MusicTracks => Instance.musicTracks;

    [Header("Music Tracks")]
    [SerializeField] AttributedMusic ambientTrack;
    [SerializeField] AttributedMusic endScreenTrack;
    [SerializeField] AttributedMusic[] musicTracks;
    
    [Header("Game Objects")]
    /// <summary>
    /// Text to update to show the music title
    /// </summary>
    [SerializeField] TMPro.TMP_Text musicTitleText;
    [Header("Settings")]
    /// <summary>
    /// Time for animating between the ambient track and a music track
    /// </summary>
    [SerializeField] float musicToAmbientFadeTime = 2f;

    /// <summary>
    /// Minimum time to wait between tracks
    /// </summary>
    [SerializeField] float minTimeBetweenTracks;
    /// <summary>
    /// Maximum time to wait between tracks
    /// </summary>
    [SerializeField] float maxTimeBetweenTracks;

    [SerializeField] AnimationCurve VolumeSettingInterpretationCurve;

    [Header("Internal")]
    [SerializeField, EditorReadOnly] bool hasAmbient = false;
    [SerializeField, EditorReadOnly] bool playingMusic = false;

    FMOD.Studio.EventInstance ambientTrackInstance;
    FMOD.Studio.EventInstance currentMusicTrack;

    /// <summary>
    /// Music queue tracked as integers in the music tracks list
    /// </summary>
    private Queue<int> musicQueue;
    private int lastPlayedTrack = -1;

    /// <summary>
    /// Tracks if there are coroutines that want to override the music. Default playing only occurs when this is 0
    /// </summary>
    int musicOverride = 0;



    void Start()
    {
        musicQueue = new Queue<int>();
        StartCoroutine(Init());
    }

    /// <summary>
    /// Returns the current music volume that should be used based on player settings and volume curve
    /// </summary>
    /// <returns></returns>
    float GetUsedMusicVolume()
    {
        return VolumeSettingInterpretationCurve.Evaluate(PlayerPrefs.GetFloat(MenuController.MusicVolumeString) / 100f);
    }

    float lastProcessedValue;
    private void Update()
    {
        //Various checks to see if the music volume needs to be updated
        if (musicOverride > 0 && PlayerPrefs.GetFloat(MenuController.MusicVolumeString) / 100f is var newValue && Mathf.Abs(newValue - lastProcessedValue) > float.Epsilon && currentMusicTrack.isValid())
        {
            lastProcessedValue = newValue;
            currentMusicTrack.setVolume(GetUsedMusicVolume());
        }
    }

    /// <summary>
    /// Generates a new music queue that does not have the last played track within the first third of the new queues
    /// </summary>
    void GenerateNewMusicQueue()
    {
        //Dont generate a new queue if there are songs remaining
        if (musicQueue.Count != 0)
            return;
        //initialize a list of music tracks into the queue
        List<int> remaining = new List<int>();
        for (int i = 0; i < musicTracks.Length; i++)
        {
            //Dont add the last played track for now (we dont want that one to play too soon to the last time it was played)
            if (i == lastPlayedTrack)
                continue;
            remaining.Add(i);
        }
        //Shuffle the queue
        Shuffle<int>(remaining);
        //Load some amount of tracks into the queue
        int avoidTooSoonReplayCount = musicTracks.Length / 3;
        for (int i = 0; i < avoidTooSoonReplayCount; i++)
        {
            musicQueue.Enqueue(remaining[0]);
            remaining.RemoveAt(0);
        }
        //Now, add the last played track (if its valid)
        if (lastPlayedTrack >= 0 && lastPlayedTrack < musicTracks.Length)
        {
            remaining.Add(lastPlayedTrack);
            Shuffle<int>(remaining);
        }
        //Add the rest to the queue
        while (remaining.Count > 0)
        {
            musicQueue.Enqueue(remaining[0]);
            remaining.RemoveAt(0);
        }
    }

    /// <summary>
    /// Shuffles a list (fisher yates)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, list.Count);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Flag that overrides music playing for the end credit sting
    /// </summary>
    public static void EndScreenOverride()
    {
        Instance.StartCoroutine(Instance.HandleEndScreenSting());
    }


    /// <summary>
    /// Initializes the music player
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        if (ambientTrack != null)
        {
            ambientTrackInstance = FMODUnity.RuntimeManager.CreateInstance(ambientTrack.track);
            ambientTrackInstance.setVolume(0f);
            ambientTrackInstance.start();
            hasAmbient = true;
        }
        StartCoroutine(HandleMusicForever());
    }
    IEnumerator HandleEndScreenSting()
    {
        musicOverride++;
        if (playingMusic)
        {
            yield return ForceStopMusic();
        }
        else
        {
            yield return (AnimUtility.LerpOverTime(GetUsedMusicVolume(), 0f, musicToAmbientFadeTime, (float val) => { ambientTrackInstance.setVolume(val); }));
        }
        FMODUnity.RuntimeManager.PlayOneShot(endScreenTrack.track);
        yield return new WaitForSeconds(10f);
        musicOverride--;
    }

    IEnumerator ForceStopMusic()
    {
        if (playingMusic == false)
            yield break;
        musicOverride++;
        yield return (AnimUtility.LerpOverTime(GetUsedMusicVolume(), 0f, musicToAmbientFadeTime, (float val) => { currentMusicTrack.setVolume(val); }));
        currentMusicTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        playingMusic = false;
        musicOverride--;
    }

    IEnumerator HandleMusicForever()
    {
        while (true)
        {
            while (musicOverride > 0) yield return new WaitForEndOfFrame();
            playingMusic = false;
            musicTitleText.text = "";
            StartCoroutine(AnimUtility.LerpOverTime(1f, 0f, musicToAmbientFadeTime, (float val) => { musicTitleText.color = new Color(musicTitleText.color.r, musicTitleText.color.g, musicTitleText.color.b, val); }));
            //Fade in ambient
            if (hasAmbient)
            {
                yield return AnimUtility.LerpOverTime(0f, GetUsedMusicVolume(), musicToAmbientFadeTime, (float val) => { ambientTrackInstance.setVolume(val); });
            }
            //Wait for a time
            yield return new WaitForSeconds(Random.Range(minTimeBetweenTracks, maxTimeBetweenTracks));
            playingMusic = true;
            //Genearte new queue
            if (musicQueue.Count == 0)
                GenerateNewMusicQueue();
            lastPlayedTrack = musicQueue.Dequeue();
            currentMusicTrack = FMODUnity.RuntimeManager.CreateInstance(musicTracks[lastPlayedTrack].track);
            currentMusicTrack.start();
            musicTitleText.text = $"<i>{musicTracks[lastPlayedTrack].title} - {musicTracks[lastPlayedTrack].author}";
            StartCoroutine(AnimUtility.LerpOverTime(0f, 1f, musicToAmbientFadeTime, (float val) => { musicTitleText.color = new Color(musicTitleText.color.r, musicTitleText.color.g, musicTitleText.color.b, val); }));
            //Crossfade out ambient
            if (hasAmbient)
                StartCoroutine(AnimUtility.LerpOverTime(GetUsedMusicVolume(), 0f, musicToAmbientFadeTime, (float val) => { ambientTrackInstance.setVolume(val); }));
            
            yield return AnimUtility.LerpOverTime(0f, GetUsedMusicVolume(), musicToAmbientFadeTime, (float val) => { currentMusicTrack.setVolume(val);});
            //Wait for it to stop
            FMOD.Studio.PLAYBACK_STATE state = FMOD.Studio.PLAYBACK_STATE.STARTING;
            while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                currentMusicTrack.getPlaybackState(out state);
                yield return new WaitForEndOfFrame();
            }
        }

    }

    protected override void Destroy()
    {

    }
}
