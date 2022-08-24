using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;
using System.Linq;

public class LevelCreator : Bestagon.Behaviours.ProtectedSceneSingleton<LevelCreator>
{
    public Camera gameCam;
    LevelShapes levelShapes = new LevelShapes();
    List<Hex> additionalWalls = new List<Hex>();
    List<Hex> goalPositions = new List<Hex>();

    Shape selectedShape = null;

    [SerializeField] private TMPro.TMP_Dropdown levelSelectDropdown;
    [SerializeField] private TMPro.TMP_InputField nameInputField;
    [SerializeField] private TMPro.TMP_Text currentModeText;
    [SerializeField, EditorReadOnly] private LevelObject openLevel;

    private LevelObject.LevelSize levelSize = LevelObject.LevelSize.One;
    private GameObject loadedLevelField;
    enum EditingMode
    {
        ERR,
        Piece,
        Wall,
        Goal
    }
    [SerializeField, EditorReadOnly] EditingMode editingMode = EditingMode.ERR;
    // Start is called before the first frame update
    void OnEnable()
    {
        Bestagon.Events.InputEvents.Register(KeyCode.Alpha1, Bestagon.Events.KeyEvent.Down, ChangeToPieceMode);
        Bestagon.Events.InputEvents.Register(KeyCode.Alpha2, Bestagon.Events.KeyEvent.Down, ChangeToGoalMode);
        Bestagon.Events.InputEvents.Register(KeyCode.Alpha3, Bestagon.Events.KeyEvent.Down, ChangeToWallMode);
    }

    private void OnDisable()
    {
        
        //Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Down, OnSelectDown);
        //Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, WhilePaintHold);
    }

