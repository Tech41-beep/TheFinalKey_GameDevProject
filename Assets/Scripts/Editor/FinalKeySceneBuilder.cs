using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// One-click setup tool (Editor only).
/// Menu: The Final Key > Build All Scenes (greybox)
/// Creates the Key and Player prefabs, the MainMenu / Game / Win / Lose scenes,
/// wires every script reference and button, and fills Build Settings.
///
/// The Game scene is a GREYBOX placeholder (cube walls + 4x4 maze). Bonsak then
/// brings in the Furnished Cabin asset and replaces / decorates the greybox.
/// </summary>
public static class FinalKeySceneBuilder
{
    private const string ScenesDir = "Assets/Scenes";
    private const string MaterialsDir = "Assets/Materials";
    private const string PrefabsDir = "Assets/Prefabs";

    // Maze layout: 4x4 cells, each 6 m wide. Centre of the maze is (0,0).
    private const float Cell = 6f;
    private const float Origin = -9f;   // centre of cell (0,0)
    private const float WallHeight = 3f;
    private const float WallThickness = 0.4f;
    private const float DoorWidth = 2.5f;

    // Open connections between neighbouring cells: c1, r1, c2, r2 (always "lower" cell first).
    private static readonly int[][] OpenEdges =
    {
        new[] {0,0, 0,1}, new[] {0,1, 0,2}, new[] {0,2, 1,2}, new[] {1,1, 1,2},
        new[] {1,0, 1,1}, new[] {1,0, 2,0}, new[] {2,0, 2,1}, new[] {2,1, 2,2},
        new[] {2,2, 2,3}, new[] {2,3, 3,3}, new[] {0,2, 0,3}, new[] {0,3, 1,3},
        new[] {2,0, 3,0}, new[] {2,1, 3,1}, new[] {3,1, 3,2},
    };

    private static HashSet<string> openSet;

    [MenuItem("The Final Key/Build All Scenes (greybox)")]
    public static void BuildAll()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        openSet = new HashSet<string>();
        foreach (int[] e in OpenEdges) openSet.Add(EdgeKey(e[0], e[1], e[2], e[3]));

        EnsureFolders();
        GameObject keyPrefab = CreateKeyPrefab();
        GameObject playerPrefab = CreatePlayerPrefab();

        BuildMainMenu();
        BuildGame(keyPrefab, playerPrefab);
        BuildResultScene("Win", "YOU ESCAPED!",
            "Congratulations! You collected all 3 keys and escaped the cabin.",
            "Restart", new Color(0.05f, 0.25f, 0.12f));
        BuildResultScene("Lose", "YOU FAILED",
            "You touched the hazard zone. Try again!",
            "Retry", new Color(0.3f, 0.06f, 0.06f));

        EditorBuildSettings.scenes = new[] { "MainMenu", "Game", "Win", "Lose" }
            .Select(n => new EditorBuildSettingsScene(ScenesDir + "/" + n + ".unity", true))
            .ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(ScenesDir + "/MainMenu.unity");

