using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [Header("UI")]
    public Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }
}
