using UnityEngine;
using UnityEngine.SceneManagement;

namespace YuriGameJam2023.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
