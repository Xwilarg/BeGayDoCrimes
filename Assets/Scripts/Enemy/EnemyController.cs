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

        protected override Vector3 Forward => Vector3.zero; // TODO

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
            if (distance > _attackDistance)
            {
                _navigation.destination = target.transform.position;
                _navigation.stoppingDistance = _attackDistance;
            }
        }

        private void FixedUpdate()
        {
            if (CharacterManager.Instance.IsMyTurn(this))
            {
                FixedUpdateParent();
                if (_navigation.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    /*if (HaveAnyTarget)
                    {
                        Attack();
                    }
                    else
                    {
                        Disable();
                    }*/
                }
            }
        }

        protected override void Stop()
        {
            _navigation.destination = transform.position;
        }
    }
}
