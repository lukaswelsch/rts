using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
   private PlacedObjectType placedObjectType;
   private Vector2Int origin;
   private PlacedObjectType.Dir dir;
   
   public PlacedObjectType PlacedObjectType {get => placedObjectType;}

   public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectType.Dir dir, PlacedObjectType placedObjectType )
   {
      Transform placedObjectTransform = Instantiate(placedObjectType.prefab, worldPosition, Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0));
      
      PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

      if(placedObject == null)
      Debug.Log("achtung");

      placedObject.placedObjectType = placedObjectType;
      placedObject.origin = origin;
      placedObject.dir = dir;

      return placedObject;
   }


    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectType.GetGridPositionList(origin, dir);
    }
}
