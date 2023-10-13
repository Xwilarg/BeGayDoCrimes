using UnityEngine;

namespace YuriGameJam2023
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        private GameObject _halo;

        [SerializeField]
        private int _maxHealth;
        private int _health;

        public void AwakeParent()
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
            if (_health <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}
