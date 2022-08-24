using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;


/// <summary>
/// Helper class used to track displaying an area of hexs on the tilemap
/// </summary>
public class HexAreaDrawer
{
    /// <summary>
    /// The tile to render for this drawer
    /// </summary>
    UnityEngine.Tilemaps.Tile tile;

    bool currentlyDisplayed = false;

    Hex[] lastDrawnPositions = null;


    public HexAreaDrawer(UnityEngine.Tilemaps.Tile tile)
    {
        this.tile = tile;
    }

    /// <summary>
    /// Clears previously drawn positions, sets new positions, and updates draw if currently show
    /// </summary>
    /// <param name="positions"></param>
    public void SetPositions(Hex[] positions)
    {
        if (lastDrawnPositions != null)
            ClearVisuals();

        lastDrawnPositions = positions;

        if (currentlyDisplayed)
            Draw();
    }

    /// <summary>
    /// Displays the area
    /// </summary>
    public void Show()
    {
        currentlyDisplayed = true;
        Draw();
    }

    /// <summary>
    /// Hides this area from being drawn
    /// </summary>
    public void Hide()
    {
        currentlyDisplayed = false;
        ClearVisuals();
    }

    void Draw()
    {
        if (lastDrawnPositions == null)
            return;
        foreach (var hex in lastDrawnPositions)
        {
            TileRenderer.DrawTile(TileRenderer.Channel.Player, hex.ToTilemap(), tile);
        }
    }

    void ClearVisuals()
    {
        if (lastDrawnPositions == null)
            return;

        foreach (var hex in lastDrawnPositions)
        {
            TileRenderer.DrawTile(TileRenderer.Channel.Player, hex.ToTilemap(), null);
        }
    }

}
