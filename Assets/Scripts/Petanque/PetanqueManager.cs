using UnityEngine;
using UnityEngine.InputSystem;

namespace YuriGameJam2023.Petanque
{
    public class PetanqueManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private float _maxLinear, _maxYaw, _maxPitch, _maxForce;

        [SerializeField]
        private float _linearSpeed, _angularSpeed, _forceSpeed;

        [SerializeField]
        private float _forceToDisplayDivider;

        [SerializeField]
        private SO.CharacterInfo[] _sprites;

        private LineRenderer _lr;

        private ActionType _currAction = ActionType.Move;

        private GameObject _ball;
        private bool _dir;
        private float _currForce;

        private int _indexSprite;

        private void Awake()
        {
            SpawnCharacter();
        }

        private void Update()
        {
            if (_ball == null) return;

            if (_currAction == ActionType.Move)
            {
                var x = _ball.transform.localPosition.x + (Time.deltaTime * _linearSpeed * (_dir ? 1f : -1f));
                if (!_dir && x < -_maxLinear)
                {
                    x = -_maxLinear;
                    _dir = true;
                }
                else if (_dir && x > _maxLinear)
                {
                    x = _maxLinear;
                    _dir = false;
                }

                _ball.transform.localPosition = new(x, _ball.transform.localPosition.y, _ball.transform.localPosition.z);
            }
            else if ( _currAction == ActionType.RotateYaw)
            {
                var a = _ball.transform.rotation.eulerAngles.y + (Time.deltaTime * _angularSpeed * (_dir ? 1f : -1f));
                if (!_dir && a > 180 && a < 360 - _maxYaw)
                {
                    a = 360 - _maxYaw;
                    _dir = true;
                }
                else if (_dir && a < 180 && a > _maxYaw)
                {
                    a = _maxYaw;
                    _dir = false;
                }

                _ball.transform.rotation = Quaternion.Euler(_ball.transform.rotation.eulerAngles.x, a, _ball.transform.rotation.eulerAngles.z);
            }
            else if (_currAction == ActionType.RotatePitch)
            {
                var a = _ball.transform.rotation.eulerAngles.x + (Time.deltaTime * _angularSpeed * (!_dir ? 1f : -1f));
                Debug.Log(a);
                if (_dir && a > 180 && a < 360 - _maxPitch)
                {
                    a = 360 - _maxPitch;
                    _dir = false;
                }
                else if (!_dir && a < 180 && a > 0f)
                {
                    a = 0f;
                    _dir = true;
                }

                _ball.transform.rotation = Quaternion.Euler(a, _ball.transform.rotation.eulerAngles.y, _ball.transform.rotation.eulerAngles.z);
            }
            else if (_currAction == ActionType.Force)
            {
                var f = _currForce + (Time.deltaTime * _forceSpeed * (_dir ? 1f : -1f));
                if (!_dir && f < 0f)
                {
                    f = 0f;
                    _dir = true;
                }
                else if (_dir && f > _maxForce)
                {
                    f = _maxForce;
                    _dir = false;
                }

                _currForce = f;
            }

            _lr.SetPositions(new[] { _ball.transform.position, _ball.transform.position + _ball.transform.forward * _currForce / _forceToDisplayDivider });
        }

        private void SpawnCharacter()
        {
            _currForce = _maxForce / 2f;

            _ball = Instantiate(_prefab, _spawnPoint);
            _ball.transform.localPosition = Vector3.zero;
            _lr = _ball.GetComponent<LineRenderer>();
            _ball.GetComponentInChildren<SpriteRenderer>().sprite = _sprites[_indexSprite].Sprite;
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _currAction++;
                _dir = true;

                if (_currAction == ActionType.Done)
                {
                    _ball.transform.rotation = Quaternion.identity;
                    var rb = _ball.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.AddForce(_ball.transform.forward * _currForce, ForceMode.Impulse);
                    _lr.enabled = false;
                    _ball = null;

                    _indexSprite++;
                    SpawnCharacter();

                    _currAction = ActionType.Move;
                }
            }
        }

        private enum ActionType
        {
            Move,
            RotateYaw,
            RotatePitch,
            Force,
            Done
        }
    }
}
