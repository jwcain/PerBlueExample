using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;
using System.Linq;


/// <summary>
/// Draws the allowed region for a player to rotate a piece
/// </summary>
public class PlayerRotationRegionDisplay : Bestagon.Behaviours.ProtectedSceneSingleton<PlayerRotationRegionDisplay>
{
    [Header("Settings")]
    [SerializeField] private float outerBufferSize = 1.2f;
    [EditorReadOnly, SerializeField] private float outerSize;
    [EditorReadOnly, SerializeField] private float innerSize;
    [SerializeField] private float boundryLineWidth;
    [SerializeField] private float indicatorLineWidth;
    [SerializeField] private Color boundryColor;
    [SerializeField] private Color indicatorColor;
    [SerializeField] private float sideIndicatorBuffer = .1f;

    [Header("Connections")]
    [SerializeField] LineRenderer mainLine;
    [SerializeField] LineRenderer backupLine;
    [SerializeField] int shapeSampleRate = 64;
    [SerializeField] Hex.Side[] validSides = null;
    [EditorReadOnly, SerializeField] float regionMin = 0f, regionMax = Mathf.PI * 2;
    [SerializeField] LineRenderer[] sideIndicators;

    /// <summary>
    /// Sets the size of the outer circle, and sets inner size to half that
    /// </summary>
    /// <param name="anchorSpan"></param>

    public static void SetOuterCircleSize(float anchorSpan)
    {
        Instance.outerSize = anchorSpan + Instance.outerBufferSize;
        Instance.innerSize = Mathf.Max(Instance.outerBufferSize, anchorSpan / 2);
        UpdateVisuals();
    }

    public static void SetPosition(Vector3 postition)
    {
        Instance.transform.position = postition + new Vector3(0, 0, -1);
        UpdateVisuals();
    }