    object ChangeToWallMode()
    {
        if (editingMode == EditingMode.Wall)
            return null;
        ExitCurrentMode();
        editingMode = EditingMode.Wall;
        currentModeText.text = System.Enum.GetName(typeof(EditingMode), editingMode);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Pressed, DrawWall);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, EraseWall);
        return null;
    }

    object ChangeToGoalMode()
    {
        if (editingMode == EditingMode.Goal)
            return null;
        ExitCurrentMode();
        editingMode = EditingMode.Goal;
        currentModeText.text = System.Enum.GetName(typeof(EditingMode), editingMode);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Pressed, DrawGoal);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, EraseGoal);
        return null;
    }

    object ChangeToPieceMode()
    {
        if (editingMode == EditingMode.Piece)
            return null;
        ExitCurrentMode();
        editingMode = EditingMode.Piece;
        currentModeText.text = System.Enum.GetName(typeof(EditingMode), editingMode);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Down, OnSelectDown);
        Bestagon.Events.InputEvents.Register(KeyCode.Space, Bestagon.Events.KeyEvent.Down, OnToggleAnchor);
        Bestagon.Events.InputEvents.Register(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, WhilePaintHold);
        Bestagon.Events.InputEvents.Register(KeyCode.S, Bestagon.Events.KeyEvent.Down, ToggleSelectedSoltionUsable);
        return null;
    }
    object ToggleSelectedSoltionUsable()
    {
        if (selectedShape == null)
            return null;
        selectedShape.IsSolutionUsable = !selectedShape.IsSolutionUsable;

        selectedShape.color = selectedShape.IsSolutionUsable ? Color.yellow : Color.black;
        selectedShape.color = Color.Lerp(selectedShape.color, Color.white, .5f);
        selectedShape.UpdateLineVisuals();
        return null;
    }
    object ExitCurrentMode()
    {
        switch (editingMode)
        {
            default:
            case EditingMode.ERR:
                break;
            case EditingMode.Piece:
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Down, OnSelectDown);
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, WhilePaintHold);
                Bestagon.Events.InputEvents.Deregister(KeyCode.Space, Bestagon.Events.KeyEvent.Down, OnToggleAnchor);
                break;
            case EditingMode.Wall:
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Pressed, DrawWall);
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, EraseWall);
                break;
            case EditingMode.Goal:
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse0, Bestagon.Events.KeyEvent.Pressed, DrawGoal);
                Bestagon.Events.InputEvents.Deregister(KeyCode.Mouse1, Bestagon.Events.KeyEvent.Pressed, EraseGoal);
                break;
        }
        return null;
    }

    object DrawWall()
    {
        if (IsOverUI() == false)
            TryDrawWallAt(MouseInfo.Hex());
        return null;
    }

    void TryDrawWallAt(Hex h)
    {
        if (additionalWalls.Contains(h) || goalPositions.Contains(h) || levelShapes.HexHasShape(h, out _) || TileRenderer.TileExists(TileRenderer.Channel.Walls, h.ToTilemap()))
        {
            //Debug.Log("SKIP");
            return;
        }
        additionalWalls.Add(h);
        TileRenderer.DrawTile(TileRenderer.Channel.Walls, h.ToTilemap(), Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/LevelWallTile"));
    }

    object EraseWall()
    {
        if (IsOverUI() == false)
            TryEraseWall(MouseInfo.Hex());
        return null;
    }

    void TryEraseWall(Hex h)
    {
        if (additionalWalls.Contains(h) == false || TileRenderer.HasChannel(TileRenderer.Channel.Walls) ==false)
            return;
        additionalWalls.Remove(h);
        TileRenderer.DrawTile(TileRenderer.Channel.Walls, h.ToTilemap(), null);
    }

    object DrawGoal()
    {
        if (IsOverUI() == false)
            TryDrawGoal(MouseInfo.Hex());
        return null;
    }

    void TryDrawGoal(Hex h)
    {
        if (goalPositions.Contains(h) || additionalWalls.Contains(h) || TileRenderer.TileExists(TileRenderer.Channel.Walls, h.ToTilemap()))
            return;
        goalPositions.Add(h);
        TileRenderer.DrawTile(TileRenderer.Channel.Special, h.ToTilemap(), Resources.Load<UnityEngine.Tilemaps.Tile>("Tiles/GoalPositionTile"));   
    }

    object EraseGoal()
    {
        if (IsOverUI() == false)
            TryEraseGoal(MouseInfo.Hex());
        return null;
    }
    void TryEraseGoal(Hex h)
    {
        if (goalPositions.Contains(h) == false || TileRenderer.HasChannel(TileRenderer.Channel.Special) == false)
            return;
        goalPositions.Remove(h);
        TileRenderer.DrawTile(TileRenderer.Channel.Special, h.ToTilemap(), null);
    }

    void DeselectShape()
    {
        if (selectedShape == null)
            return;
        selectedShape.color = selectedShape.IsSolutionUsable ? Color.yellow : Color.black;
        selectedShape.UpdateLineVisuals();
        selectedShape = null;
    }

    void SelectNewShape(Shape shape)
    {
        DeselectShape();
        selectedShape = shape;
        selectedShape.color = Color.Lerp(selectedShape.color, Color.white, 0.2222f);
        selectedShape.UpdateLineVisuals();
    }

    bool IsOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    object OnToggleAnchor()
    {
        if (selectedShape == null || selectedShape.Pieces == null || selectedShape.Pieces.Count() <= 0)
            return null;

        if (selectedShape.Pieces.Where(p => p.position == MouseInfo.Hex()) is var matchingPieces && matchingPieces !=null && matchingPieces.Count() > 0 && matchingPieces.First() is var piece && piece != null)
        {
            piece.isAnchor = !piece.isAnchor;
            selectedShape.UpdateLineVisuals();
        }

        return null;
    }

    object OnSelectDown()
    {
        if (IsOverUI())
            return null;
        if (levelShapes.HexHasShape(MouseInfo.Hex(), out var hitShape))
        {
            if (selectedShape == hitShape)
                DeselectShape();
            else
                SelectNewShape(hitShape);
        }

        return null;
    }

    object WhilePaintHold()
    {
        if (IsOverUI())
            return null;

        if (HexBounded(MouseInfo.Hex()) == false)
            return null;

        if (additionalWalls.Contains(MouseInfo.Hex()) || goalPositions.Contains(MouseInfo.Hex()))
            return null;

        bool didHitAShape = levelShapes.HexHasShape(MouseInfo.Hex(), out var hitShape);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (didHitAShape == selectedShape)
            {
                if (selectedShape == null)
                    return null;
                selectedShape.RemovePieceAtHex(MouseInfo.Hex());
                selectedShape.UpdateLineVisuals();
                if (selectedShape.Pieces.Count() <= 0)
                {
                    levelShapes.DestroyShape(selectedShape);
                }
            }
        }
        else if (didHitAShape == false)
        {
            if (selectedShape == null)
                SelectNewShape(levelShapes.MakeEditorShape());
            selectedShape.AddPiece(new Piece() { position = MouseInfo.Hex(), isAnchor = false, shape = selectedShape });
            levelShapes.UpdateVisuals();

        }
        return null;
    }

    public void Play()
    {
        if (openLevel == null)
            return;
        Save();

        GameObject.Find("CustomLevelOverride").GetComponent<CustomLevelOverride>().level = openLevel;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void Start()
    {
        PopulateLevelsList();
        LoadLevelSize(0);
    }

    public void Save()
    {
        if (openLevel == null)
            return;
        openLevel.shapes = new LevelObject.SerializedShape[levelShapes.Count()];
        openLevel.additionalWalls = additionalWalls.ToArray();
        openLevel.goalPositions = goalPositions.ToArray();
        openLevel.Size = levelSize;
        int i = 0;
        foreach (Shape editorShape in levelShapes.GetShapes())
        {
            var dataShape = new LevelObject.SerializedShape();
            dataShape.positions = editorShape.PiecePositions.ToArray();
            dataShape.anchors = editorShape.Anchors.Select(a => a.position).ToArray();
            dataShape.isSolutionUsable = editorShape.IsSolutionUsable;
            openLevel.shapes[i++] = dataShape;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(openLevel);
        Debug.Log("Save: " + openLevel.name);
#endif
    }


    public IEnumerator LoadOverTime()
    {
        string levelName = levelSelectDropdown.options[levelSelectDropdown.value].text;
        var loadedLevel = Resources.Load<LevelObject>("Levels/" + levelName);
        if (loadedLevel == null)
            throw new System.Exception("Dropdown desync");
        levelShapes.Clear();
        additionalWalls.Clear();
        goalPositions.Clear();
        openLevel = loadedLevel;
        yield return LoadLevelSizeAsync((int)loadedLevel.Size);
        foreach (var levelLoadedShape in loadedLevel.shapes)
        {
            var newShape = levelShapes.MakeEditorShape();
            foreach (var position in levelLoadedShape.positions)
            {
                newShape.AddPiece(new Piece() { position = position, isAnchor = levelLoadedShape.anchors.Contains(position), shape = selectedShape });
            }
            newShape.IsSolutionUsable = levelLoadedShape.isSolutionUsable;
            if (newShape.IsSolutionUsable)
                newShape.color = Color.yellow;
            newShape.UpdateLineVisuals();
        }

        yield return new WaitForEndOfFrame();
        while (TileRenderer.HasChannel(TileRenderer.Channel.Walls) == false) yield return new WaitForEndOfFrame();
        foreach (var wall in loadedLevel.additionalWalls)
        {
            TryDrawWallAt(wall);
        }
        yield return new WaitForEndOfFrame();
        while (TileRenderer.HasChannel(TileRenderer.Channel.Special) == false) yield return new WaitForEndOfFrame();
        foreach (var goal in loadedLevel.goalPositions)
        {
            TryDrawGoal(goal);
        }

        ChangeToPieceMode();
        for (int i = 0; i < levelSelectDropdown.options.Count; i++)
        {
            if (levelSelectDropdown.options[i].text.Equals(openLevel.name))
            {
                levelSelectDropdown.SetValueWithoutNotify(i);
                break;
            }
        }
        _loadSpamBlock = false;
        Debug.Log("Loading Complete");
    }
    bool _loadSpamBlock = false;
    public void Load()
    {
        if (_loadSpamBlock == true)
            return;
        _loadSpamBlock = true;
        StartCoroutine(LoadOverTime());
    }

    private void PopulateLevelsList()
    {
        var levels = Resources.LoadAll<LevelObject>("Levels/");
        levelSelectDropdown.ClearOptions();
        levelSelectDropdown.AddOptions(new List<string>(levels.Select(lo => lo.name).ToArray()));
    }

    bool _loadSizeAntiSpam = false;
    public void LoadLevelSize(int size)
    {
        if (_loadSizeAntiSpam)
            return;
        _loadSizeAntiSpam = true;
        StartCoroutine(LoadLevelSizeAsync(size));
    }

    private IEnumerator LoadLevelSizeAsync(int size)
    {
        if (loadedLevelField != null)
            Destroy(loadedLevelField);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        loadedLevelField = Instantiate(Resources.Load<GameObject>("LevelSize_" + size));
        gameCam.orthographicSize = loadedLevelField.GetComponent<CameraStats>().CamSize;
        levelSize = (LevelObject.LevelSize)size;
        levelSelectDropdown.SetValueWithoutNotify(size);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _loadSizeAntiSpam = false;
    }


    bool HexBounded(Hex hex)
    {
        return !TileRenderer.TileExists(TileRenderer.Channel.Walls, hex.ToTilemap());
    }

    protected override void Destroy()
    {
        //throw new System.NotImplementedException();
    }

    class LevelShapes
    {
        private List<Shape> shapes = new List<Shape>();
        public Shape MakeEditorShape()
        {
            var newShape = Shape.MakeShapeObj();
            newShape.transform.parent = LevelCreator.Instance.transform;
            newShape.color = Color.black;
            shapes.Add(newShape);
            return newShape;
        }

        public IEnumerable GetShapes() => shapes;

        public void DestroyShape(Shape shape)
        {
            if (shapes.Contains(shape))
                shapes.Remove(shape);
            Destroy(shape.gameObject);
        }
        public void Clear()
        {
            while (shapes.Count > 0)
            {
                DestroyShape(shapes[0]);
            }
        }

        public int Count()
        {
            return shapes.Count;
        }
        public void UpdateVisuals()
        {
            foreach (var shape in shapes)
            {
                shape.UpdateLineVisuals();
            }
        }

        public bool HexHasShape(Hex hex, out Shape shape)
        {
            foreach (var testShape in shapes)
            {
                if (testShape.PiecePositions.Contains(hex))
                {
                    shape = testShape;
                    return true;
                }
            }
            shape = null;
            return false;
        }
    }
}
