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
            foreach (var enemy in CharacterManager.Instance.GetCharacters<EnemyController>()
                .OrderBy(x => x.IsAlerted ? 1 : 0) // All turns that will be skipped are done last
                .ThenBy(x => Vector3.Distance(x.transform.position, x.GetClosestPlayer().transform.position)))
            {
                if (!enemy.IsAlerted)
                {
                    // Enemy can't play so we skip his turn
                    enemy.CanBePlayed = false;
                    CharacterManager.Instance.RemoveAction();
                    continue;
                }

                var player = enemy.GetClosestPlayer();
                CharacterManager.Instance.StartTurn(enemy);
                enemy.Target(player);
            }
        }
    }
}