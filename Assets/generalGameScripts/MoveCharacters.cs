using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacters : MonoBehaviour
{
    
    SelectedDictionary items;
    // Start is called before the first frame update
    void Start()
    {
        items = GetComponent<SelectedDictionary>();
    }

    // Update is called once per frame
    void Update()
    {
       // Vector3 p1 = new Vector3();
        
        
        
        if (Input.GetMouseButtonDown(1))
        {
           
            Dictionary<int, GameObject> selectedTable = items.getAll();
            
            float distanceToOther = 3f;
            
            foreach(KeyValuePair<int,GameObject> pair in selectedTable)
            {
                if(pair.Value != null)
                {
                    Debug.Log("start..");
                    
                    Vector3 p = Input.mousePosition;
                    
                     Ray ray = Camera.main.ScreenPointToRay(p);
                     
                      RaycastHit hit;
                     
                     if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("raycasting..");
                        if (hit.collider.gameObject.name == "Plane")
                        {
                            Debug.Log("moving..");
                            Vector3 target = hit.point;
                            
                            target.x += distanceToOther;
                            
                            selectedTable[pair.Key].GetComponent<ItemController>().MoveTo(target);
                        }   
                    }
                    
                    
                }
                distanceToOther += 3f;
            }
            
        }
        
       
    }
    
    
}
