using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    Map map;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

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

        int index = Random.Range(0, tilesToSpawn.Count);
        Vector3 spawnPos = tilesToSpawn[index].transform.position;
        GameObject Player = Instantiate(player, new Vector3(spawnPos.x, spawnPos.y + 1f, spawnPos.z), Quaternion.identity) as GameObject;

        StartCoroutine(StartCameraFollow());
    }

    IEnumerator StartCameraFollow()
    {
        // Wait for a short delay
        yield return new WaitForSeconds(2f); // Adjust the delay time as needed

        // Enable the camera follow script
        cameraFollow.enabled = true;
    }
}
