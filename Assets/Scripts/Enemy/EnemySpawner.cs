using UnityEngine;

namespace YuriGameJam2023.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _enemyPrefab;

        [SerializeField]
        private SO.CharacterInfo _enemyInfo;

        public void Spawn()
        {
            var instance = Instantiate(_enemyPrefab, transform.position + Vector3.up, Quaternion.identity);
            var enemy = instance.GetComponent<EnemyController>();

            enemy.Info = _enemyInfo;
            enemy.IsAlerted = true;
        }
    }
}
