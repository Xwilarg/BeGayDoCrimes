using System.Collections.Generic;
using UnityEngine;

namespace YuriGameJam2023.Effect
{
    public class AoeHint : MonoBehaviour
    {
        private LineRenderer _lr;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
        }

        public void Show(Vector3 pos, int radius)
        {
            List<Vector3> positions = new();
            for (float i = 0; i < Mathf.PI * 2 + .1f; i += .1f)
            {
                positions.Add(new Vector3(pos.x + Mathf.Cos(i) * radius, 0f, pos.z + Mathf.Sin(i) * radius));
            }
            _lr.positionCount = positions.Count;
            _lr.SetPositions(positions.ToArray());
        }
    }
}
