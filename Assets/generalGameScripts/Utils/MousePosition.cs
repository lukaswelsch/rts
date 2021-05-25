using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    public static Vector3 GetMousePosition(){
        Vector3 target = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     
        RaycastHit hit;
                     
        if (Physics.Raycast(ray, out hit,  50000.0f, 1 << 8))
        {
            target = hit.point;
        }
        return target;
    }
}
