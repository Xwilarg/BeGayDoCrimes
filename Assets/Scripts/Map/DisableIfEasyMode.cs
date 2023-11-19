using UnityEngine;
using YuriGameJam2023.Persistency;

namespace YuriGameJam2023.Map
{
    public class DisableIfEasyMode : MonoBehaviour
    {
        private void Awake()
        {
            if (PersistencyManager.Instance.SaveData.IsEasyMode)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
