using UnityEngine;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    [Header("Tiles Distribution")]
    [SerializeField] [Range(0, 1)] float waterLevel = 0.2f;
    [SerializeField] [Range(0 , 1)] float sandLevel = 0.55f;
    [SerializeField] [Range(0, 1)] float forestLevel = 0.7f;
    [SerializeField] [Range(0, 1)] float mountainLevel = 0.8f;

    [Header("Noise Values")]
    public float scale = 0.2f;
    public float fallOffScale = 0.5f;
    public int size = 100;
    public float tileSize = 4;

    Vector3 offset;
    [SerializeField] bool isMonocromatic = false;

    public GameObject[] prefabs;

    List<Vector3> walkableTiles = new List<Vector3>();

    public Tile[,] tiles;

    public StartingTile[] startingTiles;

    [System.Serializable]
    public class StartingTile
    {
        public int x;
        public int z;
        public float height;
    }

    private void Start()
    {
        offset = transform.position;
        MapGen(size);       
    }

    public bool IsMonocromatic()
    {
        return isMonocromatic;
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
        foreach (StartingTile sTile in startingTiles)
        {
            if(sTile != null)
            {
                MakeTile(sTile.x, sTile.z, sTile.height);
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y] * fallOffScale;
                
                if(tiles[x,y] == null)
                {
                    MakeTile(x, y, noiseValue);
                }
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

    void MakeTile( int x, int y , float noiseValue)
    {
        int index = SetIndex(noiseValue);
        float xPos = x * tileSize + (y % 2 == 0 ? tileSize / 2 : 0);
        float yPos = y * tileSize * 0.75f + (y * 0.45f);
        float height = noiseValue * 0.01f;

        if (index == 0)
        {
            return;
        }
        if(prefabs[index] != null)
        {
            GameObject tileObject = Instantiate(prefabs[index], new Vector3(xPos + offset.x, height, yPos + offset.z), Quaternion.identity) as GameObject;
            tiles[x, y] = tileObject.GetComponent<Tile>();
            tileObject.transform.parent = transform;
            tiles[x, y].Init(x, y);
            if (tileObject.tag == "Ground")
            {
                walkableTiles.Add(tileObject.transform.position);
            }
        }
    }
}
