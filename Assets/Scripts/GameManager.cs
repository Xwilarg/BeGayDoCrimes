using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YuriGameJam2023.VN;

namespace YuriGameJam2023
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "VN" || !DebugManager.Instance.BypassIntro) // VN scene is used to debug the VN so we always display the intro there
            {
                VNManager.Instance.ShowIntro();
            }
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
                    CharacterManager.Instance.OnClick();
                }
            }
        }

        public void OnClickCancel(InputAction.CallbackContext value)
        {
            if (value.performed && !VNManager.Instance.IsPlayingStory)
            {
                CharacterManager.Instance.OnClickCancel();
            }
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (!VNManager.Instance.IsPlayingStory)
            {
                CharacterManager.Instance.OnMovement(value.ReadValue<Vector2>());
            }
        }
    }
}
