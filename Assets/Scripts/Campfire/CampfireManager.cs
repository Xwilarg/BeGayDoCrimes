using UnityEngine;
using UnityEngine.SceneManagement;

namespace YuriGameJam2023.Campfire
{
    public class CampfireManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }
    }
}
