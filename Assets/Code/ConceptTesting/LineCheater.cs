using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineCheater : MonoBehaviour
{

    public Transform a;
    public Transform b;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (a == null || b == null)
        {
            lineRenderer.positionCount = 0;

                return;
        }
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0,a.position);
        lineRenderer.SetPosition(1,Vector3.Lerp(a.position, b.position, .5f));
        lineRenderer.SetPosition(2,b.position);
    }
}
