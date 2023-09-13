using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public TileValue tileValue;

    public enum TileValue
    {
        Water,
        WaterRocks,
        Sand,
        Grass,
        Forest,
        Mountain,
        River
    }

    public void Init(int x , int y)
    {
        xIndex = x;
        yIndex = y;
    }
}
