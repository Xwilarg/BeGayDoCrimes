﻿using UnityEngine;

namespace YuriGameJam2023.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CharacterInfo", fileName = "CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public string Name;
        public int Health;
        public SkillInfo[] Skills;
        public Sprite Sprite;
    }
}