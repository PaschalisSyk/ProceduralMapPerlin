using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public GameObject minimap;
    GameObject miniMapCamera;

    void OnEnable()
    {
        // Subscribe to the TabPressed event when the script is enabled
        PlayerController.OnTabPressed += ToggleMinimap;
    }

    private void Awake()
    {
        miniMapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");
        miniMapCamera.SetActive(false);
    }

    void OnDisable()
    {
        // Unsubscribe from the TabPressed event when the script is disabled
        PlayerController.OnTabPressed -= ToggleMinimap;
    }

    public void ToggleMinimap()
    {
        bool currentState = minimap.activeInHierarchy;
        minimap.SetActive(!currentState);
        miniMapCamera.SetActive(!currentState);
    }
}