    /// <summary>
    /// Checks if the players mouse is inside the region and returns the closest legal position
    /// </summary>
    /// <param name="mouseWorld"></param>
    /// <param name="closestLegal"></param>
    /// <param name="onThetaEdge"></param>
    /// <param name="onDistEdge"></param>
    /// <param name="clockWiseEdge"></param>
    /// <returns></returns>
    public static bool MouseInRegion(Vector3 mouseWorld, out Vector3 closestLegal, out bool onThetaEdge, out bool onDistEdge, out bool clockWiseEdge)
    {
        //Shift to unit space
        mouseWorld -= Instance.transform.position;
        //Convert to Polar
        //Ensure it is within min and max and outer and inner, otherwise
        bool modified = false;
        onThetaEdge = false;
        clockWiseEdge = false;
        onDistEdge = false;
        float theta = Mathf.Atan2(mouseWorld.y, mouseWorld.x);
        if (new AngleSpan(Instance.regionMin, Instance.regionMax).Inside(theta, out var closestBound) == false)
        {
            modified = true;
            theta = closestBound;
            clockWiseEdge = Mathf.Abs(Instance.regionMin - closestBound) < float.Epsilon;
            onThetaEdge = true;
        }

        float r = Mathf.Sqrt((mouseWorld.x * mouseWorld.x) + (mouseWorld.y * mouseWorld.y));
        //DebugTextManager.Draw("RadialRegion_Polar", mouseWorld, $"0:{theta * Mathf.Rad2Deg:00.00} | R:{r:00.00}");

        if (r < Instance.innerSize)
        {
            modified = true;
            r = Instance.innerSize;
            onDistEdge = true;
        }
        else if (r > Instance.outerSize)
        {
            modified = true;
            r = Instance.outerSize;
            onDistEdge = true;
        }
        if (modified)
            closestLegal =  new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)) * r;
        else
            closestLegal = mouseWorld;
        //Leave unit Space
        closestLegal += Instance.transform.position;
        return modified == false;
    }

    /// <summary>
    /// Loads settings into all lines
    /// </summary>
    void UpdateLineSettings()
    {
        foreach (var line in new LineRenderer[] { mainLine, backupLine })
        {
            line.startColor = boundryColor;
            line.endColor = boundryColor;
            line.startWidth = boundryLineWidth;
            line.endWidth = boundryLineWidth;
        }
    }

    static void UpdateVisuals()
    {
        if (Instance.validSides == null || Instance.validSides.Length == 0)
        {
            foreach (var line in new LineRenderer[] { Instance.mainLine, Instance.backupLine })
            {
                line.positionCount = 0;
            }
        }
        else if (Instance.validSides.Length >= 6)
        {
            Instance.DrawFullCircle();
        }
        else
        {
            Instance.DrawPartialCircle();
        }
        Instance.UpdateSideIndicators();
        Instance.UpdateLineSettings();
    }

    /// <summary>
    /// Updates the valid sides of the rotatio region, and sets the max visual rotation drawn for the region
    /// </summary>
    /// <param name="validSides"></param>
    /// <param name="visualBounds"></param>
    public static void UpdateValidity(Hex.Side[] validSides, (float lower, float upper) visualBounds)
    {
        
        Instance.validSides = validSides;
        //No valid sides
        if (validSides == null || validSides.Length == 0)
        {
            //Draw nothing
            foreach (var line in new LineRenderer[] { Instance.mainLine, Instance.backupLine })
            {
                line.positionCount = 0;
            }
        }
        //Make the intentionally naive asumption that a length of six is all sides (meaning, we dont ensure all sides are actually in that list)
        else if (validSides.Length >= 6)
        {
            Instance.DrawFullCircle();
            Instance.regionMin = 0f;
            Instance.regionMax = Mathf.PI * 2;
        }
        else
        {
            Instance.regionMin = visualBounds.lower;
            Instance.regionMax = visualBounds.upper;
            Instance.DrawPartialCircle();
        }
        Instance.UpdateSideIndicators();
        Instance.UpdateLineSettings();
    }

    void DrawFullCircle()
    {
        backupLine.loop = true;
        mainLine.loop = true;
        SetLineToArc(GetCircleArc(0, Mathf.PI * 2, innerSize), backupLine);
        SetLineToArc(GetCircleArc(0, Mathf.PI * 2, outerSize), mainLine);

    }

    void SetLineToArc(Vector3[] arc, LineRenderer line)
    {
        if (arc == null || arc.Length < 2)
        {
            line.positionCount = 0;
            line.SetPositions(new Vector3[] { });
            return;
        }

        line.positionCount = arc.Length;
        line.SetPositions(arc);
    }

    Vector3[] GetCircleArc(float startRad, float endRad, float dist)
    {
        if (Mathf.Abs(endRad - startRad) < float.Epsilon)
            return new Vector3[] { };
        while (endRad < startRad)
            endRad += Mathf.PI * 2;
        int pointCount = shapeSampleRate * validSides.Length;
        Vector3[] circleArc = new Vector3[pointCount];
        float arcSize = endRad - startRad;
        float stepRad = arcSize / (float)pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            circleArc[i] = this.transform.position + (new Vector3(Mathf.Cos(startRad + stepRad * i), Mathf.Sin(startRad + stepRad * i)) * dist);
        }
        circleArc[0] = this.transform.position + (new Vector3(Mathf.Cos(startRad), Mathf.Sin(startRad)) * dist);
        circleArc[circleArc.Length - 1] = this.transform.position + (new Vector3(Mathf.Cos(endRad), Mathf.Sin(endRad)) * dist);
        return circleArc;
    }

    private void DrawPartialCircle()
    {
        mainLine.loop = true;
        backupLine.positionCount = 0;


        Vector3[] outerArc = GetCircleArc(regionMin, regionMax, outerSize);
        Vector3[] innerArc = GetCircleArc(regionMin, regionMax, innerSize);
        List<Vector3> allpoints = new List<Vector3>();

        allpoints.AddRange(outerArc);
        allpoints.AddRange(innerArc.Reverse());

        mainLine.positionCount = allpoints.Count;
        mainLine.SetPositions(allpoints.ToArray());
    }


    /// <summary>
    /// Draws the 'notches' displaying which sides the shape can land on
    /// </summary>
    private void UpdateSideIndicators()
    {
        for (int i = 0; i < validSides.Length; i++)
        {
            sideIndicators[i].loop = false;
            sideIndicators[i].startWidth = indicatorLineWidth;
            sideIndicators[i].endWidth = indicatorLineWidth;
            sideIndicators[i].startColor = indicatorColor;
            sideIndicators[i].endColor = indicatorColor;
            sideIndicators[i].positionCount = 2;
            sideIndicators[i].SetPosition(0, this.transform.position + new Vector3(Mathf.Cos(validSides[i].Radians()), Mathf.Sin(validSides[i].Radians())) * (innerSize + sideIndicatorBuffer));
            sideIndicators[i].SetPosition(1, this.transform.position + new Vector3(Mathf.Cos(validSides[i].Radians()), Mathf.Sin(validSides[i].Radians())) * (outerSize - sideIndicatorBuffer));
        }
        for (int i = validSides.Length; i <= 5; i++)
        {
            sideIndicators[i].positionCount = 0;
        }
    }


    protected override void Destroy()
    {
        //throw new System.NotImplementedException();
    }
}
