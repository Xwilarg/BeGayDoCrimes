using UnityEngine;

namespace YuriGameJam2023.Campfire
{
    public class CharacterCamp : MonoBehaviour
    {
        [SerializeField]
        private Light _light;

        [SerializeField]
        private GameObject _interaction;

        public void ToggleLight(bool value)
        {
            _light.enabled = value;
        }

        public void ToggleInteraction(bool value)
        {
            _interaction.SetActive(value);
        }
    }
}
