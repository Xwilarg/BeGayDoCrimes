using UnityEngine;

namespace YuriGameJam2023
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        [Tooltip("If true, the intro won't be played")]
        private bool _bypassIntro;

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
