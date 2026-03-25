using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _dungeonName;
    //[SerializeField] private TMP_Text _dungeonNameEng;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _requiredAttribute;
    [SerializeField] private TMP_Text _rewardItem;
    [SerializeField] private TMP_Text _timeRequired;

    public void SetData(DungeonData data)
    {
        _dungeonName.text = data.dungeonName;
        //_dungeonNameEng.text = data.dungeonNameEng;
        _description.text = data.description;
        _requiredAttribute.text = data.requiredAttribute;
        _rewardItem.text = data.rewardItem;
        _timeRequired.text = data.timeRequired;
    }
}
