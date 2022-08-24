using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Debug class for drawing text positioned at specific locations
/// </summary>
public class TilemapDebugUtils : Bestagon.Behaviours.ProtectedSceneSingleton<TilemapDebugUtils>
{

    private static Dictionary<Vector2Int, List<Sprite>> iconObjs = new Dictionary<Vector2Int, List<Sprite>>();
    private static Dictionary<Vector2Int, TMPro.TMP_Text> textObjs = new Dictionary<Vector2Int, TMPro.TMP_Text>();
    private static GameObject tileTextReference = null;

    public static void AddText(Vector2Int location, string text, bool append = false, Color? color = null)
    {
        //Check if the text object already exists
        if (textObjs.ContainsKey(location) == false)
        {
            // If not, we need to make one
            //First get the prefab if it doesnt exist
            if (tileTextReference == null)
                tileTextReference = Resources.Load<GameObject>("TileText");
            // make a new one
            TMPro.TMP_Text newObj = Instantiate(tileTextReference).GetComponent<TMPro.TMP_Text>();
            //Set its positition
            newObj.transform.SetParent(Instance.transform, true);
            newObj.transform.position = TileRenderer.GetCenterPosOfTile(TileRenderer.Channel.Debug, location);
            textObjs.Add(location, newObj);
        }

        if (color != null)
            textObjs[location].faceColor = (Color)color;

        if (append)
            textObjs[location].text += text;
        else
            textObjs[location].text = text;
    }

    public static void AddSprite(Vector2Int location, Sprite sprite)
    {
        throw new System.NotImplementedException();
    }

    public static void RemoveSprite(Vector2Int location, Sprite sprite)
    {
        throw new System.NotImplementedException();
    }

    public static void ClearSprites(Vector2Int location)
    {
        throw new System.NotImplementedException();
    }

    public static void ClearTile(Vector2Int location)
    {
        AddText(location, "");
        ClearSprites(location);
    }

    protected override void Destroy()
    {
        textObjs = new Dictionary<Vector2Int, TMPro.TMP_Text>();
        iconObjs = new Dictionary<Vector2Int, List<Sprite>>();
        //Do not need to reset since this will always be the same object
        //tileTextReference = null;
    }
}
