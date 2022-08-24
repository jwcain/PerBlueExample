using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;

public class MouseInfo : Bestagon.Behaviours.ProtectedSceneSingleton<MouseInfo>
{
    static UnityEngine.Tilemaps.Tilemap _referenceMap;
    /// <summary>
    /// Dynamically grabbed tilemap used for converting world position to tilemap positions
    /// </summary>
    static UnityEngine.Tilemaps.Tilemap referenceMap
    {
        get
        {
            if (_referenceMap == null)
                _referenceMap = Object.FindObjectOfType<UnityEngine.Tilemaps.Tilemap>(false);
            return _referenceMap;
        }
    }


    /// <summary>
    /// The Hex the mouse is currently in
    /// </summary>
    /// <returns></returns>
    public static Hex Hex()
    {
        return RoundAnyToHex(World());
    }

    /// <summary>
    /// Rounds any vector to a hex
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Hex RoundAnyToHex(Vector3 v)
    {
        return Bestagon.Hexagon.Hex.FromUnityCell(referenceMap.WorldToCell(v));
    }

    /// <summary>
    /// Returns the mouse world position clamped to 2D space
    /// </summary>
    /// <returns></returns>
    public static Vector3 World()
    {
        Vector3 r = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        r.z = 0.0f;
        return r;
    }

    protected override void Destroy()
    {
        _referenceMap = null;
    }
}
