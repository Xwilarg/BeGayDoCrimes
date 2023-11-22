using UnityEngine;
using UnityEngine.UI;
using YuriGameJam2023.Persistency;

namespace YuriGameJam2023.Menu
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private Slider _musicSlider;

        [SerializeField]
        private Toggle _easyMode;

        [SerializeField]
        private AudioSource _bgm;

        private void Awake()
        {
            Revert();
        }

        public void OnVolumeChange(float value)
        {
            PersistencyManager.Instance.SaveData.MusicVolume = value;
            _bgm.volume = value;
        }

        public void OnEasyModeChange(bool value)
        {
            PersistencyManager.Instance.SaveData.IsEasyMode = value;
        }

        public void Apply()
        {
            PersistencyManager.Instance.Save();
        }

        public void Revert()
        {
            _musicSlider.value = PersistencyManager.Instance.SaveData.MusicVolume;
            _easyMode.isOn = PersistencyManager.Instance.SaveData.IsEasyMode;
        }
    }
}
