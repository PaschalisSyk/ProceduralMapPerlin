using UnityEngine;

//[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    float waterLevel = 0.3f;
    float sandLevel = 0.55f;
    float forestLevel = 0.7f;
    float mountainLevel = 0.8f;

    public float scale = 0.2f;
    public float fallOffScale = 0.5f;
    public int size = 100;
    public float tileSize = 1;

    public GameObject[] prefabs;

    Tile[,] tiles;

    private void Start()
    {
        MapGen(size);
    }

    public void MapGen(int size)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        tiles = new Tile[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y] * fallOffScale;
                int index = SetIndex(noiseValue);
                float xPos = x * tileSize + (y % 2 == 0 ? tileSize / 2 : 0);
                float yPos = y * tileSize * 0.75f;
                float height = noiseValue * 0.1f;

                GameObject tileObject = Instantiate(prefabs[index], new Vector3(xPos, height, yPos), Quaternion.identity);
                tileObject.transform.SetParent(GameObject.FindWithTag("Map").transform);

            }
        }
    }

    private int SetIndex(float noiseValue)
    {
        int index = 0;

        if (Random.Range(0f, 1f) <= 0.02f)
        {
            index = 1;
        }
        if (noiseValue > waterLevel)
        {
            index = 2;
        }
        if (noiseValue > sandLevel)
        {
            index = 3;
        }
        if (noiseValue > forestLevel)
        {
            index = 4;
        }
        if(noiseValue > mountainLevel)
        {
            index = 5;
        }

        return index;
    }
}
