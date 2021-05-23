using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehaviour : MonoBehaviour, IUsable
{

    [SerializeField] PlacingPreview preview;
    [SerializeField] float maxBuildDistance;
    [SerializeField] GameObject catalog;
    [SerializeField] Transform catalogParent;
    [SerializeField] CatalogVisualObject catalogElementPrefab;
    [SerializeField] TMPro.TextMeshProUGUI selectedDisplay;

    CatalogObjectInfo currentSelected;
    PlayerActionHandler actionHandler;

    public void BeginPrimary()
    {
        actionHandler.Animator.SetBool(PointingBehaviour.ANIM_Point, true);
    }

    public void EndPrimary(bool cancelled)
    {
        actionHandler.Animator.SetBool(PointingBehaviour.ANIM_Point, false);

        if (!cancelled && currentSelected != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(actionHandler.CameraTransform.position, actionHandler.CameraTransform.forward);
            Debug.DrawRay(ray.origin, ray.direction * maxBuildDistance, Color.yellow, 0.1f);
            if (Physics.Raycast(ray, out hit, maxBuildDistance))
            {
                GameHandler.Spawn(currentSelected.SpawnablesPrefab, hit.point);
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
        RaycastHit hit;
        Ray ray = new Ray(actionHandler.CameraTransform.position, actionHandler.CameraTransform.forward);
        if (Physics.Raycast(ray, out hit, maxBuildDistance))
        {
            preview.gameObject.SetActive(true);
            preview.transform.position = hit.point;
        }
        else
        {
            preview.gameObject.SetActive(false);
        }
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
            preview.SetMeshTo(null);
            selectedDisplay.text = "";
            return;
        }

        currentSelected = info;
        selectedDisplay.text = info.DisplayName;
        var filter = currentSelected.SpawnablesPrefab.GetComponent<MeshFilter>();
        if (filter)
        {
            var mesh = filter.sharedMesh;
            preview.SetMeshTo(mesh);
        }
        preview.HighlightGreen();
    }
}
