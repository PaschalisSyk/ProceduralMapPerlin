using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject dustParticle;
    [SerializeField] GameObject streamParticles;
    [SerializeField] GameObject snowParticles;
    public float spawnInterval = 7.5f;

    bool isSnowing = false;

    Map map;
    Transform[] sandTiles;
    Transform[] riverTiles;
    Transform[] iceTiles;

    private void Awake()
    {
        map = FindObjectOfType<Map>();
    }
    // Start is called before the first frame update
    void Start()
    {
        sandTiles = FindSandTiles();
        riverTiles = FindRiverTiles();
        iceTiles = FindIceTiles();

        // Start the spawn timer
        if(sandTiles.Length > 0)
        {
            InvokeRepeating("SpawnDustParticle", 2f, spawnInterval);
        }
        //InvokeRepeating("SpawnStreamParticle", 2f, 4f);
        Invoke("SpawnStreamParticle", 2f);

        if(map.enviroment == Map.Enviroment.Iceland)
        {
            if(snowParticles != null)
            {
                InvokeRepeating("SpawnSnowParticles", 2f, 30f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Transform[] FindSandTiles()
    {
        List<Transform> sandTileTransforms = new List<Transform>();

        foreach (Tile tile in map.tiles)
        {
            if(tile != null)
            {
                if (tile.tileValue == Tile.TileValue.Sand)
                {
                    sandTileTransforms.Add(tile.transform);
                }
            }
        }

        return sandTileTransforms.ToArray();
    }

    Transform[] FindRiverTiles()
    {
        List<Transform> riverTilesTrans = new List<Transform>();

        foreach (Tile tile in map.tiles)
        {
            if (tile != null)
            {
                if (tile.tileValue == Tile.TileValue.River)
                {
                    riverTilesTrans.Add(tile.transform);
                }
            }
        }

        return riverTilesTrans.ToArray();
    }

    Transform[] FindIceTiles()
    {
        List<Transform> iceTilesTrans = new List<Transform>();

        foreach (Tile tile in map.tiles)
        {
            if (tile != null)
            {
                if (tile.tileValue == Tile.TileValue.Ice)
                {
                    iceTilesTrans.Add(tile.transform);
                }
            }
        }

        return iceTilesTrans.ToArray();
    }

    //List<Transform> FindTilesOfValue(Tile.TileValue tileValue)
    //{
    //    List<Transform> riverTilesTrans = new List<Transform>();

    //    foreach (Tile tile in map.tiles)
    //    {
    //        if (tile != null)
    //        {
    //            if (tile.tileValue == tileValue)
    //            {
    //                riverTilesTrans.Add(tile.transform);
    //            }
    //        }
    //    }

    //    return riverTilesTrans;
    //}

    private void SpawnDustParticle()
    {
        // Randomly select a sand tile
        Transform selectedSandTile = sandTiles[Random.Range(0, sandTiles.Length)];

        // Get a random position within the sand tile's bounds
        Vector3 randomPosition = GetRandomPositionInTile(selectedSandTile);

        // Instantiate the dust particle prefab
        GameObject dust = Instantiate(dustParticle, randomPosition, Quaternion.identity) as GameObject;
        dust.transform.parent = transform;
        //dust.transform.localScale = new Vector3(1f, 1f, 1f);

        Destroy(dust, dust.GetComponent<ParticleSystem>().main.duration * 2.25f);
    }

    private void SpawnStreamParticle()
    {
        // Randomly select a sand tile
        //Transform riverTile = riverTiles[Random.Range(0, riverTiles.Length)];
        foreach(Transform riverTile in riverTiles)
        {
            if(Random.value < 0.3f)
            {
                // Instantiate the dust particle prefab
                GameObject stream = Instantiate(streamParticles, new Vector3(riverTile.transform.position.x, riverTile.transform.position.y + 0.1f, riverTile.transform.position.z), Quaternion.Euler(-90, 180, 0)) as GameObject;
                stream.transform.parent = transform;
            }
        }
        //Destroy(stream, stream.GetComponent<ParticleSystem>().main.duration * 2.25f);
    }

    private Vector3 GetRandomPositionInTile(Transform tile)
    {
        // Calculate random position within the bounds of the tile
        // Modify this based on the size and shape of your tiles
        Vector3 minBounds = tile.GetComponent<Collider>().bounds.min;
        Vector3 maxBounds = tile.GetComponent<Collider>().bounds.max;

        return new Vector3(Random.Range(minBounds.x, maxBounds.x), maxBounds.y + 5f, Random.Range(minBounds.z, maxBounds.z));
    }

    private void SpawnSnowParticles()
    {
        if (isSnowing)
        {
            return;
        }
        StartCoroutine(SnowingRoutine());
    }

    IEnumerator SnowingRoutine()
    {   
        Transform selectIceTile = iceTiles[Random.Range(0, iceTiles.Length)];

        GameObject snow = Instantiate(snowParticles, new Vector3(selectIceTile.position.x, selectIceTile.position.y + 10f, selectIceTile.position.z), Quaternion.identity) as GameObject;
        snow.transform.parent = transform;
        ParticleSystem ps = snow.GetComponent<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.duration = Random.Range(30f, 120f);
        main.maxParticles = Random.Range(10000, 40000);
        var emission = ps.emission;
        emission.rateOverTime = Random.Range(2000, 6000);
        ps.Play();
        isSnowing = true;
        yield return new WaitForSeconds(main.duration);
        isSnowing = false;
        if(ps.isPlaying)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        Destroy(snow, main.duration);
    }
}
