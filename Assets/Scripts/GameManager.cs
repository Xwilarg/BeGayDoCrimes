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
            SceneManager.LoadScene("Level_01", LoadSceneMode.Additive);
        }

        private void Start()
        {
            if (!DebugManager.Instance.BypassIntro)
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

        private void OnSkillSelected(InputAction.CallbackContext value, int id) // TODO: Handle controller
        {
            if (value.performed && !VNManager.Instance.IsPlayingStory)
            {
                CharacterManager.Instance.OnSkillSelected(id);
            }
        }

        public void OnSkill1(InputAction.CallbackContext value) => OnSkillSelected(value, 1);
        public void OnSkill2(InputAction.CallbackContext value) => OnSkillSelected(value, 2);
        public void OnSkill3(InputAction.CallbackContext value) => OnSkillSelected(value, 3);
        public void OnSkill4(InputAction.CallbackContext value) => OnSkillSelected(value, 4);
        public void OnSkill5(InputAction.CallbackContext value) => OnSkillSelected(value, 5);
    }
}
