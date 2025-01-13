using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    Camera camera;
    public float FOV = 90;
    public float maxZoomFOV = 15;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity = 1;

    private void Awake()
    {
        // attach camera and adjust FOV
        camera = GetComponent<Camera>();
        if (camera)
            FOV = camera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        // update currentZoom and Camera's fieldOfView
        currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        camera.fieldOfView = Mathf.Lerp(FOV, maxZoomFOV, currentZoom);
    }
}
