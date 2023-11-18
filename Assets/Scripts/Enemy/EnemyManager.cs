using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private List<EnemyController> _enemies;

        private EnemyController _currEnemy;

        private void Awake()
        {
            Instance = this;
        }

        public void StartTurn()
        {
            _enemies = CharacterManager.Instance.GetCharacters<EnemyController>()
                .OrderBy(x => x.IsAlerted ? 0 : 1) // All turns that will be skipped are done last
                .OrderBy(x => x.IsHealer ? 1 : 0) // Healers play last
                .ThenBy(x => Vector3.Distance(x.transform.position, x.GetClosest<PlayerController>().transform.position))
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
                    Character target = null;
                    if (enemy.IsHealer)
                    {
                        target = CharacterManager.Instance.GetWeakestCharacter<EnemyController>(enemy.transform, true, enemy);
                    }
                    else
                    {
                        float bestScore = 0f;
                        foreach (var pc in CharacterManager.Instance.GetCharacters<PlayerController>())
                        {
                            var score = enemy.GetTargetScore(pc);
                            Debug.Log($"Interest score for {pc.Info.Name}: {score}");
                            if (target == null || score > bestScore)
                            {
                                bestScore = score;
                                target = pc;
                            }
                        }
                    }
                    enemy.Target(target);
                }
                _currEnemy = enemy;
            }
            catch (Exception e)
            {
                StartCoroutine(WaitAndNextTurn());
                Debug.LogException(e);
            }
        }

        private IEnumerator WaitAndNextTurn()
        {
            yield return new WaitForSeconds(1f);
            CharacterManager.Instance.RemoveAction();
        }
    }
}