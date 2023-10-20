using System.Collections.Generic;
using UnityEngine;

namespace YuriGameJam2023
{
    public static class DebugHelper
    {
        public static void DrawCircle(Vector3 position, float radius)
        {
            List<Vector3> positions = new();

            for (float i = 0; i < Mathf.PI * 2 + .1f; i += .1f)
            {
                positions.Add(new Vector3(position.x + Mathf.Cos(i) * radius, 0f, position.z + Mathf.Sin(i) * radius));
            }

            Gizmos.DrawLineStrip(positions.ToArray(), false);
        }
    }
}