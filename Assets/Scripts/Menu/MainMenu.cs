using UnityEngine;
using UnityEngine.SceneManagement;
using YuriGameJam2023.Persistency;

namespace YuriGameJam2023.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _oldVersion, _newVersion;

        private void Awake()
        {
            if (PersistencyManager.Instance.SaveData.CurrentLevel == 5)
            {
                _newVersion.gameObject.SetActive(true);
            }
            else
            {
                _oldVersion.gameObject.SetActive(true);
            }
        }

        public void Play()
        {
            if (PersistencyManager.Instance.SaveData.CurrentLevel == 5)
            {
                PersistencyManager.Instance.DeleteSaveFolder();
            }
            SceneManager.LoadScene("Main");
        }
    }
}
