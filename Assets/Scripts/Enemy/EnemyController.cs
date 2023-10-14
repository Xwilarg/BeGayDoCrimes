using UnityEngine;
using UnityEngine.AI;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public class EnemyController : Character
    {
        [SerializeField]
        private int _attackDamage;

        private NavMeshAgent _navigation;

        protected override Vector3 Forward => transform.forward;

        private bool _isMyTurn;

        private void Awake()
        {
            AwakeParent();
            _navigation = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            StartParent();
        }

        /// <summary>
        /// Gets the closest player from the enemy
        /// </summary>
        /// <returns>The closest player</returns>
        public Character GetClosestPlayer()
        {
            return CharacterManager.Instance.GetClosestCharacter<PlayerController>(transform);
        }

        /// <summary>
        /// Instructs the enemy to attack the target
        /// </summary>
        /// <param name="target">The target to attack</param>
        public void Target(Character target)
        {
            var distance = Vector3.Distance(target.transform.position, transform.position);

            // Whether we are close enough to the player
            if (distance > _info.Skills[0].Range)
            {
                _navigation.destination = target.transform.position;
                _navigation.stoppingDistance = _info.Skills[0].Range;
            }

            _isMyTurn = true;
        }

        private void FixedUpdate()
        {
            if (_isMyTurn)
            {
                FixedUpdateParent();
                if ((!_navigation.pathPending && _navigation.remainingDistance < _navigation.stoppingDistance) || _distance <= 0f)
                {
                    _isMyTurn = false;
                    if (HaveAnyTarget)
                    {
                        Attack();
                    }
                    else
                    {
                        Disable();
                    }
                }
            }
        }

        protected override void StopMovements()
        {
            _navigation.destination = transform.position;
        }
    }
}
