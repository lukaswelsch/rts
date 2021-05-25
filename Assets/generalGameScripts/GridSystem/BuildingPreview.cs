using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    private Transform visual;
    private PlacedObjectType placedObjectType;

    private void Start()
    {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs eventArgs)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        RefreshVisual();
        if (GridBuildingSystem.Instance.GetPlacedObjectType() != null)
        {
            Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();

            targetPosition.y = 1f;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }
    }

    public void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectType placedObjectType = GridBuildingSystem.Instance.GetPlacedObjectType();

        if (placedObjectType != null)
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
