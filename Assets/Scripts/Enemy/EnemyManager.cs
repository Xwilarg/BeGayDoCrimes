using System.Linq;
using UnityEngine;

namespace YuriGameJam2023
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void DoAction()
        {
            var enemy = PickEnemy();

            if (enemy == null)
            {
                CharacterManager.Instance.RemoveAction();
            }
            else
            {
                var player = enemy.GetClosestPlayer();

                CharacterManager.Instance.StartTurn(enemy);
                enemy.Target(player);
            }
        }

        /// <summary>
        /// Pick the best enemy for this round
        /// </summary>
        /// <returns>The best enemy or null if no tasks</returns>
        private EnemyController PickEnemy()
        {
            var enemies = CharacterManager.Instance.GetCharacters<EnemyController>();

            // Check if any enemies have been alerted
            if (enemies.Any(x => x.IsAlerted))
            {
                // Return the alerted enemy closest to player
                return enemies
                    .Where(x => x.IsAlerted)
                    .OrderBy(x => Vector3.Distance(x.transform.position, x.GetClosestPlayer().transform.position))
                    .FirstOrDefault();
            }

            // Return no enemy for now if not alerted
            return null;
        }
    }
}