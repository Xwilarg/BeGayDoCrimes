using System;
using System.Linq;
using UnityEngine;

namespace YuriGameJam2023.Map
{
    public class TrackScroller : MonoBehaviour
    {
        [SerializeField]
        private Scrollable[] _scrollables;

        private void Awake()
        {
            foreach (var s in _scrollables)
            {
                s.Min = s.Elements.Min(x => x.transform.position.z);
                s.Max = (s.Elements.Max(x => x.transform.position.z) - s.Min) + s.Size;
            }
        }

        private void Update()
        {
            foreach (var s in _scrollables)
            {
                foreach (var e in s.Elements)
                {
                    e.transform.Translate(-Time.deltaTime * s.Speed, 0f, 0f);
                    if (e.transform.position.z < s.Min)
                    {
                        e.transform.Translate(s.Max, 0f, 0f);
                    }
                }
            }
        }
    }

    [Serializable]
    public class Scrollable
    {
        [SerializeField]
        public float Size;

        [SerializeField]
        public GameObject[] Elements;

        [SerializeField]
        public float Speed;

        public float Min { set; get; }
        public float Max { set; get; }
    }
}
