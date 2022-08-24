using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Help : MonoBehaviour
{
    [SerializeField] GameObject lineA;
    [SerializeField] GameObject lineB;
    [SerializeField] GameObject point;

    [SerializeField] LineRenderer line_AB;
    [SerializeField] LineRenderer line_PA;
    [SerializeField] LineRenderer line_PC;

    // Update is called once per frame
    void Update()
    {
        line_AB.positionCount = 2;
        line_AB.SetPosition(0, lineA.transform.position);
        line_AB.SetPosition(1, lineB.transform.position);

        line_PA.positionCount = 2;
        line_PA.SetPosition(0, lineA.transform.position);
        line_PA.SetPosition(1, point.transform.position);

        line_PC.positionCount = 2;
        line_PC.SetPosition(0, point.transform.position);
        line_PC.SetPosition(1, ProjectPointOnDirection(lineA.transform.position, lineB.transform.position - lineA.transform.position, point.transform.position));
    }

    Vector2 Perp(Vector2 v)
    {
        return new Vector2(-v.x, v.y);
    }

    /// <summary>
    /// Returns the closest point on a ray to the specified position
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="freePoint"></param>
    /// <returns></returns>
    Vector2 ProjectPointOnDirection(Vector2 origin, Vector2 direction, Vector2 freePoint)
    {
        if (direction.magnitude < float.Epsilon)
            return origin;
        return origin + (direction.normalized * (Vector2.Dot(freePoint - origin, direction) / direction.magnitude));
    }

    float calc(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a,b) / b.magnitude;
    }
}
