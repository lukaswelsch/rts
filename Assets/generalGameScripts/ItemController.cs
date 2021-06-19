using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class ItemController : MonoBehaviour
{

    public bool selectable = true;
    public float speed = 20f;

    public Vector3 newTarget;

    public Vector3 oldTarget;

    private bool objectmoved;

     NavMeshAgent agent;
       
    
    [SerializeField] PlacedObjectType placedObjectType; 

    [SerializeField] Transform bullet; 


    [SerializeField] float health;  
    private PlacedObject placedObject;

    public Transform Bullet { get=> bullet;}



    // Start is called before the first frame update
    void Start()
    {
        agent =  GetComponentInParent<NavMeshAgent>();
        newTarget = transform.position;
        oldTarget = transform.position;
        objectmoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = GridBuildingSystem.Instance.Grid.CellSize / 2;
        float step = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, newTarget) > 0.1f)
        {
          /*  PlacedObject.TransformInformation t1;
            t1.x = transform.position.x;
            t1.y = transform.position.y;
            t1.z = transform.position.z;
            this.GetComponentInParent<PlacedObject>().RpcSetTransform(t1);*/
           // print("moving..");
           // transform.position = Vector3.MoveTowards(transform.position, newTarget + new Vector3(offset, 0, offset), step);
             
        }

        if(Vector3.Distance(transform.position, newTarget) < GridBuildingSystem.Instance.Grid.CellSize  && objectmoved)
        {print("restructuring");
            GridBuildingSystem.Instance.ReLinkObjects(oldTarget, newTarget, placedObject);
            objectmoved = false;

            
        }


         if(health < 0)
        {
            GridBuildingSystem.Instance.RemoveObject(transform.position);
            gameObject.GetComponentInParent<PlacedObject>().DestroySelf();
        }
    }

   

   
    public void MoveTo(Vector3 target)
    {
     /*   if(GridBuildingSystem.Instance.CheckCanBuild(target, placedObjectType) && placedObject != null)
        {
        newTarget = target;
        this.placedObject = placedObject;
         objectmoved = true;
        }*/
            
             if(agent != null){
             agent.SetDestination(target); 
             }
             
    }


    public void Damage(float amount)
    {
            this.health -= amount;
            print("hit!");
    }


    public void SpawnBullet(Vector3 target)
    {
        this.GetComponentInParent<PlacedObject>().SpawnBullet(target);
    }
}
