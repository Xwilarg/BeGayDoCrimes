using UnityEngine;
using UnityEngine.SceneManagement;

namespace YuriGameJam2023.Menu
{
    public class MenuCampfire : MonoBehaviour
    {
        [SerializeField]
        private Light _light;
        private Color _orColor;

        private void Awake()
        {
            _orColor = _light.color;
        }

        private void OnMouseEnter()
        {
            _light.color = Color.red;
        }

        private void OnMouseExit()
        {
            _light.color = _orColor;
        }

        private void OnMouseUpAsButton()
        {
            SceneManager.LoadScene("Pétanque");
        }
    }
}
