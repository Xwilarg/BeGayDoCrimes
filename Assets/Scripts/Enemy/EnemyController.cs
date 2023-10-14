using UnityEngine;
using UnityEngine.AI;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public class EnemyController : Character
    {
        [SerializeField]
        private float _attackDistance;
        [SerializeField]
        private int _attackDamage;

        private NavMeshAgent _navigation;
        private Character _target;

        protected override Vector3 Forward => Vector3.zero; // TODO

        private void Awake()
        {
            AwakeParent();
        }

        private void Start()
        {
            _navigation = GetComponent<NavMeshAgent>();

            StartParent();
        }

        /// <summary>
        /// Gets the closest player from the enemy
        /// </summary>
        /// <returns>The closest player</returns>
        public Character GetClosestPlayer()
        {
            return PlayerManager.Instance.GetClosestCharacter<PlayerController>(transform);
        }

        /// <summary>
        /// Instructs the enemy to attack the target
        /// </summary>
        /// <param name="target">The target to attack</param>
        public void Attack(Character target)
        {
            var distance = Vector3.Distance(target.transform.position, transform.position);

            // Whether we are close enough to the player
            if (distance > _attackDistance)
            {
                _navigation.destination = target.transform.position;
                _navigation.stoppingDistance = _attackDistance;
            }

            _target = target;
        }

        private void FixedUpdate()
        {
            if (_navigation.pathStatus == NavMeshPathStatus.PathComplete && _target != null)
            {
                _target.TakeDamage(_attackDamage);
                _target = null;

                Debug.Log("Shoot!");

                PlayerManager.Instance.RemoveAction();
            }
        }

        private void OnAnimatorMove()
        {
            // TODO: Update distance
        }

        protected override void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
