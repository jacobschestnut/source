using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltCameraController : MonoBehaviour
{
    private Camera altCamera;
  
    public Transform playerObject;

    private Camera playerCamera;

    public Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = playerObject.GetComponentInChildren<Camera>();
        altCamera = GetComponentInChildren<Camera>();
        cameraOffset = transform.position - playerObject.transform.position;
    }

    void LateUpdate()
    {
        Vector3 newPosition = playerObject.transform.position + cameraOffset;
        transform.position = newPosition;
        altCamera.transform.rotation = playerCamera.transform.rotation;
    }
}
