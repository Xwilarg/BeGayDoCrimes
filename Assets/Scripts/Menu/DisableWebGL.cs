using UnityEngine;

namespace YuriGameJam2023.Menu
{
    public class DisableWebGL : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_WEBGL
            gameObject.SetActive(false);
#endif
        }
    }
}
