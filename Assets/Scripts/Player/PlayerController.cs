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
        public Vector2 Mov { set; private get; }

        private const float _speed = 300f;
        private const float _maxDistance = 3f;

        private float _distance;

        private readonly List<Character> _targets = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!_rb.isKinematic)
            {
                _rb.velocity = new Vector3(Mov.x, _rb.velocity.y, Mov.y) * Time.fixedDeltaTime * _speed;
                _distance -= (Mov * Time.fixedDeltaTime).magnitude;

                if (_distance <= 0)
                {
                    Disable();
                }
                else
                {
                    PlayerManager.Instance.DisplayDistanceText(_distance);
                }

                // Remove halo (that define targets) for all of them
                foreach (var t in _targets)
                {
                    t.ToggleHalo(false);
                }
                // Clear all target
                _targets.Clear();
                // Find all targets and set back the _targets list
                if (Physics.Raycast(new(transform.position + transform.forward * .75f, transform.forward), out RaycastHit hit, _info.Skills[0].Range))
                {
                    if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
                    {
                        var c = hit.collider.GetComponent<Character>();
                        c.ToggleHalo(true);
                        _targets.Add(c);
                    }
                }
            }
        }

        public bool CanAttack()
            => _targets.Any();

        public void Attack()
        {
            foreach (var t in _targets)
            {
                t.TakeDamage(_info.Skills[0].Damage);
            }
            Disable();
        }

        public void Enable()
        {
            _rb.isKinematic = false;
            _distance = _maxDistance;
            PlayerManager.Instance.DisplayDistanceText(_distance);
        }

        public void Disable()
        {
            foreach (var t in _targets)
            {
                t.ToggleHalo(false);
            }
            _targets.Clear();
            _rb.isKinematic = true;
            PlayerManager.Instance.UnsetPlayer();
            PlayerManager.Instance.DisplayDistanceText(0f);
        }
    }
}
