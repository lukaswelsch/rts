using System;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private PlacedObjectType placedObjectType;
    [SerializeField] private PlacedObjectType[] placedObjectList;

    private Grid<GridObject> grid;
    private PlacedObjectType.Dir dir = PlacedObjectType.Dir.Down;

    public event EventHandler OnSelectedChanged;

    public static GridBuildingSystem Instance { get; internal set; }

    public Grid<GridObject> Grid {get => grid;}

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
            return x + " " + z + "\n" + placedObject;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && placedObjectType != null)
        {
            PlaceObject(MousePosition.GetMousePosition(), placedObjectType);
        }

        if (Input.GetMouseButton(1) && Input.GetKeyDown(KeyCode.L))
        {
            RemoveObject(MousePosition.GetMousePosition());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectType.GetNextDir(dir);
        }
           if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            placedObjectType = null;
            RefreshSelectedObjectType();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placedObjectType = placedObjectList[0];
            RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placedObjectType = placedObjectList[1];
            RefreshSelectedObjectType();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            placedObjectType = placedObjectList[2];
            RefreshSelectedObjectType();
        }
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

    internal void PlaceObject(Vector3 position, PlacedObjectType internalPlacedObjectType)
    {       
        grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = internalPlacedObjectType.GetGridPositionList(new Vector2Int(x, z), dir);


        if (CheckCanBuild(position, placedObjectType))
        {
            Vector2Int rotationOffset = internalPlacedObjectType.GetRotationOffset(dir);

            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;

            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, internalPlacedObjectType);

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
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearObject();
            }
        }
    }

    internal Vector3 ConvertCoordinateToGridPosition(Vector3 target)
    {
        grid.GetXZ(target, out int x, out int z);
                    
        Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);

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

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
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
