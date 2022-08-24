using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;

/// <summary>
/// Wrapper for TileRenderer that simplifies drawing via a hex
/// </summary>
public class HexRenderer : Bestagon.Behaviours.ProtectedSceneSingleton<HexRenderer>
{
    /// <summary>
    /// Tracks created tiles for reuse
    /// </summary>
    static Dictionary<Color?, UnityEngine.Tilemaps.Tile> storedTiles = new Dictionary<Color?, UnityEngine.Tilemaps.Tile>();

    public static void DrawTile(TileRenderer.Channel channel, Hex hex, Color? color)
    {
        if (color == null)
        {
            TileRenderer.DrawTile(channel, hex.ToTilemap(), null, true);
            return;
        }
        if (storedTiles.ContainsKey(color) == false)
            storedTiles.Add(color, TileRenderer.GenerateTile(null, color));

        TileRenderer.DrawTile(channel, hex.ToTilemap(), storedTiles[color], true);
    }


    protected override void Destroy()
    {
        storedTiles.Clear();
    }
}
