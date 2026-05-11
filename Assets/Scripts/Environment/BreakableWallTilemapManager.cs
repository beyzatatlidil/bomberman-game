using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableWallTilemapManager : MonoBehaviour
{
    public Tilemap breakableTilemap;

    public Vector3Int GetCellFromWorldPosition(Vector3 worldPos)
    {
        return breakableTilemap.WorldToCell(worldPos);
    }
}
