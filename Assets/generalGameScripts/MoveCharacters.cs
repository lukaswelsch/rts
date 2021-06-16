using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacters : MonoBehaviour
{
    
    SelectedDictionary items;
    void Start()
    {
        items = GetComponent<SelectedDictionary>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Dictionary<int, GameObject> selectedTable = items.getAll();
            
            float distanceToOtherX = 0f;
            float distanceToOtherY = 0f;
            int i = 1;
            
            foreach(KeyValuePair<int,GameObject> pair in selectedTable)
            {
                

                if(pair.Value != null)
                {
                    ItemController ic = selectedTable[pair.Key].GetComponent<ItemController>();

                    PlacedObject pc =  ic.transform.parent.GetComponentInParent<PlacedObject>();

        
                    Vector3 target = MousePosition.GetMousePosition();

                   
                        target.x += distanceToOtherX;
                   
                        target.z += distanceToOtherY;


                  //  Vector3 targetn = GridBuildingSystem.Instance.ConvertCoordinateToGridPosition(target, pc.PlacedObjectType);

                 //   ic.MoveTo(targetn, pc);
                 ic.MoveTo(target);

                     distanceToOtherX +=  GridBuildingSystem.Instance.Grid.CellSize;


                  if(i % (int) Mathf.Sqrt(selectedTable.Count) == 0)  {             
                        distanceToOtherY += GridBuildingSystem.Instance.Grid.CellSize; 
                         distanceToOtherX = 0;
                  }

                  print("DistnaceToOtherX" + distanceToOtherX);
                  print("DistnaceToOtherY" + distanceToOtherY);

                 i++;

                   // distanceToOther +=  pc.PlacedObjectType.GetMaxOffset(pc.Dir) * GridBuildingSystem.Instance.Grid.CellSize;
                    
                }
                
            }
        }
         if (Input.GetKeyDown(KeyCode.B))
        {
            Dictionary<int, GameObject> selectedTable = items.getAll();
            
            
            foreach(KeyValuePair<int,GameObject> pair in selectedTable)
            {
                

                if(pair.Value != null)
                {
                    ItemController ic = selectedTable[pair.Key].GetComponent<ItemController>();
                    if(ic.Bullet != null){
                   Transform bulletTransform = Instantiate(ic.Bullet, ic.transform.position, Quaternion.identity);
                    Vector3 target = MousePosition.GetMousePosition();

                    bulletTransform.GetComponent<bullet>().Setup(target, ic);
                    }
            
                    
                }
                
            }
    }
    }
}