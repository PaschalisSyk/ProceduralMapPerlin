using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetGenerator : MonoBehaviour
{
    public float radius = 5f;
    public int subdivisions = 20;
    public GameObject[] prefabs;
    //public Map map;

    void Start()
    {
        GenerateSphereMesh();
        GenerateMap();
    }

    Mesh GenerateSphereMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        // Generate vertices
        Vector3[] vertices = new Vector3[(subdivisions + 1) * (subdivisions + 1)];
        for (int i = 0; i <= subdivisions; i++)
        {
            float latitude = Mathf.PI * (i / (float)subdivisions);
            float sinLat = Mathf.Sin(latitude);
            float cosLat = Mathf.Cos(latitude);

            for (int j = 0; j <= subdivisions; j++)
            {
                float longitude = 2f * Mathf.PI * (j / (float)subdivisions);
                float sinLong = Mathf.Sin(longitude);
                float cosLong = Mathf.Cos(longitude);

                float x = cosLong * sinLat;
                float y = cosLat;
                float z = sinLong * sinLat;

                vertices[i * (subdivisions + 1) + j] = new Vector3(x, y, z) * radius;
            }
        }

        // Generate triangles
        int[] triangles = new int[subdivisions * subdivisions * 6];
        int index = 0;
        for (int i = 0; i < subdivisions; i++)
        {
            for (int j = 0; j < subdivisions; j++)
            {
                int topLeft = i * (subdivisions + 1) + j;
                int topRight = topLeft + 1;
                int bottomLeft = (i + 1) * (subdivisions + 1) + j;
                int bottomRight = bottomLeft + 1;

                triangles[index++] = topLeft;
                triangles[index++] = bottomLeft;
                triangles[index++] = topRight;

                triangles[index++] = topRight;
                triangles[index++] = bottomLeft;
                triangles[index++] = bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;

        return mesh;
    }

    void GenerateMap()
    {
        Tile[,] tiles = new Tile[subdivisions + 1, subdivisions + 1];

        Mesh mesh = GenerateSphereMesh();

        // Map generation logic here, using subdivisions as the grid size
        //map.MapGen(subdivisions);

        // Instantiate tiles on the sphere surface based on the generated map
        for (int i = 0; i <= subdivisions; i++)
        {
            for (int j = 0; j <= subdivisions; j++)
            {
                Vector3 vertexPosition = mesh.vertices[i * (subdivisions + 1) + j];
                Quaternion rotation = Quaternion.LookRotation(vertexPosition, Vector3.up);
                GameObject tileObject = Instantiate((prefabs[0]), vertexPosition, rotation);
                tileObject.transform.parent = transform;
            }
        }
    }
}

