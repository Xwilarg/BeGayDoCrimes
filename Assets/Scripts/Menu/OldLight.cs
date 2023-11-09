using System.Collections;
using UnityEngine;

namespace YuriGameJam2023.Menu
{
    public class OldLight : MonoBehaviour
    {
        private Vector2 _blinkDuration = new(.1f, 1f);
        private Vector2 _blinkInterDuration = new(1f, 5f);

        private Light _light;

        private bool _rotUp = true;
        private const float _minRot = 80f;
        private const float _maxRot = 100f;
        private const float _rotSpeed = 10f;

        private void Awake()
        {
            _light = GetComponent<Light>();

            StartCoroutine(Blink());
        }

        private void Update()
        {
            // TODO: Can't make it work
            /*
            if (_rotUp)
            {
                transform.Rotate(Time.deltaTime * _rotSpeed, 0f, 0f);
                if (transform.rotation.eulerAngles.x < _minRot) _rotUp = false;
            }
            else
            {
                transform.Rotate(-Time.deltaTime * _rotSpeed, 0f, 0f);
                if (transform.rotation.eulerAngles.x > _maxRot) _rotUp = true;
            }
            */
        }

        private IEnumerator Blink()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_blinkInterDuration.x, _blinkInterDuration.y));
                _light.enabled = false;
                yield return new WaitForSeconds(Random.Range(_blinkDuration.x, _blinkDuration.y));
                _light.enabled = true;
            }
        }
    }
}
