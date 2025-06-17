using UnityEngine;
using UnityEditor;
using System.Text;

public class LevelEditorWindow : EditorWindow
{
    private LevelData currentLevel;
    private Vector2 scrollPosition;
    private int selectedTileIndex = 0;
    private string[] tileLabels;
    private char[,] gridData;
    private bool isDragging = false;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);
        
        // Level Data selection
        currentLevel = (LevelData)EditorGUILayout.ObjectField("Current Level", currentLevel, typeof(LevelData), false);
        
        if (currentLevel == null)
        {
            EditorGUILayout.HelpBox("Select or create a LevelData asset to start editing.", MessageType.Info);
            if (GUILayout.Button("Create New Level"))
            {
                CreateNewLevel();
            }
            return;
        }        // Check for tile database
        if (currentLevel.tileDatabase == null)
        {
            EditorGUILayout.HelpBox("Level needs a Tile Database! Assign one or create a new one.", MessageType.Error);
            if (GUILayout.Button("Create Tile Database"))
            {
                CreateTileDatabase();
            }
            return;
        }

        EditorGUILayout.Space();
        
        // Level properties
        currentLevel.levelName = EditorGUILayout.TextField("Level Name", currentLevel.levelName);
        currentLevel.width = EditorGUILayout.IntSlider("Width", currentLevel.width, 5, 50);
        currentLevel.height = EditorGUILayout.IntSlider("Height", currentLevel.height, 5, 50);
        
        EditorGUILayout.Space();
        
        // Rotation controls toggle        // Tile palette
        DrawTilePalette();
        
        EditorGUILayout.Space();
        
        // Visual grid editor
        DrawGridEditor();
        
        EditorGUILayout.Space();
        
        // Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Grid"))
        {
            ClearGrid();
        }
        if (GUILayout.Button("Apply to Layout String"))
        {
            ApplyGridToLayoutString();
        }
        if (GUILayout.Button("Load from Layout String"))
        {
            LoadGridFromLayoutString();
        }
        EditorGUILayout.EndHorizontal();
          // Layout string (for manual editing)        EditorGUILayout.LabelField("Layout String (Manual Edit):", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Use: . (grass), M (mountain), ~ (water), T (tree), G (goal), S (start)");
        currentLevel.levelLayout = EditorGUILayout.TextArea(currentLevel.levelLayout, GUILayout.Height(100));
        
        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(currentLevel);
        }
    }    void DrawTilePalette()
    {
        if (currentLevel.tileDatabase == null)
        {
            EditorGUILayout.HelpBox("No tile database assigned.", MessageType.Warning);
            return;
        }

        var allTiles = currentLevel.tileDatabase.GetAllTiles();
        if (allTiles == null || allTiles.Length == 0)
        {
            EditorGUILayout.HelpBox("No tiles defined in the database.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("Tile Palette:", EditorStyles.boldLabel);
        
        // Create labels if needed
        if (tileLabels == null || tileLabels.Length != allTiles.Length)
        {
            tileLabels = new string[allTiles.Length];
            for (int i = 0; i < allTiles.Length; i++)
            {
                var tile = allTiles[i];
                if (tile != null)
                {
                    tileLabels[i] = $"'{tile.character}' - {tile.tileName}";
                }
                else
                {
                    tileLabels[i] = "NULL";
                }
            }
        }
        
        // Tile selection toolbar
        selectedTileIndex = GUILayout.Toolbar(selectedTileIndex, tileLabels, GUILayout.Height(30));
        
        // Clamp selection index
        selectedTileIndex = Mathf.Clamp(selectedTileIndex, 0, allTiles.Length - 1);
        
        if (selectedTileIndex >= 0 && selectedTileIndex < allTiles.Length && allTiles[selectedTileIndex] != null)
        {
            var selectedTile = allTiles[selectedTileIndex];
            EditorGUILayout.LabelField($"Selected: '{selectedTile.character}' - {selectedTile.tileName}");        }
    }

    void DrawGridEditor()
    {
        if (currentLevel == null) return;
        
        // Initialize grid if needed
        if (gridData == null || gridData.GetLength(0) != currentLevel.width || gridData.GetLength(1) != currentLevel.height)
        {
            InitializeGrid();
            LoadGridFromLayoutString();
        }
        
        EditorGUILayout.LabelField("Visual Grid Editor:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Click to place tiles, drag to paint multiple tiles");
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        try
        {
            Event e = Event.current;
            
            // Draw grid
            for (int y = currentLevel.height - 1; y >= 0; y--) // Reverse Y for proper display
            {
                EditorGUILayout.BeginHorizontal();
                
                for (int x = 0; x < currentLevel.width; x++)
                {
                    char currentChar = gridData[x, y];
                    string buttonText = currentChar == '\0' ? "." : currentChar.ToString();
                    
                    // Color code tiles
                    Color originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = GetTileColor(currentChar);
                    
                    if (GUILayout.Button(buttonText, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        PlaceTileAt(x, y);
                    }
                    
                    GUI.backgroundColor = originalColor;
                    
                    // Handle dragging
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    if (e.type == EventType.MouseDrag && lastRect.Contains(e.mousePosition))
                    {
                        PlaceTileAt(x, y);
                        isDragging = true;
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (Event.current.type == EventType.MouseUp)
            {
                isDragging = false;
            }
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }
    }    Color GetTileColor(char tileChar)
    {
        // Advanced color system - now enabled!
        if (currentLevel != null && currentLevel.tileDatabase != null)
        {
            var definition = currentLevel.tileDatabase.GetTileDefinition(tileChar);
            if (definition != null)
            {
                return definition.editorColor;
            }
        }
        
        // Fallback simple color coding based on character
        switch (tileChar)
        {
            case '.': return Color.green;        // Grass
            case 'M': return Color.gray;         // Mountain
            case '~': return Color.blue;         // Water
            case 'R': return Color.cyan;         // River
            case 'T': return Color.green * 0.7f; // Tree
            case 'G': return Color.yellow;       // Goal
            case 'S': return Color.magenta;      // Start
            case 'B': return Color.white;        // Bridge
            case 'C': return new Color(0.5f, 0.3f, 0.1f); // Cliff
            case 'H': return new Color(0.6f, 0.8f, 0.4f); // Hill
            default: return Color.white;
        }
    }void PlaceTileAt(int x, int y)
    {
        if (currentLevel?.tileDatabase == null) return;
        
        var allTiles = currentLevel.tileDatabase.GetAllTiles();        if (selectedTileIndex < 0 || selectedTileIndex >= allTiles.Length) return;
        if (allTiles[selectedTileIndex] == null) return;
        if (gridData == null) return;
        
        gridData[x, y] = allTiles[selectedTileIndex].character;
    }

    // Advanced rotation - commented out for now
    /*
    void RotateTileAt(int x, int y)
    {
        if (!showRotationControls || rotationData == null) return;
        if (currentLevel?.tileDatabase == null) return;
        
        char tileChar = gridData[x, y];
        var definition = currentLevel.tileDatabase.GetTileDefinition(tileChar);
        if (definition == null || !definition.canRotate) return;
        
        rotationData[x, y] = (rotationData[x, y] + 1) % definition.maxRotations;
    }
    */

    void InitializeGrid()
    {
        gridData = new char[currentLevel.width, currentLevel.height];
        // rotationData = new int[currentLevel.width, currentLevel.height]; // Advanced feature
    }

    void ClearGrid()
    {
        if (gridData != null)
        {
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    gridData[x, y] = '\0';
                    // if (rotationData != null) rotationData[x, y] = 0; // Advanced feature
                }
            }
        }
    }

    void ApplyGridToLayoutString()
    {
        if (gridData == null) return;
        
        StringBuilder sb = new StringBuilder();
        // StringBuilder rotSb = new StringBuilder(); // Advanced feature
        
        for (int y = currentLevel.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < currentLevel.width; x++)
            {
                char c = gridData[x, y];
                sb.Append(c == '\0' ? '.' : c);
                
                // Advanced rotation support - commented out
                /*
                if (showRotationControls && rotationData != null)
                {
                    rotSb.Append(rotationData[x, y].ToString());
                }
                */
            }
            if (y > 0)
            {
                sb.AppendLine();
                // if (showRotationControls) rotSb.AppendLine(); // Advanced feature
            }
        }
        
        currentLevel.levelLayout = sb.ToString();
        // if (showRotationControls) currentLevel.rotationLayout = rotSb.ToString(); // Advanced feature
        
        EditorUtility.SetDirty(currentLevel);
    }

    void LoadGridFromLayoutString()
    {
        if (string.IsNullOrEmpty(currentLevel.levelLayout) || gridData == null) return;
        
        string[] lines = currentLevel.levelLayout.Split('\n');
        // string[] rotLines = null; // Advanced feature
        
        // Advanced rotation loading - commented out
        /*
        if (showRotationControls && !string.IsNullOrEmpty(currentLevel.rotationLayout))
        {
            rotLines = currentLevel.rotationLayout.Split('\n');
        }
        */
        
        for (int y = 0; y < currentLevel.height && y < lines.Length; y++)
        {
            string line = lines[y].Trim();
            // string rotLine = rotLines != null && y < rotLines.Length ? rotLines[y].Trim() : ""; // Advanced
            
            for (int x = 0; x < currentLevel.width && x < line.Length; x++)
            {
                gridData[x, currentLevel.height - 1 - y] = line[x]; // Reverse Y
                
                // Advanced rotation loading - commented out
                /*
                if (rotationData != null && x < rotLine.Length && char.IsDigit(rotLine[x]))
                {
                    rotationData[x, currentLevel.height - 1 - y] = rotLine[x] - '0';
                }
                */
            }
        }
    }    void CreateTileDatabase()
    {
        var database = ScriptableObject.CreateInstance<TileDatabase>();
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Tile Database",
            "TileDatabase",
            "asset",
            "Choose location for tile database");
            
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(database, path);
            AssetDatabase.SaveAssets();
            
            currentLevel.tileDatabase = database;
            EditorUtility.SetDirty(currentLevel);
            
            Selection.activeObject = database;
        }
    }

    void CreateNewLevel()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New Level",
            "NewLevel",
            "asset",
            "Choose location for new level");
            
        if (!string.IsNullOrEmpty(path))
        {
            LevelData newLevel = CreateInstance<LevelData>();
            newLevel.levelName = "New Level";
            newLevel.width = 10;
            newLevel.height = 10;
            newLevel.levelLayout = "MMMMMMMMMM\nM........M\nM........M\nM...S....M\nM........M\nM....G...M\nM........M\nM........M\nM........M\nMMMMMMMMMM";
            
            AssetDatabase.CreateAsset(newLevel, path);
            AssetDatabase.SaveAssets();
            
            currentLevel = newLevel;
            Selection.activeObject = newLevel;
        }
    }
}
