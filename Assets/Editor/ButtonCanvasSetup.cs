using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// ButtonCanvas 계층구조 자동 생성 스크립트.
/// 메뉴: Tools > Setup > Build ButtonCanvas Hierarchy
/// </summary>
public static class ButtonCanvasSetup
{
    [MenuItem("Tools/Setup/Build ButtonCanvas Hierarchy")]
    public static void BuildButtonCanvasHierarchy()
    {
        // ButtonCanvas 찾기
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Canvas buttonCanvas = null;
        foreach (var c in canvases)
            if (c.name == "ButtonCanvas") { buttonCanvas = c; break; }

        if (buttonCanvas == null)
        {
            Debug.LogError("[ButtonCanvasSetup] 씬에서 ButtonCanvas를 찾을 수 없습니다.");
            return;
        }

        RectTransform canvasRect = buttonCanvas.GetComponent<RectTransform>();

        // ── 1. MaximizedButtons 그룹 ─────────────────────────────────
        GameObject maximizedButtons = GetOrCreate("MaximizedButtons", canvasRect);
        SetFullStretch(maximizedButtons.GetComponent<RectTransform>() ?? maximizedButtons.AddComponent<RectTransform>());

        // 최소화 버튼 (우측 드래그 → 최소화, MaximizedScreenController 연결)
        CreateButton(maximizedButtons.transform, "MinimizeButton", "≡",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-44f, -10f), new Vector2(44f, 44f));

        // 토글 버튼 (클릭 통과 전환)
        CreateButton(maximizedButtons.transform, "ToggleButton_Max", "⊙",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-92f, -10f), new Vector2(44f, 44f));

        // 설정 버튼
        CreateButton(maximizedButtons.transform, "SettingsButton", "⚙",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-140f, -10f), new Vector2(44f, 44f));

        // 게임 종료 버튼
        GameObject quitBtn = CreateButton(maximizedButtons.transform, "QuitButton", "✕",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-188f, -10f), new Vector2(44f, 44f));
        AddQuitListener(quitBtn);

        // ── 2. MinimizedButtons 그룹 ─────────────────────────────────
        GameObject minimizedButtons = GetOrCreate("MinimizedButtons", canvasRect);
        SetFullStretch(minimizedButtons.GetComponent<RectTransform>() ?? minimizedButtons.AddComponent<RectTransform>());

        // 최대화 버튼 (좌측 드래그 → 메인화면, MinimizedScreenController 연결)
        CreateButton(minimizedButtons.transform, "MaximizeButton", "≡",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-10f, -10f), new Vector2(44f, 44f));

        // 토글 버튼
        CreateButton(minimizedButtons.transform, "ToggleButton_Min", "⊙",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-58f, -10f), new Vector2(44f, 44f));

        // 초기 활성화 상태: MaximizedButtons만 활성, MinimizedButtons 비활성
        maximizedButtons.SetActive(true);
        minimizedButtons.SetActive(false);

        EditorUtility.SetDirty(buttonCanvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(buttonCanvas.gameObject.scene);

        Debug.Log("[ButtonCanvasSetup] ButtonCanvas 계층구조 생성 완료!\n" +
                  "남은 작업:\n" +
                  "  1. MinimizeButton → MaximizedScreenController의 minimizeButtonRect에 연결\n" +
                  "  2. MaximizeButton → MinimizedScreenController의 minimizeButtonRect에 연결\n" +
                  "  3. ToggleButton → ClickThroughToggle 컴포넌트 연결\n" +
                  "  4. SettingsButton → PopupManager.OpenSettings() 연결\n" +
                  "  5. ScreenStateManager의 OnScreenStateChanged에 그룹 활성화 로직 연결");
    }

    // ── 헬퍼 메서드 ──────────────────────────────────────────────────

    private static GameObject GetOrCreate(string name, RectTransform parent)
    {
        // 이미 있으면 재사용
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    private static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>버튼 오브젝트 생성 (Image + Button + TMP 라벨)</summary>
    private static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        // 이미 있으면 재사용
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        go.AddComponent<Button>();

        // 라벨 텍스트
        GameObject labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        RectTransform labelRt = labelGo.AddComponent<RectTransform>();
        SetFullStretch(labelRt);

        TextMeshProUGUI tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 20;
        tmp.color = Color.white;

        return go;
    }

    /// <summary>QuitButton에 Application.Quit 리스너 연결</summary>
    private static void AddQuitListener(GameObject quitBtn)
    {
        Button btn = quitBtn.GetComponent<Button>();
        if (btn == null) return;
        btn.onClick.AddListener(() => Application.Quit());
    }
}
