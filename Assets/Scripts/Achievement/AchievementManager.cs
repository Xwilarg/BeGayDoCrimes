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
            { AchievementID.CompleteGame, new() { Name = "Be Gay, Do Crimes", Description = "Complete the game" } },
            { AchievementID.JumpTrainYuki, new() { Name = "So Long, and Thanks for All the Fish", Description = "Jump out of the train with Yuki" } },
            { AchievementID.AggroFriend, new() { Name = "Keep your Friends Close, and your Enemies Closer", Description = "Attempt to use aggro spell on an allie with Willow" } },
            { AchievementID.Credits, new() { Name = "The Curtain Rises", Description = "Open the credits" } },
            { AchievementID.Petanque, new() { Name = "Eggs Finder", Description = "Play a full game of pétanque" } },
            { AchievementID.Effects3, new() { Name = "Feeling Under the Weather?", Description = "Have an enemy under 3 differents effects" } },

            { AchievementID.Rel_MC, new() { Name = "Sticky Parental Issues", Description = "Reach max support for Makra and Claire" } },
            { AchievementID.Rel_WC, new() { Name = "Not Fucking Around", Description = "Reach max support for Willow and Claire"} },
            { AchievementID.Rel_WM, new() { Name = "A Love Sweater than Cinnamon", Description = "Reach max support for Willow and Makra"} },
            { AchievementID.Rel_YC, new() { Name = "Burning Dreams for the Future", Description = "Reach max support for Yuki and Claire"} },
            { AchievementID.Rel_YM, new() { Name = "Red Thread of Fate", Description = "Reach max support for Yuki and Makra"} },
            { AchievementID.Rel_YW, new() { Name = "My Heart Beating for You", Description = "Reach max support for Yuki and Willow"} }
        };
    }

    public enum AchievementID
    {
        CompleteGame,
        JumpTrainYuki,
        AggroFriend,
        Petanque,
        Credits,
        Effects3,

        Rel_MC,
        Rel_WC,
        Rel_WM,
        Rel_YC,
        Rel_YM,
        Rel_YW
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}
