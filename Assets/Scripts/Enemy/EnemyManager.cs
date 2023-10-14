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
            var enemies = CharacterManager.Instance.GetCharacters<EnemyController>();

            // Pick the enemy that is closest to a player
            var enemy = enemies
                .OrderBy(x => Vector3.Distance(x.transform.position, x.GetClosestPlayer().transform.position))
                .First();

            var player = enemy.GetClosestPlayer();

            enemy.Target(player);
        }
    }
}