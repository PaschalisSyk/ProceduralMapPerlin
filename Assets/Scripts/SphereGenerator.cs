using UnityEngine;

public class SphereTileGenerator : MonoBehaviour
{
    public int rows = 10; // Number of rows on the sphere
    public int columns = 10; // Number of columns on the sphere
    public float radius = 1f; // Radius of the sphere
    public GameObject tilePrefab; // Prefab for the tile

    private void Start()
    {
        //GenerateTilesOnSphere();
    }

   public void GenerateTilesOnSphere()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate the normalized position on the sphere
                float u = (float)col / (columns - 1);
                float v = (float)row / (rows - 1);
                float theta = u * 2f * Mathf.PI;
                float phi = v * Mathf.PI;

                // Convert spherical coordinates to Cartesian coordinates
                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi);

                // Calculate the position with the radius
                Vector3 position = new Vector3(x, y, z) * radius;

                // Instantiate the tile at the calculated position
                Instantiate(tilePrefab, position, Quaternion.identity, transform);

            }
        }
    }
}
