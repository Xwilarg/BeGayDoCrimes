using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using YuriGameJam2023.Effect;
using YuriGameJam2023.Player;

namespace YuriGameJam2023
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [SerializeField]
        [Tooltip("Text used to display the distance the character can still walk")]
        private TMP_Text _distanceText;

        [SerializeField]
        [Tooltip("Text used to display the amount of actions left")]
        private TMP_Text _actionCountText;

        [SerializeField]
        [Tooltip("Cinemachine virtual camera")]
        private CinemachineVirtualCamera _vCam;

        [SerializeField]
        [Tooltip("Object used to display spell Area Of Effect hints")]
        private AoeHint _aoeHint;

        [SerializeField]
        [Tooltip("Tooltip that confirm if the player want to end his action")]
        private GameObject _disablePopup;

        [SerializeField]
        [Tooltip("Tooltip that confirm if the player want to end his turn")]
        private GameObject _endTurnPopup;

        [SerializeField]
        [Tooltip("Where the camera should look at when no player is selected")]
        private Transform _cameraDefaultLookAt;

        private Character _currentPlayer;

        private const int _totalActionCountRef = 5;
        private int _totalActionCount = _totalActionCountRef;
        private bool _isPlayerTurn = true;

        private readonly List<Character> _characters = new();

        private bool IsUIActive => _disablePopup.activeInHierarchy || _endTurnPopup.activeInHierarchy;

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
            _vCam.LookAt = _cameraDefaultLookAt;
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
                EndTurn();
            }
            else if (!_isPlayerTurn)
            {
                EnemyManager.Instance.DoAction();
            }
        }

        public void EndTurn()
        {
            _isPlayerTurn = !_isPlayerTurn;
            _totalActionCount = _totalActionCountRef;

            _actionCountText.text = $"Actions Left: {_totalActionCount}";

            if (!_isPlayerTurn)
            {
                EnemyManager.Instance.DoAction();
            }
        }

        public void StartTurn(Character c)
        {
            _currentPlayer = c;
            _currentPlayer.Enable();

            _vCam.LookAt = _currentPlayer.transform;
        }

        /// <summary>
        /// Disable the display of all effects
        /// </summary>
        public void ResetEffectDisplay()
        {
            _aoeHint.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show a hint on the floor that represent where an attack will land
        /// </summary>
        public void ShowAoeHint(Vector3 pos, int radius)
        {
            _aoeHint.gameObject.SetActive(true);
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

        public IEnumerable<T> GetCharacters<T>()
        {
            return _characters
                .Where(x => x is T)
                .Cast<T>();
        }

        public void DisableConfirm()
        {
            _currentPlayer.Disable();
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && _isPlayerTurn && !IsUIActive)
            {
                if (_currentPlayer == null) // We aren't controlling a player...
                {
                    // ... so if we click on one we take possession of it
                    if (_isPlayerTurn && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
                    {
                        StartTurn(hit.collider.GetComponent<PlayerController>());
                    }
                }
                else
                {
                    if (((PlayerController)_currentPlayer).CanAttack)
                    {
                        _currentPlayer.Attack();
                    }
                }
            }
        }

        public void OnClickCancel(InputAction.CallbackContext value)
        {
            if (value.performed && _isPlayerTurn)
            {
                // Close popups
                if (_endTurnPopup.activeInHierarchy)
                {
                    _endTurnPopup.SetActive(false);
                }
                else if (_disablePopup.activeInHierarchy)
                {
                    _disablePopup.SetActive(false);
                }
                // Open popup to end turn
                else if (_currentPlayer == null)
                {
                    _endTurnPopup.SetActive(true);
                }
                // If we aren't already disabling the player, open popup to end the current action
                else if (_currentPlayer != null && !_currentPlayer.PendingAutoDisable)
                {
                    _disablePopup.SetActive(true);
                }
            }
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (_currentPlayer != null && _isPlayerTurn && !IsUIActive)
            {
                ((PlayerController)_currentPlayer).Mov = value.ReadValue<Vector2>();
            }
        }
    }
}
