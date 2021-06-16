using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public static class GridGridReader {
    public static void WriteGrid(this NetworkWriter writer, Grid<GridObject> grid){
        writer.Write<Grid<GridObject>>(grid);
    }
    public static Grid<GridObject> ReadGrid(this NetworkReader reader)
    {
        Grid<GridObject> grid = reader.Read<Grid<GridObject>>();
        return grid;
    }
}