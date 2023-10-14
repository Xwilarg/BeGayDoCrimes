using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _info;

        [SerializeField]
        private GameObject _halo;

        [SerializeField]
        private int _maxHealth;
        private int _health;

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
        protected abstract void Stop();

        protected void AwakeParent()
        {
            _health = _maxHealth;
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
                Stop();
                CharacterManager.Instance.DisplayDistanceText(0f);
            }
            else
            {
                CharacterManager.Instance.DisplayDistanceText(_distance);
            }

            // Remove halo (that define targets) for all of them
            foreach (var t in _targets)
            {
                t.ToggleHalo(false);
            }
            // Clear all target
            // TODO: Don't do that each frame
            _targets.Clear();
            CharacterManager.Instance.ResetEffectDisplay();

            var currSkill = _info.Skills[0];

            // Find all targets and set back the _targets list
            switch (currSkill.Type)
            {
                case SO.SkillType.CloseContact:
                    if (Physics.Raycast(new(transform.position + Forward * .75f, Forward), out RaycastHit hit, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(hit.collider.gameObject);
                    }
                    break;

                case SO.SkillType.AOE:
                    foreach (var coll in Physics.OverlapSphere(transform.position + Forward * 2f * currSkill.Range, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(coll.gameObject);
                    }
                    CharacterManager.Instance.ShowAoeHint(transform.position + Forward * 2f * currSkill.Range, currSkill.Range);
                    break;

                default: throw new NotImplementedException();
            }
        }

        public void Attack()
        {
            Stop();
            foreach (var t in _targets)
            {
                t.TakeDamage(_info.Skills[0].Damage);
            }
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
        }

        public virtual void Disable()
        {
            foreach (var t in _targets)
            {
                t.ToggleHalo(false);
            }
            _targets.Clear();
            CharacterManager.Instance.ResetEffectDisplay();
            CharacterManager.Instance.UnsetPlayer();
            CharacterManager.Instance.DisplayDistanceText(0f);
            CharacterManager.Instance.RemoveAction();
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
            _health = Clamp(_health - damage, 0, _maxHealth);
            if (_health == 0)
            {
                Die();
            }
        }

        protected void Die()
        {
            Disable();
            CharacterManager.Instance.UnregisterCharacter(this);
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
