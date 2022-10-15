using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform _cameraPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = _cameraPos.position;
    }
}
