using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;

public static class HexExtensions
{

    public static Vector3 UnityPosition(this Hex hex)
    {
        return TileRenderer.GetCenterPosOfTile(TileRenderer.Channel.Debug, hex.ToTilemap());
    }

    /// <summary>
    /// Draws a circle at the hex position
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="innerCirleSize"></param>
    /// <param name="c"></param>
    public static void DebugDrawCircle(this Hex hex, float innerCirleSize, Color c)
    {
        int step = 16;
        float angle = 360f / (float)step;

        Vector3 CirclePoint(float angleDegrees)
        {
            return hex.UnityPosition() + new Vector3(Mathf.Cos(angleDegrees * Mathf.Deg2Rad) * innerCirleSize, Mathf.Sin(angleDegrees * Mathf.Deg2Rad) * innerCirleSize);
        }


        for (int i = 0; i <= step; i++)
            Debug.DrawLine(CirclePoint(i * angle), CirclePoint((i + 1) * angle), c);

    }
}
