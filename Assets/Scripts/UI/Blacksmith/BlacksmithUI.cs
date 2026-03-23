using UnityEngine;

public class BlacksmithUI : MonoBehaviour
{
    [SerializeField] private GameObject popup;

    public void OpenPopup()
    {
        popup.SetActive(true);
    }

    public void ClosePopup()
    {
        popup.SetActive(false);
    }
}