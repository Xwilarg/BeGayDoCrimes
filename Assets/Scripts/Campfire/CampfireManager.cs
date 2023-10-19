using UnityEngine;
using UnityEngine.SceneManagement;
using YuriGameJam2023.Player;

namespace YuriGameJam2023.Campfire
{
    public class CampfireManager : MonoBehaviour
    {
        private Light _selected;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Update()
        {
            if (_selected != null)
            {
                _selected.gameObject.SetActive(false);
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
            {
                _selected = hit.collider.GetComponentInChildren<Light>();
                _selected.gameObject.SetActive(true);
            }
            else
            {
                _selected = null;
            }
        }
    }
}
