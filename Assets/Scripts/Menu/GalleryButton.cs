using UnityEngine;
using YuriGameJam2023.SO;

namespace YuriGameJam2023.Menu
{
    public class GalleryButton : MonoBehaviour
    {
        [SerializeField]
        private GalleryImageInfo[] _images;

        [SerializeField]
        private Gallery _gallery;

        public void OpenGallery()
        {
            _gallery.gameObject.SetActive(true);
            _gallery.SetData(_images);
        }
    }
}
