using TMPro;
using UnityEngine;

public class GearsetCraftPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gearsetNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(GearsetNameData data)
    {
        if (data == null)
        {
            Debug.LogError("data가 null입니다.");
            return;
        }

        gearsetNameText.text = data.gearsetName;
        descriptionText.text = data.description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("팝업 닫기");
        gameObject.SetActive(false);
    }
}