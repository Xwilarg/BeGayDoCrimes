using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/SkillInfo", fileName = "SkillInfo")]
    public class SkillInfo : ScriptableObject
    {
        public RangeType Type;
        public int Range;
        public int Damage;
        public EffectInfo[] Effects;
        public Sprite Sprite;
    }
}