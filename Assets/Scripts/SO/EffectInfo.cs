using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/EffectInfo", fileName = "EffectInfo")]
    public class EffectInfo : ScriptableObject
    {
        [Header("Metadata")]
        public string Name;
        public Sprite Sprite;

        [Header("Effects")]
        [Tooltip("Damage dealt by the effect")]
        public SkillInfo AdditionalDamage;
        [Tooltip("When this effect is active, it cancel out the following effects")]
        public EffectInfo[] Cancels;
        [Tooltip("Duration the effect is active by")]
        public int Duration;
        [Tooltip("Does the effect prevent the character to move")]
        public bool PreventMovement;
        public bool DoesAggro;

        public override bool Equals(object obj)
        {
            return obj is EffectInfo info &&
                   this == info;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(EffectInfo a, EffectInfo b)
        {
            if (a is null) return b is null;
            if (b is null) return false;
            return a.Name == b.Name;
        }
        public static bool operator !=(EffectInfo a, EffectInfo b)
            => !(a == b);
    }
}