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
    Map map;

    private void Start()
    {
        map = transform.root.GetComponent<Map>();
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

        Quaternion rotation = Quaternion.identity;

        if(grassPref[prefabIndexToSpawn].tag == "Batch")
        {
            // Instantiate the batch prefab
            GameObject _grass = Instantiate(grassPref[prefabIndexToSpawn], transform.position, Quaternion.identity) as GameObject;

            //Set instantiation point
            point = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.max.y, GetComponent<Collider>().bounds.center.z);

            // Get all the children of the batch prefab
            Transform[] children = _grass.GetComponentsInChildren<Transform>();

            // Loop through the children and modify them
            foreach (Transform child in children)
            {
                if(Random.Range(0f , 1f) <= 0.3f)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                    child.rotation = Quaternion.Euler(0, Random.Range(-40, 40), 0);
                }
            }

            // Instantiate the selected prefab
            GameObject updatedGrass = Instantiate(_grass, point, rotation) as GameObject;
            Destroy(_grass);
            updatedGrass.transform.parent = transform;
            StaticBatchingUtility.Combine(updatedGrass);
        }
        else
        {
            // Instantiate the selected prefab
            GameObject grass = Instantiate(grassPref[prefabIndexToSpawn], point, rotation) as GameObject;
            grass.transform.parent = transform;
            if (map.IsMonocromatic())
            {
                AssignMaterial(grass);
            }
            grass.isStatic = true;
            StaticBatchingUtility.Combine(grass);
        }
    }

    void AssignMaterial(GameObject go)
    {
        go.GetComponent<MeshRenderer>().material.SetColor("Color_FA85148A", GetComponent<MeshRenderer>().material.color);
        go.GetComponent<MeshRenderer>().material.SetColor("Color_369F793F", GetComponent<MeshRenderer>().material.color);
    }
}
