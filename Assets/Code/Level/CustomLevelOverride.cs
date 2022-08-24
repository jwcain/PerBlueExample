using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A helper script, doesnt destroy on load. 
/// Its existence tells other parts of the code that the current level was loaded from the level editor and not to treat it as a normal level in the sequence
/// </summary>
public class CustomLevelOverride : MonoBehaviour
{
    /// <summary>
    /// Direct reference to the level to load/override with
    /// </summary>
    public LevelObject level;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
