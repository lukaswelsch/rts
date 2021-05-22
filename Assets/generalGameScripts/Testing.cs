using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Testing : MonoBehaviour
{
    Grid<bool> grid;

    private void Start() {
        grid = new Grid<bool>(40, 20, 5f, new Vector3(0,0), (Grid<bool> g, int x, int z) => false);
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
            
            
                        
                        grid.SetGridObject(target, true);
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
            
            
                        Debug.Log(grid.GetGridObject(target));
                        }
                    }
        }
    }
}
