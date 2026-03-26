using UnityEngine;
using UnityEngine.SceneManagement;

public static class Manager
{
    public static GameManager Game => GameManager.Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        GameObject rootPrefab = Resources.Load<GameObject>("Prefabs/@Managers");

        if (rootPrefab == null)
        {
            Debug.LogError("[Manager] @Managers 프리팹을 찾을 수 없습니다.");
            return;
        }

        Object.Instantiate(rootPrefab);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[Manager] Scene Loaded: {scene.name}");
    }
}