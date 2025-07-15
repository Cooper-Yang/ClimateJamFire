using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 30f;
    public float scrollSpeed = 2f;
    public float minOrthoSize = 10f;
    public float maxOrthoSize = 20f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float initialOrthoSize;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialOrthoSize = GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        CameraMove();
        CameraZoom();
        CameraReset();
    }

    // Move the camera using WASD keys
    // W - forward, A - left, S - backward, D - right
    void CameraMove()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // W
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += forward * panSpeed * Time.deltaTime;
        }
        // A
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= right * panSpeed * Time.deltaTime;
        }
        // S
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= forward * panSpeed * Time.deltaTime;
        }
        // D
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += right * panSpeed * Time.deltaTime;
        }
    }

    // Zoom the camera in and out using the mouse scroll wheel
    void CameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Camera cam = GetComponent<Camera>();
            cam.orthographicSize -= scroll * scrollSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);
        }
    }

    // Reset the camera position when space is pressed
    void CameraReset()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            GetComponent<Camera>().orthographicSize = initialOrthoSize;
        }
    }
}