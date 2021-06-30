using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
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

    [SyncVar]
    public int connectedClients;


    GridBuildingSystem gridBuildingSystem;
    //  NetworkConnectionToClient = this.GetComponent<NetworkIdentity>();

    Image energyController;

    void Start()
    {
        gridBuildingSystem = GameObject.Find("Testing").GetComponent<GridBuildingSystem>();
        // CmdSetAuthority();
        currentEnergy = energyMax;

        energyController = GameObject.Find("EnergyUI").GetComponent<Image>();
    }

    public override void OnStartLocalPlayer()
    {
        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        playerMaterial = playerMaterials[playerNumber];

        GetConnectionCount();

        playerNumber = connectedClients;

        SetupPlayerNumber(playerNumber);

    }

    [Command]
    void GetConnectionCount()
    {
        connectedClients = NetworkServer.connections.Count;
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
            placedObject.playerNumber = connectedClients;

            NetworkServer.Spawn(placedObject.gameObject, connectionToClient);



            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridBuildingSystem.grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }

            RpcUpdateGridSystem(position, placeObjectNumber);
        }
    }


    [ClientRpc]
    public void RpcUpdateGridSystem(Vector3 position, int placeObjectNumber)
    {
        PlacedObjectType placedObjectType = placedObjectList[placeObjectNumber];

        gridBuildingSystem.grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObjectType.GetGridPositionList(new Vector2Int(x, z), gridBuildingSystem.dir);

        if (gridPositionList != null)
        {
            PlacedObject placedObject = FindObjectsOfType<PlacedObject>()[0];

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
            placedObject.energyCost = energyCost;
            placedObject.currentEnergyCost = energyCost;


            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridBuildingSystem.grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }

        }
        StartCoroutine(HandleEnergyCost(placedObject, energyCost, position, placeObjectNumber));

    }



    IEnumerator HandleEnergyCost(PlacedObject placedObject, float energyCost, Vector3 position, int placeObjectNumber)
    {
        while (placedObject.currentEnergyCost > 0)
        {
            currentEnergy -= 5;
            yield return new WaitUntil(() => currentEnergy > 0);
            placedObject.UpdateEnergyCost(5);
        }
        if (placedObject.currentEnergyCost <= 0)
        {
            if (placedObject != null) placedObject.localDestroySelf();

            gridBuildingSystem.RemoveObject(position);

            PlaceObjectServer(position, placeObjectNumber);
        }
    }


    private bool beeinghandeld = false;
    IEnumerator RegenerateEnergy()
    {
        beeinghandeld = true;
        currentEnergy += 50;

        yield return new WaitForSeconds(1);

        beeinghandeld = false;

    }



    public void SpawnTrikeFromOutside(PlacedObject target)
    {
        GameObject localPlayer = NetworkClient.localPlayer.gameObject;
        localPlayer.GetComponent<PlayerController>().SpawnTrikeCmd(target);
    }


    internal PlacedObject localPlacedObject;

    public void SpawnTrikeCmd(PlacedObject target)
    {


        //evtl brauche ich hier den Type int
        // PlacedObject localPlacedObject = target;
        if (localPlacedObject == null) print("Didnt find placedobject");

        print(target);
        if (localPlacedObject != null && localPlacedObject.hasAuthority)
        {
            PlacedObjectType placedObjectType = placedObjectList[2];


            Vector3 positionToPlace = localPlacedObject.transform.position;

            float distanceToOther = gridBuildingSystem.grid.CellSize;

            for (int i = 0; i < 5; i++)
            {
                positionToPlace.x += distanceToOther;

                if (gridBuildingSystem.CheckCanBuild(positionToPlace, placedObjectType))
                {
                    SpawnTrike(positionToPlace, 2);
                    return;
                }
            }

            positionToPlace = localPlacedObject.transform.position;

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

        energyController.fillAmount = currentEnergy / energyMax;

        if (currentEnergy < energyMax && !beeinghandeld)
        {
            StartCoroutine(RegenerateEnergy());

        }

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
            RefreshSelectedObjectType();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placedObjectType = placedObjectList[0];
            placeObjectNumber = 0;
            RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placedObjectType = placedObjectList[1];
            placeObjectNumber = 1;
            RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
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
