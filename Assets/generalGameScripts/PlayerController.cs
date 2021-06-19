using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public PlacedObjectType placedObjectType;

    public int placeObjectNumber = 0;

    [SerializeField] public PlacedObjectType[] placedObjectList;

      public static PlayerController Instance { get; internal set; }


       public event EventHandler OnSelectedChanged;

    GridBuildingSystem gridBuildingSystem;
     //  NetworkConnectionToClient = this.GetComponent<NetworkIdentity>();

     void Start()
{
        gridBuildingSystem = GameObject.Find("Testing").GetComponent<GridBuildingSystem>();
       // CmdSetAuthority();
    }

    [Command]
    void CmdSetAuthority()
    {
       // if(!isLocalPlayer) return;
        //NetworkIdentity mv = GameObject.Find("EventSystem").GetComponent<NetworkIdentity>();
        //mv.AssignClientAuthority(connectionToClient);
    }


[Client]
    internal void PlaceObject(Vector3 position, int placeObjectNumber)
    {
         if (!isLocalPlayer) return;
             PlaceObjectServer(position, placeObjectNumber);
    }

   [Command (requiresAuthority = false)]
    internal void PlaceObjectServer(Vector3 position,  int placeObjectNumber)
    {
        PlacedObjectType placedObjectType = placedObjectList[placeObjectNumber];
         print("trying to place object");

//Hier ist etwas null
        gridBuildingSystem.grid.GetXZ(position, out int x, out int z);

//hier ist etwas null, evtl placedObjectType auf dem Client
        List<Vector2Int> gridPositionList = placedObjectType.GetGridPositionList(new Vector2Int(x, z), gridBuildingSystem.dir);


        if (gridBuildingSystem.CheckCanBuild(position, placedObjectType))
        {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(gridBuildingSystem.dir);

            Vector3 placedObjectWorldPosition = gridBuildingSystem.grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * gridBuildingSystem.grid.CellSize;

          
    
     Transform placedObjectTransform = Instantiate(placedObjectType.prefab, placedObjectWorldPosition, Quaternion.Euler(0, placedObjectType.GetRotationAngle(gridBuildingSystem.dir), 0));
      


      PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

      placedObject.PlacedObjectType = placedObjectType;
      placedObject.Dir = gridBuildingSystem.dir;
      placedObject.Origin = new Vector2Int(x, z);


     NetworkServer.Spawn(placedObject.gameObject, connectionToClient );


            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridBuildingSystem.grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }
        }
    }


    // Update is called once per frame
    [Client]
    void Update()
    {
         if (!isLocalPlayer) return;

         if (Input.GetMouseButtonDown(0) && placedObjectType != null)
        {
            PlaceObjectServer(MousePosition.GetMousePosition(), placeObjectNumber);
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
            placedObjectType = null;
            //gridBuildingSystem.placedObjectType = null;
           RefreshSelectedObjectType();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //gridBuildingSystem.placedObjectType = placedObjectList[0];
            placedObjectType = placedObjectList[0];
            placeObjectNumber = 0;
         //   gridBuildingSystem.RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
          // gridBuildingSystem.placedObjectType = placedObjectList[1];
            placedObjectType = placedObjectList[1];
            placeObjectNumber = 1;
          //  gridBuildingSystem.RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
           // gridBuildingSystem.placedObjectType = placedObjectList[2];
            placedObjectType = placedObjectList[2];
            placeObjectNumber = 2;
            RefreshSelectedObjectType();
        }
    }


    internal PlacedObjectType GetPlacedObjectType()
    {
        return placedObjectType;
    }

    internal Quaternion GetPlacedObjectRotation()
    {
        return placedObjectType != null ? Quaternion.Euler(0, placedObjectType.GetRotationAngle(gridBuildingSystem.dir), 0) : Quaternion.identity;
    }
    private void DeselectObjectType()
    {
        placedObjectType = null; RefreshSelectedObjectType();
    }
    
    public void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    internal Vector2Int GetRotationOffsetMovement(PlacedObjectType placedObjectTyp, PlacedObjectType.Dir dir)
    {
        if (placedObjectType != null)
        {
            return placedObjectType.GetRotationOffset(dir);
        }
        return Vector2Int.zero;
    }

    internal Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = MousePosition.GetMousePosition();
        gridBuildingSystem.grid.GetXZ(mousePosition, out int x, out int z);


        if (placedObjectType != null)
        {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(gridBuildingSystem.dir);

            Vector3 placedObjectWorldPosition = gridBuildingSystem.grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * gridBuildingSystem.grid.CellSize;

            return placedObjectWorldPosition;
        }
        else
        {
            print("placed object type is null");
            return mousePosition;
        }
    }
}
