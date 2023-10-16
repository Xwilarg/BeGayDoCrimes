using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace YuriGameJam2023
{
    public abstract class Character : MonoBehaviour
    {
        private SO.CharacterInfo _info;
        public SO.CharacterInfo Info
        {
            set
            {
                _info = value;
                GetComponentInChildren<SpriteRenderer>().sprite = value.Sprite;
                Health = Info.Health;
            }
            get => _info;
        }

        [SerializeField]
        private GameObject _halo;

        [SerializeField]
        private Image _healthBar;

        private int _health;
        private int Health
        {
            set
            {
                _health = value;
                _healthBar.rectTransform.localScale = new Vector2(value / (float)Info.Health, 1f);
            }
            get => _health;
        }

        public int CurrentSkill { set; get; }

        private float _maxDistance = 10f;
        protected float _distance;

        private readonly List<Character> _targets = new();
        protected bool HaveAnyTarget => _targets.Any();

        public bool PendingAutoDisable { private set; get; }

        protected abstract Vector3 Forward { get; }

        private Vector3 _lastPos;

        /// <summary>
        /// Force the current agent to stop
        /// </summary>
        protected abstract void StopMovements();

        protected void AwakeParent()
        {
            GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        }

        protected void StartParent()
        {
            CharacterManager.Instance.RegisterCharacter(this);
        }

        protected void FixedUpdateParent()
        {
            _distance -= Vector3.Distance(transform.position, _lastPos);
            _lastPos = transform.position;

            if (_distance <= 0f)
            {
                StopMovements();
                CharacterManager.Instance.DisplayDistanceText(0f);
            }
            else
            {
                CharacterManager.Instance.DisplayDistanceText(_distance);
            }

            // TODO: Don't do that each frame
            ClearAllHalo();
            CharacterManager.Instance.ResetEffectDisplay();

            var currSkill = Info.Skills[CurrentSkill];

            // Find all targets and set back the _targets list
            switch (currSkill.Type)
            {
                case SO.RangeType.CloseContact:
                    if (Physics.Raycast(new(transform.position + Forward * .75f, Forward), out RaycastHit hit, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(hit.collider.gameObject);
                    }
                    break;

                case SO.RangeType.AOE:
                    foreach (var coll in Physics.OverlapSphere(transform.position + Forward * 1.5f * currSkill.Range, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(coll.gameObject);
                    }
                    CharacterManager.Instance.ShowAoeHint(transform.position + Forward * 1.5f * currSkill.Range, currSkill.Range);
                    break;

                default: throw new NotImplementedException();
            }
        }

        public void Attack()
        {
            int damage = Info.Skills[CurrentSkill].Damage;
            foreach (var t in _targets)
            {
                Debug.Log($"[{this}] Attacking {t} for {damage} damage");
                t.TakeDamage(damage);
            }
            StopMovements();
            StartCoroutine(WaitAndDisable(1f));
        }

        private IEnumerator WaitAndDisable(float timer)
        {
            PendingAutoDisable = true;
            yield return new WaitForSeconds(timer);
            Disable();
        }

        public virtual void Enable()
        {
            _lastPos = transform.position;
            PendingAutoDisable = false;
            _distance = _maxDistance;
            CharacterManager.Instance.DisplayDistanceText(_distance);
            CurrentSkill = 0;
        }

        public virtual void Disable()
        {
            ClearAllHalo();
            CharacterManager.Instance.ResetEffectDisplay();
            CharacterManager.Instance.UnsetPlayer();
            CharacterManager.Instance.DisplayDistanceText(0f);
            CharacterManager.Instance.RemoveAction();
        }

        public void ClearAllHalo()
        {
            foreach (var t in _targets)
            {
                if (t != null)
                {
                    t.ToggleHalo(false);
                }
            }
            _targets.Clear();
        }

        /// <summary>
        /// Add the element given in parameter to the list of target we can attack
        /// </summary>
        private void AddToTarget(GameObject go)
        {
            var c = go.GetComponent<Character>();
            c.ToggleHalo(true);
            _targets.Add(c);
        }

        public void ToggleHalo(bool value)
        {
            _halo.SetActive(value);
        }

        public void TakeDamage(int damage)
        {
            Health = Clamp(Health - damage, 0, Info.Health);
            if (Health == 0)
            {
                Die();
            }
        }

        protected void Die()
        {
            if (CharacterManager.Instance.AmIActive(this))
            {
                Disable();
            }
            CharacterManager.Instance.UnregisterCharacter(this);
            Destroy(gameObject);
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public override string ToString()
        {
            return $"{Info.name} ({Health}/{Info.Health}HP)";
        }
    }
}
