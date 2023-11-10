using UnityEngine;
using UnityEngine.SceneManagement;
using YuriGameJam2023.Persistency;

namespace YuriGameJam2023.Menu
{
    public class LevelSelect : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("DebugManager", LoadSceneMode.Additive);
        }

        public void PlayLevel(int level)
        {
            PersistencyManager.Instance.SaveData.CurrentLevel = level;
            SceneManager.LoadScene("Main");
        }
    }
}