        EditorUtility.DisplayDialog("The Final Key",
            "Done! 4 scenes were created and added to Build Settings.\n\nOpen MainMenu and press Play.", "OK");
    }

    // ------------------------------------------------------------------ folders

    private static void EnsureFolders()
    {
        string[] dirs =
        {
            ScenesDir, MaterialsDir, PrefabsDir + "/Player", PrefabsDir + "/Keys",
            PrefabsDir + "/Environment", "Assets/Models", "Assets/UI", "Assets/Audio",
            "Assets/FurnishedCabin"
        };
        foreach (string d in dirs) Directory.CreateDirectory(d);
        AssetDatabase.Refresh();
    }

    // ------------------------------------------------------------------ prefabs

    private static GameObject CreateKeyPrefab()
    {
        Material gold = GetMaterial("Key_Gold", new Color(1f, 0.82f, 0.1f));

        GameObject root = new GameObject("Key");
        SphereCollider col = root.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.6f;
        root.AddComponent<KeyCollectible>();

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        head.name = "Head";
        head.transform.SetParent(root.transform, false);
        head.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        head.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        head.transform.localScale = new Vector3(0.3f, 0.03f, 0.3f);

        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shaft.name = "Shaft";
        shaft.transform.SetParent(root.transform, false);
        shaft.transform.localPosition = new Vector3(0f, -0.05f, 0f);
        shaft.transform.localScale = new Vector3(0.06f, 0.4f, 0.06f);

        GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tooth.name = "Tooth";
        tooth.transform.SetParent(root.transform, false);
        tooth.transform.localPosition = new Vector3(0.07f, -0.2f, 0f);
        tooth.transform.localScale = new Vector3(0.15f, 0.06f, 0.06f);

        foreach (Transform child in root.transform)
        {
            child.GetComponent<Renderer>().sharedMaterial = gold;
            Object.DestroyImmediate(child.GetComponent<Collider>());
        }

        string path = PrefabsDir + "/Keys/Key.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreatePlayerPrefab()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";

        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = Vector3.zero;
        cc.stepOffset = 0.3f;
        player.AddComponent<PlayerController>();

        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.transform.SetParent(player.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        Camera cam = camGO.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        camGO.AddComponent<AudioListener>();

        CameraController look = camGO.AddComponent<CameraController>();
        SetRef(look, "playerBody", player.transform);

        PlayerInteraction interaction = camGO.AddComponent<PlayerInteraction>();
        SetRef(interaction, "playerCamera", cam);

        string path = PrefabsDir + "/Player/Player.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, path);
        Object.DestroyImmediate(player);
        return prefab;
    }

    // ------------------------------------------------------------------ MainMenu

    private static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateUICamera(new Color(0.07f, 0.07f, 0.1f));
        SceneController sc = new GameObject("SceneController").AddComponent<SceneController>();
        Transform canvas = CreateCanvas("MenuCanvas");
        CreateEventSystem();

        MakeText(canvas, "Title", "THE FINAL KEY", 110, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0, 220), new Vector2(1400, 160), Color.white);
        MakeText(canvas, "Subtitle", "Find 3 keys. Escape the cabin.", 40, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0, 110), new Vector2(1200, 70), new Color(0.8f, 0.8f, 0.85f));

        MakeButton(canvas, "StartButton", "Start Game", new Vector2(0, -20), sc.LoadGame);
        MakeButton(canvas, "ExitButton", "Exit", new Vector2(0, -140), sc.QuitGame);

        EditorSceneManager.SaveScene(scene, ScenesDir + "/MainMenu.unity");
    }

    // ------------------------------------------------------------------ Win / Lose

    private static void BuildResultScene(string sceneName, string title, string message,
        string retryLabel, Color background)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateUICamera(background);
        SceneController sc = new GameObject("SceneController").AddComponent<SceneController>();
        Transform canvas = CreateCanvas(sceneName + "Canvas");
        CreateEventSystem();

        MakeText(canvas, "Title", title, 110, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0, 220), new Vector2(1400, 160), Color.white);
        MakeText(canvas, "Message", message, 40, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(1300, 120), new Color(0.9f, 0.9f, 0.9f));

        MakeButton(canvas, retryLabel + "Button", retryLabel, new Vector2(0, -40), sc.LoadGame);
        MakeButton(canvas, "MainMenuButton", "Main Menu", new Vector2(0, -160), sc.LoadMainMenu);

        EditorSceneManager.SaveScene(scene, ScenesDir + "/" + sceneName + ".unity");
    }

    // ------------------------------------------------------------------ Game

    private static void BuildGame(GameObject keyPrefab, GameObject playerPrefab)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.6f);

        GameObject lightGO = new GameObject("Directional Light");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Material floorMat = GetMaterial("Floor_Wood", new Color(0.45f, 0.3f, 0.18f));
        Material wallMat = GetMaterial("Wall_Beige", new Color(0.82f, 0.76f, 0.64f));
        Material exitMat = GetMaterial("Exit_Green", new Color(0.1f, 0.7f, 0.25f));
        Material hazardMat = GetMaterial("Hazard_Red", new Color(0.9f, 0.1f, 0.1f));

        // --- Environment (greybox) ---
        GameObject env = new GameObject("Environment");
        Block("Floor", env.transform, new Vector3(0, -0.1f, 0), new Vector3(24.8f, 0.2f, 24.8f), floorMat);

        GameObject walls = new GameObject("MazeWalls");
        walls.transform.SetParent(env.transform, false);
        BuildOuterWalls(walls.transform, wallMat);
        BuildMazeWalls(walls.transform, wallMat);

        // --- Hazard: a strip across the middle of cell (1,1). The player must jump over it. ---
        GameObject hazard = Block("Hazard", env.transform, new Vector3(-3f, 0.05f, -3f),
            new Vector3(5.4f, 0.1f, 1.5f), hazardMat);
        hazard.GetComponent<Collider>().isTrigger = true;
        hazard.AddComponent<Hazard>();

        // --- Exit (cell 3,3, north wall) ---
        GameObject exit = new GameObject("Exit");
        exit.transform.position = new Vector3(9f, 1.25f, 10.8f);
        BoxCollider exitCol = exit.AddComponent<BoxCollider>();
        exitCol.isTrigger = true;
        exitCol.size = new Vector3(3f, 2.5f, 2f);
        exit.AddComponent<ExitDoor>();
        GameObject exitVisual = Block("ExitVisual", exit.transform, new Vector3(9f, 1.3f, 11.7f),
            new Vector3(2.4f, 2.6f, 0.2f), exitMat, false);
        exitVisual.transform.SetParent(exit.transform, true);

        // --- Keys (dead ends of the maze) ---
        GameObject keys = new GameObject("Keys");
        PlaceKey(keyPrefab, keys.transform, "Key_1", new Vector3(-3f, 1f, 9f));
        PlaceKey(keyPrefab, keys.transform, "Key_2", new Vector3(9f, 1f, -9f));
        PlaceKey(keyPrefab, keys.transform, "Key_3", new Vector3(9f, 1f, 3f));

        // --- Player ---
        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.transform.position = new Vector3(-9f, 0.95f, -9f);

        // --- GameManager ---
        new GameObject("GameManager").AddComponent<GameManager>();

        // --- HUD ---
        Transform canvas = CreateCanvas("GameUI");
        UIManager ui = canvas.gameObject.AddComponent<UIManager>();

        Text objective = MakeText(canvas, "ObjectiveText", "Objective: Collect 3 Keys and escape", 34,
            TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(30, -30), new Vector2(900, 60), Color.white);
        Text keyCount = MakeText(canvas, "KeyCountText", "Keys: 0/3", 44,
            TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-30, -30), new Vector2(400, 70), new Color(1f, 0.85f, 0.2f));
        Text prompt = MakeText(canvas, "PromptText", "Press E to Collect", 40,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, -120), new Vector2(800, 70), Color.white);
        Text message = MakeText(canvas, "MessageText", "", 42,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 160), new Vector2(1400, 120), new Color(1f, 0.9f, 0.4f));
        MakeText(canvas, "ControlsText", "WASD move   Shift run   Space jump   E interact", 26,
            TextAnchor.LowerLeft, new Vector2(0, 0), new Vector2(30, 30), new Vector2(1000, 50), new Color(1f, 1f, 1f, 0.7f));
        MakeText(canvas, "Crosshair", "+", 40,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(60, 60), new Color(1f, 1f, 1f, 0.8f));

        SetRef(ui, "objectiveText", objective);
        SetRef(ui, "keyCountText", keyCount);
        SetRef(ui, "promptText", prompt);
        SetRef(ui, "messageText", message);

        EditorSceneManager.SaveScene(scene, ScenesDir + "/Game.unity");
    }

    private static void PlaceKey(GameObject prefab, Transform parent, string name, Vector3 position)
    {
        GameObject key = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        key.name = name;
        key.transform.SetParent(parent, false);
        key.transform.position = position;
    }

    // ------------------------------------------------------------------ maze helpers

    private static string EdgeKey(int c1, int r1, int c2, int r2)
    {
        return c1 + "," + r1 + "-" + c2 + "," + r2;
    }

    private static void BuildOuterWalls(Transform parent, Material mat)
    {
        float half = (4 * Cell) / 2f;                 // 12
        float length = 4 * Cell + WallThickness;      // 24.4
        float y = WallHeight / 2f;
        Block("Wall_North", parent, new Vector3(0, y, half), new Vector3(length, WallHeight, WallThickness), mat);
        Block("Wall_South", parent, new Vector3(0, y, -half), new Vector3(length, WallHeight, WallThickness), mat);
        Block("Wall_East", parent, new Vector3(half, y, 0), new Vector3(WallThickness, WallHeight, length), mat);
        Block("Wall_West", parent, new Vector3(-half, y, 0), new Vector3(WallThickness, WallHeight, length), mat);
    }

    private static void BuildMazeWalls(Transform parent, Material mat)
    {
        // Walls between horizontal neighbours (run along Z).
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                float x = Origin + Cell * c + Cell / 2f;
                float z = Origin + Cell * r;
                bool open = openSet.Contains(EdgeKey(c, r, c + 1, r));
                BuildWallSegment(parent, "Wall_V_" + c + "_" + r, new Vector3(x, 0, z), true, open, mat);
            }
        }

        // Walls between vertical neighbours (run along X).
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                float x = Origin + Cell * c;
                float z = Origin + Cell * r + Cell / 2f;
                bool open = openSet.Contains(EdgeKey(c, r, c, r + 1));
                BuildWallSegment(parent, "Wall_H_" + c + "_" + r, new Vector3(x, 0, z), false, open, mat);
            }
        }
    }

    // A wall segment between two cells. If "open", it has a doorway in the middle.
    private static void BuildWallSegment(Transform parent, string name, Vector3 center,
        bool alongZ, bool open, Material mat)
    {
        float y = WallHeight / 2f;

        if (!open)
        {
            Vector3 size = alongZ
                ? new Vector3(WallThickness, WallHeight, Cell + WallThickness)
                : new Vector3(Cell + WallThickness, WallHeight, WallThickness);
            Block(name, parent, center + Vector3.up * y, size, mat);
            return;
        }

        float len = Cell / 2f + WallThickness / 2f - DoorWidth / 2f;   // 1.95
        float offset = DoorWidth / 2f + len / 2f;                       // 2.225
        for (int i = 0; i < 2; i++)
        {
            float sign = i == 0 ? -1f : 1f;
            Vector3 pos = center + Vector3.up * y +
                (alongZ ? new Vector3(0, 0, sign * offset) : new Vector3(sign * offset, 0, 0));
            Vector3 size = alongZ
                ? new Vector3(WallThickness, WallHeight, len)
                : new Vector3(len, WallHeight, WallThickness);
            Block(name + "_Part" + i, parent, pos, size, mat);
        }
    }

    // ------------------------------------------------------------------ generic helpers

    private static GameObject Block(string name, Transform parent, Vector3 worldCenter,
        Vector3 size, Material mat, bool keepCollider = true)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = worldCenter;
        go.transform.localScale = size;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!keepCollider) Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    private static Material GetMaterial(string name, Color color)
    {
        string path = MaterialsDir + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = null;
            if (GraphicsSettings.currentRenderPipeline != null)
                shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void SetRef(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(field);
        if (prop == null)
        {
            Debug.LogError("FinalKeySceneBuilder: field '" + field + "' not found on " + target.name);
            return;
        }
        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ------------------------------------------------------------------ UI helpers

    private static Font GetFont()
    {
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Font.CreateDynamicFontFromOSFont("Arial", 16);
        return f;
    }

    private static void CreateUICamera(Color background)
    {
        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = background;
        camGO.AddComponent<AudioListener>();
    }

    private static Transform CreateCanvas(string name)
    {
        GameObject go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return go.transform;
    }

    private static void CreateEventSystem()
    {
        GameObject go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        UnityEngine.InputSystem.UI.InputSystemUIInputModule module =
            go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        module.AssignDefaultActions();
#else
        go.AddComponent<StandaloneInputModule>();
#endif
    }

    private static Text MakeText(Transform parent, string name, string content, int fontSize,
        TextAnchor align, Vector2 anchor, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Text t = go.AddComponent<Text>();
        t.font = GetFont();
        t.text = content;
        t.fontSize = fontSize;
        t.alignment = align;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    private static Button MakeButton(Transform parent, string name, string label,
        Vector2 anchoredPos, UnityAction onClick)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(420, 90);

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.28f);
        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;

        Text text = MakeText(go.transform, "Label", label, 38, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.white);
        RectTransform lrt = text.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        UnityEventTools.AddPersistentListener(btn.onClick, onClick);
        return btn;
    }
}
