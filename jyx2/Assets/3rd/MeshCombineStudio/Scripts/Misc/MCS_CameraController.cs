using UnityEngine;
using System.Collections;


namespace MeshCombineStudio
{
    public class MCS_CameraController : MonoBehaviour {

        public float speed = 10;
        public float mouseMoveSpeed = 1;
        public float shiftMulti = 3f;
        public float controlMulti = 0.5f;

        Vector3 oldMousePosition;
        GameObject cameraMountGO, cameraChildGO;

        Transform cameraMountT, cameraChildT, t;

        private void Awake()
        {
            t = transform;
            CreateParents();
        }

        void CreateParents()
        {
            cameraMountGO = new GameObject("CameraMount");
            cameraChildGO = new GameObject("CameraChild");

            cameraMountT = cameraMountGO.transform;
            cameraChildT = cameraChildGO.transform;

            cameraChildT.SetParent(cameraMountT);

            cameraMountT.position = t.position;
            cameraMountT.rotation = t.rotation;

            t.SetParent(cameraChildT);
        }

        private void Update()
        {
            Vector3 deltaMouse = (Input.mousePosition - oldMousePosition) * mouseMoveSpeed * (Time.deltaTime * 60);

            if (Input.GetMouseButton(1))
            {
                cameraMountT.Rotate(0, deltaMouse.x, 0, Space.Self);
                cameraChildT.Rotate(-deltaMouse.y, 0, 0, Space.Self);
            }

            oldMousePosition = Input.mousePosition;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move.z = speed;
            else if (Input.GetKey(KeyCode.S)) move.z = -speed;
            else if (Input.GetKey(KeyCode.A)) move.x = -speed;
            else if (Input.GetKey(KeyCode.D)) move.x = speed;
            else if (Input.GetKey(KeyCode.Q)) move.y = -speed;
            else if (Input.GetKey(KeyCode.E)) move.y = speed;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) move *= shiftMulti;
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) move *= controlMulti;

            move *= Time.deltaTime * 60;

            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(cameraChildT.eulerAngles.x, cameraMountT.eulerAngles.y, 0);
            move = rotation * move;
            // move = cameraMountT.rotation * move;

            cameraMountT.position += move;
        }
    }
}