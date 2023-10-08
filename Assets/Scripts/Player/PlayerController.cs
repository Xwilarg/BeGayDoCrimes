using System.Collections.Generic;
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
                _rb.velocity = new Vector3(Mov.x, _rb.velocity.y, Mov.y) * Time.fixedDeltaTime * 300f;
                _distance -= (Mov * Time.fixedDeltaTime).magnitude;

                if (_distance <= 0)
                {
                    Disable();
                }
                else
                {
                    PlayerManager.Instance.DisplayDistanceText(_distance);
                }

                foreach (var t in _targets)
                {
                    t.ToggleHalo(false);
                }
                _targets.Clear();
                if (Physics.Raycast(new(transform.position + transform.forward * .75f, transform.forward), out RaycastHit hit, _info.Skills[0].Range))
                {
                    Debug.Log(hit.collider.name);
                    if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<Character>().ToggleHalo(true);
                    }
                }
            }
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
