 // GridBuildingSystem.cs
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GridBuildingSystem : NetworkBehaviour
{
    [SerializeField] public PlacedObjectType placedObjectType;
    [SerializeField] public PlacedObjectType[] placedObjectList;

    public Grid<GridObject> grid;
    public PlacedObjectType.Dir dir = PlacedObjectType.Dir.Down;

    public event EventHandler OnSelectedChanged;

    public static GridBuildingSystem Instance { get; internal set; }

    public Grid<GridObject> Grid { get => grid; }

    void Awake()
    {
        Instance = this;
        grid = new Grid<GridObject>(40, 20, 5f, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x, z;
        private PlacedObject placedObject;

        public PlacedObject PlacedObject { get => placedObject; set { placedObject = value; grid.TriggerGridObjectChanged(x, z); } }

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void ClearObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

        public override string ToString()
        {
            if (placedObject != null)
                return x + " " + z + "objekt" + "\n";
            return x + " " + z + "\n";
        }
    }

    internal void ReLinkObjects(Vector3 oldTarget, Vector3 newTarget, PlacedObject placedObject)
    {


        GridObject gridObject = grid.GetGridObject(oldTarget);
        if (placedObject != null)
        {
            List<Vector2Int> gridPositionList2 = placedObject.GetGridPositionList();

            foreach (Vector2Int gridPosition in gridPositionList2)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearObject();
            }


        }



        grid.GetXZ(newTarget, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObject.PlacedObjectType.GetGridPositionList(new Vector2Int(x, z), dir);

        Vector2Int rotationOffset = placedObject.PlacedObjectType.GetRotationOffset(dir);

        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            placedObject.Origin = new Vector2Int(x, z);
            grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
        }

    }



    // Update is called once per frame
    void Update()
    {
       
    }

    internal bool CheckCanBuild(Vector3 position, PlacedObjectType placedObjectType)
    {
        grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObjectType.GetGridPositionList(new Vector2Int(x, z), dir);

        bool canbuild = true;
        foreach (var gridPosition in gridPositionList)
        {
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canbuild = false;
            }
        }

        return canbuild;
    }

  [Client]
    internal void PlaceObject(Vector3 position)
    {
        print("trying to place object");
             PlaceObjectServer(position);
    }

   [Command (requiresAuthority = false)]
    internal void PlaceObjectServer(Vector3 position, NetworkConnectionToClient sender = null)
    {
    
        if (!NetworkServer.active) return;

        grid.GetXZ(position, out int x, out int z);

//hier ist etwas null, evtl placedObjectType auf dem Client
        List<Vector2Int> gridPositionList = placedObjectType.GetGridPositionList(new Vector2Int(x, z), dir);


        if (CheckCanBuild(position, placedObjectType))
        {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);

            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;

          
    
     Transform placedObjectTransform = Instantiate(placedObjectType.prefab, placedObjectWorldPosition, Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0));
      


      PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

      placedObject.PlacedObjectType = placedObjectType;
      placedObject.Dir = dir;
      placedObject.Origin = new Vector2Int(x, z);


     NetworkServer.Spawn(placedObject.gameObject, sender );

    // placedObject.Create(new Vector2Int(0,0), placedObject.Dir, placedObject.PlacedObjectType);

            
            if(placedObject == null) 
                print("error placedojbet null");

                if(placedObject.PlacedObjectType == null)
                print("placed object type null");

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).PlacedObject = placedObject;
            }
        }
    }

    internal void RemoveObject(Vector3 position)
    {
        GridObject gridObject = grid.GetGridObject(position);
        PlacedObject placedObject = gridObject.PlacedObject;

        if (placedObject != null)
        {
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                print(gridPosition);
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearObject();
            }
        }
    }

    internal Vector3 ConvertCoordinateToGridPosition(Vector3 target, PlacedObjectType internalPlacedObjectType)
    {
        grid.GetXZ(target, out int x, out int z);

        Vector2Int rotationOffset = internalPlacedObjectType.GetRotationOffset(dir);

        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;

        return grid.GetWorldPosition(x, z);
    }

    internal PlacedObjectType GetPlacedObjectType()
    {
        return placedObjectType;
    }

    internal Quaternion GetPlacedObjectRotation()
    {
        return placedObjectType != null ? Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0) : Quaternion.identity;
    }
    private void DeselectObjectType()
    {
        placedObjectType = null; RefreshSelectedObjectType();
    }
    
   [Command (requiresAuthority = false)]
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
        grid.GetXZ(mousePosition, out int x, out int z);


        if (placedObjectType != null)
        {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);

            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;

            return placedObjectWorldPosition;
        }
        else
        {
            print("placed object type is null");
            return mousePosition;
        }
    }

}
