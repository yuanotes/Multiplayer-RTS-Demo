using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    // Start is called before the first frame update
    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(
            transform.position + mainCameraTransform.rotation * Vector3.forward,
            mainCameraTransform.rotation * Vector3.up
            );
    }
}
