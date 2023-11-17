using System.Collections;
using UnityEngine;
using YuriGameJam2023.Achievement;

namespace YuriGameJam2023.Player
{
    public class PlayerController : Character
    {
        [SerializeField]
        private int _loveRange;

        [SerializeField]
        private int _loveIncrease;

        [SerializeField]
        private GameObject _heartSprite;

        private Rigidbody _rb;
        private Vector2 _mov;
        public Vector2 Mov
        {
            set => _mov = value;
            get => _mov;
        }
        private Vector3 _forward = Vector3.forward;

        protected override Vector3 Forward => _forward;

        private const float _speed = 300f;


        private bool CanMove => !_rb.isKinematic && _distance > 0f && !PendingAutoDisable;
        public bool CanAttack => !_rb.isKinematic && !PendingAutoDisable && HaveAnyTarget;
        protected override int TeamId => 1;

        private void Awake()
        {
            AwakeParent();

            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            StartParent();
        }

        private void FixedUpdate()
        {
            if (!_rb.isKinematic)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, 1 << 7))
                {
                    var direction = hit.point - transform.position;
                    direction.y = 0f;

                    _forward = direction.normalized;
                }
            }
            if (CanMove) // Are we the character currently being moved by the player
            {
                _rb.velocity = new Vector3(Mov.x * Time.fixedDeltaTime * _speed, _rb.velocity.y, Mov.y * Time.fixedDeltaTime * _speed);

                FixedUpdateParent();
            }
            if (!_rb.isKinematic)
            {
                UpdateAttackEffects();
            }
        }

        public void TryBurnHouse()
        {
            if (!_rb.isKinematic && !PendingAutoDisable && _isTargetingHouse)
            {
                AchievementManager.Instance.Unlock(AchievementID.BurnHouse);
            }
        }

        private IEnumerator DisplayHeart()
        {
            _heartSprite.SetActive(true);
            yield return new WaitForSeconds(1f);
            _heartSprite.SetActive(false);
        }

        public override void Attack()
        {
            base.Attack();

            // Check for any close players to bond with
            var colliders = Physics.OverlapSphere(transform.position, _loveRange, 1 << 6);

            foreach (var collider in colliders)
            {
                var character = collider.GetComponent<Character>();

                if (collider.CompareTag("Player") && character != this && !_targets.Contains(character))
                {
                    StartCoroutine(DisplayHeart());
                    StartCoroutine(collider.GetComponent<PlayerController>().DisplayHeart());
                    CharacterManager.Instance.IncreaseLove(this, collider.GetComponent<Character>(), _loveIncrease);
                }
            }
        }

        protected override void StopMovements()
        {
            _rb.velocity = Vector3.zero;
        }

        public override void Enable()
        {
            base.Enable();
            _rb.isKinematic = false;
        }

        public override void Disable()
        {
            base.Disable();
            _rb.isKinematic = true;
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // In the color of love
            Gizmos.color = Color.magenta;
            DebugHelper.DrawCircle(transform.position, _loveRange);
        }
    }
}
