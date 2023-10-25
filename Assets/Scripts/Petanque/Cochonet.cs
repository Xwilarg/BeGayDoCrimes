using UnityEngine;

namespace Assets.Scripts.Petanque
{
    public class Cochonet : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0f, 1.0f), 1f).normalized * 10f, ForceMode.Impulse);
        }
    }
}
