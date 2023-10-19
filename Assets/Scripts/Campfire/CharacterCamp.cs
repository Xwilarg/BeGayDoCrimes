using UnityEngine;

namespace YuriGameJam2023.Campfire
{
    public class CharacterCamp : MonoBehaviour
    {
        [SerializeField]
        private Light _light;

        public void Toggle(bool value)
        {
            _light.enabled = value;
        }
    }
}
