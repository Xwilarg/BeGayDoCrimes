using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuriGameJam2023.GUI
{
    public class RotateBillboard : MonoBehaviour
    {
    
        // Update is called once per frame
        void Update()
        {
            transform.localRotation *= Vector3.Dot(transform.position - Camera.main.transform.position, transform.forward) > 0.0f ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }
}
