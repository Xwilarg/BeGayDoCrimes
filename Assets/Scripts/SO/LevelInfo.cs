using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        [Header("Level")]
        [Tooltip("Scene to load for the current level")]
        public string SceneName;
        [Tooltip("Additional defeat condition along with loosing the whole team")]
        public DefeatCondition AdditionalDefeatCondition;
        [Tooltip("Victory condition")]
        public VictoryCondition VictoryCondition;
        [Tooltip("Story to be played before the fight")]
        public TextAsset PreBattleVN;
        [Tooltip("Story to be played after winning")]
        public TextAsset PostVictoryVN;
        [Tooltip("Do we move the camera at a specific point while playing the story?")]
        public bool MoveCamOnVictory;
        [Tooltip("Position where we are moving the camera while playing the story")]
        public Vector2 CamPosOnVictory;

        [Header("Firecamp")]
        [Tooltip("Scene to be loaded on top of the firecamp")]
        public string FirecampSceneName;
        [Tooltip("Story to be played when entering the camp")]
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
        AllReachPoint,
        PlantBomb
    }
}