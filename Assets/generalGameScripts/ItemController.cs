using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public bool selectable = true;
    public float speed = 20f;

    public Vector3 newTarget;

    public Vector3 oldTarget;

    private bool objectmoved = false;

    PlacedObjectType placedObjectType; 



    // Start is called before the first frame update
    void Awake()
    {
        newTarget = transform.position;
        oldTarget = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, newTarget) > 0.1f)
        {
            float offset = GridBuildingSystem.Instance.Grid.CellSize / 2;
            transform.position = Vector3.MoveTowards(transform.position, newTarget + new Vector3(offset, 0, offset), step);
            objectmoved = true;
        }
        else {
        if (objectmoved)
        {
            objectmoved = false;


            GridBuildingSystem.Instance.PlaceObject(newTarget, placedObjectType);
            GridBuildingSystem.Instance.RemoveObject(oldTarget);

            oldTarget = transform.position;
        }
        }

    }

    public void MoveTo(Vector3 target)
    {
        /*if(Vector3.Distance(transform.position, newTarget.position) < speed * Time.deltaTime){
                transform.position = Vector3.MoveTowards(transform.position, target, step);
        }*/




       PlacedObject po =  gameObject.GetComponentInParent(typeof(PlacedObject)) as PlacedObject;

       placedObjectType = po.PlacedObjectType;

        if(GridBuildingSystem.Instance.CheckCanBuild(target, placedObjectType))
        {
        newTarget = target;
        }
    }

}
