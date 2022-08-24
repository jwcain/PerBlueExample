using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


/// <summary>
/// Serialized level representation
/// </summary>
[CreateAssetMenu(menuName ="LevelObj")]
public class LevelObject : ScriptableObject
{
    const string levelSavePrefix = "LEVEL_DATA_";
    public enum CompletionStatus { Locked = 10, Unlocked = 20, Starred = 100 }

    public CompletionStatus GetCompletionStatus()
    {
        if (PlayerPrefs.HasKey(levelSavePrefix + this.name))
            return (CompletionStatus)PlayerPrefs.GetInt(levelSavePrefix + this.name);

        return CompletionStatus.Locked;
    }

    public void SetCompletionStatus(CompletionStatus status)
    {
        PlayerPrefs.SetInt(levelSavePrefix + this.name, (int)status);
    }

    public enum LevelSize { Zero=0, One=1, Two=2, Three=3 }

    [SerializeField] public LevelSize Size;

    /// <summary>
    /// [TODO]
    /// </summary>
    [SerializeField] public int moveCountForStar;

    [System.Serializable]
    public class SerializedShape
    {
        /// <summary>
        /// If this piece is used in the sollution (is a 'gold' shape)
        /// </summary>
        [SerializeField] public bool isSolutionUsable;
        [SerializeField] public Bestagon.Hexagon.Hex[] anchors;
        [SerializeField] public Bestagon.Hexagon.Hex[] positions;

        public static SerializedShape FromShape(Shape shape)
        {
            var serializedShape = new SerializedShape();
            serializedShape.anchors = shape.Anchors.Select(a => a.position).ToArray();
            serializedShape.positions = shape.Pieces.Select(p => p.position).ToArray();
            return serializedShape;
        }
    }

    [SerializeField] public SerializedShape[] shapes;
    [SerializeField] public Bestagon.Hexagon.Hex[] additionalWalls;
    [SerializeField] public Bestagon.Hexagon.Hex[] goalPositions;

    public void Unlock()
    {
        if (GetCompletionStatus() == CompletionStatus.Locked)
            SetCompletionStatus(CompletionStatus.Unlocked);
    }
    public void TryForStar(int moveCount)
    {
        if (moveCount <= moveCountForStar)
            SetCompletionStatus(CompletionStatus.Starred);
    }

    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(levelSavePrefix + this.name);
    }
}
