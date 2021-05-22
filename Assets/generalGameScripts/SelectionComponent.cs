using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionComponent : MonoBehaviour
{
    void Start()
    {
        GetComponentInChildren<Renderer>().material.color = Color.red;
    }

    private void OnDestroy()
    {
        GetComponentInChildren<Renderer>().material.color = Color.white;
    }
}
