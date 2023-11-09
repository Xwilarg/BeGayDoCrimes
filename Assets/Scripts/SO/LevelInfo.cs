using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        [Header("Level")]
        public string SceneName;
        public DefeatCondition AdditionalDefeatCondition;
        public VictoryCondition VictoryCondition;

        [Header("Firecamp")]
        public string FirecampSceneName;
        public TextAsset FirecampVN;
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