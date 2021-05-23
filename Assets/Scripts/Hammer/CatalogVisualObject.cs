using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogVisualObject : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI nameField;
    [SerializeField] Image icon;
    [SerializeField] Button select;

    public void Setup(HammerBehaviour hammer, CatalogObjectInfo info)
    {
        icon.sprite = info.Icon;
        nameField.text = info.DisplayName;
        select.onClick.AddListener(delegate { hammer.SwitchTo(info); });
    }
}
