using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YuriGameJam2023
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private List<EnemyController> _enemies;

        private EnemyController _currEnemy;
        private float _turnTimer = -1f;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (_turnTimer > 0f)
            {
                _turnTimer -= Time.deltaTime;
                if (_turnTimer < 0f)
                {
                    _currEnemy.EndTurn();
                    _turnTimer = -1f;
                    Debug.LogError("Enemy timeout, this mean the enemy didn't manage to play after 10 seconds!");
                }
            }
        }

        public void StartTurn()
        {
            _enemies = CharacterManager.Instance.GetCharacters<EnemyController>()
                .OrderBy(x => x.IsAlerted ? 0 : 1) // All turns that will be skipped are done last
                .ThenBy(x => Vector3.Distance(x.transform.position, x.GetClosestPlayer().transform.position))
                .ToList();
            DoAction();
        }

        public void DoAction()
        {
            if (!_enemies.Any())
            {
                CharacterManager.Instance.RemoveAction();
                return;
            }

            var enemy = _enemies[0];
            _enemies.RemoveAt(0);

            if (!enemy.IsAlerted && enemy.TargetPos == null)
            {
                // Enemy can't play so we skip his turn
                enemy.CanBePlayed = false;
                StartCoroutine(WaitAndNextTurn());
                return;
            }

            try
            {
                CharacterManager.Instance.StartTurn(enemy);

                if (!enemy.IsAlerted)
                {
                    enemy.Target(enemy.TargetPos);
                }
                else if (enemy.TargetOverride != null)
                {
                    enemy.Target(enemy.TargetOverride);
                }
                else
                {
                    var player = enemy.GetClosestPlayer();
                    enemy.Target(player);
                }
                _currEnemy = enemy;
                _turnTimer = 10f;
            }
            catch (Exception e)
            {
                StartCoroutine(WaitAndNextTurn());
                Debug.LogException(e);
            }
        }

        public void StopTimeoutTimer()
        {
            _turnTimer = -1f;
        }

        private IEnumerator WaitAndNextTurn()
        {
            yield return new WaitForSeconds(1f);
            CharacterManager.Instance.RemoveAction();
        }
    }
}