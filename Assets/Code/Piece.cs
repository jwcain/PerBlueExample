using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;
using System.Linq;

[System.Serializable]
public class Piece
{
    public Shape shape;
    [SerializeField] public Hex position = Hex.zero;
    [SerializeField] public bool isAnchor = false;
    [HideInInspector] public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    public Vector3 VisualPosition => (renderers == null || renderers.Count == 0 ? Vector3.zero : renderers[0].transform.position);

    /// <summary>
    /// Sets up visuals for this shape based on piece adjaceny information
    /// </summary>
    /// <param name="shape"></param>
    public void DrawAsPartOfShape(Shape shape)
    {
        if (renderers == null)
            renderers = new List<SpriteRenderer>();
        while (renderers.Count > 0)
        {
            GameObject.Destroy(renderers[0].gameObject);
            renderers.RemoveAt(0);
        }

        if (isAnchor)
        {
            SpriteRenderer anchorRenderer = MakeRendererInstance(shape, $"{position} Anchor", position.UnityPosition(), "HexShapeSprites/HexShape_000000");
            renderers.Add(anchorRenderer);
            //TODO: make these colors customizable in the editor
            ColorUtility.TryParseHtmlString(shape.IsSolutionUsable ? "#208EA3" : "#8D9F9B", out var c);
            anchorRenderer.color = c;
            anchorRenderer.sortingOrder++;
            anchorRenderer.transform.localScale = Vector3.one * 0.66f * Shape.ShapeSizeScaler;
        }
        Color boarderColor = new Color(0, 0, 0, 0);

        //TODO: Want to make individual sprites for each possible combination (This is probably better done with a shader, with the base textures being overlayed as needed)
        for (int i = 0; i < 6; i++)
        {
            Hex.Side side = (Hex.Side)i;
            if (shape.PiecePositions.Contains(position + side.Offset()))
            {
                string substring;
                switch (side)
                {
                    default:
                        substring = "000000";
                        break;
                    case Hex.Side.Right:
                        substring = "000001";
                        break;
                    case Hex.Side.UpRight:
                        substring = "000010";
                        break;
                    case Hex.Side.UpLeft:
                        substring = "000100";
                        break;
                    case Hex.Side.Left:
                        substring = "001000";
                        break;
                    case Hex.Side.DownLeft:
                        substring = "010000";
                        break;
                    case Hex.Side.DownRight:
                        substring = "100000";
                        break;
                }
                if (MakeRendererInstance(shape, $"{position} -> {side}", position.UnityPosition(), $"HexShapeSprites/HexShape_{substring}") is var r && r != null)
                {
                    var boarder = MakeRendererInstance(shape, $"{position} -> {side} boarder", position.UnityPosition(), $"HexShapeSprites/HexShape_{substring}");
                    boarder.color = boarderColor;
                    boarder.transform.localScale = boarder.transform.localScale * 1.05f;
                    boarder.sortingOrder = -100;
                    renderers.Add(boarder);
                    renderers.Add(r);
                }
            }
        }

        if (MakeRendererInstance(shape, $"{position} Center", position.UnityPosition(), $"HexShapeSprites/HexShape_000000") is var q && q != null)
        {
            var boarder = MakeRendererInstance(shape, $"{position} Center boarder", position.UnityPosition(), $"HexShapeSprites/HexShape_000000");
            boarder.color = boarderColor;
            boarder.transform.localScale = boarder.transform.localScale * 1.05f;
            boarder.sortingOrder = -100;
            renderers.Add(boarder);
            renderers.Add(q);
        }
    }

    public void DestroySelf()
    {
        //Destroy all spawned renderes
        foreach (var item in renderers)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
    private SpriteRenderer MakeRendererInstance(Shape shape, string objName, Vector3 objPosition, string spritePath)
    {
        Sprite sprite = Resources.Load<Sprite>(spritePath);

        if (sprite == null)
        {
            Debug.LogWarning($"{objName} Failed Sprite Grab {spritePath}");
            return null;
        }

        GameObject go = new GameObject();
        go.transform.parent = shape.transform;
        go.transform.position = objPosition;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * Shape.ShapeSizeScaler;
        go.name = objName;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = shape.color;

        return renderer;
    }
}