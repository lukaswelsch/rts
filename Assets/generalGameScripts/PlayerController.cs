using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // Start is called before the first frame update
   
        GridBuildingSystem gridBuildingSystem;
      //  NetworkConnectionToClient = this.GetComponent<NetworkIdentity>();

     void Start()
{


    gridBuildingSystem = GameObject.Find("Testing").GetComponent<GridBuildingSystem>();

}

    [Command]
    void CmdSetAuthority(NetworkIdentity identity)
    {
                    identity.AssignClientAuthority(connectionToClient);
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0) && gridBuildingSystem.placedObjectType != null)
        {
            gridBuildingSystem.PlaceObject(MousePosition.GetMousePosition());
        }

        if (Input.GetMouseButton(1) && Input.GetKeyDown(KeyCode.L))
        {
            gridBuildingSystem.RemoveObject(MousePosition.GetMousePosition());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gridBuildingSystem.dir = PlacedObjectType.GetNextDir(gridBuildingSystem.dir);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            gridBuildingSystem.placedObjectType = null;
            gridBuildingSystem.RefreshSelectedObjectType();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gridBuildingSystem.placedObjectType = gridBuildingSystem.placedObjectList[0];
            gridBuildingSystem.RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gridBuildingSystem.placedObjectType = gridBuildingSystem.placedObjectList[1];
            gridBuildingSystem.RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gridBuildingSystem.placedObjectType = gridBuildingSystem.placedObjectList[2];
            gridBuildingSystem.RefreshSelectedObjectType();
        }
    }
}
