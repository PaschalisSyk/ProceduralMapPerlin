using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FogManager : MonoBehaviour
{
    [System.Serializable]
    public class EnvironmentEvent : UnityEvent<EnvironmentProfile> { }

    public EnvironmentEvent onEnvironmentChange;

    // Start is called before the first frame update
    void Start()
    {
        onEnvironmentChange.AddListener(OnEnvironmentChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnvironmentChange(EnvironmentProfile newEnvironment)
    {
        GetComponent<MeshRenderer>().material.SetColor("_FogColor", newEnvironment.fogColor);
    }
}
