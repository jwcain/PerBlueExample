using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;


/// <summary>
/// Controls player audio via delegate events
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerFX : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference audio_PickupShape;
    [SerializeField] private FMODUnity.EventReference audio_ShapeSlide;
    [SerializeField] private FMODUnity.EventReference audio_Collision;
    [SerializeField] private FMODUnity.EventReference audio_RotateStall;
    [SerializeField] public FMODUnity.EventReference audio_SolutionShapeSlideIntoPlace;
    [SerializeField] public FMODUnity.EventReference audio_DropPiece;

    [Header("Sliding")]
    FMOD.Studio.EventInstance slideNoiseInstance;
    [SerializeField] AnimationCurve noiseSpeedCurve;
    [SerializeField] float distanceForFullSlideVolume = 0.33f;
    [SerializeField, EditorReadOnly] float usedVolume = 0f;
    [SerializeField] float slideVolumeFallow = 0.33f;
    SimpleMovingAverage mouseMoveAverage = new SimpleMovingAverage(16);
    [SerializeField] float delta;

    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        //Initialize the sliding noise
        slideNoiseInstance = FMODUnity.RuntimeManager.CreateInstance(audio_ShapeSlide);
        slideNoiseInstance.start();
        SetSlideAttributes(Vector3.zero);
        slideNoiseInstance.setVolume(0f);

        PlayerController.onPlayerActionDone += (pieceInteractedWith) => {
            if (LevelManager.IsShapeFullyInGoal(pieceInteractedWith.shape))
                FMODUnity.RuntimeManager.PlayOneShot(audio_SolutionShapeSlideIntoPlace, transform.position);
            else
                FMODUnity.RuntimeManager.PlayOneShot(audio_DropPiece, transform.position);
        };

        PlayerController.onPlayerActionStart += (pieceInteractedWith) => {
            FMODUnity.RuntimeManager.PlayOneShot(audio_PickupShape, playerController.HeldPieceVisualPosition());
        };

        onVisualCollision += (collisionLocation) => {
            FMODUnity.RuntimeManager.PlayOneShot(audio_Collision, collisionLocation);
        };

        playerController.playerRotateHitsEdge.Register((bool old, bool n) =>
        {
            playerController.IsColliding(out var visualCollisonLocation);
            if (n)
                FMODUnity.RuntimeManager.PlayOneShot(audio_RotateStall, visualCollisonLocation);

            return null;
        });

    }

    private void Update()
    {
        UpdateSlideVolume();
    }

    Vector3 lastMousePos;
    void UpdateSlideVolume()
    {
        //Slowly fade out volume if the player is colliding or not holding a piece
        if (playerController.IsHoldingPiece() == false || playerController.IsColliding(out _))
        {
            usedVolume -= slideVolumeFallow * Time.deltaTime;
            SetSlideAttributes(Vector3.zero);
            if (usedVolume < 0f)
                usedVolume = 0f;
        }
        else
        {
            delta = ((MouseInfo.World() - lastMousePos).magnitude * Time.deltaTime) / (distanceForFullSlideVolume * Time.deltaTime);
            SetSlideAttributes((MouseInfo.World() - lastMousePos));
            if (delta > 1f)
                delta = 1f;

            usedVolume = noiseSpeedCurve.Evaluate(mouseMoveAverage.Update(delta));
            if (usedVolume > 1f)
                usedVolume = 1f;
        }
        slideNoiseInstance.setVolume(usedVolume);
        lastMousePos = MouseInfo.World();
    }

    void SetSlideAttributes(Vector3 dir)
    {
        var soundLoc = playerController.HeldPieceVisualPosition();
        slideNoiseInstance.set3DAttributes(new FMOD.ATTRIBUTES_3D()
        {
            position = new FMOD.VECTOR() { x = soundLoc.x, y = soundLoc.y, z = soundLoc.z },
            forward = new FMOD.VECTOR() { x = 1f, y = 0f, z = 0f },
            up = new FMOD.VECTOR() { x = 0f, y = 1f, z = 0f },
            velocity = new FMOD.VECTOR() { x = dir.x, y = dir.y, z = dir.z }
        }); ;
    }
}
