using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Map map;
    [SerializeField] GameObject player;

    void Start()
    {
        map = FindObjectOfType<Map>();
        SpawnPlayer();        
    }

    void SpawnPlayer()
    {
        List<Tile> tilesToSpawn = new List<Tile>();
        foreach (Tile tile in map.tiles)
        {
            if(tile != null)
            {
                if (tile.tag == "Ground")
                {
                    tilesToSpawn.Add(tile);
                }
            }
        }
        if (tilesToSpawn.Count == 0)
        {
            Debug.LogError("No availiable tiles to spawn player");
            return;
        }
        print(tilesToSpawn.Count);

        int index = Random.Range(0, tilesToSpawn.Count);
        Vector3 spawnPos = tilesToSpawn[index].transform.position;
        GameObject Player = Instantiate(player, new Vector3(spawnPos.x, spawnPos.y + 1f, spawnPos.z), Quaternion.identity) as GameObject;
    }
}
