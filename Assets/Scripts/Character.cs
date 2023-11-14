using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YuriGameJam2023.Achievement;
using YuriGameJam2023.Player;
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

        [SerializeField]
        private Transform _effectSpawner;

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

        protected readonly List<Character> _targets = new();

        protected bool HaveAnyTarget => _targets.Any();
        protected bool HaveAnyNonFriendlyTarget => _targets.Any(x => !x.CompareTag(tag));

        public bool PendingAutoDisable { private set; get; }

        protected abstract Vector3 Forward { get; }

        private Vector3 _lastPos;

        private bool _canBePlayed = true;
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
        private readonly List<GameObject> _vfxs = new();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("DieZone"))
            {
                if (_info.Name == "Yuki")
                {
                    AchievementManager.Instance.Unlock(AchievementID.JumpTrainYuki);
                }
                transform.parent = collision.transform;
                if (CharacterManager.Instance.AmIActive(this))
                {
                    Disable();
                }
                CharacterManager.Instance.UnregisterCharacter(this);
            }
        }

        public virtual void EndTurn()
        {
            // Apply all damage related to effects
            foreach (var eff in _effects)
            {
                if (eff.Key.AdditionalDamage != null)
                {
                    if (TakeDamage(null, eff.Key.AdditionalDamage))
                    {
                        return; // Character died, no need to do anything else
                    }
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
                //TODO display removed effects
            }
            if (!_effects.Any(x => x.Key.DoesAggro))
            {
                TargetOverride = null;
            }

            // Update UI
            RerenderEffect();
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
        }

        protected void UpdateAttackEffects()
        {
            // TODO: Don't do that each frame
            ClearAllHalo();
            CharacterManager.Instance.ResetEffectDisplay();

            var currSkill = Info.Skills[CurrentSkill];

            // Find all targets and set back the _targets list
            switch (currSkill.Type)
            {
                case RangeType.Self:
                    AddToTarget(gameObject);
                    break;

                case RangeType.CloseContact:
                    if (Physics.Raycast(new(transform.position + Vector3.down * .5f + Forward * .5f, Forward), out RaycastHit hit, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                    {
                        AddToTarget(hit.collider.gameObject);
                    }
                    break;

                case RangeType.AOE:
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
            _distance = _effects.Any(x => x.Key.PreventMovement) ? 0f : _maxDistance + _maxDistance * _effects.Sum(x => x.Key.IncreaseSpeed);
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
            EnemyManager.Instance.StopTimeoutTimer();
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

        private const float _effectSpacing = 5.0f;
        private void SpawnEffect(List<Tuple<bool, string>> effects)
        {
            for (int i = 0; i < effects.Count; i++) {
                EffectParticle eff = Instantiate(CharacterManager.Instance.EffectsParticle, this._effectSpawner);
                eff.transform.localPosition += Vector3.up * _effectSpacing * ((effects.Count - 1) - i) ;
                eff.Added = effects[i].Item1;
                eff.Name = effects[i].Item2;
            }
        }

        /// <returns>true is the character is dead</returns>
        public virtual bool TakeDamage(Character attacker, SkillInfo skill)
        {
            List<Tuple<bool, string>> effects = new List<Tuple<bool, string>>();

            int dmg;
            if (attacker == null)
            {
                dmg = skill.Damage;
            }
            else
            {
                dmg = Mathf.RoundToInt(skill.Damage + skill.Damage * Mathf.Clamp(attacker._effects.Sum(x => x.Key.IncreaseDamage), -1f, 1f));
            }
            if (dmg != 0) {
                effects.Add(Tuple.Create(dmg > 0, -dmg + "HP"));
            }
            Debug.Log($"[{this}] Took {dmg} damage from {attacker?.ToString() ?? skill.name}");
            Health = Clamp(Health - dmg, 0, Info.Health);
            if (Health == 0)
            {
                SpawnEffect(effects);
                Die();
                return true;
            }
            else if (skill != null && skill.Effects.Any())
            {
                Assert.IsNotNull(attacker, $"No attacker was provided for the skill {skill.name}");
                foreach (var effect in skill.Effects)
                {
                    if (this is PlayerController && effect.Name == "Aggro")
                    {
                        AchievementManager.Instance.Unlock(AchievementID.AggroFriend);
                        continue; // Can't aggro friends
                    }

                    var value = (TeamId == attacker.TeamId ? 0 : 1) + effect.Duration;
                    if (_effects.ContainsKey(effect))
                    {
                        _effects[effect] += value;
                    }
                    else
                    {
                        effects.Add(Tuple.Create(true, "+" + effect.name));
                        _effects.Add(effect, value);
                    }

                    if (effect.DoesAggro)
                    {
                        TargetOverride = attacker;
                    }
                }
                // Cancel old effects
                foreach (var eff in _effects.Keys.ToArray()) {
                    foreach (var cancel in eff.Cancels) {
                        if (_effects.Remove(cancel)) {
                            effects.Add(Tuple.Create(false, "-" + cancel.name));
                        }
                    }
                }

                if (this is EnemyController && _effects.Count >= 3)
                {
                    AchievementManager.Instance.Unlock(AchievementID.Effects3);
                }

                RerenderEffect();
            }
            SpawnEffect(effects);
            return false;
        }

        private void RerenderVfx()
        {
            foreach (var vfx in _vfxs) // TODO: Don't destroy everytimes
            {
                Destroy(vfx);
            }
            _vfxs.Clear();
            foreach (var eff in _effects) {
                if (eff.Key.Vfx != null) {
                    var vfx = Instantiate(eff.Key.Vfx, transform);
                    _vfxs.Add(vfx);
                }
            }
        }

        private void RerenderEffectBar() {
            for (int i = 0; i < _effectContainer.childCount; i++) Destroy(_effectContainer.GetChild(i).gameObject);
            foreach (var eff in _effects) {
                var go = Instantiate(_effectPrefab, _effectContainer);
                go.GetComponent<Image>().sprite = eff.Key.Sprite;
            }
        }

        private void RerenderEffect()
        {
            RerenderEffectBar();
            RerenderVfx();
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
