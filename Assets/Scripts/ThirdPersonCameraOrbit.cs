using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraOrbit : MonoBehaviour
{
    public Transform cameraPivot;
    public Transform mainCamera;

    public float mouseSensitivity = 120f;
    public float minY = -20f;
    public float maxY = 45f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}