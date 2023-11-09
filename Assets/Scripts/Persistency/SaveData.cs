using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace YuriGameJam2023.Persistency
{
    public class SaveData
    {
        /// <summary>
        /// Make a suppport conversation available to be played
        /// </summary>
        public void UnlockSupport(string support)
        {
            UnityEngine.Debug.Log($"[SUPPORT] Unlocking support {support}");
            if (!AvailableSupport.ContainsKey(support))
            {
                AvailableSupport.Add(support, 0);
            }
            else
            {
                AvailableSupport[support] += 1;
            }
        }

        /// <summary>
        /// Get the current support level for the parameter
        /// </summary>
        public int GetCurrentSupportLevel(string support)
            => SupportData.ContainsKey(support) ? SupportData[support] : 0;

        /// <summary>
        /// Is the current support far enough to reach the level in parameter (so to be played)
        /// </summary>
        public bool CanPlaySupport(string support, int level)
            => AvailableSupport.ContainsKey(support) && AvailableSupport[support] >= level && (!SupportData.ContainsKey(support) || SupportData[support] < level);

        /// <summary>
        /// Play a support conversation
        /// </summary>
        public void PlaySupport(string support)
        {
            if (!SupportData.ContainsKey(support))
            {
                SupportData.Add(support, 0);
            }
            else
            {
                SupportData[support] += 1;
            }
        }

        public int CurrentLevel { set; get; } = 1;

        [EditorBrowsable(EditorBrowsableState.Never)] // TODO: Doesn't work
        public Dictionary<string, int> SupportData { set; get; } = new();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<string, int> AvailableSupport { set; get; } = new();

        public override string ToString()
        {
            return $"[Current Level] {CurrentLevel}\n" +
                $"[Available Support] {string.Join(", ", AvailableSupport.Select(x => $"{x.Key} => {x.Value}"))}\n" +
                $"[Support Data] {string.Join(", ", SupportData.Select(x => $"{x.Key} => {x.Value}"))}\n";
        }
    }
}
