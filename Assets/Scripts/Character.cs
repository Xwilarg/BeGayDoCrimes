using UnityEngine;

namespace YuriGameJam2023
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        private GameObject _halo;

        [SerializeField]
        private int _maxHealth;
        protected int _health;

        private void Awake()
        {
            _health = _maxHealth;
        }

        public void ToggleHalo(bool value)
        {
            _halo.SetActive(value);
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
        }
    }
}
