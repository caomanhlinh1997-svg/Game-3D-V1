using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CreatePuzzleHierarchyTool : EditorWindow
{
    [MenuItem("Tools/WordShot/Create PuzzleScene Hierarchy")]
    public static void CreateHierarchy()
    {
        // Root
        GameObject root = new GameObject("PuzzleRoot");

        //----------------------------------------------------------------
        // CANVAS
        //----------------------------------------------------------------
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(root.transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        //----------------------------------------------------------------
        // BACKGROUND
        //----------------------------------------------------------------
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform);
        Image bgImg = bgGO.AddComponent<Image>();
        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        bgImg.color = new Color(0.1f, 0.1f, 0.15f);

        //----------------------------------------------------------------
        // PUZZLE PANEL
        //----------------------------------------------------------------
        GameObject panelGO = new GameObject("PuzzlePanel");
        panelGO.transform.SetParent(canvasGO.transform);
        Image panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.25f);

        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(1000, 500);
        panelRT.anchoredPosition = Vector2.zero;

        //----------------------------------------------------------------
        // SLOTS PARENT
        //----------------------------------------------------------------
        GameObject slotsGO = new GameObject("SlotsParent");
        slotsGO.transform.SetParent(panelGO.transform);
        RectTransform slotsRT = slotsGO.AddComponent<RectTransform>();
        slotsRT.anchorMin = new Vector2(0.5f, 0.75f);
        slotsRT.anchorMax = new Vector2(0.5f, 0.75f);
        slotsRT.anchoredPosition = Vector2.zero;
        slotsRT.sizeDelta = new Vector2(900, 120);

        //----------------------------------------------------------------
        // PIECES PARENT
        //----------------------------------------------------------------
        GameObject piecesGO = new GameObject("PiecesParent");
        piecesGO.transform.SetParent(panelGO.transform);
        RectTransform piecesRT = piecesGO.AddComponent<RectTransform>();
        piecesRT.anchorMin = new Vector2(0.5f, 0.3f);
        piecesRT.anchorMax = new Vector2(0.5f, 0.3f);
        piecesRT.anchoredPosition = Vector2.zero;
        piecesRT.sizeDelta = new Vector2(900, 200);

        //----------------------------------------------------------------
        // CONFIRM BUTTON
        //----------------------------------------------------------------
        GameObject btnGO = new GameObject("BtnConfirm");
        btnGO.transform.SetParent(panelGO.transform);
        Button btn = btnGO.AddComponent<Button>();
        Image btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.6f, 1f);

        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0f);
        btnRT.anchorMax = new Vector2(0.5f, 0f);
        btnRT.anchoredPosition = new Vector2(0, 50);
        btnRT.sizeDelta = new Vector2(250, 80);

        // Text of confirm button
        GameObject btnTextGO = new GameObject("Text");
        btnTextGO.transform.SetParent(btnGO.transform);
        Text btnText = btnTextGO.AddComponent<Text>();
        btnText.text = "CONFIRM";

        // 🔥 FIX FONT HERE
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        btnText.alignment = TextAnchor.MiddleCenter;

        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        //----------------------------------------------------------------
        // WIN PANEL
        //----------------------------------------------------------------
        GameObject winGO = new GameObject("WinPanel");
        winGO.transform.SetParent(canvasGO.transform);
        Image winImg = winGO.AddComponent<Image>();
        winImg.color = new Color(0, 1, 0, 0.25f);
        winGO.SetActive(false);

        RectTransform winRT = winGO.GetComponent<RectTransform>();
        winRT.anchorMin = Vector2.zero;
        winRT.anchorMax = Vector2.one;
        winRT.offsetMin = Vector2.zero;
        winRT.offsetMax = Vector2.zero;

        GameObject winTextGO = new GameObject("Text");
        winTextGO.transform.SetParent(winGO.transform);
        Text winText = winTextGO.AddComponent<Text>();
        winText.text = "YOU WIN!";
        winText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // FIXED
        winText.fontSize = 90;
        winText.alignment = TextAnchor.MiddleCenter;

        RectTransform winTextRT = winTextGO.GetComponent<RectTransform>();
        winTextRT.anchorMin = Vector2.zero;
        winTextRT.anchorMax = Vector2.one;
        winTextRT.offsetMin = Vector2.zero;
        winTextRT.offsetMax = Vector2.zero;

        //----------------------------------------------------------------
        // LOSE PANEL
        //----------------------------------------------------------------
        GameObject loseGO = new GameObject("LosePanel");
        loseGO.transform.SetParent(canvasGO.transform);
        Image loseImg = loseGO.AddComponent<Image>();
        loseImg.color = new Color(1, 0, 0, 0.25f);
        loseGO.SetActive(false);

        RectTransform loseRT = loseGO.GetComponent<RectTransform>();
        loseRT.anchorMin = Vector2.zero;
        loseRT.anchorMax = Vector2.one;
        loseRT.offsetMin = Vector2.zero;
        loseRT.offsetMax = Vector2.zero;

        GameObject loseTextGO = new GameObject("Text");
        loseTextGO.transform.SetParent(loseGO.transform);
        Text loseText = loseTextGO.AddComponent<Text>();
        loseText.text = "WRONG!";
        loseText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // FIXED
        loseText.fontSize = 90;
        loseText.alignment = TextAnchor.MiddleCenter;

        RectTransform loseTextRT = loseTextGO.GetComponent<RectTransform>();
        loseTextRT.anchorMin = Vector2.zero;
        loseTextRT.anchorMax = Vector2.one;
        loseTextRT.offsetMin = Vector2.zero;
        loseTextRT.offsetMax = Vector2.zero;

        //----------------------------------------------------------------
        // MANAGERS
        //----------------------------------------------------------------
        new GameObject("PuzzleManager");
        new GameObject("PuzzleGeneratorRuntime");
        new GameObject("ThemeManager");

        //----------------------------------------------------------------
        Debug.Log("✔ PuzzleScene Hierarchy created successfully (Font FIXED)!");
    }
}
