using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRotationIndicator : MonoBehaviour
{

    [SerializeField] protected GameObject rotationIndicator;
    private static DebugRotationIndicator instance;
    public static void ShowAndUpdateIndicator(Vector3 position, float angleDegrees)
    {
        if (instance == null)
        {
            //Spawn new isntance
            instance = GameObject.Instantiate(Resources.Load<GameObject>("DebugRotationIndicator")).GetComponent<DebugRotationIndicator>();
        }
        instance.gameObject.SetActive(true);
        instance.transform.position = position;
        instance.rotationIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, angleDegrees);
    }

    public static void HideIndicator()
    {
        instance.gameObject.SetActive(false);
    }

}
