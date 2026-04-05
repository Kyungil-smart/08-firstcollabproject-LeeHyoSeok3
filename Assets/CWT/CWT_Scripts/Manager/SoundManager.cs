using DesignPattern;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] List<SoundData> data = new List<SoundData>();

    AudioSource _oneShot; // 한번 재생할 변수
    AudioSource _loopShot; // 반복 재생할 변수
    AudioSource _bgmShot; // bgm 재생할 변수

    // 현재 BGM 볼륨값을 외부에서 가져갈 수 있게 해주는 프로퍼티
    public float BGMVolume => _bgmShot.volume;
    // 현재 SFX 볼륨값을 외부에서 가져갈 수 있게 해주는 프로퍼티
    public float SFXVolume => _oneShot.volume;

    protected override void OnAwake()
    {   
        // 오디오소스를 생성해서 붙여준다.
        _oneShot = gameObject.AddComponent<AudioSource>();
        _loopShot = gameObject.AddComponent<AudioSource>();
        _bgmShot = gameObject.AddComponent<AudioSource>();
        // 반복재생 해야하는 클립은 반복해준다.
        _loopShot.loop = true;
        _bgmShot.loop = true;
    }

    // 소리 이름으로 해당 SoundData를 찾아서 돌려주는 함수
    private SoundData OpenClip(string name)
    {
        foreach(SoundData result in data)
        {
            if (result.soundName == name)
            {
                return result;
            }

        }
        return null;
    }

    // 한번 재생하는 음원소스 호출용
    public void OneShot(string name)
    {
        SoundData result = OpenClip(name); // 사운드데이터에서 찾는 이름이 없으면 리턴
        if (result == null) return;
        _oneShot.PlayOneShot(result.clip); // 소스파일 찾으면 호출
    }

    public void PlayClick()
    {
        OneShot("Click");
    }

    // 반복 재생하는 음원소스 호출용
    public void LoopShot(string name)
    {
        SoundData result = OpenClip(name);
        if (result == null) return;
        _loopShot.clip = result.clip;
        _loopShot.Play();
    }
    // BGM 재생하는 음원소스 호출용
    public void BGMShot(string name)
    {
        SoundData result = OpenClip(name);
        if (result == null) return;

        if (_bgmShot.clip == result.clip && _bgmShot.isPlaying) return;

        _bgmShot.clip = result.clip;
        _bgmShot.Play();
    }

    // 반복 재생 종료시키는 함수
    public void LoopStop()
    {
        _loopShot.Stop();
    }

    // bgm 재생 종료시키는 함수
    public void BGMStop()
    {
        _bgmShot.Stop();
    }

    // SFX볼륨 조절하는 함수
    public void ControlSFXVolume(float vol)
    {
        _oneShot.volume = vol;
        _loopShot.volume = vol;
    }

    // BGM 볼륨 조절하는 함수
    public void ControlBGMVolume(float vol)
    {
        _bgmShot.volume = vol;
    }
}
