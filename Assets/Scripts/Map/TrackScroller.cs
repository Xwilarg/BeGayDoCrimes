using UnityEngine;

namespace YuriGameJam2023.Map
{
    public class TrackScroller : MonoBehaviour
    {
        [SerializeField]
        private float _size;

        [SerializeField]
        private float _min;

        [SerializeField]
        private GameObject[] _tracks;

        [SerializeField]
        private float _speed;

        private void Update()
        {
            foreach (var track in _tracks)
            {
                track.transform.Translate(-Time.deltaTime * _speed, 0f, 0f);
                if (track.transform.position.z < _min)
                {
                    track.transform.Translate(-_min * 2f, 0f, 0f);
                }
            }
        }
    }
}
