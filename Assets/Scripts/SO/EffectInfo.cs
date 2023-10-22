using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/EffectInfo", fileName = "EffectInfo")]
    public class EffectInfo : ScriptableObject
    {
        public EffectType Effect;
        public Sprite Sprite;
    }
}