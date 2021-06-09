using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private Vector3 shootDestination;
    private ItemController ic; 

    // Start is called before the first frame update
    internal void Setup(Vector3 shootDest, ItemController ic)
    {
        
        this.shootDestination = shootDest;
        transform.eulerAngles = new Vector3(0,0,0);
        this.ic = ic;
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 10f;
        float step = moveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, shootDestination) > 0.1f)
        {
          transform.position = Vector3.MoveTowards(transform.position, shootDestination, step);
        }
        if(Vector3.Distance(transform.position, shootDestination) < GridBuildingSystem.Instance.Grid.CellSize)
        {
            Destroy(gameObject);
        }
       
    }

    public void OnTriggerEnter(Collider col)
    {

         ItemController itemController = col.GetComponent<ItemController>();
        if(itemController != null && itemController != ic)
        {
        print("hit item controller");
         itemController.Damage(2);
         Destroy(gameObject);
        }
    }
}
