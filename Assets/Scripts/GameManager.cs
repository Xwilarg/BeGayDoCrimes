using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YuriGameJam2023.Persistency;
using YuriGameJam2023.SO;
using YuriGameJam2023.VN;

namespace YuriGameJam2023
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private EffectInfo[] _effectsInfo;

        public EffectInfo GetEffectInfo(EffectType eff)
        {
            return _effectsInfo.First(x => x.Effect == eff);
        }

        private void Awake()
        {
            Instance = this;
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
            SceneManager.LoadScene("Level_01", LoadSceneMode.Additive);
            SceneManager.LoadScene("DebugManager", LoadSceneMode.Additive);
        }

        private void Start()
        {
            if (!DebugManager.Instance.BypassIntro)
            {
                if (PersistencyManager.Instance.SaveData.CurrentLevel == 1)
                {
                    VNManager.Instance.ShowIntro();
                }
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
