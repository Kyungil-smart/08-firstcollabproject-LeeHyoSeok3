using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerPositionRelay : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AnvilGoldController anvilGoldController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (anvilGoldController != null)
        {
            anvilGoldController.SetPointerScreenPosition(eventData.position);
        }
    }
}