using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuriGameJam2023.Effect
{
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
