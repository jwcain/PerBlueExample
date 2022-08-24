using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Bestagon.Hexagon;
using UnityEngine.Tilemaps;
using System;

public class LevelManager : Bestagon.Behaviours.ProtectedSceneSingleton<LevelManager>
{
    public static IEnumerable<Shape> Shapes => Instance.shapes;
    public static IEnumerable<LevelObject> Levels => Instance.levels;
    public static bool Loading { get { return _loading; } }


    /// <summary>
    /// Game object to enable/disable for the first screen the player loads
    /// </summary>
    [Header("Game Object Links")]
    [SerializeField] GameObject FirstScreenStuff;
    /// <summary>
    /// Reference to the game camera, used to load settings required for different level sizes
    /// </summary>
    [SerializeField] Camera gameCam;
    /// <summary>
    /// UI element used to fade in and out from levels
    /// </summary>
    [SerializeField] UnityEngine.UI.Image fadoutImage;
    [SerializeField] private TMPro.TMP_Text titleText;

    [Header("Levels")]
    [SerializeField] private LevelObject[] levels;

    [Header("Settings")]
    [SerializeField] private FMODUnity.EventReference audio_LevelComplete;
    [SerializeField] private Color goalColor;
    [SerializeField] private Color normalColor;

    /// <summary>
    /// How far away from a wall to stop shapes during a rotation
    /// </summary>
    [Range(0f, Mathf.PI / 3f)]
    [SerializeField] private float wallPointCollisionVal = 0.42f;
    /// <summary>
    /// How far away from another shape to stop shapes during a rotation
    /// </summary>
    [Range(0f, Mathf.PI / 3f)]
    [SerializeField] private float shapePointCollisionVal = 0.235f;
    [SerializeField] private float ShapeSize = 1f;

    [Header("Internal")]
    [SerializeField, EditorReadOnly] private List<Shape> shapes = new List<Shape>();
    [SerializeField, EditorReadOnly] private int _currentLevelIndex = 0;




    private LevelObject CurrentLevel => levels[_currentLevelIndex];
    private GameObject currentLevelInstance;
    /// <summary>
    /// Tracks if the level manager is in the process of loading a new level
    /// </summary>
    private static bool _loading = false;
    /// <summary>
    /// Tracks if the level is a custom level from the level creator
    /// </summary>
    private bool isLevelOverrided = false;

    public delegate void _onLevelLoad();
    public static _onLevelLoad OnLevelLoad;


    /// <summary>
    /// Calculates if the provided shape is fully within the bounds of goal squares
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsShapeFullyInGoal(Shape s)
    {
        //Fails if the provided shape cant be used in a position (or if the level isnt setup properly)
        if (s.IsSolutionUsable == false || Instance == null || Instance.CurrentLevel == null)
            return false;

        //Use link to count the amount of positions that are not contained in the goal positions set
        return s.PiecePositions.Count(p => Instance.CurrentLevel.goalPositions.Contains(p) == false) == 0;
    }


    private void Start()
    {
        //Set the internal shape size based on the level manager setting
        Shape.ShapeSizeScaler = ShapeSize;

        //Try and detect if there is a custom level that should be played instead of the normal queue
        if (GameObject.Find("CustomLevelOverride") is var overrideLevel && overrideLevel != null && overrideLevel.GetComponent<CustomLevelOverride>() is var c && c != null && c.level != null)
        {
            FirstScreenStuff.SetActive(false);
            levels = new LevelObject[] { c.level };
            isLevelOverrided = true;
        }
        //If its the normal level sequence, enable the first sceen objets
        else
            FirstScreenStuff.SetActive(true);

        //The startup sequence increments the currentl level index by default, so start at negative one -- making the first level index 0
        _currentLevelIndex = -1;
        StartCoroutine(LoadLevelAnimated());


        PlayerController.onPlayerActionDone += CheckCompletion;
    }

