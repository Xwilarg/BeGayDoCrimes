using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/SkillInfo", fileName = "SkillInfo")]
    public class SkillInfo : ScriptableObject
    {
        public SkillType Type;
        public int Range;
    }
}