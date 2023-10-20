using YuriGameJam2023.Persistency;
using UnityEngine;

namespace YuriGameJam2023
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            if (_deleteSaveData)
            {
                PersistencyManager.Instance.DeleteSaveFolder();
            }
#endif
        }

        [SerializeField]
        [Tooltip("If true, the intro won't be played")]
        private bool _bypassIntro;

        [SerializeField]
        [Tooltip("Delete existing save data when the game start")]
        private bool _deleteSaveData;

        public bool BypassIntro
        {
            get
            {
#if UNITY_EDITOR
                return _bypassIntro;
#else
                return false;
#endif
            }
        }
    }
}
