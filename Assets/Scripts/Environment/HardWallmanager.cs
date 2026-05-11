using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HardWallManager : MonoBehaviour
{
    public Tilemap wallHardTilemap;

    // Her hücrenin son hasar aldığı frame
    private Dictionary<Vector3Int, int> lastHitFrame = new();
    private Dictionary<Vector3Int, int> hitCounter = new();

    public void DamageCell(Vector3Int cell)
    {
        if (wallHardTilemap == null)
            return;

        if (!wallHardTilemap.HasTile(cell))
            return;

        int currentFrame = Time.frameCount;

        // 🔒 AYNI FRAME'DE GELEN HASARI YOK SAY
        if (lastHitFrame.ContainsKey(cell) && lastHitFrame[cell] == currentFrame)
            return;

        lastHitFrame[cell] = currentFrame;

        if (!hitCounter.ContainsKey(cell))
            hitCounter[cell] = 0;

        hitCounter[cell]++;

        Debug.Log($"HardWall hit {hitCounter[cell]} at {cell}");

        if (hitCounter[cell] >= 2)
        {
            wallHardTilemap.SetTile(cell, null);
            hitCounter.Remove(cell);
            lastHitFrame.Remove(cell);

            Debug.Log("HardWall DESTROYED at " + cell);
        }
    }
}
