using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace YuriGameJam2023.Campfire
{
    public class CharacterCamp : MonoBehaviour
    {
        [SerializeField]
        private Light _light;

        [SerializeField]
        private GameObject _interaction;

        [SerializeField]
        private TextAsset _randomLines;

        [SerializeField]
        private TMP_Text _sentenceText;

        private string[] _sentences;

        private void Awake()
        {
            _sentences = _sentenceText.text.Replace("\r", string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerator ShowRandomSentence()
        {
            _sentenceText.gameObject.SetActive(true);
            _sentenceText.text = _sentences[UnityEngine.Random.Range(0, _sentences.Length)];
            yield return new WaitForSeconds(5f);
            _sentenceText.gameObject.SetActive(false);
        }

        public void HideSentence()
        {
            _sentenceText.gameObject.SetActive(false);
        }

        public SO.CharacterInfo Info;

        public void ToggleLight(bool value)
        {
            _light.enabled = value;
        }

        public void ToggleInteraction(bool value)
        {
            _interaction.SetActive(value);
        }

        public override bool Equals(object obj)
            => obj is CharacterCamp camp && this == camp;

        public override int GetHashCode() // I'm very sorry
            => Info?.Name?.GetHashCode() ?? 0;

        public static bool operator ==(CharacterCamp a, CharacterCamp b)
        {
            if (a is null) return b is null;
            if (b is null) return false;

            return a.Info?.Name == b.Info?.Name;
        }

        public static bool operator !=(CharacterCamp a, CharacterCamp b)
            => !(a == b);

        public override string ToString()
        {
            return Info.Name;
        }
    }
}
