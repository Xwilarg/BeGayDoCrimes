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

        private void Awake()
        {
            Instance = this;
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

            if (!enemy.IsAlerted)
            {
                // Enemy can't play so we skip his turn
                enemy.CanBePlayed = false;
                StartCoroutine(WaitAndNextTurn());
                return;
            }

            try
            {
                CharacterManager.Instance.StartTurn(enemy);

                if (enemy.TargetOverride != null)
                {
                    enemy.Target(enemy.TargetOverride);
                }
                else
                {
                    var player = enemy.GetClosestPlayer();
                    enemy.Target(player);
                }
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