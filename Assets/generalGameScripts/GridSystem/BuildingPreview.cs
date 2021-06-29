using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuildingPreview : NetworkBehaviour
{
    private Transform visual;
    private PlacedObjectType placedObjectType;


    private PlayerController playerController;

    private void Start()
    {

        RefreshVisual();

        if (!isLocalPlayer) return;

        playerController.OnSelectedChanged += Instance_OnSelectedChanged;

        if (PlayerController.Instance != null)

            PlayerController.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs eventArgs)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        RefreshVisual();
        if (playerController != null && playerController.GetPlacedObjectType() != null)
        {

            Vector3 targetPosition = playerController.GetMouseWorldSnappedPosition();

            targetPosition.y = 1f;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerController.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }
    }

    public void RefreshVisual()
    {
        if (playerController == null && NetworkClient.localPlayer != null)
            playerController = NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>();

        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
        PlacedObjectType placedObjectType = null;

        if (playerController != null)
            placedObjectType = playerController.placedObjectType;

        if (playerController != null && placedObjectType != null)
        {
            visual = Instantiate(placedObjectType.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            // SetLayerRecursive(visual.gameObject, 11);
        }

    }

    private void SetLayerRecursive(GameObject gameObject, int v)
    {
        throw new NotImplementedException();
    }
}
