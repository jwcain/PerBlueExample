using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ShapeMeshGenerator))]
public class ShapeMeshTester : MonoBehaviour
{
    [SerializeField] public Bestagon.Hexagon.Hex[] positions;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ShapeMeshGenerator>().BuildMesh(positions);
    }

}
