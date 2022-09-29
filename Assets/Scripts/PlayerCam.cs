using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] float xSen;
    [SerializeField] float ySen;

    [SerializeField] Transform orientation;

    float xRotation;
    float yRotation;

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
        if (Input.GetKey(KeyCode.LeftAlt) && Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) && Cursor.lockState == CursorLockMode.None && Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void RotateCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSen;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySen;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
