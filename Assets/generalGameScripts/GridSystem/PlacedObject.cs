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

    [SerializeField] public Material[] materialList;

    public PlacedObjectType.Dir Dir { get => dir; set => dir = value; }

    public PlacedObjectType PlacedObjectType { get => placedObjectType; set => placedObjectType = value; }

    public Vector2Int Origin { get => origin; set => origin = value; }

    [SyncVar] public int playerNumber = 0;





    public void Start()
    {
        RpcUpdateMaterial();
    }


    public void RpcUpdateMaterial()
    {

        GetComponentInChildren<Renderer>().material = materialList[playerNumber];
    }


    public static void LinkObjects(PlacedObject placedObject)
    {

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



    [Command(requiresAuthority = false)]
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void localDestroySelf()
    {
        Destroy(this.gameObject);
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectType.GetGridPositionList(origin, dir);
    }

    public void OnMouseOver()
    {

        print("factory gedrückt");

    }



    [Command]
    public void SpawnTrike()
    {
        print("Spawning Trike....");
    }

    [Command]
    public void SpawnBullet(Vector3 target, NetworkConnectionToClient connectionToClient = null)
    {

        ItemController ic = GetComponentInChildren<ItemController>();

        Transform bulletTransform = Instantiate(ic.Bullet, ic.transform.position, Quaternion.identity);
        //        Bullet bullet = (Bullet) Instantiate(ic.Bullet, ic.transform.position, Quaternion.identity);

        Bullet bullet = bulletTransform.GetComponent<Bullet>();

        bullet.shootDestination = target;

        bullet.Setup(target, ic.GetComponentInParent<NetworkIdentity>());

        NetworkServer.Spawn(bullet.gameObject, connectionToClient);



    }
}
