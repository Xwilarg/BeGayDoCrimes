using YuriGameJam2023.Persistency;
using UnityEngine;

namespace YuriGameJam2023
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Instance { get; private set; }

        private static bool _didDebugDelete;

        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            if (!_didDebugDelete && _deleteSaveData)
            {
                Debug.Log("[DEBUG] Save file deleted");
                _didDebugDelete = true;
                PersistencyManager.Instance.DeleteSaveFolder();
            }
#endif
        }

        [SerializeField]
        [Tooltip("Delete existing save data when the game start")]
        private bool _deleteSaveData;

        [SerializeField]
        [Tooltip("Unlock all support dialogues")]
        private bool _unlockAllSupports;

        public bool AutounlockSupports
        {
            get
            {
#if UNITY_EDITOR
                return _unlockAllSupports;
#else
                return false;
#endif
            }
        }
    }
}
