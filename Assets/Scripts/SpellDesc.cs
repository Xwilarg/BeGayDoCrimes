using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YuriGameJam2023.SO;
public class SpellDesc : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _descText;
    
    [SerializeField]
    private GameObject _dmgGroup;
    
    [SerializeField]
    private TMP_Text _dmgText;

    [SerializeField]
    private Transform _effectGroup;

    [SerializeField]
    private GameObject _effectPrefab;

    private void Start() {
        Debug.Assert(_descText != null);
        Debug.Assert(_dmgGroup != null);
        Debug.Assert(_dmgText != null);
        Debug.Assert(_effectGroup != null);
        Debug.Assert(_effectPrefab != null);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Show() {
        this.gameObject.SetActive(true);
    }

    public void SetSpell(SkillInfo skill) {
        _descText.text = skill.Description;
        if (skill.Damage > 0) {
            _dmgGroup.SetActive(true);
            _dmgText.text = "Dmg: " + skill.Damage;
        }
        else
        {
            _dmgGroup.SetActive(false);
        }
        for (int i = 0; i < _effectGroup.childCount; i++) Destroy(_effectGroup.GetChild(i).gameObject);
        for (int i = 0; i < skill.Effects.Length; i++)
        {
            var go = Instantiate(_effectPrefab, _effectGroup);
            go.GetComponent<Image>().sprite = skill.Effects[i].Sprite;
        }
    }
}
