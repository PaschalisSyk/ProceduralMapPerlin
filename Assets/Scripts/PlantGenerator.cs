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
            tileBounds.max.y + 0.1f,
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

        Quaternion rotation = Quaternion.Euler(0, Random.Range(-60, 60), 0);

        if (grassPref[prefabIndexToSpawn].tag == "Batch")
        {
            if (Random.Range(0f, 1f) <= 0.4f)
            {
                return;
            }
            //Set instantiation point
            point = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.max.y, GetComponent<Collider>().bounds.center.z);
            // Instantiate the batch prefab
            GameObject _grass = Instantiate(grassPref[prefabIndexToSpawn], point, Quaternion.identity) as GameObject;
            // Get all the children of the batch prefab
            Transform[] children = _grass.GetComponentsInChildren<Transform>();

            // Loop through the children and modify them
            foreach (Transform child in children)
            {
                if (child != _grass.transform) // Skip the root of the batch prefab
                {
                    if (Random.Range(0f, 1f) <= 0.4f)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                        child.rotation = Quaternion.Euler(0, Random.Range(-70, 70), 0);

                        float value = Random.Range(child.localScale.x * 0.65f, child.localScale.x * 1.1f);
                        child.localScale = new Vector3(value, value, value);
                        if (map.IsMonocromatic())
                        {
                            child.GetComponentInChildren<MeshRenderer>().material.SetColor("Color_FA85148A", GetComponent<MeshRenderer>().material.color);
                            //child.GetComponentInChildren<MeshRenderer>().material.SetColor("Color_369F793F", GetComponent<MeshRenderer>().material.color);
                        }
                    }
                }
                
            }

            // Instantiate the selected prefab
            GameObject updatedGrass = Instantiate(_grass, point, Quaternion.identity) as GameObject;
            Destroy(_grass);
            updatedGrass.transform.parent = transform;
            //StaticBatchingUtility.Combine(updatedGrass);
        }
        else
        {
            if (Random.value <= 0.75f)
            {
                return;
            }
            // Instantiate the selected prefab
            GameObject grass = Instantiate(grassPref[prefabIndexToSpawn], point, rotation) as GameObject;
            grass.transform.parent = transform;
            float value = Random.Range(grass.transform.localScale.x * 0.65f, grass.transform.localScale.x * 1.1f);
            grass.transform.localScale = new Vector3(value, value, value);
            grass.transform.position = point;
            grass.transform.rotation = Quaternion.Euler(0, Random.Range(-90, 90), 0);
            if (map.IsMonocromatic())
            {
                AssignMaterial(grass);
            }
            grass.isStatic = true;
            //StaticBatchingUtility.Combine(grass);
        }
    }

    void AssignMaterial(GameObject go)
    {
        go.GetComponent<MeshRenderer>().material.SetColor("Color_FA85148A", GetComponent<MeshRenderer>().material.color);
        go.GetComponent<MeshRenderer>().material.SetColor("Color_369F793F", GetComponent<MeshRenderer>().material.color);
    }
}
