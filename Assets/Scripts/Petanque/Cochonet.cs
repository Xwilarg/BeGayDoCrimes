using UnityEngine;

namespace Assets.Scripts.Petanque
{
    public class Cochonet : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-.25f, .25f), Random.Range(0f, 1.0f), 1f).normalized * Random.Range(5f, 15f), ForceMode.Impulse);
        }

        private bool _isOnFloor = false;
        private void OnCollisionEnter(Collision collision)
        {
            if (!_isOnFloor && collision.collider.CompareTag("Terrain"))
            {
                GetComponent<Rigidbody>().drag = 1f;
                _isOnFloor = true;
            }
        }
    }
}
