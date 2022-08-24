using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Helper class for drawing tiles to a tilemap programatically
/// </summary>
[RequireComponent(typeof(Tilemap))]
public class TileRenderer : MonoBehaviour
{
    /// <summary>
    /// Specifies a a max region for drawing
    /// </summary>
    public static readonly RectInt innerArea = new RectInt(-500, -500, 1000, 1000);

    /// <summary>
    /// Different channels for rendering
    /// </summary>
    public enum Channel { Debug, Player, Background, Shapes, Walls, Special }

    /// <summary>
    /// The channel this TileRenderer renders for
    /// </summary>
    public Channel channel;

    /// <summary>
    /// The internal map of channels to tilemaps
    /// </summary>
    private static readonly Dictionary<Channel, Tilemap> channels = new Dictionary<Channel, Tilemap>();

    /// <summary>
    /// The default tile used for rendering tiles
    /// </summary>
    private static Tile defaultTile = null;

    /// <summary>
    /// Returns the center world position for the specified tilemap position
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector2 GetCenterPosOfTile(Channel channel, Vector2Int position)
    {
        Vector3 t;
        if (channels != null && channels.ContainsKey(channel) && channels[channel] != null)
            t = channels[channel].GetCellCenterWorld((Vector3Int)position);
        else
            t = Object.FindObjectOfType<Tilemap>().GetCellCenterWorld((Vector3Int)position);
        return new Vector2(t.x, t.y);
    }

    /// <summary>
    /// Returns the tile position for the specified world point
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="worldPoint"></param>
    /// <returns></returns>
    public static Vector2Int GetTileForWorldPoint(Channel channel, Vector2 worldPoint)
    {
        Vector3Int ret = channels[channel].WorldToCell(new Vector3(worldPoint.x, worldPoint.y));
        return new Vector2Int(ret.x, ret.y);
    }

    /// <summary>
    /// returns if the specified channel has a tile drawn at the specified position
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static bool TileExists(Channel channel, Vector2Int cell)
    {
        return channels[channel].GetTile((Vector3Int)cell) != null;
    }

    /// <summary>
    /// Returns if the specified channel has been connected
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public static bool HasChannel(Channel channel)
    {
        return channels.ContainsKey(channel);
    }


    private void Start()
    {
        //Register this tilemap for its channel
        if (channels.ContainsKey(channel))
            channels[channel] = GetComponent<Tilemap>();
        else
            channels.Add(channel, GetComponent<Tilemap>());

        GenerateTile();
    }

    private void OnDestroy()
    {
        //This tilemap is no longer rendering for this channel
        if (channels.ContainsKey(channel) && channels[channel] == this)
            channels.Remove(channel);
    }

    /// <summary>
    /// Draws a box AROUND the area specified
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="area"></param>
    /// <param name="tile"></param>
    public static void DrawPerimiter(Channel channel, RectInt area, Tile tile, bool breakBounds = false)
    {
        Debug.Log(area);
        bool warningFlag = false;
        int xStart = area.x - 1;
        int yStart = area.y - 1;
        //Positions for the permiter (broken out for readability)
        int bottomRow = yStart;
        int topRow = yStart + area.height + 1;
        int leftColumn = xStart;
        int rightColumn = xStart + area.width + 1;
        //Draw top and bottom rows
        for (int x = 0; x < area.width + 2; x++)
        {
            if (DrawTile(channel, new Vector2Int(xStart + x, bottomRow), tile, breakBounds) == false)
                warningFlag = true;
            if (DrawTile(channel, new Vector2Int(xStart + x, topRow), tile, breakBounds) == false)
                warningFlag = true;
        }

        //Draw left and right column
        for (int y = 0; y < area.height + 2; y++)
        {
            if (DrawTile(channel, new Vector2Int(leftColumn, yStart + y), tile, breakBounds) == false)
                warningFlag = true;
            if (DrawTile(channel, new Vector2Int(rightColumn, yStart + y), tile, breakBounds) == false)
                warningFlag = true;
        }
        if (warningFlag) Debug.LogWarning("Tried to draw out of bounds");
    }

    /// <summary>
    /// Fills an area with a specific tile
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="area"></param>
    /// <param name="tile"></param>
    public static void DrawArea(Channel channel, RectInt area, Tile tile, bool breakBounds = false)
    {
        bool warningFlag = false;
        for (int x = 0; x < area.width; x++)
        {
            for (int y = 0; y < area.height; y++)
            {
                if (DrawTile(channel, new Vector2Int(area.x + x, area.y + y), tile, breakBounds) == false)
                    warningFlag = true;
            }
        }
        if (warningFlag) Debug.LogWarning("Tried to draw out of bounds");
    }

    /// <summary>
    /// Draws a line from the start position in the direction of the vector
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="start"></param>
    /// <param name="vector"></param>
    /// <param name="tile"></param>
    /// <param name="breakBounds"></param>
    public static void DrawLine(Channel channel, Vector2Int start, Vector2Int vector, Tile tile, bool breakBounds = false)
    {
        bool warningFlag = false;
        int magnitude = (int)vector.magnitude;
        Vector2Int normal = new Vector2Int((vector.x != 0) ? 1 : 0, (vector.y != 0) ? 1 : 0);
        for (int i = 0; i < magnitude; i++)
        {
            if (DrawTile(channel, start + (normal * i), tile, breakBounds) == false)
                warningFlag = true;
        }
        if (warningFlag) Debug.LogWarning("Tried to draw out of bounds");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="pos"></param>
    /// <param name="tile"></param>
    /// <returns>if the tile was drawn in bounds</returns>
    public static bool DrawTile(Channel channel, Vector2Int pos, Tile tile, bool breakBounds = false)
    {
        //if (memo.Contains(pos))
        //				Debug.LogError("Redraw");
        //memo.Add(pos);
        if (breakBounds == false && (pos.x < innerArea.x || pos.y < innerArea.y || pos.x > innerArea.x + innerArea.width - 1 || pos.y > innerArea.y + innerArea.height - 1))
            return false;
        channels[channel].SetTile((Vector3Int)pos, tile);
        return true;
    }

    /// <summary>
    /// Loads a tile from the 'Resources/Tiles' folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Tile GetResourceTile(string name)
    {
        return Resources.Load<Tile>("Tiles/" + name);
    }

    /// <summary>
    /// Genenerates a new Tile at runtime. Will be a copy of DefaultTile unless override values are set
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Tile GenerateTile(Sprite sprite = null, Color? color = null)
    {
        //Load the default tile if it is null
        if (defaultTile == null)
            defaultTile = GetResourceTile("DefaultTile");
        //Create a copy
        Tile gen = ScriptableObject.Instantiate<Tile>(defaultTile);
        //Null check set values
        if (sprite != null)
            gen.sprite = sprite;
        if (color != null)
            gen.color = (Color)color;
        //Return the generated Tile
        return gen;
    }
}
