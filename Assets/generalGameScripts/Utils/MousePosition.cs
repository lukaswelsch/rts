using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePosition : MonoBehaviour
{
    public static Vector3 GetMousePosition()
    {
        Vector3 target = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50000.0f, 1 << 8))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                target = hit.point;
            }

        }
        return target;
    }

    public static PlacedObject GetMousePositionObjects()
    {
        PlacedObject gameObject = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50000.0f, 1 << 6))
        {
            gameObject = hit.transform.GetComponentInParent<PlacedObject>();
        }
        return gameObject;
    }
}
