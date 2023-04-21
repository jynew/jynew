using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class NavigationCamera : MonoBehaviour
    {
        public static float fov;
        public SO_NavigationCamera data;

        Quaternion rot;
        Vector3 currentSpeed;
        float tStamp;
        float deltaTime;

        Vector3 startPosition, position;
        Quaternion startRotation;
        float scrollWheel;

        void Awake()
        {
            tStamp = Time.realtimeSinceStartup;

            startPosition = position = transform.position;
            startRotation = rot = transform.rotation;
        }
         
        void OnDestroy()
        {
            RestoreCam();
        }

        void Update()
        {
            scrollWheel = Input.mouseScrollDelta.y * data.mouseScrollWheelMulti;
        }

        void LateUpdate()
        {
            deltaTime = Time.realtimeSinceStartup - tStamp;
            tStamp = Time.realtimeSinceStartup;

            Vector2 deltaMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButtonDown(1))
            {
                rot = transform.rotation;
                deltaMouse = Vector2.zero;
            }

            Vector3 speed = Vector3.zero;

            if (Input.GetMouseButton(1))
            {
                // rot *= Quaternion.Euler(new Vector3(-deltaMouse.y * data.mouseSensitity * deltaTime * 100, deltaMouse.x * data.mouseSensitity * deltaTime * 100, 0));

                Quaternion oldRot = transform.rotation;
                transform.Rotate(0, deltaMouse.x * data.mouseSensitity * 1.66f, 0, Space.World);
                transform.Rotate(-deltaMouse.y * data.mouseSensitity * 1.66f, 0, 0, Space.Self);
                rot = transform.rotation;
                transform.rotation = oldRot;

                if (Input.GetKey(KeyCode.W)) speed.z = 1;
                else if (Input.GetKey(KeyCode.S)) speed.z = -1;

                if (Input.GetKey(KeyCode.D)) speed.x = 1;
                else if (Input.GetKey(KeyCode.A)) speed.x = -1;

                if (Input.GetKey(KeyCode.E)) speed.y = 1;
                else if (Input.GetKey(KeyCode.Q)) speed.y = -1;

                speed *= GetSpeedMulti();
            }

            if (Input.GetMouseButton(2))
            {
                speed.x = -deltaMouse.x;
                speed.y = -deltaMouse.y;

                speed *= GetSpeedMulti();
                currentSpeed = speed;
            }
            else Lerp2Way(ref currentSpeed, speed, data.speedUpLerpMulti, data.speedDownLerpMulti);

            position += transform.TransformDirection(currentSpeed * deltaTime) + (transform.forward * scrollWheel * deltaTime);

            transform.rotation = rot;
            transform.position = position;
        }

        public void SetCam()
        {
            transform.rotation = rot;
            transform.position = position;
        }

        public void RestoreCam()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        float GetSpeedMulti()
        {
            if (Input.GetKey(KeyCode.LeftShift)) return data.speedFast;
            if (Input.GetKey(KeyCode.LeftControl)) return data.speedSlow;
            else return data.speedNormal;
        }

        void Lerp2Way(ref Vector3 v, Vector3 targetV, float upMulti, float downMulti)
        {
            Lerp2Way(ref v.x, targetV.x, upMulti, downMulti);
            Lerp2Way(ref v.y, targetV.y, upMulti, downMulti);
            Lerp2Way(ref v.z, targetV.z, upMulti, downMulti);
        }

        void Lerp2Way(ref float v, float targetV, float upMulti, float downMulti)
        {
            float multi;
            if (Mathf.Abs(v) < Mathf.Abs(targetV)) multi = upMulti; else multi = downMulti;
            v = Mathf.Lerp(v, targetV, multi * deltaTime);
        }
    }
}