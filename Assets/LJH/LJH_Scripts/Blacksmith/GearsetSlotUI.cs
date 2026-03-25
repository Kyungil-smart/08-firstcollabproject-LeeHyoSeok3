using UnityEngine;

public class GearsetSlotUI : MonoBehaviour
{
    [SerializeField] private string gearsetName;
    [SerializeField] private GearsetNameCSVLoader csvLoader;
    [SerializeField] private GearsetCraftPopupUI popupUI;

    public void OnClickSlot()
    {
        GearsetNameData data = csvLoader.GetByName(gearsetName);

        if (data == null)
        {
            Debug.LogWarning($"'{gearsetName}' 장비 데이터를 찾지 못했습니다.");
            return;
        }

        popupUI.Show(data);
    }
    
    public void ClosePopup()
    {
        popupUI.Hide();
    }
}