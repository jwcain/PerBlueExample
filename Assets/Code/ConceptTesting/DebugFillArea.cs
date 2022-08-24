using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;

public class DebugFillArea : MonoBehaviour
{
    Dictionary<Bestagon.Hexagon.Hex.Side, Color> sideToColor = new Dictionary<Bestagon.Hexagon.Hex.Side, Color>() {
        { Bestagon.Hexagon.Hex.Side.Right, Color.blue },
        { Bestagon.Hexagon.Hex.Side.UpRight, Color.cyan },
        { Bestagon.Hexagon.Hex.Side.UpLeft, Color.green },
        { Bestagon.Hexagon.Hex.Side.Left, Color.yellow },
        { Bestagon.Hexagon.Hex.Side.DownLeft, Color.red },
        { Bestagon.Hexagon.Hex.Side.DownRight, Color.magenta }
    };


    public int radius = 5;
    // Start is called before the first frame update
    void Start()
    {

        foreach (var hex in Hex.Area(new Hex(0, 0), radius))
        {
            Color c = Color.white;// Color.Lerp(Color.white, Color.black, 0.5f);
            //HexRenderer.DrawTile(TileRenderer.Channel.Background, hex, c);
            int distance = Hex.Distance(Hex.zero, hex);
            if (Hex.InLine(Hex.zero, hex) == false)
                distance = 0;

            TilemapDebugUtils.AddText(hex.ToTilemap(), hex.ToString(), false, Color.black);


        }

        //HexRenderer.DrawTile(Hex.zero, Color.yellow);


        //for (int i = 2; i < 6; i++)
        //{
        //    Hex hex = (Hex.Side.UpRight.Offset() * i) + (Hex.Side.Left.Offset());
        //    Hex rotation = Hex.Rotate(Hex.zero, hex, true);
        //    List<Hex> list = Hex.RotationSweep(Hex.zero, hex, true);
        //    for (int k = 0; k < list.Count; k++)
        //    {
        //        Color c = Color.Lerp(Color.white, sideToColor[(Hex.Side)i], k / (float)list.Count);
        //        HexRenderer.DrawTile(list[k], c);

        //    }

        //    TilemapDebugUtils.AddText(hex.ToTilemap(), "A", false, Color.black);
        //    TilemapDebugUtils.AddText(rotation.ToTilemap(), "B", false, Color.black);
        //}


    }

}
