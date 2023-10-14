using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using YuriGameJam2023.Effect;

namespace YuriGameJam2023.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private PlayerController _currentPlayer;

        [SerializeField]
        private TMP_Text _distanceText;

        [SerializeField]
        private TMP_Text _actionCountText;

        [SerializeField]
        private CinemachineVirtualCamera _vCam;

        [SerializeField]
        private AoeHint _aoeHint;

        private int _totalActionCount = 5;

        private void Awake()
        {
            Instance = this;
            _actionCountText.text = $"Actions Left: {_totalActionCount}";
        }

        public void UnsetPlayer()
        {
            _currentPlayer = null;
        }

        public void DisplayDistanceText(float value)
        {
            _distanceText.text = $"Distance: {value:N1}";
        }

        public void RemoveAction()
        {
            _totalActionCount--;
            _actionCountText.text = $"Actions Left: {_totalActionCount}";
            if (_totalActionCount == 0)
            {
                // TODO: AI turn
            }
        }

        /// <summary>
        /// Disable the display of all effects
        /// </summary>
        public void ResetEffectDisplay()
        {
            _aoeHint.enabled = false;
        }

        public void ShowAoeHint(Vector3 pos, int radius)
        {
            _aoeHint.enabled = true;
            _aoeHint.Show(pos, radius);
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (_currentPlayer == null) // We aren't controlling a player...
                {
                    // ... so if we click on one we take possession of it
                    if (_totalActionCount > 0 && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
                    {
                        _currentPlayer = hit.collider.GetComponent<PlayerController>();
                        _currentPlayer.Enable();

                        _vCam.LookAt = _currentPlayer.transform;
                    }
                }
                else
                {
                    if (_currentPlayer.CanAttack())
                    {
                        _currentPlayer.Attack();
                    }
                }
            }
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.Mov = value.ReadValue<Vector2>();
            }
        }
    }
}
