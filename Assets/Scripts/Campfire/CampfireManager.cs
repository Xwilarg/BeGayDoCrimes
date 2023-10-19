using UnityEngine;
using UnityEngine.SceneManagement;

namespace YuriGameJam2023.Campfire
{
    public class CampfireManager : MonoBehaviour
    {
        private CharacterCamp _selected;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Update()
        {
            if (_selected != null)
            {
                _selected.Toggle(false);
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.CompareTag("Player"))
            {
                _selected = hit.collider.GetComponent<CharacterCamp>();
                _selected.Toggle(true);
            }
            else
            {
                _selected = null;
            }
        }
    }
}
