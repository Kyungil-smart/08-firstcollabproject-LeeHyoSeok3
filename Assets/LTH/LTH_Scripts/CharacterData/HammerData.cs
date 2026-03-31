using UnityEngine;

// 해머 레벨에 따라 망치를 변경하도록 만든 데이터입니다.
// 아트마다 손 위치가 달라서 위치 오프셋도 지정하도록 했습니다.

[CreateAssetMenu(fileName = "HammerData", menuName = "Level/Hammer")]
public class HammerData : ScriptableObject
{
    public Sprite hammerHead; // 망치 머리
    public Sprite hammerHandle; // 망치 손잡이

    // 해머마다 손 위치 오프셋 저장(현재 아트마다 손 위치가 달라 지정할 필요가 있음)
    public Vector2 handOffset;
}
