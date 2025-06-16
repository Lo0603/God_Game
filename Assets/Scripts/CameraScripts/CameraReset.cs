using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReset : MonoBehaviour, IResettable
{
    // Start is called before the first frame update
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector3 initialScale;
    private Camera cam;
    private float initialSize;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        initialScale = transform.localScale;
        cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            initialSize = cam.orthographicSize;
        }
    }


    public void ResetState()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        transform.localScale = initialScale;
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = initialSize;
        }
    }
}
