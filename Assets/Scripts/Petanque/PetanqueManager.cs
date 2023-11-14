using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YuriGameJam2023.Achievement;
using YuriGameJam2023.VN;

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

        [SerializeField]
        private GameObject _cochonet;

        [SerializeField]
        private TextAsset _story;

        [SerializeField]
        private GameObject _gameOver;

        [SerializeField]
        private TMP_Text _gameOverText;

        private List<Ball> _balls = new();

        private LineRenderer _lr;

        private ActionType _currAction = ActionType.Move;

        private GameObject _ball;
        private bool _dir;
        private float _currForce;

        private int _indexSprite;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Start()
        {
            VNManager.Instance.ShowStory(_story, () =>
            {
                Instantiate(_cochonet, _spawnPoint).transform.localPosition = Vector3.zero;
                SpawnCharacter();
            });
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

        public void MainMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        private void SpawnCharacter()
        {
            if (_indexSprite == _sprites.Length)
            {
                AchievementManager.Instance.Unlock(AchievementID.Petanque);
                _gameOver.SetActive(true);
                _gameOverText.text = "Final Score:\n" + (100 - _balls.Sum(x => Vector3.Distance(_cochonet.transform.position, x.transform.position))).ToString("n2");
                return;
            }

            _currForce = _maxForce / 2f;

            _ball = Instantiate(_prefab, _spawnPoint);
            _ball.transform.localPosition = Vector3.zero;
            _lr = _ball.GetComponent<LineRenderer>();
            _ball.GetComponentInChildren<SpriteRenderer>().sprite = _sprites[_indexSprite].Sprite;

            _balls.Add(_ball.GetComponent<Ball>());
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (VNManager.Instance.IsPlayingStory)
                {
                    VNManager.Instance.DisplayNextDialogue();
                }
                else
                {
                    if (_currAction == ActionType.Pending)
                    {
                        return;
                    }

                    _currAction++;
                    _dir = true;

                    if (_currAction == ActionType.Done)
                    {
                        var rb = _ball.GetComponent<Rigidbody>();
                        rb.isKinematic = false;
                        rb.AddForce(_ball.transform.forward * _currForce, ForceMode.Impulse);
                        _ball.transform.rotation = Quaternion.identity;
                        _lr.enabled = false;
                        _ball = null;

                        _indexSprite++;

                        StartCoroutine(WaitAndProceed());
                    }
                }
            }
        }

        private IEnumerator WaitAndProceed()
        {
            while (!_balls.All(x => x.IsDone))
            {
                yield return new WaitForSeconds(.1f);
            }
            SpawnCharacter();

            _currAction = ActionType.Move;
        }

        private enum ActionType
        {
            Move,
            RotateYaw,
            RotatePitch,
            Force,
            Done,

            Pending
        }
    }
}
