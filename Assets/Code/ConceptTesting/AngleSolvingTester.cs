using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AngleSolvingTester : MonoBehaviour
{
    public LineRenderer arcDrawer;
    public GameObject origin;
    public GameObject angleSpecifier;
    public GameObject[] collisionPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var t = AngleSpan.ValidAnglesBetweenPoints(origin.transform.position, angleSpecifier.transform.position, collisionPoints.Select<GameObject, Vector3>(g => g.transform.position).ToArray());
        Vector3 a = origin.transform.position + PolarToCartesian(t.lower, (angleSpecifier.transform.position - origin.transform.position).magnitude);
        Vector3 b = origin.transform.position + PolarToCartesian(t.upper, (angleSpecifier.transform.position - origin.transform.position).magnitude);
        int x = 34;
        arcDrawer.positionCount = x;
        arcDrawer.SetPosition(0, origin.transform.position);
        arcDrawer.SetPosition(1, a);
        arcDrawer.SetPosition(x - 2, b);
        arcDrawer.SetPosition(x - 1, origin.transform.position);
        for (int i = 2; i < x - 2; i++)
        {
            arcDrawer.SetPosition(i, Vector3.Slerp(a,b, (float)i / (float)x ));
        }
    }
    (float theta, float r) CartesianToPolar(Vector3 vector)
    {
        return (Mathf.Atan2(vector.y, vector.x), Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y)));
    }

    Vector3 PolarToCartesian(float theta, float r)
    {
        return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0f);
    }

}
