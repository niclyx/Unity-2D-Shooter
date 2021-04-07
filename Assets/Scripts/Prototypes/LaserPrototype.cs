using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPrototype : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Vector3.up * 6f * Time.deltaTime);
    }
}
