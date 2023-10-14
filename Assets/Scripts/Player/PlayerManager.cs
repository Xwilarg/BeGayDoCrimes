using Cinemachine;
using System.Collections.Generic;
using System.Linq;
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

        private const int _totalActionCountRef = 5;
        private int _totalActionCount = _totalActionCountRef;
        private bool _isPlayerTurn = true;

        private List<Character> _characters = new();

        private void Awake()
        {
            Instance = this;
            _actionCountText.text = $"Actions Left: {_totalActionCount}";
        }

        public void RegisterCharacter(Character c)
        {
            _characters.Add(c);
        }
        public void UnregisterCharacter(Character c)
        {
            _characters.Remove(c);
        }

        /// <summary>
        /// Deselect a player (aka we are not controlling it anymore)
        /// </summary>
        public void UnsetPlayer()
        {
            _currentPlayer = null;
        }

        /// <summary>
        /// Show the distance the player can still walk during this turn
        /// </summary>
        public void DisplayDistanceText(float value)
        {
            _distanceText.text = $"Distance: {value:N1}";
        }

        /// <summary>
        /// Remove an action, when a player is out of actions, his turn ends
        /// </summary>
        public void RemoveAction()
        {
            _totalActionCount--;
            _actionCountText.text = $"Actions Left: {_totalActionCount}";
            if (_totalActionCount == 0)
            {
                _isPlayerTurn = !_isPlayerTurn;
                _totalActionCount = _totalActionCountRef;
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

        /// <summary>
        /// Show a hint on the floor that represent where an attack will land
        /// </summary>
        public void ShowAoeHint(Vector3 pos, int radius)
        {
            _aoeHint.enabled = true;
            _aoeHint.Show(pos, radius);
        }

        public Character GetClosestCharacter<T>(Transform transform)
            where T : Character
        {
            return _characters
                .Where(x => x is T)
                .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                .ElementAt(0);
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (_currentPlayer == null) // We aren't controlling a player...
                {
                    // ... so if we click on one we take possession of it
                    if (_isPlayerTurn && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
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
