using System;
using UnityEngine;
using YuriGameJam2023.Achievement;

namespace YuriGameJam2023.Campfire
{
    [Serializable]
    public class Couple
    {
        public CharacterCamp A;
        public CharacterCamp B;

        public TextAsset[] Stories;

        public AchievementID MaxRankAchievement;
    }
}
