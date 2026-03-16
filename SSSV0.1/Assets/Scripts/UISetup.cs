using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to your Canvas. Automatically positions all UI elements on Start.
/// Can be removed after layout is confirmed.
/// </summary>
public class UISetup : MonoBehaviour
{
    void Start()
    {
        SetupLayout();
    }

    void SetupLayout()
    {
        // PLAYER STATUS - top left
        SetupElement("PlayerStatusText",
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0.35f, 1),
            pivot: new Vector2(0, 1), anchoredPos: new Vector2(10, -10),
            sizeDelta: new Vector2(0, 120));
        StyleText("PlayerStatusText", 14, TextAlignmentOptions.TopLeft, Color.white);

        // AI STATUS - top right
        SetupElement("AIStatusText",
            anchorMin: new Vector2(0.65f, 1), anchorMax: new Vector2(1, 1),
            pivot: new Vector2(1, 1), anchoredPos: new Vector2(-10, -10),
            sizeDelta: new Vector2(0, 120));
        StyleText("AIStatusText", 14, TextAlignmentOptions.TopRight, Color.white);

        // DISTANCE - top center
        SetupElement("DistanceText",
            anchorMin: new Vector2(0.35f, 1), anchorMax: new Vector2(0.65f, 1),
            pivot: new Vector2(0.5f, 1), anchoredPos: new Vector2(0, -10),
            sizeDelta: new Vector2(0, 40));
        StyleText("DistanceText", 20, TextAlignmentOptions.Center, Color.yellow);

        // ROUND INFO - below distance
        SetupElement("RoundInfoText",
            anchorMin: new Vector2(0.35f, 1), anchorMax: new Vector2(0.65f, 1),
            pivot: new Vector2(0.5f, 1), anchoredPos: new Vector2(0, -55),
            sizeDelta: new Vector2(0, 50));
        StyleText("RoundInfoText", 14, TextAlignmentOptions.Center, Color.white);

        // SCORECARD - below round info
        SetupElement("ScorecardText",
            anchorMin: new Vector2(0.35f, 1), anchorMax: new Vector2(0.65f, 1),
            pivot: new Vector2(0.5f, 1), anchoredPos: new Vector2(0, -105),
            sizeDelta: new Vector2(0, 80));
        StyleText("ScorecardText", 12, TextAlignmentOptions.Center, Color.white);

        // COMBAT LOG - middle
        SetupElement("CombatLog",
            anchorMin: new Vector2(0.02f, 0.35f), anchorMax: new Vector2(0.98f, 0.7f),
            pivot: new Vector2(0.5f, 0.5f), anchoredPos: Vector2.zero,
            sizeDelta: Vector2.zero);

        Transform combatLogText = FindDeepChild(transform, "CombatLogText");
        if (combatLogText != null)
        {
            RectTransform rt = combatLogText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(-20, 0);

            TextMeshProUGUI tmp = combatLogText.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.fontSize = 13;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.TopLeft;
                tmp.enableWordWrapping = true;
            }
        }

        Transform content = FindDeepChild(transform, "Content");
        if (content != null)
        {
            ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = content.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
        }

        Image combatLogBg = FindDeepChild(transform, "CombatLog")?.GetComponent<Image>();
        if (combatLogBg != null)
            combatLogBg.color = new Color(0.05f, 0.05f, 0.1f, 0.9f);

        // ACTION COUNT - above move panel
        SetupElement("ActionCountText",
            anchorMin: new Vector2(0.02f, 0.28f), anchorMax: new Vector2(0.98f, 0.34f),
            pivot: new Vector2(0.5f, 0.5f), anchoredPos: Vector2.zero,
            sizeDelta: Vector2.zero);
        StyleText("ActionCountText", 16, TextAlignmentOptions.Center, Color.green);

        // MOVE SELECTION PANEL - bottom
        SetupElement("MoveSelectionPanel",
            anchorMin: new Vector2(0.02f, 0.02f), anchorMax: new Vector2(0.98f, 0.28f),
            pivot: new Vector2(0.5f, 0), anchoredPos: Vector2.zero,
            sizeDelta: Vector2.zero);

        Image panelBg = FindDeepChild(transform, "MoveSelectionPanel")?.GetComponent<Image>();
        if (panelBg != null)
            panelBg.color = new Color(0.08f, 0.08f, 0.15f, 0.95f);

        Transform moveButtonContainer = FindDeepChild(transform, "MoveButtonContainer");
        if (moveButtonContainer != null)
        {
            RectTransform rt = moveButtonContainer.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(-10, -10);

            ScrollRect sr = moveButtonContainer.parent.GetComponent<ScrollRect>();
            if (sr == null) sr = moveButtonContainer.parent.gameObject.AddComponent<ScrollRect>();
            sr.content = rt;
            sr.horizontal = false;
            sr.vertical = true;
            sr.scrollSensitivity = 30;

            GridLayoutGroup grid = moveButtonContainer.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = moveButtonContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(280, 80);
            grid.spacing = new Vector2(8, 8);
            grid.padding = new RectOffset(5, 5, 5, 5);
            grid.constraint = GridLayoutGroup.Constraint.Flexible;

            ContentSizeFitter csf = moveButtonContainer.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = moveButtonContainer.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // CONFIRM BUTTON - above move panel right
        SetupElement("ConfirmActionsButton",
            anchorMin: new Vector2(0.7f, 0.28f), anchorMax: new Vector2(0.98f, 0.34f),
            pivot: new Vector2(1, 0), anchoredPos: Vector2.zero,
            sizeDelta: Vector2.zero);
        StyleButtonText("ConfirmActionsButton", "CONFIRM ACTIONS", 14, new Color(0.1f, 0.5f, 0.1f, 1f));

        // RESTART BUTTON - center
        SetupElement("RestartButton",
            anchorMin: new Vector2(0.35f, 0.45f), anchorMax: new Vector2(0.65f, 0.55f),
            pivot: new Vector2(0.5f, 0.5f), anchoredPos: Vector2.zero,
            sizeDelta: Vector2.zero);
        StyleButtonText("RestartButton", "RESTART FIGHT", 18, new Color(0.5f, 0.1f, 0.1f, 1f));

        // CANVAS BACKGROUND
        Image canvasBg = GetComponent<Image>();
        if (canvasBg == null) canvasBg = gameObject.AddComponent<Image>();
        canvasBg.color = new Color(0.04f, 0.04f, 0.08f, 1f);
        canvasBg.raycastTarget = false;

        Debug.Log("UISetup: Layout complete!");
    }

    void SetupElement(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        Transform t = FindDeepChild(transform, name);
        if (t == null)
        {
            Debug.LogWarning($"UISetup: Could not find '{name}'");
            return;
        }

        RectTransform rt = t.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    void StyleText(string name, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        Transform t = FindDeepChild(transform, name);
        if (t == null) return;

        TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
        if (tmp == null) return;

        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.enableWordWrapping = true;
    }

    void StyleButtonText(string buttonName, string text, float fontSize, Color bgColor)
    {
        Transform t = FindDeepChild(transform, buttonName);
        if (t == null) return;

        Image img = t.GetComponent<Image>();
        if (img != null) img.color = bgColor;

        TextMeshProUGUI tmp = t.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
        }
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
