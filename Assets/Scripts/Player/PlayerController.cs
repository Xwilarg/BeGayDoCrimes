using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YuriGameJam2023.Player
{
    public class PlayerController : Character
    {
        [SerializeField]
        private SO.CharacterInfo _info;

        private Rigidbody _rb;
        private Vector2 _mov;
        public Vector2 Mov
        {
            set
            {
                _mov = value;
                if (value.magnitude != 0f)
                {
                    _forward = new(_mov.x, 0f, _mov.y);
                }
            }
            get => _mov;
        }
        private Vector3 _forward = Vector3.forward;

        private const float _speed = 300f;
        private const float _maxDistance = 3f;

        private float _distance;

        private readonly List<Character> _targets = new();

        public bool PendingAutoDisable { private set; get; }


        private bool CanMove => !_rb.isKinematic && _distance > 0f && !PendingAutoDisable;
        public bool CanAttack => !_rb.isKinematic && !PendingAutoDisable && _targets.Any();


        private void Awake()
        {
            AwakeParent();

            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (CanMove) // Are we the character currently being moved by the player
            {
                _rb.velocity = new Vector3(Mov.x, _rb.velocity.y, Mov.y) * Time.fixedDeltaTime * _speed;
                _distance -= (Mov * Time.fixedDeltaTime).magnitude;

                PlayerManager.Instance.DisplayDistanceText(_distance);

                // Remove halo (that define targets) for all of them
                foreach (var t in _targets)
                {
                    t.ToggleHalo(false);
                }
                // Clear all target
                // TODO: Don't do that each frame
                _targets.Clear();
                PlayerManager.Instance.ResetEffectDisplay();

                var currSkill = _info.Skills[0];

                // Find all targets and set back the _targets list
                switch (currSkill.Type)
                {
                    case SO.SkillType.CloseContact:
                        if (Physics.Raycast(new(transform.position + _forward * .75f, _forward), out RaycastHit hit, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                        {
                            AddToTarget(hit.collider.gameObject);
                        }
                        break;

                    case SO.SkillType.AOE:
                        foreach (var coll in Physics.OverlapSphere(transform.position + _forward * 2f * currSkill.Range, currSkill.Range, 1 << LayerMask.NameToLayer("Character")))
                        {
                            AddToTarget(coll.gameObject);
                        }
                        PlayerManager.Instance.ShowAoeHint(transform.position + _forward * 2f * currSkill.Range, currSkill.Range);
                        break;

                    default: throw new NotImplementedException();
                }
            }
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

        protected override void Die()
        {
            Disable();
            base.Die();
        }

        public void Attack()
        {
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

        public void Enable()
        {
            PendingAutoDisable = false;
            _rb.isKinematic = false;
            _distance = _maxDistance;
            PlayerManager.Instance.DisplayDistanceText(_distance);
        }

        public void Disable()
        {
            PlayerManager.Instance.ResetEffectDisplay();
            foreach (var t in _targets)
            {
                t.ToggleHalo(false);
            }
            _targets.Clear();
            _rb.isKinematic = true;
            PlayerManager.Instance.UnsetPlayer();
            PlayerManager.Instance.DisplayDistanceText(0f);
            PlayerManager.Instance.RemoveAction();
        }
    }
}
