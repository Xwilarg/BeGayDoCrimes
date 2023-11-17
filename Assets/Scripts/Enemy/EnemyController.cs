using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;
using YuriGameJam2023.SO;

namespace YuriGameJam2023
{
    public class EnemyController : Character
    {
        [SerializeField]
        private Transform _targetPos;
        public Transform TargetPos => _targetPos;

        [SerializeField]
        private bool _startAwareOfPlayer;

        [SerializeField]
        private bool _hidden;

        [SerializeField]
        private int _alertRange;

        [SerializeField]
        private bool _slowerWhenNotAlerted;

        [SerializeField]
        private SO.CharacterInfo _enemyInfo;

        private float _baseSpeed;

        public bool IsHealer => Info.Skills[0].Damage < 0f;

        private NavMeshAgent _navigation;

        protected override Vector3 Forward => transform.forward;

        private bool _isMyTurn;
        private Character _target;

        public bool IsAlerted { get; set; } = false;

        protected override int TeamId => 0;

        private void Awake()
        {
            AwakeParent();
            IsAlerted = _startAwareOfPlayer;
            _navigation = GetComponent<NavMeshAgent>();
            _baseSpeed = _navigation.speed;
            if (_enemyInfo != null) Info = _enemyInfo;

            if (_hidden) Hide(true);
        }

        private void Start()
        {
            StartParent();
        }

        /// <summary>
        /// Gets the closest player from the enemy
        /// </summary>
        /// <returns>The closest player</returns>
        public Character GetClosest<T>()
            where T : Character
        {
            return CharacterManager.Instance.GetClosestCharacter<T>(transform, true, this);
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
            Hide(false);

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
        /// Hides the enemy
        /// </summary>
        /// <param name="hidden">Whether the enemy should be hidden</param>
        public void Hide(bool hidden)
        {
            foreach (var child in gameObject.GetComponentsInChildren<Renderer>())
            {
                child.enabled = !hidden;
            }

            gameObject.GetComponentInChildren<Canvas>().enabled = !hidden;
        }

        /// <summary>
        /// Instructs the enemy to attack the target
        /// </summary>
        /// <param name="target">The target to attack</param>
        public void Target(Character target)
        {
            var distance = Vector3.Distance(target.transform.position, transform.position);

            // Whether we are close enough to the player
            if (distance > Info.Skills[0].Range + 1f)
            {
                _navigation.destination = target.transform.position;
                _navigation.stoppingDistance = Info.Skills[0].Range + 1f;
            }

            _isMyTurn = true;
            _target = target;
            _navigation.speed = _baseSpeed;
        }

        public void Target(Transform target)
        {
            var path = new NavMeshPath();
            if (!_navigation.CalculatePath(target.transform.position, path))
            {
                Debug.LogError("Cannot calculate path for " + name + " to target, this is bad!");

                // You have been bad, now die
                Die();
                return;
            }

            if (!IsAlerted && _slowerWhenNotAlerted)
            {
                _distance /= 2f;
                _navigation.speed = _baseSpeed / 2f;
            }
            else
            {
                _navigation.speed = _baseSpeed;
            }

            _navigation.path = path;
            //_navigation.destination = target.transform.position;
            _navigation.stoppingDistance = 0f;

            _isMyTurn = true;
            _target = null;
        }

        private void FixedUpdate()
        {
            if (_isMyTurn)
            {
                FixedUpdateParent();
                UpdateAttackEffects();
                var isMovementDone =
                    _distance <= 0f || // We can't move anymore
                    (!_navigation.pathPending && // OR if there is no path pending AND
                        (_target != null && _navigation.remainingDistance < Info.Skills[0].Range + 1f) || // In the case we have a target: we are in range for the skill
                        (_target == null && _navigation.remainingDistance == 0f) // In the case we don't have a target: we reached the position where we needed to go
                    );
                if (isMovementDone)
                {
                    // Check if we have no targets, but are close to one
                    if (_target != null && ((IsHealer && !HaveAnyFriendlyTarget) || (!IsHealer && !HaveAnyNonFriendlyTarget)) &&
                        Vector3.Distance(_target.transform.position, transform.position) <= Info.Skills[0].Range + 1f)
                    {
                        var direction = _target.transform.position - transform.position;
                        direction.y = 0f;

                        var rotation = Quaternion.LookRotation(direction.normalized);

                        // Rotate towards the target this frame and return
                        var old = transform.rotation;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * (_navigation.angularSpeed / 2f));

                        if (old == transform.rotation)
                        {
                            Debug.LogWarning($"{Info.Name} failed to find a target");
                            Disable(); // We somehow didn't find a target
                        }

                        return;
                    }

                    _isMyTurn = false;

                    if ((IsHealer && HaveAnyFriendlyTarget) || (!IsHealer && HaveAnyNonFriendlyTarget))
                    {
                        StopMovements();
                        StartCoroutine(WaitAndAttack());
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

        private IEnumerator WaitAndAttack()
        {
            // Choose what skill we want to use depending on what is the best effective one
            int _bestIndex = 0;
            int _bestEffectiveness = 0;

            int i = 0;
            foreach (var skill in Info.Skills)
            {
                CurrentSkill = i;
                UpdateAttackEffects();

                if (_targets.Any(x => x is EnemyController))
                {
                    continue;
                }

                var eff = skill.Damage * _targets.Count;
                if (eff > _bestEffectiveness)
                {
                    _bestIndex = i;
                    _bestEffectiveness = eff;
                }

                i++;
            }

            CurrentSkill = _bestIndex;
            UpdateAttackEffects();

            yield return new WaitForSeconds(.5f);

            Attack();
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = IsAlerted ? Color.red : Color.green;
            DebugHelper.DrawCircle(transform.position, _alertRange);
        }

        public override bool TakeDamage(Character attacker, SkillInfo skill)
        {
            if (base.TakeDamage(attacker, skill))
            {
                return true;
            }

            // What the hell, who is hurting me! I'm alerted!
            Alert(true);

            return false;
        }

        public override void EndTurn()
        {
            StopMovements();
            base.EndTurn();
        }

        protected override void StopMovements()
        {
            _navigation.destination = transform.position;
        }
    }
}
