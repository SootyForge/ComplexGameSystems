using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckersMkII
{
    public class CameraOrbit : MonoBehaviour
    {
        // Distance the camera is from world zero
        public float distance = 10f;
        // X and Y rotation speed
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        // X and Y rotation limits
        public float yMin = 15f;
        public float yMax = 80f;
        // Current x & y rotation
        private float x = 0.0f;
        private float y = 0.0f;

        // Use this for initialization
        void Start()
        {
            // Get current rotation of camera
            Vector3 euler = transform.eulerAngles;
            x = euler.y;
            y = euler.x;
        }

        // Called every frame after Update
        void LateUpdate()
        {
            // Is the Left mouse button pressed?
            if (Input.GetMouseButton(1))
            {
                // Hide the cursor
                Cursor.visible = false;
                // Get input X and Y offsets
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                // Offset rotation with mouse X and Y offsets
                x += mouseX * xSpeed * Time.deltaTime;
                y -= mouseY * ySpeed * Time.deltaTime;
                // Clamp the Y between min and max limits
                y = Mathf.Clamp(y, yMin, yMax);
            }
            // Is the left button not pressed?
            else
            {
                // Show the cursor
                Cursor.visible = true;
            }
            // Update transform
            transform.rotation = Quaternion.Euler(y, x, 0);
            transform.position = -transform.forward * distance;
        }
    } 
}
