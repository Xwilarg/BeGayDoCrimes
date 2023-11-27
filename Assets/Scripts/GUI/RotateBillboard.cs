using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuriGameJam2023.GUI {
    public class RotateBillboard : MonoBehaviour
    {
    
        // Update is called once per frame
        void Update()
        {
            this.transform.localRotation *= Vector3.Dot(this.transform.position - Camera.main.transform.position, this.transform.forward) > 0.0f ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }
}
