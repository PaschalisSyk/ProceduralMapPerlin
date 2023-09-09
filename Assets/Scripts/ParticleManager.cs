using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject dustParticle;
    public float spawnInterval = 7.5f;

    Map map;
    Transform[] sandTiles;

    private void Awake()
    {
        map = FindObjectOfType<Map>();
    }
    // Start is called before the first frame update
    void Start()
    {
        sandTiles = FindSandTiles();

        // Start the spawn timer
        InvokeRepeating("SpawnDustParticle", 2f, spawnInterval);
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

    private Vector3 GetRandomPositionInTile(Transform tile)
    {
        // Calculate random position within the bounds of the tile
        // Modify this based on the size and shape of your tiles
        Vector3 minBounds = tile.GetComponent<Collider>().bounds.min;
        Vector3 maxBounds = tile.GetComponent<Collider>().bounds.max;

        return new Vector3(Random.Range(minBounds.x, maxBounds.x), maxBounds.y + 5f, Random.Range(minBounds.z, maxBounds.z));
    }
}
