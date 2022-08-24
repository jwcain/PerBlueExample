using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a music track within FMOD and wraps necessary information for attribution
/// </summary>
[CreateAssetMenu(menuName = "xAttributedMusic")]
public class AttributedMusic : ScriptableObject
{
    [SerializeField] public string title;
    [SerializeField] public FMODUnity.EventReference track;
    [SerializeField] public string author;
    [SerializeField] public string[] websites;
    [SerializeField] public string liscense;
}
