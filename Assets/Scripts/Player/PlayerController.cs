using UnityEngine;

namespace YuriGameJam2023.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rb;
        public Vector2 Mov { set; private get; }

        private const float _speed = 300f;
        private const float _maxDistance = 3f;

        private float _distance;

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
            _rb.isKinematic = true;
            PlayerManager.Instance.UnsetPlayer();
            PlayerManager.Instance.DisplayDistanceText(0f);
        }
    }
}
