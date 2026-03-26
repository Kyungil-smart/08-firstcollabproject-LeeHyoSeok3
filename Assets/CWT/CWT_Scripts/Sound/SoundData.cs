using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string soundName; // 팀원이 재생 요청할 때 쓰는 이름
    public AudioClip clip; // 재생될 오디오 클립
    public bool isLoop; // 반복재생 할건지?

}
