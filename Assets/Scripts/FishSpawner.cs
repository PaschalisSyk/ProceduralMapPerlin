using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefab;
    public Transform waterArea;
    public int numberOfFish = 10;

    void Start()
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            int index = Random.Range(0, 3);
            Vector3 randomPosition = GetRandomPositionInWaterArea();
            if(fishPrefab[index] !=null)
            {
                GameObject fish = Instantiate(fishPrefab[index], randomPosition, Quaternion.identity) as GameObject;
                float value = Random.Range(fish.transform.localScale.x * 0.5f, fish.transform.localScale.x * 1.5f);
                fish.transform.parent = transform;
                fish.transform.localScale = new Vector3(value, value, value);
            }            
        }
    }

    Vector3 GetRandomPositionInWaterArea()
    {
        Bounds waterBounds = waterArea.GetComponent<Renderer>().bounds;
        Vector3 randomPosition = new Vector3(
            Random.Range(waterBounds.min.x, waterBounds.max.x),
            Random.Range(waterBounds.min.y, waterBounds.max.y - 1f),
            Random.Range(waterBounds.min.z, waterBounds.max.z)
        );

        return randomPosition;
    }
}
