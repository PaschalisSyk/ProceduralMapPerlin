using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject dustParticle;
    [SerializeField] GameObject streamParticles;
    [SerializeField] GameObject snowParticles;
    [SerializeField] GameObject flyingLeavesGO;

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
        if(map._environmentProfile.hasRiver)
        {
            riverTiles = FindRiverTiles();
            Invoke("SpawnStreamParticle", 2f);
        }

        if(map.enviroment == Map.Enviroment.Iceland)
        {
            iceTiles = FindIceTiles();
            if (snowParticles != null)
            {
                InvokeRepeating("SpawnSnowParticles", 2f, 30f);
            }
        }
        if(map.enviroment == Map.Enviroment.Desert)
        {
            sandTiles = FindSandTiles();
            InvokeRepeating("SpawnDustParticle", 2f, spawnInterval);
        }
        if(map.enviroment == Map.Enviroment.Grassland)
        {
            InvokeRepeating("PinkLeavesParticles", 2f, 200f);
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
        if(sandTiles.Length > 0)
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
    }

    private void SpawnStreamParticle()
    {
        // Randomly select a sand tile
        //Transform riverTile = riverTiles[Random.Range(0, riverTiles.Length)];
        foreach(Transform riverTile in riverTiles)
        {
            if(Random.value < 0.1f)
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
        if(iceTiles.Length > 0)
        {
            Transform selectIceTile = iceTiles[Random.Range(0, iceTiles.Length)];

            GameObject snow = Instantiate(snowParticles, new Vector3(selectIceTile.position.x, selectIceTile.position.y + 5f, selectIceTile.position.z), Quaternion.identity) as GameObject;
            snow.transform.parent = transform;
            ParticleSystem ps = snow.GetComponent<ParticleSystem>();
            ps.Stop();
            var main = ps.main;
            main.duration = Random.Range(30f, 120f);
            main.maxParticles = Random.Range(10000, 40000);
            if (Random.value <= 0.5f)
            {
                var fo = ps.forceOverLifetime;
                fo.enabled = true;
                fo.x = Random.Range(-0.15f, 0.15f);
            }
            var emission = ps.emission;
            emission.rateOverTime = Random.Range(2000, 6000);
            ps.Play();
            isSnowing = true;
            yield return new WaitForSeconds(main.duration);
            isSnowing = false;
            if (ps.isPlaying)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            Destroy(snow, main.duration * 0.25f);
        }       
    }

    void PinkLeavesParticles()
    {

        GameObject[] pinkTrees = GameObject.FindGameObjectsWithTag("PinkTree");

        if(pinkTrees.Length > 0)
        {
            //Transform tree = pinkTrees[Random.Range(0, pinkTrees.Length)].transform;
            if(flyingLeavesGO != null)
            {
                for (int i = 0; i < pinkTrees.Length; i++)
                {
                    if (Random.value < 0.25f)
                    {
                        return;
                    }
                    ParticleSystem ps = Instantiate(flyingLeavesGO.GetComponent<ParticleSystem>(),
                    new Vector3(pinkTrees[i].transform.position.x + 5f, pinkTrees[i].transform.position.y + 3f, pinkTrees[i].transform.position.z), Quaternion.identity) as ParticleSystem;
                    ps.transform.parent = transform;
                    PlayParticles(ps);
                    StartCoroutine(DelayStopParticles(ps));
                }
            }
;        }

    }

    public void PlayParticles(ParticleSystem particleSystem)
    {
        StartCoroutine(FadeInParticles(particleSystem));
    }

    // Stop the particle system with a smooth fade out
    public void StopParticles(ParticleSystem particleSystem)
    {
        StartCoroutine(FadeOutParticles(particleSystem));
    }

    IEnumerator FadeInParticles(ParticleSystem ps)
    {
        // Enable the particle system
        ps.Play();

        // Gradual fade in
        float duration = 3.0f; // Adjust the duration as needed
        float elapsedTime = 0f;

        Color startColor = ps.main.startColor.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Full opacity

        while (elapsedTime < duration)
        {
            // Calculate the color based on the elapsed time
            Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);

            // Set the start color to control the opacity
            var mainModule = ps.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);


            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator FadeOutParticles(ParticleSystem ps)
    {
        // Gradual fade out
        float duration = 5.0f; // Adjust the duration as needed
        float elapsedTime = 0f;

        Color startColor = ps.main.startColor.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Full transparency

        while (elapsedTime < duration)
        {
            // Calculate the alpha based on the elapsed time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Set the start lifetime multiplier to control the opacity
            Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);

            // Set the start color to control the opacity
            if(ps != null)
            {
                var mainModule = ps.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);
            }
            
            yield return null;
        }

        // Disable the particle system after fading out
        ps.Stop();
    }

    IEnumerator DelayStopParticles(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(Random.Range(30f, 240f));
        StopParticles(particleSystem);
        yield return new WaitForSeconds(30f);
        Destroy(particleSystem.gameObject);
    }
}
