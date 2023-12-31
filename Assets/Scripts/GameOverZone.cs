﻿using UnityEngine;

namespace YuriGameJam2023
{
    public class GameOverZone : MonoBehaviour
    {
        [SerializeField]
        private GameOverZoneBehavior _zoneBehavior;

        public void OnTriggerEnter(Collider other)
        {
            if (_zoneBehavior == GameOverZoneBehavior.Defeat)
            {
                if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyController>().EndTurn();
                    CharacterManager.Instance.GameOver("An enemy reached the exit");
                }
            }
            else if (_zoneBehavior == GameOverZoneBehavior.Victory)
            {
                if (other.CompareTag("Player"))
                {
                    CharacterManager.Instance.Victory();
                }
            }
            else
            {
                if (other.CompareTag("Player"))
                {
                    CharacterManager.Instance.TriggerSpecialZone();
                }
            }
        }

        private enum GameOverZoneBehavior
        {
            Victory,
            Defeat,
            Special
        }
    }
}
