using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YuriGameJam2023.Persistency;

namespace YuriGameJam2023.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _achievementPanel;

        [SerializeField]
        private TMP_Text _title, _description;

        public static AchievementManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Unlock(AchievementID achievement)
        {
            if (PersistencyManager.Instance.SaveData.UnlockedAchievements.Contains(achievement))
            {
                return;
            }
            var data = Achievements[achievement];
            _title.text = data.Name;
            _description.text = data.Description;
            _achievementPanel.SetActive(true);
            PersistencyManager.Instance.SaveData.UnlockedAchievements.Add(achievement);
            PersistencyManager.Instance.Save();
            StartCoroutine(WaitAndClosePopup());
        }

        private IEnumerator WaitAndClosePopup()
        {
            yield return new WaitForSeconds(3f);
            _achievementPanel.SetActive(false);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.CompleteGame, new() { Name = "Be Gay, Do Crimes", Description = "Complete the game" } }
        };
    }

    public enum AchievementID
    {
        CompleteGame
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}
