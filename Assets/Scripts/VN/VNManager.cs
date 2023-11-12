using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YuriGameJam2023.SO;

namespace YuriGameJam2023.VN
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { private set; get; }

        [SerializeField]
        private TextDisplay _display;

        [SerializeField]
        private VNCharacterInfo[] _characters;
        private List<VNCharacterInfo> _currentCharacter;

        private Story _story;

        [SerializeField]
        private GameObject _container;

        [SerializeField]
        private GameObject _namePanel;

        [SerializeField]
        private Transform _bustContainer;

        [SerializeField]
        private Image _bustImagePrefab;

        [SerializeField]
        private TMP_Text _nameText;

        private bool _isSkipEnabled;
        private float _skipTimer;
        private float _skipTimerRef = .1f;

        private Action _onDone;

        private void Awake()
        {
            Instance = this;
        }

        public bool IsPlayingStory => _container.activeInHierarchy;

        private void Update()
        {
            if (_isSkipEnabled)
            {
                _skipTimer -= Time.deltaTime;
                if (_skipTimer < 0)
                {
                    _skipTimer = _skipTimerRef;
                    DisplayNextDialogue();
                }
            }
        }

        public void ShowStory(TextAsset asset, Action onDone)
        {
            Debug.Log($"[STORY] Playing {asset.name}");
            _display.SetStyle(FontStyles.Normal);
            _currentCharacter = new();
            _onDone = onDone;
            _story = new(asset.text);
            _isSkipEnabled = false;
            DisplayStory(_story.Continue());
        }

        private void DisplayStory(string text)
        {
            _container.SetActive(true);

            for (int i = 0; i < _bustContainer.childCount; i++) Destroy(_bustContainer.GetChild(i).gameObject);
            
            foreach (var tag in _story.currentTags)
            {
                var s = tag.Split(' ');
                var contentList = s.Skip(1).Select(x => x.ToUpperInvariant());
                var content = string.Join(' ', contentList);
                switch (s[0])
                {
                    case "speaker":
                        _currentCharacter = new();
                        if (content == "NONE")
                        {
                            Debug.Log($"[STORY] Speaker set to none");
                        }
                        else
                        {
                            foreach (var c in contentList)
                            {
                                var target = _characters.FirstOrDefault(x => x.Name.ToUpperInvariant() == c);
                                if (target == null)
                                {
                                    Debug.LogError($"[STORY] Unable to find character {c}");
                                }
                                else
                                {
                                    _currentCharacter.Add(target);
                                }
                            }
                            Debug.Log($"[STORY] Speaker set to {string.Join(", ", _currentCharacter.Select(x => x.Name))}");
                        }
                        break;

                    case "format":
                        if (content == "NONE") _display.SetStyle(FontStyles.Normal);
                        else if (content == "ITALIC") _display.SetStyle(FontStyles.Italic);
                        else Debug.LogError($"[STORY] Unable to find format {content}");
                        break;

                    default:
                        Debug.LogError($"Unknown story key: {s[0]}");
                        break;
                }
            }
            _display.ToDisplay = text;
            if (!_currentCharacter.Any())
            {
                _namePanel.SetActive(false);
            }
            else
            {
                _namePanel.SetActive(true);
                _nameText.text = string.Join(" & ", _currentCharacter.Select(x => x.DisplayName));
                _bustContainer.gameObject.SetActive(false);
                foreach (var c in _currentCharacter)
                {
                    if (c.Sprite != null)
                    {
                        _bustContainer.gameObject.SetActive(true);
                        var bust = Instantiate(_bustImagePrefab, _bustContainer);
                        bust.GetComponent<Image>().sprite = c.Sprite;
                    }
                }
            }
        }

        public void DisplayNextDialogue()
        {
            if (!_container.activeInHierarchy)
            {
                return;
            }
            if (!_display.IsDisplayDone)
            {
                // We are slowly displaying a text, force the whole display
                _display.ForceDisplay();
            }
            else if (_story.canContinue && // There is text left to write
                !_story.currentChoices.Any()) // We are not currently in a choice
            {
                DisplayStory(_story.Continue());
            }
            else if (!_story.canContinue && !_story.currentChoices.Any())
            {
                _container.SetActive(false);
                _onDone?.Invoke();
            }
        }

        public void ToggleSkip(bool value)
            => _isSkipEnabled = value;

        public void OnNextDialogue(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DisplayNextDialogue();
            }
        }
    }
}