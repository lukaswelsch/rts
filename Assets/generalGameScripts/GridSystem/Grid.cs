using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int z;
    }
    
    public float CellSize  => cellSize;
 
    public Grid (int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> initGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        
        gridArray = new TGridObject[width, height];
 
        
         for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                 gridArray[x,z] = initGridObject(this, x, z);
                
            }
         }
        
       
        if(false){

        debugTextArray = new TextMesh[width, height];
 
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                debugTextArray[x,z] = CreateWorldText(null, gridArray[x, z].ToString(), GetWorldPosition(x, z) + new Vector3(cellSize,0,cellSize) * .5f, 5, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }
 
    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x,0,z ) * cellSize + originPosition;
    }
 
    public void SetValue(int x, int z, TGridObject value) {
        if (x >= 0 && z >= 0 && x < width && z < height) {
            gridArray[x, z] = value;
            debugTextArray[x, z].text = gridArray[x, z].ToString();
        }
    }
 
    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetValue(x, z, value);
    }
 
    public TGridObject GetGridObject(int x, int z) {
        if (x >= 0 && z >= 0 && x < width && z < height) {
            return gridArray[x, z];
        } else {
            return default(TGridObject);
        }
    }
 
    public TGridObject GetGridObject (Vector3 worldPosition) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }
    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }
 
    public TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        return textMesh;
    }

     public void TriggerGridObjectChanged(int x, int z) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }
}
 
