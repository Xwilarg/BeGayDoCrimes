using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YuriGameJam2023.SO;

namespace YuriGameJam2023.Menu
{
    public class Gallery : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Image _image;

        private GalleryImageInfo[] _data;

        private int _index;

        public void SetData(GalleryImageInfo[] data)
        {
            _data = data;
            _index = 0;

            _text.text = _data[_index].Text;
            _image.sprite = _data[_index].Sprite;
        }

        public void Next()
        {
            _index++;
            if (_index == _data.Length)
            {
                _index = 0;
            }

            _text.text = _data[_index].Text;
            _image.sprite = _data[_index].Sprite;
        }
    }
}
