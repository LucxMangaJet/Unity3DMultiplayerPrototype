using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerBehaviour : MonoBehaviour, IUsable
{

    [SerializeField] float maxBuildDistance;
    [SerializeField] float rotationSpeed;
    [SerializeField] GameObject catalog;
    [SerializeField] Transform catalogParent;
    [SerializeField] CatalogVisualObject catalogElementPrefab;
    [SerializeField] TMPro.TextMeshProUGUI selectedDisplay;
    [SerializeField] Material previewMaterial;

    CatalogObjectInfo currentSelected;
    PlayerActionHandler actionHandler;
    private float yaw = 0;
    PreviewMaker preview;

    public void BeginPrimary()
    {
        actionHandler.Animator.SetBool(PointingBehaviour.ANIM_Point, true);
    }

    public void EndPrimary(bool cancelled)
    {
        actionHandler.Animator.SetBool(PointingBehaviour.ANIM_Point, false);

        if (!cancelled && currentSelected != null)
        {
            var target = CalculateSpawnPosition();

            if (target.HasValue)
            {
                GameHandler.Spawn(currentSelected.SpawnablesPrefab, target.Value, Quaternion.Euler(0, yaw, 0));
            }
        }
    }

    public void BeginSecondary()
    {
        catalog.SetActive(true);

        //Wrong place for this, TEMP
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void EndSecondary(bool cancelled)
    {
        catalog.SetActive(false);

        //Wrong place for this, TEMP
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!preview) return;

        var target = CalculateSpawnPosition();

        if (target.HasValue)
        {
            preview.transform.position = target.Value;
            preview.gameObject.SetActive(true);
            preview.transform.rotation = Quaternion.Euler(0, yaw, 0);
        }
        else
        {
            preview.gameObject.SetActive(false);
        }

        float rotInput = actionHandler.Input.Building.Rotate.ReadValue<float>();
        yaw += rotInput * Time.deltaTime * rotationSpeed;
    }

    public Vector3? CalculateSpawnPosition()
    {
        RaycastHit hit;
        Ray ray = new Ray(actionHandler.CameraTransform.position, actionHandler.CameraTransform.forward);
        bool snapWithVolume = actionHandler.Input.Building.SnapWithVolume.ReadValue<float>() > 0;

        Vector3 offset = Vector3.zero;



        if (snapWithVolume)
        {
            var point = preview.ClosetsPoint(preview.transform.position + ray.direction * maxBuildDistance);

            if (point.HasValue)
                offset = preview.transform.position - point.Value;
        }

        if (Physics.Raycast(ray, out hit, maxBuildDistance))
        {
            bool snapToFloor = actionHandler.Input.Building.SnapToFloor.ReadValue<float>() > 0;

            Vector3 hitPoint = hit.point;
            if (snapToFloor)
            {
                ray = new Ray(hit.point, Vector3.down);
                if (Physics.Raycast(ray, out hit, maxBuildDistance))
                {
                    return hit.point + offset;
                }
                else
                {
                    return hitPoint + offset;
                }
            }
            else
            {
                return hitPoint + offset;
            }
        }

        return null;
    }

    public void Initialize(PlayerActionHandler actionHandler)
    {
        this.actionHandler = actionHandler;

        SwitchTo(currentSelected);

        var objects = CatalogHandler.GetAllObjects();

        foreach (var obj in objects)
        {
            var inst = Instantiate(catalogElementPrefab, catalogParent);
            inst.Setup(this, obj);
        }

        catalog.SetActive(false);

    }

    public void SwitchTo(CatalogObjectInfo info)
    {
        if (info == null)
        {
            currentSelected = null;

            if (preview != null)
                Destroy(preview);
            preview = null;
            selectedDisplay.text = "";
            return;
        }

        currentSelected = info;
        selectedDisplay.text = info.DisplayName;

        var previewGO = Instantiate(info.SpawnablesPrefab);
        preview = previewGO.AddComponent<PreviewMaker>();
        preview.Initiate(previewMaterial);
    }
}
