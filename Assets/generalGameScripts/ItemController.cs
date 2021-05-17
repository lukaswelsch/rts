using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public bool selectable = true;
    public float speed = 20f;
    
    public Vector3 newTarget;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        newTarget = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if(Vector3.Distance(transform.position, newTarget) > 0.1f){
                transform.position = Vector3.MoveTowards(transform.position, newTarget, step);
        }

    }
    
    public void MoveTo(Vector3 target)
    {
        /*if(Vector3.Distance(transform.position, newTarget.position) < speed * Time.deltaTime){
                transform.position = Vector3.MoveTowards(transform.position, target, step);
        }*/

       newTarget = target;
    }
     
}
