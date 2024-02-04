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
    public List<SpawnRange> animalPref;
    public Color fogColor;
}

[System.Serializable]
public class SpawnRange
{
    public GameObject animalPrefab;
    public int minSpawnCount;
    public int maxSpawnCount;
}