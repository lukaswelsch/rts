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

    public float energyCost;

    public float currentEnergyCost;


    ItemController itemController;





    public void Start()
    {
        RpcUpdateMaterial();

        itemController = GetComponentInChildren<ItemController>();
    }


    public void RpcUpdateMaterial()
    {

        GetComponentInChildren<Renderer>().material = materialList[playerNumber];
    }

    public void ActivateShadows()
    {
        MeshRenderer[] mesh = this.GetComponentsInChildren<MeshRenderer>();

        foreach (var meshRenderer in mesh)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        }
    }


    public void UpdateEnergyCost(float amount)
    {
        currentEnergyCost -= amount;

        itemController.UpdateDissolveShader(currentEnergyCost / energyCost);

        print("Prozentsatz ist:" + currentEnergyCost / energyCost);
    }



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
