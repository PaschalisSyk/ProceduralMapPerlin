using UnityEngine;

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
}