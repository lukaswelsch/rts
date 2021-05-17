using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Testing : MonoBehaviour
{
    Grid grid;
    int value;
    private void Start() {
        grid = new Grid(4, 2, 10f, new Vector3(20,0));
    }
 
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 p = Input.mousePosition;
                    
                     Ray ray = Camera.main.ScreenPointToRay(p);
                     
                      RaycastHit hit;
                     
                     if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("raycasting..");
                         if (hit.collider.gameObject.name == "Plane")
                        {
            
                            Vector3 target = hit.point;
            
            
                        
                        grid.SetValue(target, 10);
                        }
                    }
                    
                    
        }
 
        if (Input.GetMouseButtonDown(1)) {
            
             Vector3 p = Input.mousePosition;
                    
                     Ray ray = Camera.main.ScreenPointToRay(p);
                     
                      RaycastHit hit;
                     
                     if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("raycasting..");
               if (hit.collider.gameObject.name == "Plane")
                        {
                            Vector3 target = hit.point;
            
            
                        Debug.Log(grid.GetValue(target));
                        }
                    }
        }
    }
}
