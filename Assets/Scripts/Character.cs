using UnityEngine;
using YuriGameJam2023.Player;

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

        public void StartParent()
        {
            PlayerManager.Instance.RegisterCharacter(this);
        }

        public void ToggleHalo(bool value)
        {
            _halo.SetActive(value);
        }

        public void TakeDamage(int damage)
        {
            _health = Clamp(_health - damage, 0, _maxHealth);
            if (_health == 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            PlayerManager.Instance.UnregisterCharacter(this);
            Destroy(gameObject);
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
