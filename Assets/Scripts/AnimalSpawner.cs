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

    Vector3 FindValidPosition(Vector3 initialPosition, float maxDistance, int maxAttempts = 5)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomOffset = Random.onUnitSphere * Random.Range(1f, maxDistance);
            Vector3 candidatePosition = initialPosition + randomOffset;

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            {
                // Found a valid position
                return hit.position;
            }
        }

        Debug.LogError("Failed to find a valid position on the NavMesh after multiple attempts.");
        return Vector3.zero; // You might want to handle this case differently based on your needs.
    }

    void SpawnAnimal(GameObject go)
    {
        Vector3 spawnPoint = GameManager.Instance.tilesToSpawn[Random.Range(0, GameManager.Instance.tilesToSpawn.Count)].transform.position;
        //Vector3 validPosition = Vector3.zero;
        //if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
        //{
        //    validPosition = hit.position;
        //}
        //else
        //{
        //    //Debug.LogError("Failed to find a valid position on the NavMesh.");
        //    validPosition = FindValidPosition(validPosition, 10f);
        //}

        GameObject animalPref = Instantiate(go, new Vector3(spawnPoint.x, spawnPoint.y + 0.5f, spawnPoint.z), Quaternion.identity) as GameObject;
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
