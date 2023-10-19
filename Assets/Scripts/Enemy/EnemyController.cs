using System.Collections.Generic;
using System.Linq;
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
        private int _alertRange;

        [SerializeField]
        private SO.CharacterInfo _enemyInfo;

        private NavMeshAgent _navigation;

        protected override Vector3 Forward => transform.forward;

        private bool _isMyTurn;
        private Character _target;

        public bool IsAlerted { get; private set; } = false;

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
        /// Gets the first player in range
        /// </summary>
        /// <param name="range">The range</param>
        /// <returns>The player in range or null</returns>
        public Character GetPlayerInRange(float range)
        {
            var colliders = Physics.OverlapSphere(transform.position, range);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    // Check if there is no terrain between the enemy and the player
                    if (!Physics.Linecast(transform.position, collider.transform.position, 1 << 7))
                    {
                        return collider.GetComponent<Character>();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets other enemies in range
        /// </summary>
        /// <param name="range">The range</param>
        /// <returns>The enemies in range</returns>
        public IEnumerable<EnemyController> GetCloseFriends(float range)
        {
            var enemies = CharacterManager.Instance.GetCharacters<EnemyController>();

            return enemies
                .Where(x => x != this)
                .Where(x => Vector3.Distance(x.transform.position, transform.position) < range);
        }

        /// <summary>
        /// Alerts the enemy of danger
        /// </summary>
        /// <param name="alertOthers">Whether we should scream for help</param>
        public void Alert(bool alertOthers)
        {
            IsAlerted = true;

            Debug.Log(name + " has been alerted");

            if (alertOthers)
            {
                // Alert other nearby enemies
                foreach (var enemy in GetCloseFriends(_alertRange))
                {
                    enemy.Alert(false);
                }
            }
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

            // TODO: Not every frame?
            if (!_isMyTurn && !IsAlerted)
            {
                var player = GetPlayerInRange(_alertRange);

                if (player != null)
                {
                    Alert(true);

                    Debug.Log(name + " just saw walking past them " + player.name + ", we're alerted!");
                }
            }
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = IsAlerted ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, _alertRange);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            // What the hell, who is hurting me! I'm alerted!
            Alert(true);
        }

        protected override void StopMovements()
        {
            _navigation.destination = transform.position;
        }
    }
}
