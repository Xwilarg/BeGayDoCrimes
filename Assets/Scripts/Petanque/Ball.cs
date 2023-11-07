using UnityEngine;

namespace YuriGameJam2023.Petanque
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody _rb;

        private bool _touchedFloor = false;

        public bool IsDone
            => _rb.isKinematic || (_touchedFloor && _rb.velocity.magnitude < .01f);

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("OOB"))
            {
                _rb.isKinematic = true;
            }
            else if (collision.collider.CompareTag("Terrain"))
            {
                _touchedFloor = true;
            }
        }
    }
}
