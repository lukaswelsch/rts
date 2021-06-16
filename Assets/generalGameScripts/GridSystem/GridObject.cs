
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