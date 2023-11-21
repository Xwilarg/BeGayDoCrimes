using UnityEngine;

namespace YuriGameJam2023.Effect
{
    public class CloseRangeHint : MonoBehaviour
    {
        private LineRenderer[] _lrs;

        private void Awake()
        {
            _lrs = GetComponentsInChildren<LineRenderer>();
        }

        public void Show(Vector3 pos)
        {
            _lrs[0].positionCount = 2;
            _lrs[1].positionCount = 2;

            var dist = .25f;
            _lrs[0].SetPositions(new[]
            {
                new Vector3(pos.x - dist, 0f, pos.z + dist),
                new Vector3(pos.x + dist, 0f, pos.z - dist),
            });
            _lrs[1].SetPositions(new[]
            {
                new Vector3(pos.x - dist, 0f, pos.z - dist),
                new Vector3(pos.x + dist, 0f, pos.z + dist),
            });
        }
    }
}
