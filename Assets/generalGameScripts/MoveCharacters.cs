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
                    
                    Vector3 target = MousePosition.GetMousePosition();

                    target.x += distanceToOther;


                    Vector3 targetn = GridBuildingSystem.Instance.ConvertCoordinateToGridPosition(target);

                    selectedTable[pair.Key].GetComponent<ItemController>().MoveTo(targetn);

                    
                            
                    
                }
                distanceToOther += 5f;
            }
        }
    }
}
