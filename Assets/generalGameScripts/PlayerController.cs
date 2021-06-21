using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] Vector3[] startPositions;

    [SerializeField] public PlacedObjectType placedObjectType;

    [SerializeField] public PlacedObjectType[] placedObjectList;


    [SyncVar]
    public Color color;

    private Vector3 startingPositition = Vector3.zero;

    public int placeObjectNumber = 0;

    public static PlayerController Instance { get; internal set; }


    public event EventHandler OnSelectedChanged;

    public Material playerMaterial;

    [SerializeField] public Material[] playerMaterials;


    public int playerNumber = 1;
    //  [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;


    public float energyMax = 1000f;
    public float currentEnergy = 0f;

    public float current_energyCost = 0f;


    GridBuildingSystem gridBuildingSystem;
    //  NetworkConnectionToClient = this.GetComponent<NetworkIdentity>();

    void Start()
    {
        gridBuildingSystem = GameObject.Find("Testing").GetComponent<GridBuildingSystem>();
        // CmdSetAuthority();
        currentEnergy = energyMax;
    }

    [Command]
    void CmdSetAuthority()
    {
        // if(!isLocalPlayer) return;
        //NetworkIdentity mv = GameObject.Find("EventSystem").GetComponent<NetworkIdentity>();
        //mv.AssignClientAuthority(connectionToClient);
    }




    public override void OnStartLocalPlayer()
    {
        // Camera.main.transform.SetParent(transform);
        //Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        playerMaterial = playerMaterials[playerNumber];

        SetupPlayerNumber(playerNumber);

        playerNumber++;
    }

    [Command]
    public void SetupPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }


    [Client]
    internal void PlaceObject(Vector3 position, int placeObjectNumber, int playerNumber)
    {
        if (!isLocalPlayer) return;
        PlaceObjectServer(position, placeObjectNumber);
    }

    [Command(requiresAuthority = false)]
    internal void PlaceObjectServer(Vector3 position, int placeObjectNumber)
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
            placedObject.playerNumber = playerNumber;

            NetworkServer.Spawn(placedObject.gameObject, connectionToClient);



            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridBuildingSystem.grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }
        }
    }

    public void SpawnTrike(Vector3 position, int placeObjectNumber)
    {
        PlacedObjectType pt = placedObjectList[placeObjectNumber];
        float energyCost = pt.energyCost;

        PlacedObject placedObject = null;


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


            placedObject = placedObjectTransform.GetComponent<PlacedObject>();

            placedObject.PlacedObjectType = placedObjectType;
            placedObject.Dir = gridBuildingSystem.dir;
            placedObject.Origin = new Vector2Int(x, z);
            placedObject.playerNumber = 0;



            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridBuildingSystem.grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }

        }
        StartCoroutine(StartTask(placedObject, energyCost, position, placeObjectNumber));

    }



    IEnumerator StartTask(PlacedObject placedObject, float energyCost, Vector3 position, int placeObjectNumber)
    {
        while (energyCost > 0)
        {
            currentEnergy -= 5;
            yield return new WaitUntil(() => currentEnergy > 0);
            energyCost -= 5;
        }
        if (energyCost <= 0)
        {
            print(position);

            if (placedObject == null) print("Placedobject you want to delete is null");
            else placedObject.localDestroySelf();

            gridBuildingSystem.RemoveObject(position);

            PlaceObjectServer(position, placeObjectNumber);
        }
    }


    private bool beeinghandeld = false;
    IEnumerator WaitSomeTime()
    {
        beeinghandeld = true;
        currentEnergy += 50;

        yield return new WaitForSeconds(1);

        beeinghandeld = false;

    }


    void SpawnTrikeCmd(PlacedObject target)
    {
        print("triyng to create trike");
        //evtl brauche ich hier den Type 
        PlacedObject placedObject = target;
        if (placedObject == null) print("Didnt find placedobject");

        print(target);
        if (placedObject != null && placedObject.hasAuthority)
        {
            PlacedObjectType placedObjectType = placedObjectList[2];


            Vector3 positionToPlace = placedObject.transform.position;

            float distanceToOther = gridBuildingSystem.grid.CellSize;

            for (int i = 0; i < 5; i++)
            {
                positionToPlace.x += distanceToOther;

                //das klappt so auf dem Client nicht !! 

                if (gridBuildingSystem.CheckCanBuild(positionToPlace, placedObjectType))
                {
                    SpawnTrike(positionToPlace, 2);
                    return;
                }
            }

            positionToPlace = placedObject.transform.position;

            for (int i = 0; i < 5; i++)
            {
                positionToPlace.z += distanceToOther;
                if (gridBuildingSystem.CheckCanBuild(positionToPlace, placedObjectType))
                {
                    SpawnTrike(positionToPlace, 2);
                    return;
                }
            }
        }

    }

    [Client]
    void Update()
    {
        if (!isLocalPlayer) return;

        if (currentEnergy < energyMax && !beeinghandeld)
        {
            StartCoroutine(WaitSomeTime());

        }

        if (Input.GetMouseButtonDown(0) && placedObjectType != null)
        {
            PlaceObjectServer(MousePosition.GetMousePosition(), placeObjectNumber);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SpawnTrikeCmd(MousePosition.GetMousePositionObjects());

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
