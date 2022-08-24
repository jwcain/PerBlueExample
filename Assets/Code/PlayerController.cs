using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Bestagon.Hexagon;

public class PlayerController : MonoBehaviour
{

    public delegate void PieceEvent(Piece pieceInteractedWith);
    public delegate void OnVisualCollision(Vector3 collisionLocation);

    public static PieceEvent onPlayerActionDone;
    public static PieceEvent onPlayerActionStart;
    public static OnVisualCollision onVisualCollision;

    /// <summary>
    /// Tracks if the player is making a collision happen
    /// </summary>
    public Bestagon.Events.Monitor<bool> playerCollisionMonitor = new Bestagon.Events.Monitor<bool>();

    /// <summary>
    /// Tracks if the player's rotation has hit an edge
    /// </summary>
    public Bestagon.Events.Monitor<bool> playerRotateHitsEdge = new Bestagon.Events.Monitor<bool>();

    /// <summary>
    /// The piece the player clicked on to interact with
    /// </summary>
    Piece heldPiece = null;
    /// <summary>
    /// The anchor the shape will rotate around
    /// </summary>
     Piece mainAnchor = null;
    /// <summary>
    /// The other anchor on the shape (that is not being rotated around)
    /// </summary>
    Piece secondAnchor = null;

    [Header("Rotation")]
    [SerializeField, EditorReadOnly] Hex.Side anchorSide = Hex.Side.Right;
    [SerializeField, EditorReadOnly] Hex.Side[] validCounterClockwiseRotations;
    [SerializeField, EditorReadOnly] Hex.Side[] validClockwiseRotations;

    [Header("Slide")]
    /// <summary>
    /// Tracks how close the piece is alowed to get to a collision on a slide before stopping
    /// </summary>
    [SerializeField] float maxCollisionDistPercent = 1f;

    [Header("Visuals")]
    [SerializeField] LineRenderer holdLideIndicator;
    [SerializeField] LineRenderer shapeSlideIndicatorLine;
    [SerializeField] int holdLineSampleRate = 16;

    enum PlayerState { Free, Rotating, Sliding }
    [Header("Other")]
    [EditorReadOnly, SerializeField] PlayerState playerState = PlayerState.Free;

    [SerializeField] float piecePickupRange = .5f;
    /// <summary>
    /// How long to wait before the collision indicators are shown to the player
    /// Avoids flicker them on and off if the player makes small collisions while moving quickly
    /// </summary>
    [SerializeField] float timeUntilIndicatorIsShownForSlider;

    //Collision information
    private Vector3 visualCollisonLocation;
    HexAreaDrawer collisionDrawer;
    HexAreaDrawer validPathDrawer;
    Bestagon.Events.Monitor<float> collisionIndicatorTimer = new Bestagon.Events.Monitor<float>((float x, float y) => { return Mathf.Abs(x-y) < float.Epsilon; });
    Bestagon.Events.Monitor<bool> heldShapeIsCollidingWithWall = new Bestagon.Events.Monitor<bool>();

