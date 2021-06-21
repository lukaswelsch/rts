using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenRoundMenu : MonoBehaviour
{

    public GameObject roundMenu;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;

            roundMenu.SetActive(true);
            roundMenu.GetComponent<RectTransform>().transform.position = mousePos;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //roundMenu.SetActive(false);
        }

    }
}
