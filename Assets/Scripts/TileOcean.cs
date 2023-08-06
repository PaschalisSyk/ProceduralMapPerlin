using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class TileOcean : MonoBehaviour
{
    public int size = 40;
    public float tileSize = 1.0f;
    public GameObject[] tilePrefabs;

    Tile[,] oceanTiles;

    void Start()
    {
        Field(size);
    }

    void Field(int size)
    {
        oceanTiles = new Tile[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float xPos = x * tileSize + (y % 2 == 0 ? tileSize / 2 : 0);
                float yPos = y * tileSize * 0.75f + (y * 0.45f);
                float height = Random.Range(0, 0.03f);

                if (oceanTiles[x , y] == null)
                {
                    GameObject tileObject = Instantiate(tilePrefabs[0], new Vector3(xPos - size, height, yPos - size * 0.75f), Quaternion.identity) as GameObject;
                    tileObject.transform.parent = transform;
                    tileObject.GetComponent<Tile>().Init(x, y);
                    tileObject.isStatic = true;
                    StaticBatchingUtility.Combine(tileObject);
                }
            }
        }
    }
}
