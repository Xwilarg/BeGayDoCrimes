using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuriGameJam2023.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private PlayerController _currentPlayer;

        [SerializeField]
        private TMP_Text _distanceText;

        private void Awake()
        {
            Instance = this;
        }

        public void UnsetPlayer()
        {
            _currentPlayer = null;
        }

        public void DisplayDistanceText(float value)
        {
            _distanceText.text = $"{value:N1}";
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (_currentPlayer == null) // We aren't controlling a player...
                {
                    // ... so if we click on one we take possession of it
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
                    {
                        _currentPlayer = hit.collider.GetComponent<PlayerController>();
                        _currentPlayer.Enable();
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
