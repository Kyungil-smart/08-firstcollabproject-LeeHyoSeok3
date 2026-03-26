using UnityEngine;

/// <summary>
/// 전역/씬 싱글톤 통합 구조
/// isPersistent = true → DontDestroyOnLoad 대상
/// isPersistent = false → 씬마다 새로 생성됨
/// </summary>
namespace DesignPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        [Tooltip("true: 씬 이동 시 유지 / false: 씬마다 새로 생성됨")]
        [SerializeField] protected bool isPersistent = true;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"{typeof(T).Name} Singleton instance not found in scene.");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // 중복 인스턴스 제거
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;

            if (isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }

            OnAwake();
        }

        /// <summary>
        /// 하위 클래스에서 Awake 이후 로직 구현
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// 강제 제거 (예: 씬 리셋 시)
        /// </summary>
        public static void Release()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
