using UnityEngine;
using UnityEngine.EventSystems;
using Kirurobo;

/// <summary>
/// DragMoveHandle 프리팹(UniWindowMoveHandle)에 추가로 부착하는 브릿지 컴포넌트.
/// 드래그 종료 후 화면 이탈 예외처리(ScreenBoundsHandler)를 연결합니다.
///
/// 사용법: DragMoveHandle 프리팹의 동일 GameObject에 이 컴포넌트를 추가하세요.
/// </summary>
[RequireComponent(typeof(UniWindowMoveHandle))]
public class DragMoveHandleBridge : MonoBehaviour, IEndDragHandler, IPointerUpHandler
{
    private ScreenBoundsHandler _boundsHandler;

    private void Start()
    {
        _boundsHandler = FindObjectOfType<ScreenBoundsHandler>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _boundsHandler?.CheckAndCorrectBounds();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _boundsHandler?.CheckAndCorrectBounds();
    }
}
