using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    private Camera cam;
    private float targetZoom;
    private float zoomFactor = 3f;
    private float zoomLerpSpeed = 10;
    private Transform camTargetMove;
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        camTargetMove = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CamPan();
        CamZoom();
    }
    private void CamPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 moveTo = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += moveTo;
        }
    }
    private void CamZoom()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }
}
