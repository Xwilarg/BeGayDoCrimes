using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YuriGameJam2023.VN;

namespace YuriGameJam2023.Campfire
{
    public class CampfireManager : MonoBehaviour
    {
        [SerializeField]
        private Couple[] _couples;

        [SerializeField]
        private CharacterCamp[] _characters;

        private CharacterCamp _current;
        private CharacterCamp _selected;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Update()
        {
            if (_selected != null)
            {
                _selected.ToggleLight(false);
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
            {
                _selected = hit.collider.GetComponent<CharacterCamp>();
                _selected.ToggleLight(true);
            }
            else
            {
                _selected = null;
            }
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && !VNManager.Instance.IsPlayingStory)
            {
                if (_current != null)
                {
                    _current.ToggleLight(false);
                }
                _current = null;

                foreach (var c in _characters)
                {
                    c.ToggleInteraction(false);
                }

                if (_selected != null)
                {
                    _current = _selected;
                    _selected = null;
                }
            }
        }
    }
}
