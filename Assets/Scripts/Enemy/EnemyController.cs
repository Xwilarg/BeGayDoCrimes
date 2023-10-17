using UnityEngine;
using UnityEngine.AI;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public class EnemyController : Character
    {
        [SerializeField]
        private int _attackDamage;

        [SerializeField]
        private SO.CharacterInfo _enemyInfo;

        private NavMeshAgent _navigation;

        protected override Vector3 Forward => transform.forward;

        private bool _isMyTurn;
        private Character _target;

        private void Awake()
        {
            AwakeParent();
            _navigation = GetComponent<NavMeshAgent>();
            Info = _enemyInfo;
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
            if (distance > Info.Skills[0].Range)
            {
                _navigation.destination = target.transform.position;
                _navigation.stoppingDistance = Info.Skills[0].Range;
            }

            _isMyTurn = true;
            _target = target;
        }

        private void FixedUpdate()
        {
            if (_isMyTurn)
            {
                FixedUpdateParent();
                if ((!_navigation.pathPending && _navigation.remainingDistance < Info.Skills[0].Range) || _distance <= 0f)
                {
                    // Check if we have no targets, but are close to one
                    if (!HaveAnyNonFriendlyTarget &&
                        Vector3.Distance(_target.transform.position, transform.position) < Info.Skills[0].Range)
                    {
                        var direction = _target.transform.position - transform.position;
                        direction.y = 0f;

                        var rotation = Quaternion.LookRotation(direction.normalized);

                        // Rotate towards the target this frame and return
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * (_navigation.angularSpeed / 2));

                        return;
                    }

                    _isMyTurn = false;

                    if (HaveAnyNonFriendlyTarget)
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
