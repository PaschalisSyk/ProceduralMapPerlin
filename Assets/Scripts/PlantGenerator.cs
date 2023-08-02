using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public GameObject[] grassPref;
    public LayerMask obstructionLayer;
    public float placementRadius = 1.0f;
    public int maxPlantsToSpawn = 10;

    public float[] prefabProbabilities;

    private void Start()
    {
        GenerateGrass();
    }

    void GenerateGrass()
    {
        Collider tileCollider = GetComponent<Collider>();

        for (int i = 0; i < maxPlantsToSpawn; i++)
        {
            // Randomly select a point within the bounds of the grass tile
            Vector3 randomPoint = GetRandomPointOnGrassTile(tileCollider.bounds);

            // Check for obstructions around the selected point
            if (!IsObstructed(randomPoint))
            {
                // Instantiate a random plant prefab at the selected point
                //int randomPlantIndex = Random.Range(0, grassPref.Length);
                SpawnPlants(randomPoint);

            }
        }
    }

    private Vector3 GetRandomPointOnGrassTile(Bounds tileBounds)
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(tileBounds.min.x +0.5f, tileBounds.max.x - 0.5f),
            tileBounds.max.y,
            Random.Range(tileBounds.min.z + 0.5f, tileBounds.max.z - 0.5f));

        return randomPoint;
    }

    private bool IsObstructed(Vector3 point)
    {
        // Check if there are any colliders (obstructions) within the placement radius
        Collider[] colliders = Physics.OverlapSphere(point, placementRadius, obstructionLayer);

        // If there are any obstructions, return true; otherwise, return false
        return colliders.Length > 0;
    }

    private void SpawnPlants(Vector3 point)
    {
        // Calculate the total sum of probabilities
        float totalProbability = 0f;
        for (int i = 0; i < prefabProbabilities.Length; i++)
        {
            totalProbability += prefabProbabilities[i];
        }

        // Generate a random number between 0 and the total probability
        float randomValue = Random.Range(0f, totalProbability);

        // Loop through the probabilities to find the prefab to spawn
        int prefabIndexToSpawn = 0;
        float cumulativeProbability = prefabProbabilities[0];
        while (randomValue > cumulativeProbability && prefabIndexToSpawn < prefabProbabilities.Length - 1)
        {
            prefabIndexToSpawn++;
            cumulativeProbability += prefabProbabilities[prefabIndexToSpawn];
        }

        // Instantiate the selected prefab
        GameObject grass = Instantiate(grassPref[prefabIndexToSpawn], point, Quaternion.identity) as GameObject;
        //GameObject grass = Instantiate(grassPref[randomPlantIndex], randomPoint, Quaternion.identity) as GameObject;
        grass.transform.parent = transform;
        grass.isStatic = true;
        StaticBatchingUtility.Combine(grass);
    }
}