    #region Unity
    private void Start()
    {
        collisionDrawer = new HexAreaDrawer(Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/CollisionTile"));
        validPathDrawer = new HexAreaDrawer(Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/PathTile"));

        LevelManager.OnLevelLoad += OnLevelLoad;
        heldShapeIsCollidingWithWall.Value = false;

        collisionIndicatorTimer.Value = 0.0f;
        collisionIndicatorTimer.Register((float from, float to) => { 
            if (from >= 0.0f && to <= 0.0f && heldShapeIsCollidingWithWall.Value == true)
            {
                collisionDrawer.Show();
                validPathDrawer.Show();
            }
            return null;
        });

        heldShapeIsCollidingWithWall.Register((bool from, bool to) => {
            if (to)
            {
                collisionIndicatorTimer.Value = timeUntilIndicatorIsShownForSlider;
            }
            else
            {
                collisionDrawer.Hide();
                validPathDrawer.Hide();
            }
            return null;
        });


        playerCollisionMonitor.Register((bool old, bool n) => {
            if (n && heldPiece != null)
                onVisualCollision?.Invoke(visualCollisonLocation);
            return null; 
        });
    }



    // Update is called once per frame
    void Update()
    {
        //Dont update while the level manager is loading
        if (LevelManager.Loading)
            return;

        //Switch to various states
        switch (playerState)
        {
            default:
            case PlayerState.Free:
                HandleFreeState();
                break;
            case PlayerState.Rotating:
                HandleRotatingState();
                break;
            case PlayerState.Sliding:
                HandleSlidingState();
                break;
        }

        //Draw a helper line to show the player what they are holding onto
        if (heldPiece != null)
        {
            DrawLineToHeldPiece();
        }
        else
        {
            ClearLineToHeldPiece();
        }

        //Decay the collision timer if it is above 0
        if (collisionIndicatorTimer.Value > 0.0f)
            collisionIndicatorTimer.Value -= Time.deltaTime;
    }
    #endregion

    #region Data Access
    public bool IsHoldingPiece()
    {
        return heldPiece != null;
    }
    public bool IsColliding(out Vector3 visualPosition)
    {
        visualPosition = visualCollisonLocation;
        return playerCollisionMonitor.Value;
    }
    public Vector3 HeldPieceVisualPosition()
    {
        if (heldPiece == null)
            return Vector3.zero;
        return heldPiece.VisualPosition;
    }
    #endregion

    #region Delegate Listeners
    public void OnLevelLoad()
    {
        //Force the player to free mode when the level is loaded
        switch (playerState)
        {
            default:
            case PlayerState.Free:
                break;
            case PlayerState.Rotating:
                ExitRotatingState();
                break;
            case PlayerState.Sliding:
                ExitSlidingState();
                break;
        }
    }
    #endregion

    #region Free State
    void HandleFreeState() {
        if (Input.GetMouseButtonDown(0) && TryGrabPiece(false))
        {
            playerState = PlayerState.Sliding;
            heldPiece.shape.transform.position = heldPiece.position.UnityPosition();
            heldPiece.shape.UpdateLineVisuals();
            this.transform.position = heldPiece.position.UnityPosition();
            DrawSlideLine();
            return;
        }
        if (Input.GetMouseButtonDown(1) && TryGrabPiece(true))
        {
            EnterRotatingState();
            return;
        }
    }

    bool TryGrabPiece(bool anchorOnly = false)
    {
        if (heldPiece != null || MenuController.MenuOpen)
            return false;

        Hex mouseHover = MouseInfo.Hex();
        foreach (var shape in LevelManager.Shapes)
        {
            foreach (var piece in anchorOnly ? shape.Anchors : shape.Pieces)
            {
                if (piece.position.Equals(mouseHover))
                {
                    //We found the piece we are trying to pickup, but is it close enough to the anchor positon?
                    if ((MouseInfo.World() - piece.position.UnityPosition()).magnitude > piecePickupRange)
                    {
                        break;
                    }

                    heldPiece = piece;
                    mainAnchor = shape.Anchors.First(p => p != heldPiece);
                    secondAnchor = shape.Anchors.First(p => p != mainAnchor);
                    Hex.GetRelativeSide(mainAnchor.position, secondAnchor.position, out anchorSide, out _);
                    onPlayerActionStart?.Invoke(heldPiece);
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region General Visual Code
    private void ClearLineToHeldPiece()
    {
        holdLideIndicator.positionCount = 0;
    }

    void DrawLineToHeldPiece()
    {
        holdLideIndicator.positionCount = holdLineSampleRate;
        holdLideIndicator.SetPosition(0, MouseInfo.World());
        holdLideIndicator.SetPosition(holdLineSampleRate - 1, HeldPieceVisualPosition());
        for (int i = 1; i < holdLineSampleRate - 1; i++)
            holdLideIndicator.SetPosition(i, Vector3.Lerp(MouseInfo.World(), HeldPieceVisualPosition(), (float)i / (float)holdLineSampleRate));
    }

    void ResetShapeVisuals()
    {
        if (heldPiece == null)
            return;
        heldPiece.shape.transform.position = Vector3.zero;
        heldPiece.shape.transform.localRotation = Quaternion.identity;
        heldPiece.shape.UpdateLineVisuals();
        PlayerRotationRegionDisplay.UpdateValidity(new Hex.Side[] { }, (0f, 0f));
    }
    #endregion

    #region Sliding

    void DrawSlideLine()
    {
        Hex startHex = LevelManager.HexRaycast(secondAnchor.shape, mainAnchor.position, anchorSide.Inverse());
        Hex endHex = LevelManager.HexRaycast(secondAnchor.shape, secondAnchor.position, anchorSide);
        shapeSlideIndicatorLine.positionCount = holdLineSampleRate;
        Vector3 startPosition = startHex.UnityPosition();
        Vector3 endPosition = endHex.UnityPosition();
        shapeSlideIndicatorLine.SetPosition(0, startPosition);
        shapeSlideIndicatorLine.SetPosition(holdLineSampleRate - 1, endPosition);
        for (int i = 1; i < holdLineSampleRate - 1; i++)
            shapeSlideIndicatorLine.SetPosition(i, Vector3.Lerp(startPosition, endPosition, (float)i / (float)holdLineSampleRate));
    }
    void ClearSlideLine()
    {
        shapeSlideIndicatorLine.positionCount = 0;
    }
    void ExitSlidingState()
    {
        playerState = PlayerState.Free;
        ResetShapeVisuals();
        ClearSlideLine();
        onPlayerActionDone?.Invoke(heldPiece);
        heldShapeIsCollidingWithWall.Value = false;
        collisionDrawer.Hide();
        validPathDrawer.Hide();
        collisionDrawer.SetPositions(null);
        validPathDrawer.SetPositions(null);

        heldPiece = null;
    }
    void HandleSlidingState() {
        if (heldPiece != null && Input.GetMouseButtonUp(0))
        {
            ExitSlidingState();
            return;
        }
        var projectedPoint = ProjectPointOnDirection(heldPiece.position.UnityPosition(), anchorSide.Offset().UnityPosition(), MouseInfo.World(), out _);
        heldPiece.shape.transform.position = projectedPoint;
        var RoundedHex = MouseInfo.RoundAnyToHex(projectedPoint);

        Hex.RoundToSide(heldPiece.position,  projectedPoint, out var side);
        var positionsAfterSlide = heldPiece.shape.PositionsAfterSlide(side);
        LevelManager.CollisionType collisionInDirection = LevelManager.CheckCollisions(heldPiece.shape, positionsAfterSlide, out var lastCollisionHexes);

        if (collisionInDirection != LevelManager.CollisionType.None)
        {
            collisionDrawer.SetPositions(lastCollisionHexes);
            validPathDrawer.SetPositions(positionsAfterSlide.Where(p => lastCollisionHexes.Contains(p) == false && heldPiece.shape.PiecePositions.Contains(p) == false).ToArray());
            var testPosition = (side.Offset().UnityPosition() * (maxCollisionDistPercent  * (collisionInDirection == LevelManager.CollisionType.Wall ? 1f : 2f)));
            //Clamp the visuals if the limit is closer
            if ((projectedPoint - (Vector2)heldPiece.position.UnityPosition()).magnitude > testPosition.magnitude)
            {
                heldShapeIsCollidingWithWall.Value = true;
                heldPiece.shape.transform.position = heldPiece.position.UnityPosition() + testPosition;
                visualCollisonLocation = heldPiece.position.UnityPosition() + (side.Offset().UnityPosition() / (collisionInDirection == LevelManager.CollisionType.Wall ? 2f : 1f));
                playerCollisionMonitor.Value = true;
            }
            else
            {
                heldShapeIsCollidingWithWall.Value = false;
                playerCollisionMonitor.Value = false;
            }
        }
        else if (RoundedHex != heldPiece.position)//and implicitly there is no collision
        {
            heldShapeIsCollidingWithWall.Value = false;
            heldPiece.shape.Slide(side, false);
        }
    }

    /// <summary>
    /// Returns the closest point on a ray to the specified position. Out specifies the distance 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="freePoint"></param>
    /// <returns></returns>
    Vector2 ProjectPointOnDirection(Vector2 origin, Vector2 direction, Vector2 freePoint, out float dist)
    {
        if (direction.magnitude < float.Epsilon)
        {
            dist = 0f;
            return origin;
        }

        dist = (Vector2.Dot(freePoint - origin, direction) / direction.magnitude);
        return origin + (direction.normalized * dist);
    }

    #endregion

    #region Rotating State

    void SetShapeVisualsForRotation()
    {
        if (heldPiece == null || mainAnchor == null)
            return;
        heldPiece.shape.transform.position = mainAnchor.position.UnityPosition();
        heldPiece.shape.transform.localRotation = Quaternion.identity;
        heldPiece.shape.UpdateLineVisuals();
    }

    void ExitRotatingState()
    {
        ResetShapeVisuals();
        playerState = PlayerState.Free;
        onPlayerActionDone?.Invoke(heldPiece);
        heldShapeIsCollidingWithWall.Value = false;
        collisionDrawer.Hide();
        validPathDrawer.Hide();
        collisionDrawer.SetPositions(null);
        validPathDrawer.SetPositions(null);
        heldPiece = null;
    }
    void HandleRotatingState()
    {
        if (heldPiece != null && Input.GetMouseButtonUp(1))
        {
            ExitRotatingState();
            return;
        }
        PlayerRotationRegionDisplay.MouseInRegion(MouseInfo.World(), out var clampedCursorWorld, out var onThetaEdge, out var _, out var clockwiseEdge);
        playerRotateHitsEdge.Value = onThetaEdge;
        float theta = Mathf.Atan2(clampedCursorWorld.y - mainAnchor.position.UnityPosition().y, clampedCursorWorld.x - mainAnchor.position.UnityPosition().x);



        Hex.RoundToSide(mainAnchor.position, clampedCursorWorld, out var roundedSide);
        if (roundedSide != anchorSide)
        {
            void RotateHeld(bool clockwise)
            {
                if (heldPiece == null || mainAnchor == null)
                    return;
                heldPiece.shape.Rotate(mainAnchor.position, clockwise);
                Hex.GetRelativeSide(mainAnchor.position, heldPiece.position, out anchorSide, out _);
                SetShapeVisualsForRotation();
            }

            if (validClockwiseRotations.Contains(roundedSide))
            {
                RotateHeld(true);
            }
            else if (validCounterClockwiseRotations.Contains(roundedSide))
            {
                RotateHeld(false);
            }

        }

        if (onThetaEdge)
        {
            var positionsAfterSlide = heldPiece.shape.ShapeRotationalSweep(mainAnchor.position, clockwiseEdge);
            LevelManager.CheckCollisions(heldPiece.shape, positionsAfterSlide, out var lastCollisionHexes);
            collisionDrawer.SetPositions(lastCollisionHexes);
            validPathDrawer.SetPositions(positionsAfterSlide.Where(p => lastCollisionHexes.Contains(p) == false && heldPiece.shape.PiecePositions.Contains(p) == false).ToArray());
            heldShapeIsCollidingWithWall.Value = true;
        }
        else
        {
            collisionDrawer.Hide();
            validPathDrawer.Hide();
            heldShapeIsCollidingWithWall.Value = false;
        }

        mainAnchor.shape.transform.rotation = Quaternion.Euler(0f, 0f, (theta - anchorSide.Radians()) * Mathf.Rad2Deg);
    }
    void EnterRotatingState()
    {
        playerState = PlayerState.Rotating;
        SetShapeVisualsForRotation();
        PlayerRotationRegionDisplay.SetPosition(mainAnchor.position.UnityPosition());
        PlayerRotationRegionDisplay.SetOuterCircleSize((heldPiece.position.UnityPosition() - mainAnchor.position.UnityPosition()).magnitude);
        validClockwiseRotations = heldPiece.shape.ValidRotationSidesInDirection(mainAnchor, heldPiece, true);
        validCounterClockwiseRotations = heldPiece.shape.ValidRotationSidesInDirection(mainAnchor, heldPiece, false);

        //Combine the clockwise and counterclockwise valid rotation positions
        var validSides = new List<Hex.Side>(validClockwiseRotations);
        for (int i = 0; i < validCounterClockwiseRotations.Length; i++)
        {
            if (validSides.Contains(validCounterClockwiseRotations[i]) == false)
                validSides.Add(validCounterClockwiseRotations[i]);
        }


        PlayerRotationRegionDisplay.UpdateValidity(validSides.ToArray(), CalculateVisualRotationSpace(validSides));
    }
    (float lower, float upper) CalculateVisualRotationSpace(List<Hex.Side> validSides)
    {
        (float theta, float r) armAgnle = AngleSpan.CartesianToPolar(secondAnchor.position.UnityPosition() - mainAnchor.position.UnityPosition());

        float lowerBound = 0f;
        float upperBound = 0f;
        if (validSides.Count != 6 && validSides.Count != 0)
        {
            float clockwiseOffset = float.MaxValue;
            float counterOffset = float.MaxValue;
            foreach (Piece piece in mainAnchor.shape.Pieces)
            {
                if (piece == mainAnchor)
                    continue;
                //Lower is Clockwise
                {
                    LevelManager.MaxAngleProjection(mainAnchor.shape, mainAnchor.position, piece.position, true, out var hitResult, out var possibleAngle);
                    if (hitResult != LevelManager.CollisionType.None && clockwiseOffset > possibleAngle)
                        clockwiseOffset = possibleAngle;
                }
                //Upper is Counterclockwise
                {
                    LevelManager.MaxAngleProjection(mainAnchor.shape, mainAnchor.position, piece.position, false, out var hitResult, out var possibleAngle);
                    if (hitResult != LevelManager.CollisionType.None && counterOffset > possibleAngle)
                        counterOffset = possibleAngle;
                }
            }
            lowerBound = armAgnle.theta - clockwiseOffset;
            upperBound = armAgnle.theta + counterOffset;

            while (lowerBound < 0.0f)
                lowerBound += Mathf.PI * 2;
            while (upperBound < 0.0f)
                upperBound += Mathf.PI * 2;
            while (lowerBound > Mathf.PI * 2)
                lowerBound -= Mathf.PI * 2;
            while (upperBound > Mathf.PI * 2)
                upperBound -= Mathf.PI * 2;
        }
        return (lowerBound, upperBound);
    }
    #endregion

}
