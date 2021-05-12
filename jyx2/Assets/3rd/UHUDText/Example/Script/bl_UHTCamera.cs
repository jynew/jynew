using UnityEngine;
using System.Collections;

public class bl_UHTCamera : MonoBehaviour {

    // WASDQE Panning
    public float minPanSpeed = 0.1f;    // Starting panning speed
    public float maxPanSpeed = 1000f;   // Max panning speed
    public float panTimeConstant = 20f; // Time to reach max panning speed

    // Mouse right-down rotation
    public float rotateSpeed = 10; // mouse down rotation speed about x and y axes
    public float zoomSpeed = 2;    // zoom speed

    float panT = 0;
    float panSpeed = 10;
    Vector3 panTranslation;
    bool wKeyDown = false;
    bool aKeyDown = false;
    bool sKeyDown = false;
    bool dKeyDown = false;
    bool qKeyDown = false;
    bool eKeyDown = false;

    Vector3 lastMousePosition;
    new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        //
        // WASDQE Panning

        // read key inputs
        wKeyDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        aKeyDown = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        sKeyDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        dKeyDown = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        qKeyDown = Input.GetKey(KeyCode.Q);
        eKeyDown = Input.GetKey(KeyCode.E);

        // determine panTranslation
        panTranslation = Vector3.zero;
        if (dKeyDown && !aKeyDown)
            panTranslation += Vector3.right * Time.deltaTime * panSpeed;
        else if (aKeyDown && !dKeyDown)
            panTranslation += Vector3.left * Time.deltaTime * panSpeed;

        if (wKeyDown && !sKeyDown)
            panTranslation += Vector3.forward * Time.deltaTime * panSpeed;
        else if (sKeyDown && !wKeyDown)
            panTranslation += Vector3.back * Time.deltaTime * panSpeed;

        if (qKeyDown && !eKeyDown)
            panTranslation += Vector3.down * Time.deltaTime * panSpeed;
        else if (eKeyDown && !qKeyDown)
            panTranslation += Vector3.up * Time.deltaTime * panSpeed;
        transform.Translate(panTranslation, Space.Self);

        // Update panSpeed
        if (wKeyDown || aKeyDown || sKeyDown ||
            dKeyDown || qKeyDown || eKeyDown)
        {
            panT += Time.deltaTime / panTimeConstant;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panT * panT);
        }
        else
        {
            panT = 0;
            panSpeed = minPanSpeed;
        }

        //
        // Mouse Rotation
        if (Input.GetMouseButton(1))
        {
            // if the game window is separate from the editor window and the editor
            // window is active then you go to right-click on the game window the
            // rotation jumps if  we don't ignore the mouseDelta for that frame.
            Vector3 mouseDelta;
            if (lastMousePosition.x >= 0 &&
                lastMousePosition.y >= 0 &&
                lastMousePosition.x <= Screen.width &&
                lastMousePosition.y <= Screen.height)
                mouseDelta = Input.mousePosition - lastMousePosition;
            else
                mouseDelta = Vector3.zero;

            var rotation = Vector3.up * Time.deltaTime * rotateSpeed * mouseDelta.x;
            rotation += Vector3.left * Time.deltaTime * rotateSpeed * mouseDelta.y;
            transform.Rotate(rotation, Space.Self);

            // Make sure z rotation stays locked
            rotation = transform.rotation.eulerAngles;
            rotation.z = 0;
            transform.rotation = Quaternion.Euler(rotation);
        }

        lastMousePosition = Input.mousePosition;

        //
        // Mouse Zoom
        camera.fieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
    }
}