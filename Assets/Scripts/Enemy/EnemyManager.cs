using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using YuriGameJam2023.Player;

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
            var enemies = PlayerManager.Instance.GetCharacters<EnemyController>();

            // Pick the enemy that is closest to a player
            var enemy = enemies
                .OrderBy(x => Vector3.Distance(x.transform.position, x.GetClosestPlayer().transform.position))
                .First();

            var player = enemy.GetClosestPlayer();

            enemy.Attack(player);
        }
    }
}