    IEnumerator LoadLevelObjects(LevelObject level)
    {
        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);
        if (_currentLevelIndex > 0)
            FirstScreenStuff.SetActive(false);
        currentLevelInstance = Instantiate(Resources.Load<GameObject>("LevelSize_" + ((int)level.Size)));
        gameCam.orthographicSize = currentLevelInstance.GetComponent<CameraStats>().CamSize;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        foreach (var wall in level.additionalWalls)
        {
            TileRenderer.DrawTile(TileRenderer.Channel.Walls, wall.ToTilemap(), Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/LevelWallTile"));

        }
        foreach (var goal in level.goalPositions)
        {
            TileRenderer.DrawTile(TileRenderer.Channel.Special, goal.ToTilemap(), Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/GoalPositionTile"));
        }

        foreach (var shape in level.shapes)
        {
            var obj = MakeShape(shape.isSolutionUsable);
            foreach (var position in shape.positions)
            {
                obj.AddPiece(new Piece() { position = position, isAnchor = shape.anchors.Contains(position), shape = obj });
            }
            obj.IsSolutionUsable = shape.isSolutionUsable;
            obj.UpdateLineVisuals();
        }
        if (FirstScreenStuff.activeSelf)
            titleText.text = "";
        else
            titleText.text = level.name;

        if (isLevelOverrided == false)
            level.Unlock();
        OnLevelLoad?.Invoke();
    }

    bool canLevelSelect = true;
    public void LevelSelectLoad(LevelObject level)
    {
        if (canLevelSelect == false)
            return;
        canLevelSelect = false;
        StartCoroutine(LoadLevelAnimated(level));
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == level)
            {
                _currentLevelIndex = i;
                break;
            }
        }

    }

    IEnumerator ClearLevel()
    {
        while (shapes.Count > 0)
        {
            Destroy(shapes[0].gameObject);
            shapes.RemoveAt(0);
        }
        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);
        yield return new WaitForEndOfFrame();
    }

    public Shape MakeShape(bool isSolutionUsable)
    {
        var newShape = Shape.MakeShapeObj();
        newShape.transform.parent = null;
        newShape.color = isSolutionUsable ? goalColor : normalColor;
        shapes.Add(newShape);
        return newShape;
    }

    private void OnDestroy()
    {
        PlayerController.onPlayerActionDone -= CheckCompletion;
        OnLevelLoad = null;
        //Shape.onShapeMove -= CheckCompletion;
    }

    void CheckCompletion(Piece interactedPiece)
    {
        bool foundInvalid = false;
        foreach (var goalPosition in CurrentLevel.goalPositions)
        {
            if (shapes.Where(s => s.Pieces.Any(p => p.position == goalPosition)) is var found && found.Count() > 0 && found.First() is Shape usedShape)
            {
                //There is a shape on this peice, so now we must validate that this shape has all pieces on a goal position
                if (IsShapeFullyInGoal(usedShape) == false) 
                {
                    foundInvalid = true;
                    return;
                }
            }
            else
            {
                foundInvalid = true;
                return;
            }

            //A goal postion is invalid if
            //  It has no solution viable piece on it
            //  The shape of its viable piece has a piece that is not on a goal position

        }
        if (foundInvalid == false)
        {
            //COMPLETE!
            FMODUnity.RuntimeManager.PlayOneShot(audio_LevelComplete);

            StartCoroutine(LoadLevelAnimated());
        }
    }

    bool canReset = false;
    public void ResetLevel()
    {
        if (canReset == false)
            return;

        canReset = false;
        StartCoroutine(LoadLevelAnimated(CurrentLevel));
    }

    IEnumerator LoadLevelAnimated(LevelObject levelOverride = null)
    {
        _loading = true;
        fadoutImage.gameObject.SetActive(true);
        yield return LerpBlockoutImage(fadoutImage.color.a, 1.0f, 1f - (fadoutImage.color.a / 1f));
        yield return ClearLevel();
        yield return new WaitForSeconds(.1f);
        if (levelOverride == null)
        {
            _currentLevelIndex++;
            if (_currentLevelIndex >= levels.Length || _currentLevelIndex < 0)
            {
                _currentLevelIndex = 0;
                if (isLevelOverrided == false)
                {
                    MenuController.Instance.SetEndScreen();
                    waitingOnEndScreenButton = true;
                    while (waitingOnEndScreenButton) yield return new WaitForEndOfFrame();
                }
            }
            //Debug.Log($"{_currentLevelIndex + 1}/{levels.Length}");
        }
        yield return LoadLevelObjects(levelOverride == null ? CurrentLevel : levelOverride);
        _loading = false; // Do this here so the player can control during the fade in
        yield return LerpBlockoutImage(1.0f, 0.0f, 1f);
        fadoutImage.gameObject.SetActive(false);
        canReset = true;
        canLevelSelect = true;
    }

    IEnumerator LerpBlockoutImage(float alphaFrom, float alphaTo, float time)
    {

        void SetAlpha(float alpha)
        {
            fadoutImage.color = new Color(fadoutImage.color.r, fadoutImage.color.g, fadoutImage.color.b, alpha);

        }
        float timer = time;
        SetAlpha(alphaFrom);

        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            SetAlpha(Mathf.Lerp(alphaTo, alphaFrom, timer / time));
            yield return new WaitForEndOfFrame();
        }
        SetAlpha(alphaTo);
    }
    bool waitingOnEndScreenButton = false;
    public void EndScreenButtonPressed()
    {
        waitingOnEndScreenButton = false;
    }

    public enum CollisionType { None, Wall, Shape }
    /// <summary>
    /// Checks for collisions, ignoring pieces that are part of the current shape
    /// </summary>
    /// <param name="targetShape"></param>
    /// <param name="collisonHexes"></param>
    /// <returns></returns>
    public static CollisionType CheckCollisions(Shape targetShape, Bestagon.Hexagon.Hex[] collisonHexes, out Hex[] hexes)
    {
        CollisionType collisionType = CollisionType.None;
        List<Hex> hitHexes = new List<Hex>();
        foreach (var hex in collisonHexes)
        {
            if (TileRenderer.TileExists(TileRenderer.Channel.Walls, hex.ToTilemap()))
            {
                hitHexes.Add(hex);
                collisionType = CollisionType.Wall;
                continue;
            }

            foreach (var shape in LevelManager.Shapes)
            {
                if (shape == targetShape)
                    continue;
                if (shape.PiecePositions.Contains(hex))
                {
                    hitHexes.Add(hex);
                    if (collisionType == CollisionType.None)
                        collisionType = CollisionType.Shape;
                }
            }

        }

        hexes = hitHexes.ToArray();
        return collisionType;
    }

    public static void MaxAngleProjection(Shape shapeToIgnore, Hex about, Hex startingPosition, bool clockwise, out CollisionType collisionType, out float angle)
    {
        angle = 0f;
        collisionType = CollisionType.None;
        Hex position = startingPosition;
        while (angle < MathF.PI * 2 && collisionType == CollisionType.None)
        {
            var rotated = Hex.Rotate(about, position, clockwise);
            collisionType = LevelManager.CheckCollisions(shapeToIgnore, Hex.RotationSweep(about, position, clockwise).ToArray(), out var collisionHexes);
            if (collisionType != CollisionType.None)
            {
                float closest = float.MaxValue;
                var points = CollisionPoints(shapeToIgnore, collisionHexes);
                (float theta, float r) orignalPolar = AngleSpan.CartesianToPolar(position.UnityPosition() - about.UnityPosition());
                for (int i = 0; i < points.Length; i++)
                {
                    (float theta, float r) pointPolar = AngleSpan.CartesianToPolar(points[i] - about.UnityPosition());
                    float p = Mathf.Abs(Mathf.DeltaAngle(orignalPolar.theta * Mathf.Rad2Deg,  pointPolar.theta* Mathf.Rad2Deg) * Mathf.Deg2Rad);
                    if (p < closest)
                        closest = p;
                    Debug.DrawLine(about.UnityPosition(), points[i], Color.white, 1f);
                }
                angle += closest;
            }
            else
                angle += MathF.PI / 3f;

            //Rotate the position for the next iteration
            position = rotated;
        }
        //Debug.Break();
    }


    public static CollisionType CheckCollision(Shape shapeToIgnore, Hex position)
    {
        if (TileRenderer.TileExists(TileRenderer.Channel.Walls, position.ToTilemap()))
        {
            return CollisionType.Wall;
        }

        foreach (var shape in LevelManager.Shapes)
        {
            if (shape == shapeToIgnore)
                continue;
            if (shape.PiecePositions.Contains(position))
            {
                return CollisionType.Shape;
            }
        }
        return CollisionType.None;
    }


    public static Vector3[] CollisionPoints(Shape targetShape, Bestagon.Hexagon.Hex[] collisonHexes)
    {
        List<Vector3> hits = new List<Vector3>();
        foreach (var hex in collisonHexes)
        {
            float size = -1f;
            if (TileRenderer.TileExists(TileRenderer.Channel.Walls, hex.ToTilemap()))
            {
                size = Instance.wallPointCollisionVal;
            }
            else
            {
                foreach (var shape in LevelManager.Shapes)
                {
                    if (shape == targetShape)
                        continue;
                    if (shape.PiecePositions.Contains(hex))
                    {
                        size = Instance.shapePointCollisionVal;
                        break;
                    }
                }
            }

            if (size >= 0f)
            {
                foreach (Hex.Side item in System.Enum.GetValues(typeof(Hex.Side)))
                {
                    hits.Add(hex.UnityPosition() + (size * new Vector3(Mathf.Cos(item.SidePointRadians().clockwise), Mathf.Sin(item.SidePointRadians().clockwise))));
                }
            }
        }
        return hits.ToArray();
    }

    /// <summary>
    /// Returns the last valid position that does not collide
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Hex HexRaycast(Shape shapeToIgnore, Hex start, Hex.Side dir)
    {
        var ret = start;
        int SafteyIter = 128;
        while (SafteyIter-- > 0)
        {
            var perspective = ret + dir.Offset();
            if (LevelManager.CheckCollisions(shapeToIgnore, new Hex[] { perspective }, out var _) != LevelManager.CollisionType.None)
                break;
            else
                ret = perspective;
        }
        return ret;
    }


    protected override void Destroy()
    {
        //throw new System.NotImplementedException();
    }
}
