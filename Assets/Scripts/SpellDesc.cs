using TMPro;
using UnityEngine;
using YuriGameJam2023.SO;

namespace YuriGameJam2023
{
    public class SpellDesc : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _titleText;

        [SerializeField]
        private TMP_Text _descText;

        [SerializeField]
        private GameObject _dmgGroup;

        [SerializeField]
        private TMP_Text _dmgText;

        private void Start()
        {
            Debug.Assert(_descText != null);
            Debug.Assert(_dmgGroup != null);
            Debug.Assert(_dmgText != null);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetSpell(SkillInfo skill)
        {
            _titleText.text = skill.Name;
            _descText.text = skill.Description;

            if (skill.Damage > 0)
            {
                _dmgGroup.SetActive(true);
                _dmgText.text = "Dmg: " + skill.Damage;
            }
            else
            {
                _dmgGroup.SetActive(false);
            }
        }
    }
}