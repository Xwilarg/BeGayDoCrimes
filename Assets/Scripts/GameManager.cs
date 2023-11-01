using System.Collections;
using TMPro;
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

        [SerializeField]
        private LevelInfo[] _levels;

        [SerializeField]
        private TMP_Text _explanationText;

        private void Awake()
        {
            Instance = this;

            Debug.Log($"Current level: {PersistencyManager.Instance.SaveData.CurrentLevel}");

            var currLevel = _levels[PersistencyManager.Instance.SaveData.CurrentLevel - 1];
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
            SceneManager.LoadScene(currLevel.SceneName, LoadSceneMode.Additive);
            SceneManager.LoadScene("DebugManager", LoadSceneMode.Additive);

            _explanationText.text =
                "Victory Condition:\n" +
                currLevel.VictoryCondition switch
                {
                    VictoryCondition.KillAll => "Kill all enemies"
                } + "\n\n" +
                "Defeat Condition:\nLoose all characters\n" +
                (currLevel.AdditionalDefeatCondition == DefeatCondition.None ? string.Empty :
                currLevel.AdditionalDefeatCondition switch
                {
                    DefeatCondition.EnemyReachPoint => "Enemy reach the exit"
                });
            StartCoroutine(WaitAndRemoveText());
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

        private IEnumerator WaitAndRemoveText()
        {
            yield return new WaitForSeconds(3f);
            _explanationText.gameObject.SetActive(false);
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
