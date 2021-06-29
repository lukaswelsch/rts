// GridBuildingSystem.cs
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GridBuildingSystem : NetworkBehaviour
{
    public Grid<GridObject> grid;
    public PlacedObjectType.Dir dir = PlacedObjectType.Dir.Down;


    public static GridBuildingSystem Instance { get; internal set; }

    public Grid<GridObject> Grid { get => grid; }

    void Awake()
    {
        Instance = this;
        //fullsize 200x200
        grid = new Grid<GridObject>(50, 50, 5f, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
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

    public PlacedObject GetPlacedObject(Vector3 position)
    {
        grid.GetXZ(position, out int x, out int z);

        return grid.GetGridObject(x, z).PlacedObject;
    }




}
