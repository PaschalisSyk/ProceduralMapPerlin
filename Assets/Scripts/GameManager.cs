using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Map map;
    //[SerializeField] CameraFollow cameraFollow;
    [SerializeField] GameObject player;
    public List<Tile> tilesToSpawn = new List<Tile>();
    private bool newGame = true;

    private void OnEnable()
    {
        //PlayerController.OnChangeEnviroment += ReloadScene;
        Portal.OnChangeEnvironment += ReloadScene;
    }

    private void OnDisable()
    {
        //PlayerController.OnChangeEnviroment -= ReloadScene;
        Portal.OnChangeEnvironment -= ReloadScene;
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

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Initialize();
        NewGame();
    }

    public bool NewGame()
    {
        return newGame;
    }

    void SpawnPlayer()
    {
        foreach (Tile tile in map.tiles)
        {
            if (tile != null)
            {
                if (tile.tag == "Ground" && tile.tileValue != Tile.TileValue.Ice)
                {
                    tilesToSpawn.Add(tile);
                }
            }
        }

        Vector3 spawnPos = GameObject.FindGameObjectWithTag("SpawnTile").GetComponent<Transform>().position;
        GameObject Player = Instantiate(player, new Vector3(spawnPos.x, spawnPos.y + 1f, spawnPos.z), Quaternion.identity) as GameObject;

        StartCoroutine(StartCameraFollow());
        
    }

    IEnumerator StartCameraFollow()
    {
        // Wait for a short delay
        yield return new WaitForSeconds(2f); // Adjust the delay time as needed

        // Enable the camera follow script
        FindObjectOfType<CameraFollow>().enabled = true;
    }

    public void ReloadScene(EnvironmentType newEnvironment)
    {
        newGame = false;
        tilesToSpawn.Clear();
        Inventory.instance.SaveInventory();
        string currentSceneName = SceneManager.GetActiveScene().name;
        map.ChangeEnvironmentType(newEnvironment);
        SceneManager.LoadScene(currentSceneName);
    }

    public void Initialize()
    {
        map = FindObjectOfType<Map>();
        SpawnPlayer();
        if (!newGame)
        {
            Inventory.instance.LoadInventory();
        }
    }
}
