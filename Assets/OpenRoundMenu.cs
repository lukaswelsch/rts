using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class OpenRoundMenu : NetworkBehaviour
{

    public GameObject roundMenu;
    private Camera cam;

    private CameraController cameraController = null;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cameraController = cam.GetComponent<CameraController>();
        cameraController.CameraMoved += OnCameraMoved;
    }

    public void DeactivateRoundMenu()
    {
        if (roundMenu != null)
            roundMenu.SetActive(false);
    }

    private void OnCameraMoved(object sender, System.EventArgs eventArgs)
    {
        if (sender != null)
            DeactivateRoundMenu();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;

            PlacedObject targetObject = MousePosition.GetMousePositionObjects();

            if (targetObject != null)
            {
                NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>().localPlacedObject = targetObject;
                roundMenu.SetActive(true);
                roundMenu.GetComponent<RectTransform>().transform.position = mousePos;
            }


        }
        if (Input.GetMouseButtonDown(0))
        {
            //roundMenu.SetActive(false);
        }

    }
}
