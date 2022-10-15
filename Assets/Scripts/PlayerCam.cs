using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] float _xSen;
    [SerializeField] float _ySen;

    [SerializeField] Transform _orientation;

    float _xRotation;
    float _yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        CursorControl();
    }

    void CursorControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKey(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void RotateCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _xSen;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _ySen;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }
}
