using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YuriGameJam2023.SO;

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

        public Light Halo { private set; get; }

        [SerializeField]
        private Image _healthBar;

        [SerializeField]
        private Transform _effectContainer;

        [SerializeField]
        private GameObject _effectPrefab;

        public Character TargetOverride { protected set; get; } // Not handled for PlayerController

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
        protected bool HaveAnyNonFriendlyTarget => _targets.Any(x => !x.CompareTag(tag));

        public bool PendingAutoDisable { private set; get; }

        protected abstract Vector3 Forward { get; }

        private Vector3 _lastPos;

        private bool _canBePlayed;
        public bool CanBePlayed
        {
            set
            {
                _canBePlayed = value;
                _sr.color = _canBePlayed ? Color.white : Color.gray;
            }
            get => _canBePlayed;
        }

        private SpriteRenderer _sr;

        private readonly Dictionary<EffectInfo, int> _effects = new();
        public void EndTurn()
        {
            // Apply all damage related to effects
            foreach (var eff in _effects)
            {
                if (eff.Key.AdditionalDamage != null)
                {
                    TakeDamage(this, eff.Key.AdditionalDamage);
                }
            }

            // Remove effects
            foreach (var key in _effects.Keys.ToList())
            {
                _effects[key]--;
                if (_effects[key] == 0)
                {
                    _effects.Remove(key);
                }
            }
            if (!_effects.Any(x => x.Key.DoesAggro))
            {
                TargetOverride = null;
            }

            // Update UI
            UpdateSkills();
        }

        protected abstract int TeamId { get; }

        /// <summary>
        /// Force the current agent to stop
        /// </summary>
        protected abstract void StopMovements();

        protected void AwakeParent()
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
            Halo = GetComponentInChildren<Light>();
            Halo.gameObject.SetActive(false);
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
                    if (Physics.Raycast(new(transform.position + Forward * .5f, Forward), out RaycastHit hit, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(hit.collider.gameObject);
                    }
                    break;

                case SO.RangeType.AOE:
                    foreach (var coll in Physics.OverlapSphere(transform.position + Forward * 1.5f * currSkill.Range, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        // Check whether the target is not behind a wall
                        if (!Physics.Linecast(transform.position, coll.transform.position, 1 << 7))
                        {
                            AddToTarget(coll.gameObject);
                        }
                    }
                    CharacterManager.Instance.ShowAoeHint(transform.position + Forward * 1.5f * currSkill.Range, currSkill.Range);
                    break;

                default: throw new NotImplementedException();
            }
        }

        public virtual void Attack()
        {
            foreach (var t in _targets)
            {
                t.TakeDamage(this, Info.Skills[CurrentSkill]);
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
            _distance = _effects.Any(x => x.Key.PreventMovement) ? 0f : _maxDistance;
            CharacterManager.Instance.DisplayDistanceText(_distance);
            CurrentSkill = 0;
        }

        public virtual void Disable()
        {
            CanBePlayed = false;
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
                    t.Halo.gameObject.SetActive(false);
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
            c.Halo.gameObject.SetActive(true);
            c.Halo.color = _info.Skills[CurrentSkill].Damage >= 0 ? Color.red : Color.green;
            _targets.Add(c);
        }

        public virtual void TakeDamage(Character attacker, SkillInfo skill)
        {
            Debug.Log($"[{this}] Took {skill.Damage} damage from {attacker}");
            Health = Clamp(Health - skill.Damage, 0, Info.Health);
            if (Health == 0)
            {
                Die();
            }
            else if (skill != null)
            {
                foreach (var effect in skill.Effects)
                {
                    var value = (TeamId == attacker.TeamId ? 0 : 1) + effect.Duration;
                    if (_effects.ContainsKey(effect))
                    {
                        _effects[effect] += value;
                    }
                    else
                    {
                        _effects.Add(effect, value);
                    }

                    if (effect.DoesAggro)
                    {
                        TargetOverride = attacker;
                    }
                }

                UpdateSkills();
            }
        }

        private void UpdateSkills()
        {
            // Cancel effects
            foreach (var eff in _effects.Keys.ToArray())
            {
                foreach (var cancel in eff.Cancels)
                {
                    _effects.Remove(cancel);
                }
            }

            for (int i = 0; i < _effectContainer.childCount; i++) Destroy(_effectContainer.GetChild(i).gameObject);

            foreach (var eff in _effects)
            {
                var go = Instantiate(_effectPrefab, _effectContainer);
                go.GetComponent<Image>().sprite = eff.Key.Sprite;
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

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            var p = transform.position + Forward * .5f;
            Gizmos.DrawLine(p, p + Forward * 2f);
        }
    }
}
