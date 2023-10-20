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

        public override bool Equals(object obj)
            => obj is CharacterCamp camp && name == camp.name;

        public override int GetHashCode()
            => name.GetHashCode();

        public static bool operator ==(CharacterCamp a, CharacterCamp b)
        {
            if (a is null) return b is null;
            if (b is null) return false;
            return a.name == b.name;
        }

        public static bool operator !=(CharacterCamp a, CharacterCamp b)
            => !(a == b);
    }
}
