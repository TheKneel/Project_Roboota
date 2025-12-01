using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera Camera;

    public float xSensitivity = 10f;
    public float ySensitivity = 10f;

    private float xRotation = 0f;

    public void ProcessLook(Vector2 input) 
    { 
        float mouseX = input.x;
        float mouseY = input.y;
        //calculate roation of mouse to look up and down 
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        //apply this to camera transform
        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //rotate our player to left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
