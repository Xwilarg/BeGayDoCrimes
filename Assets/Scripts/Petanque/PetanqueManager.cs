using UnityEngine;

namespace YuriGameJam2023.Petanque
{
    public class PetanqueManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private float _xMin;
    }
}
