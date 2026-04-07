using UnityEngine;
using DesignPattern;

public class HotkeyManager : Singleton<HotkeyManager>
{
    private const string MinimizeHotkeyPrefKey = "Hotkey.Minimize";
    private const string ClickThroughHotkeyPrefKey = "Hotkey.ClickThrough";

    private KeyCode _miniMizeKey = KeyCode.None;
    private KeyCode _clickThroughKey = KeyCode.None;

    public KeyCode MinimizeKey
    {
        get
        {
            return _miniMizeKey;
        }
    }

    public KeyCode ClickThroughKey
    {
        get
        {
            return _clickThroughKey;
        }
    }

    private void Start()
    {
        LoadHotkeys();
    }

    private void Update()
    {
        if (_miniMizeKey != KeyCode.None && Input.GetKeyDown(_miniMizeKey))
        {
            ToggleScreenState();
        }

        if (_clickThroughKey != KeyCode.None && Input.GetKeyDown(_clickThroughKey))
        {
            ToggleClickThrough();
        }
    }

    public void SetHotKey(HotkeyType type, KeyCode newKey)
    {
        if (newKey == KeyCode.Escape || newKey == KeyCode.LeftWindows) return;

        switch (type)
        {
            case HotkeyType.Minimize:
                if (newKey == _clickThroughKey)
                {
                    _clickThroughKey = KeyCode.None;
                }
                _miniMizeKey = newKey;
                break;

            case HotkeyType.ClickThrough:
                if (newKey == _miniMizeKey)
                {
                    _miniMizeKey = KeyCode.None;
                }
                _clickThroughKey = newKey;
                break;
        }

        SaveHotkeys();
    }

    private void LoadHotkeys()
    {
        _miniMizeKey = (KeyCode)PlayerPrefs.GetInt(MinimizeHotkeyPrefKey, (int)KeyCode.None);
        _clickThroughKey = (KeyCode)PlayerPrefs.GetInt(ClickThroughHotkeyPrefKey, (int)KeyCode.None);
    }

    private void SaveHotkeys()
    {
        PlayerPrefs.SetInt(MinimizeHotkeyPrefKey, (int)_miniMizeKey);
        PlayerPrefs.SetInt(ClickThroughHotkeyPrefKey, (int)_clickThroughKey);
        PlayerPrefs.Save();
    }

    private void ToggleScreenState()
    {
        if (ScreenStateManager.Instance == null)
            return;

        switch (ScreenStateManager.Instance.CurrentState)
        {
            case ScreenStateManager.ScreenState.Main:
                ScreenStateManager.Instance.GoToMinimized();
                break;

            case ScreenStateManager.ScreenState.Minimized:
                ScreenStateManager.Instance.GoToMain();
                break;
        }
    }

    private void ToggleClickThrough()
    {
        if (WindowSystemManager.Instance == null)
            return;

        bool now = WindowSystemManager.Instance.IsClickThrough;
        WindowSystemManager.Instance.SetClickThrough(!now);
    }
}

public enum HotkeyType
{
    Minimize,
    ClickThrough
}