using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;
using System.Linq;
using Bestagon;

public class Shape : MonoBehaviour
{
    /// <summary>
    /// Tracks the number of spawned shapes during the lifespan of the game (so they can be uniquely identified)
    /// </summary>
    public static ulong shapeCount = 0;

    /// <summary>
    /// Used to scale all shape visuals
    /// </summary>
    public static float ShapeSizeScaler = 1f;

    /// <summary>
    /// Color to draw this shape as
    /// </summary>
    [SerializeField] public Color color;

    [SerializeField] public bool IsSolutionUsable;

    #region Piece Information

    /// <summary>
    /// Positions of every piece in this shape
    /// </summary>
    [HideInInspector] public IEnumerable<Hex> PiecePositions => _pieces.Select(piece => piece.position);


    [HideInInspector] public IEnumerable<Piece> Pieces => _pieces;

    /// <summary>
    /// All anchors in this shape
    /// </summary>
    [HideInInspector] public IEnumerable<Piece> Anchors => _pieces.Where(piece => piece.isAnchor);

    /// <summary>
    /// All non Anchor pieces in this shape
    /// </summary>
    [HideInInspector] public IEnumerable<Hex> NonAnchorPieces => _pieces.Where(piece => piece.isAnchor == false).Select(piece => piece.position);

    /// <summary>
    /// Internal list of all pieces
    /// </summary>
    [SerializeField] private List<Piece> _pieces = new List<Piece>();

    public delegate void OnShapeMove();
    public static OnShapeMove onAnyShapeMove;
    public OnShapeMove onShapeMove;


    #endregion

    #region Rotation

    /// <summary>
    /// Rotates the whole shape around the specified hex
    /// </summary>
    /// <param name="about"></param>
    /// <param name="clockwise"></param>
    /// <param name="updateVisuals"></param>
    public void Rotate(Hex about, bool clockwise, bool updateVisuals = true)
    {
        foreach (var piece in _pieces)
            piece.position = Hex.Rotate(about, piece.position, clockwise);

        if (updateVisuals)
            UpdateLineVisuals();

        onShapeMove?.Invoke();
    }

    /// <summary>
    /// Returns all positons the shape must pass through in order to 
    /// </summary>
    /// <param name="about"></param>
    /// <param name="clockwise"></param>
    /// <returns></returns>
    public Hex[] ShapeRotationalSweep(Hex about, bool clockwise)
    {
        return PositionalSweep(about, _pieces.Select(piece => piece.position).ToArray(), clockwise);
    }

    private static Hex[] PositionalSweep(Hex about, Hex[] positions, bool clockwise)
    {
        List<Hex> results = new List<Hex>();
        foreach (var position in positions)
        {
            foreach (var hex in Hex.RotationSweep(about, position, clockwise))
            {
                results.Add(hex);
            }
        }

        return results.ToArray();
    }


    public Hex.Side[] ValidRotationSidesInDirection(Piece anchor, Piece handle, bool clockwise)
    {
        List<Hex.Side> validSides = new List<Hex.Side>();
        Hex.Side currentSide;
        //Figure out what the current orientation is between anchors and set that as the 'current orientation'
        Hex.GetRelativeSide(anchor.position, handle.position, out currentSide, out _);
        validSides.Add(currentSide);
        //  Create a frame of reference set of positions based on the starting orientation
        Hex[] piecePositions = _pieces.Select(piece => piece.position).ToArray();
        //Start a loop
        int safteyIter = 32;
        while (safteyIter-- > 0)
        {
            Hex[] rotationsCollisionHexes = PositionalSweep(anchor.position, piecePositions, clockwise);
            if (LevelManager.CheckCollisions(this, rotationsCollisionHexes, out var _) != LevelManager.CollisionType.None)
            {
                //There was a collision, we are done, break the loop
                break;
            }
            else
            {
                currentSide = currentSide.RotationalExpansion(1,clockwise)[0];
                //Stop if we've been here before
                if (validSides.Contains(currentSide))
                    break;
                validSides.Add(currentSide);
                piecePositions = piecePositions.Select(position => Hex.Rotate(anchor.position, position, clockwise)).ToArray();
            }

        }
        return validSides.ToArray();
    }
    #endregion

    #region Slide

    public void Slide(Hex.Side slideDir, bool UpdateVisuals = true)
    {
        foreach (var piece in _pieces)
            piece.position += slideDir.Offset();
        if (UpdateVisuals)
            UpdateLineVisuals();
        onShapeMove?.Invoke();
    }

    /// <summary>
    /// Collects Hex positions after a side
    /// </summary>
    /// <param name="slideDir"></param>
    /// <returns></returns>
    public Hex[] PositionsAfterSlide(Hex.Side slideDir)
    {
        return _pieces.Select(piece => piece.position + slideDir.Offset()).ToArray();
    }
    #endregion

    #region Modification
    public void AddPiece(Piece piece)
    {
        _pieces.Add(piece);
    }

    public void RemovePieceAtHex(Hex h)
    {
        var found = Pieces.Where(p => p.position.Equals(h)).ToArray();
        foreach (var piece in found)
        {
            _pieces.Remove(piece);
            piece.DestroySelf();
        }
        
    }

    public void UpdateLineVisuals()
    {
        foreach (var piece in Pieces)
        {
            piece.DrawAsPartOfShape(this);
        }
    }
    #endregion

    #region Unity
    private void Start()
    {
        onShapeMove += onAnyShapeMove;
    }

    private void OnDestroy()
    {
        onShapeMove -= onAnyShapeMove;
    }


    public static Shape MakeShapeObj()
    {
        GameObject obj = new GameObject();
        obj.name = "Shape " + shapeCount++;
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.AddComponent<ShapeMeshGenerator>();
        return obj.AddComponent<Shape>();
    }
    #endregion


}
