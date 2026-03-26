using UnityEngine;
using DesignPattern;

public class GameManager : Singleton<GameManager>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("GameManager Awake");
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Debug.Log("게임 초기화 시작");

        Debug.Log("게임 초기화 완료");
    }
}