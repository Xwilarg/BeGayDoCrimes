using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
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

        public bool IsUnlocked(AchievementID id)
            => PersistencyManager.Instance.SaveData.UnlockedAchievements.Contains(id);

        public void Unlock(AchievementID achievement)
        {
            if (IsUnlocked(achievement))
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
            yield return new WaitForSeconds(5f);
            _achievementPanel.SetActive(false);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.CompleteGame, new() { Name = "Be Gay, Do Crimes", Description = "Complete the game", Hint = "Try playing the game and come back" } },
            { AchievementID.JumpTrainYuki, new() { Name = "So Long, and Thanks for All the Fish", Description = "Jump out of the train with Yuki", Hint = "Stay safe while riding public transportations" } },
            { AchievementID.AggroFriend, new() { Name = "Keep your Friends Close, and your Enemies Closer", Description = "Attempt to use the aggro skill on an allie with Willow", Hint = "Some skills are only meant for the bad guys" } },
            { AchievementID.HideUseless, new() { Name = "Hidden in Plain Sight", Description = "Attempt to use the hide skill when all your allies are out as Makra", Hint = "It's too late for that now!" } },
            { AchievementID.Credits, new() { Name = "The Curtain Rises", Description = "Open the credits", Hint = "Show respect to some great people" } },
            { AchievementID.Petanque, new() { Name = "Cracking the Egg", Description = "Play a full game of pétanque", Hint = "Get more of these and come back" } },
            { AchievementID.Effects4, new() { Name = "Feeling Under the Weather?", Description = "Have an enemy under 4 differents effects", Hint = "The hardest stains requires the strongest cleaning efforts" } },
            { AchievementID.Insult3, new() { Name = "I'm not a Rapper", Description = "Insult 3 enemies at once", Hint = "With bad enough words, you can easily ruin everyone day!" } },
            { AchievementID.Cancel, new() { Name = "Innovative Strategy", Description = "Attempt to use both spiderweb and fire on a target", Hint = "Who through playing this game would require paying attention?!" } },
            { AchievementID.KillGeneral, new() { Name = "Four Girls Army", Description = "Kill the general in the last level", Hint = "Kill the strongest enemy of the game then go directly to join without passing go" } },
            { AchievementID.BurnHouse, new() { Name = "What are we Even Fighting for Anymore", Description = "Attempt to burn the girl's house", Hint = "The first level sure would go by faster if the plot point were to disappear" } },

            { AchievementID.Rel_MC, new() { Name = "Sticky Parental Issues", Description = "Reach max support for Makra and Claire", Hint = "Get two specifics characters to love each other enough" } },
            { AchievementID.Rel_WC, new() { Name = "Not Fucking Around", Description = "Reach max support for Willow and Claire", Hint = "Get two specifics characters to love each other enough"} },
            { AchievementID.Rel_WM, new() { Name = "A Love Sweeter than Cinnamon", Description = "Reach max support for Willow and Makra", Hint = "Get two specifics characters to love each other enough"} },
            { AchievementID.Rel_YC, new() { Name = "Burning Dreams for the Future", Description = "Reach max support for Yuki and Claire", Hint = "Get two specifics characters to love each other enough"} },
            { AchievementID.Rel_YM, new() { Name = "Red Thread of Fate", Description = "Reach max support for Yuki and Makra", Hint = "Get two specifics characters to love each other enough"} },
            { AchievementID.Rel_YW, new() { Name = "My Heart Beats for You", Description = "Reach max support for Yuki and Willow", Hint = "Get two specifics characters to love each other enough"} }
        };
    }

    public enum AchievementID
    {
        CompleteGame,
        JumpTrainYuki,
        AggroFriend,
        Petanque,
        Credits,
        Effects4,
        HideUseless,
        Insult3,
        Cancel,
        KillGeneral,
        BurnHouse,

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
        public string Hint;
    }
}
