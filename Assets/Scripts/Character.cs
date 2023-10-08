using UnityEngine;

namespace YuriGameJam2023
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        private GameObject _halo;

        public void ToggleHalo(bool value)
        {
            _halo.SetActive(value);
        }
    }
}
