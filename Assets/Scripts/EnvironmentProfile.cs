using UnityEngine;
using System.Collections.Generic;

public enum EnvironmentType
{
    Grassland,
    Forest,
    Desert,
    Iceland
}

[System.Serializable]
public class EnvironmentProfile
{
    public EnvironmentType enviromentType;
    public GameObject[] tilePrefabs;
    public bool hasRiver;
    public List<GameObject> animals;
}