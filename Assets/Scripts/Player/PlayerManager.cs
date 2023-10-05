using UnityEngine;
using UnityEngine.InputSystem;

namespace YuriGameJam2023.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private PlayerController _currentPlayer;

        private void Awake()
        {
            Instance = this;
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && _currentPlayer == null)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
                {
                    _currentPlayer = hit.collider.GetComponent<PlayerController>();
                    _currentPlayer.Enable();
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
