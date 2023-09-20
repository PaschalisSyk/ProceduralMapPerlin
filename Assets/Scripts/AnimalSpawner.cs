using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class AnimalSpawner : MonoBehaviour
{
    public NavMeshSurface meshSurface;
    public List<SpawnRange> _animalPrefs;
    Map map;

    void Start()
    {
        map = GameManager.Instance.map;

        BakeSurface();

        Invoke("Spawn", 1);
    }

    void BakeSurface()
    {
        meshSurface.BuildNavMesh();
    }

    void SpawnAnimal(GameObject go)
    {
        Vector3 spawnPoint = GameManager.Instance.tilesToSpawn[Random.Range(0, GameManager.Instance.tilesToSpawn.Count)].transform.position;
        GameObject animalPref = Instantiate(go, new Vector3(spawnPoint.x, spawnPoint.y + 1f, spawnPoint.z), Quaternion.identity) as GameObject;
        float value = Random.Range(animalPref.transform.localScale.x * 0.3f, animalPref.transform.localScale.x * 0.8f);
        animalPref.transform.localScale = new Vector3(value, value, value);
        animalPref.transform.parent = transform;
    }

    void Spawn()
    {
        //foreach (GameObject animal in animalPrefs)
        //{
        //    int animalsToSpawn = Random.Range(10, 20);
        //    if (animal != null)
        //    {
        //        for (int i = 0; i < animalsToSpawn; i++)
        //        {
        //            SpawnAnimal(animal);
        //        }
        //    }
        //}
        foreach (var spawnRange in _animalPrefs)
        {
            if (spawnRange.animalPrefab != null)
            {
                int animalsToSpawn = Random.Range(spawnRange.minSpawnCount, spawnRange.maxSpawnCount);

                for (int i = 0; i < animalsToSpawn; i++)
                {
                    SpawnAnimal(spawnRange.animalPrefab);
                }
            }
        }

        BakeSurface();
    }

}
