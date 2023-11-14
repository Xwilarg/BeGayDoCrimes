using UnityEngine;

namespace YuriGameJam2023.Enemy
{
    public class EnemyTrain : MonoBehaviour
    {
        [SerializeField]
        private int _speed;

        [SerializeField]
        private float _stopPosition;

        [SerializeField]
        private float _endPosition;

        [SerializeField]
        private GameObject _spawner;

        private int _step;
        private Vector3 _position;
        private Vector3 _initPosition;

        private void Awake()
        {
            _initPosition = transform.position;

            Hide(true);
        }

        private void Hide(bool hidden)
        {
            foreach (var child in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                child.enabled = !hidden;
            }
        }

        /// <summary>
        /// Drive the train to the stop position to spawn an enemy
        /// </summary>
        public void Drive()
        {
            Hide(false);
            transform.position = _initPosition;

            _position = new Vector3(_initPosition.x, _initPosition.y, _stopPosition);
            _step = 1;
        }

        private void FixedUpdate()
        {
            if (_step > 0)
            {
                // Choo choo
                transform.position = Vector3.MoveTowards(transform.position, _position, _speed * Time.deltaTime);

                if (transform.position == _position )
                {
                    if (_step == 1)
                    {
                        _spawner.GetComponent<EnemySpawner>().Spawn();

                        _position = new Vector3(_initPosition.x, _initPosition.y, _endPosition);
                        _step = 2;
                    }
                    else
                    {
                        _step = 0;
                    }
                }
            }
        }
    }
}
