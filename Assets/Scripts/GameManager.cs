using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Map map;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] GameObject player;
    public List<Tile> tilesToSpawn = new List<Tile>();

    private void OnEnable()
    {
        PlayerController.OnChangeEnviroment += ReloadScene;
    }

    private void OnDisable()
    {
        PlayerController.OnChangeEnviroment -= ReloadScene;
    }

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

        //DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        map = FindObjectOfType<Map>();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        foreach (Tile tile in map.tiles)
        {
            if (tile != null)
            {
                if (tile.tag == "Ground")
                {
                    tilesToSpawn.Add(tile);
                }
            }
        }

        //if (tilesToSpawn.Count == 0)
        //{
        //    Debug.LogError("No availiable tiles to spawn player");
        //    return;
        //}

        //int index = Random.Range(0, tilesToSpawn.Count);
        //Vector3 spawnPos = tilesToSpawn[index].transform.position;

        Vector3 spawnPos = GameObject.FindGameObjectWithTag("SpawnTile").GetComponent<Transform>().position;
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

    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        map.ChangeEnvironmentType(EnvironmentType.Desert);
        SceneManager.LoadScene(currentSceneName);
    }
}
