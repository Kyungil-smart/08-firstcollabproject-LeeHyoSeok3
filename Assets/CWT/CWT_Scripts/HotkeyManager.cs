using UnityEngine;
using DesignPattern;

public class HotkeyManager : Singleton<HotkeyManager>
{
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

    private void Update()
    {
        if (_miniMizeKey != KeyCode.None)
        {
            if (Input.GetKeyDown(_miniMizeKey))
            {
                ScreenStateManager.Instance?.GoToMinimized();
            }
        }

        if (_clickThroughKey != KeyCode.None)
        {
            if (Input.GetKeyDown(_clickThroughKey))
            {
                bool now = WindowSystemManager.Instance.IsClickThrough;

                WindowSystemManager.Instance.SetClickThrough(!now);
            }
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

    }
}

public enum HotkeyType
{
    Minimize,
    ClickThrough
}