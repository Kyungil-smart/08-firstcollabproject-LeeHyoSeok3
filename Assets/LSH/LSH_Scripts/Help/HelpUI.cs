using UnityEngine;

// 박사님, 이 클래스는 화면상의 UI들을 끄고 켜는 '관리자' 역할을 합니다.
public class HelpUI : MonoBehaviour
{
    // 유니티 에디터에서 'HelpOverlay' 패널을 연결할 칸을 만듭니다.
    [SerializeField]
    private GameObject helpOverlayPanel;

    // 게임이 시작될 때 실행되는 부분입니다.
    private void Start()
    {
        // 처음에는 도움말이 보이지 않아야 하므로 꺼둡니다.
        if (helpOverlayPanel != null)
        {
            helpOverlayPanel.SetActive(false);
        }
    }

    // [물음표 버튼]에 연결할 메서드입니다. 
    // 누를 때마다 켜져 있으면 끄고, 꺼져 있으면 켭니다. (토글 기능)
    public void ToggleHelp()
    {
        if (helpOverlayPanel != null)
        {
            // 현재 패널이 켜져 있는지(activeSelf) 확인하여 그 반대로 설정합니다.
            bool isActive = helpOverlayPanel.activeSelf;
            helpOverlayPanel.SetActive(!isActive);
        }
    }

    // [빈 공간(배경 패널)]을 클릭했을 때 호출할 메서드입니다.
    // 무조건 도움말을 끕니다.
    public void CloseHelp()
    {
        if (helpOverlayPanel != null)
        {
            helpOverlayPanel.SetActive(false);
        }
    }
}