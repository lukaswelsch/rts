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
            
            float distanceToOther = 0f;
            
            foreach(KeyValuePair<int,GameObject> pair in selectedTable)
            {
                

                if(pair.Value != null)
                {
                    ItemController ic = selectedTable[pair.Key].GetComponent<ItemController>();

                    PlacedObject pc =  ic.GetComponentInParent<PlacedObject>();
                    
                    Vector3 target = MousePosition.GetMousePosition();

                    target.x += distanceToOther;

                   

                    Vector3 targetn = GridBuildingSystem.Instance.ConvertCoordinateToGridPosition(target, pc.PlacedObjectType);

                    ic.MoveTo(targetn, pc);

                    distanceToOther +=  pc.PlacedObjectType.GetMaxOffset(pc.Dir) * GridBuildingSystem.Instance.Grid.CellSize;
                    
                }
                
            }
        }
         if (Input.GetKeyDown(KeyCode.B))
        {
            Dictionary<int, GameObject> selectedTable = items.getAll();
            
            float distanceToOther = 0f;
            
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