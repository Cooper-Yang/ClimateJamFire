using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Make the canvas face the camera
        transform.LookAt(transform.position + cam.forward);
    }
}

