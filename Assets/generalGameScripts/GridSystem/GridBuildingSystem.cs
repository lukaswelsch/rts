using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private PlacedObjectType placedObject;
    [SerializeField] private PlacedObjectType[] placedObjectList;
  
    private Grid<GridObject> grid;
    private PlacedObjectType.Dir dir = PlacedObjectType.Dir.Down;
    
    void Awake()
    {
        
        grid = new Grid<GridObject>(40, 20, 5f, new Vector3(0,0,0), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }
    
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x,z;
        private Transform transformObject;
        
        public Transform TransformObject { get => transformObject ; set => transformObject = value;}
        
        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
        
        public void ClearTransform(){
            transformObject = null;
        }
        
        public bool CanBuild(){
            return transformObject == null;
        }
        
        public override string ToString()
        {
        
            return x + " " + z + "\n" + transformObject;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0)) {

                     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     
                      RaycastHit hit;
                     
                     if (Physics.Raycast(ray, out hit,  50000.0f, 1 << 8))
                    {
//                         Debug.Log("raycasting..");
//                          if (hit.collider.gameObject.name == "Plane")
//                         {
            
                            Vector3 target = hit.point;
            
                            grid.GetXZ(target, out int x, out int z);
                            
                            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList(new Vector2Int(x,z), dir);
                            
                            
                            bool canbuild = true;
                            foreach(var gridPosition in gridPositionList)
                            {
                                if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                                {
                                    canbuild = false; 
                                }
                            }

                             
                             if(canbuild)
                             {  
                                 
                                Vector2Int rotationOffset = placedObject.GetRotationOffset(dir);
                                
                                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize; // 5 = GetGridCellSize
                                 
                                Transform trans =  Instantiate(placedObject.prefab, placedObjectWorldPosition, transform.rotation  * Quaternion.Euler(0, placedObject.GetRotationAngle(dir),0) );
                                
                                foreach(Vector2Int gridPosition in gridPositionList){
                                    grid.GetGridObject(gridPosition.x, gridPosition.y).TransformObject = trans;
                                }
                                
                            }
//                         }
                    }
                    
                    
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectType.GetNextDir(dir);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
           placedObject = placedObjectList[0];
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
           placedObject = placedObjectList[1];
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
           placedObject = placedObjectList[2];
        }
    }
}
