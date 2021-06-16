using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuildingPreview : NetworkBehaviour
{
    private Transform visual;
    private PlacedObjectType placedObjectType;

    private void Start()
    {
        RefreshVisual();

        if(PlayerController.Instance != null)

       PlayerController.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs eventArgs)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        RefreshVisual();
        if (PlayerController.Instance != null && PlayerController.Instance.GetPlacedObjectType() != null)
        {
            
            Vector3 targetPosition = PlayerController.Instance.GetMouseWorldSnappedPosition();

            targetPosition.y = 1f;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, PlayerController.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }
    }

    public void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
        PlacedObjectType placedObjectType = null;

        if(PlayerController.Instance != null)
          placedObjectType = PlayerController.Instance.placedObjectType;

        if (PlayerController.Instance != null && placedObjectType != null)
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
