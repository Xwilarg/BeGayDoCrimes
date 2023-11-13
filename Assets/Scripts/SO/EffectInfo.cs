using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/EffectInfo", fileName = "EffectInfo")]
    public class EffectInfo : ScriptableObject
    {
        [Header("Visual")]
        public string Name;
        public Sprite Sprite;
        public GameObject Vfx;

        [Header("Effects")]
        public bool IsBuff;
        [Tooltip("Damage dealt by the effect")]
        public SkillInfo AdditionalDamage;
        [Tooltip("When this effect is active, it cancel out the following effects")]
        public EffectInfo[] Cancels;
        [Tooltip("Duration the effect is active by")]
        public int Duration;
        [Tooltip("Does the effect prevent the character to move")]
        public bool PreventMovement;
        public bool DoesAggro;
        [Range(-1f, 1f)]
        public float IncreaseDamage;

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