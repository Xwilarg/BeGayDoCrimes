using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YuriGameJam2023.SO;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(Billboard))]
public class EffectParticle : MonoBehaviour
{
    [SerializeField]
    private float _maxLifetime = 3.0f;
    [SerializeField]
    private AnimationCurve _displayCurve = AnimationCurve.Linear(0.0f, 1.0f,1.0f, 0.0f);
    [SerializeField]
    private Color positiveColor = Color.white;
    [SerializeField]
    private Color negativeColor = Color.white;
    [SerializeField]
    private float speed = 1.0f;

    private float _lifetime = 0.0f;

    TMP_Text _text;
    void Awake()
    {
        _text = this.GetComponent<TMP_Text>();
    }

    public string Name
    {
        set =>_text.text = value;
    }

    public bool Added
    {
        set;
        get;
    }

    void Update()
    {
        _lifetime += Time.deltaTime;
        this.transform.localPosition += Vector3.up * speed;
        if (_lifetime > _maxLifetime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Color current = Added ? negativeColor : positiveColor;
            if (_maxLifetime > 0.0f) {
                current.a *= _displayCurve.Evaluate(_lifetime / _maxLifetime);
            }
            _text.color = current;
        }
    }
}
