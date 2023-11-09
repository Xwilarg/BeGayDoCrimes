using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        public string SceneName;
        public DefeatCondition AdditionalDefeatCondition;
        public VictoryCondition VictoryCondition;
    }

    public enum DefeatCondition
    {
        None,
        EnemyReachPoint
    }

    public enum VictoryCondition
    {
        KillAll,
        AllReachPoint
    }
}