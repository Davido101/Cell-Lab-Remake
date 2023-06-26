using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Substrate substrate;
    public float scrollSpeed = 3.5f;
    public float moveSpeed = 3.5f;
    public Camera camera;
    Vector3 originalPos;
    
    void Start()
    {
        camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
    }

    void Update()
    {
        UpdateCamera();
        substrate.update();
    }

    void FixedUpdate()
    {
        substrate.fixedupdate();
    }

    void UpdateCamera()
    {
        // Zoom
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        float zoom = Mathf.Pow(scrollSpeed, -scrollAmount);
        camera.orthographicSize *= zoom;

        // Move
        if (Input.GetMouseButtonDown(1))
        {
            originalPos = camera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 movement = originalPos - camera.ScreenToWorldPoint(Input.mousePosition);
            camera.transform.position += movement;
        }
    }
}
