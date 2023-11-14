using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YuriGameJam2023.Achievement
{
    public class AchievementDisplay : MonoBehaviour
    {
        public static AchievementDisplay Instance { private set; get; }

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Button _fiftyPercents;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            float ttUnlocked = 0;
            for (int i = 0; i < _container.childCount; i++) Destroy(_container.GetChild(i).gameObject);
            foreach (var achievement in AchievementManager.Instance.Achievements)
            {
                var a = Instantiate(_prefab, _container);
                var unlocked = AchievementManager.Instance.IsUnlocked(achievement.Key);
                if (!unlocked)
                {
                    a.GetComponentInChildren<Image>().color = Color.black;
                }
                else
                {
                    ttUnlocked++;
                }
                var texts = a.GetComponentsInChildren<TMP_Text>(); // TODO: Ewh
                texts[0].text = unlocked ? achievement.Value.Name : "???";
                texts[1].text = unlocked ? achievement.Value.Description : $"Hint: {achievement.Value.Hint}";
            }
            ttUnlocked /= AchievementManager.Instance.Achievements.Count;
            _fiftyPercents.interactable = ttUnlocked >= .5f;
        }
    }
}
