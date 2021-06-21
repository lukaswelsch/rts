using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimitLeft;
    public Vector2 panLimitRight;

    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;


    public event EventHandler CameraMoved;


    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }


        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }


        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        pos.y -= scrollSpeed * scroll * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, panLimitLeft.x, panLimitRight.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, panLimitLeft.y, panLimitRight.y);

        if (transform.position != pos)
        {
            CameraMoved?.Invoke(this, EventArgs.Empty);
            transform.position = pos;

        }

    }
}
