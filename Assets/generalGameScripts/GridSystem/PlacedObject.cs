using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlacedObject : NetworkBehaviour
{
   private PlacedObjectType placedObjectType;
   private Vector2Int origin;
   private PlacedObjectType.Dir dir;

   public PlacedObjectType.Dir Dir {get => dir; set => dir = value;}
   
   public PlacedObjectType PlacedObjectType {get => placedObjectType; set => placedObjectType = value;}

   public Vector2Int Origin {get => origin; set => origin = value;}


public static void LinkObjects(PlacedObject placedObject){
   
}

 /* public static Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectType.Dir dir, PlacedObjectType placedObjectType )
   {
      Transform placedObjectTransform = Instantiate(placedObjectType.prefab, worldPosition, Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0));
      
      PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

      NetworkServer.Spawn(placedObject.gameObject );


      if(placedObject == null)
      Debug.Log("achtung");

      placedObject.placedObjectType = placedObjectType;
      placedObject.origin = origin;
      placedObject.dir = dir;

      return placedObject;
   }*/
   

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectType.GetGridPositionList(origin, dir);
    }

    public void Update(){
       

    }
 public struct TransformInformation
 {
     public float x;
     public float y;
     public float z;
 }
[ClientRpc]
     public void RpcSetTransform(TransformInformation ti)

     {
         gameObject.transform.position = new Vector3(ti.x, ti.y, ti.z);
     }

}
