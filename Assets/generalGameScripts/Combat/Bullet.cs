using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SyncVar]
    public Vector3 shootDestination;
    private NetworkIdentity networkIdentity;

    // Start is called before the first frame update
    internal void Setup(Vector3 shootDest, NetworkIdentity networkIdentity)
    {
        if (shootDest == Vector3.zero) print("achtung shootdet leer");
        this.shootDestination = shootDest;
        transform.eulerAngles = new Vector3(0, 0, 0);
        this.networkIdentity = networkIdentity;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 10f;
        float step = moveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, shootDestination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, shootDestination, step);
        }
        if (Vector3.Distance(transform.position, shootDestination) < 1)
        {
            Destroy(gameObject);
        }

    }


    public void OnTriggerEnter(Collider col)
    {
        if (!hasAuthority) return;

        NetworkIdentity id = col.transform.GetComponentInParent<NetworkIdentity>();

        ItemController itemController = col.GetComponent<ItemController>();

        PlacedObject placedObject = col.GetComponentInParent<PlacedObject>();

        print(placedObject);


        if (placedObject != null && !placedObject.hasAuthority && itemController != null && id != networkIdentity && !id.hasAuthority)
        {

            this.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            ParticleSystem.EmissionModule em = this.gameObject.GetComponentInChildren<ParticleSystem>().emission;
            em.enabled = true;

            itemController.Damage(2);
            this.GetComponent<MeshRenderer>().enabled = false;
        }
    }


}

