using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Mirror;

public class Bomb : NetworkBehaviour
{

    public float explodeDelay = 1.5f;
    public int power = 1;
    public GameObject explosionPrefab;
    public PlayerController owner;

    private List<IExplosionObserver> observers = new List<IExplosionObserver>();

    private Tilemap staticMap;
    private Tilemap breakableMap;
    private Tilemap hardMap;

    private PlayerController player;

    // Hard wall 2 vuruş
    private static Dictionary<Vector3Int, int> hardHits = new();

    void Start()
    {
        // 🔍 Playerı bul
player = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).FirstOrDefault();




        // 🔍 Sahnedeki tilemap'leri bul
Tilemap[] maps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);


        foreach (var map in maps)
        {
            if (map.gameObject.name.Contains("Static"))
                staticMap = map;
            else if (map.gameObject.name.Contains("Break"))
                breakableMap = map;
            else if (map.gameObject.name.Contains("Hard"))
                hardMap = map;
        }

        if (staticMap == null || breakableMap == null || hardMap == null)
        {
            Debug.LogError(" Tilemap bulunamadı!");
            return;
        }

        //  Bombayı hücre merkezine oturt
        Vector3Int cell = staticMap.WorldToCell(transform.position);
        transform.position = staticMap.GetCellCenterWorld(cell);

        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeDelay);

        //  Merkez
        Vector3Int centerCell = staticMap.WorldToCell(transform.position);
        ExplodeAtCell(centerCell);
        CheckPlayerAtCell(centerCell);

        // 4 yön
        Spread(Vector3Int.up);
        Spread(Vector3Int.down);
        Spread(Vector3Int.left);
        Spread(Vector3Int.right);
        if (owner != null)
    owner.OnBombExploded();


        Destroy(gameObject);
    }

    void Spread(Vector3Int dir)
    {
        Vector3Int startCell = staticMap.WorldToCell(transform.position);

        for (int i = 1; i <= power; i++)
        {
            Vector3Int cell = startCell + dir * i;

            //  Static wall  dur
            if (staticMap.HasTile(cell))
                break;

            ExplodeAtCell(cell);
            CheckPlayerAtCell(cell);

            //  Breakable  1 vuruş
           if (breakableMap.HasTile(cell))
{
    breakableMap.SetTile(cell, null);

    // breakable kırılınca power-up çıkabilir
    Vector3 spawnPos = staticMap.GetCellCenterWorld(cell);

PowerUpSpawner spawner = FindFirstObjectByType<PowerUpSpawner>();
if (spawner != null)
{
    spawner.TrySpawn(spawnPos);
}

break;
}


            // Hard → 2 vuruş
            if (hardMap.HasTile(cell))
            {
                if (!hardHits.ContainsKey(cell))
                    hardHits[cell] = 0;

                hardHits[cell]++;

                if (hardHits[cell] >= 2)
                {
                    hardMap.SetTile(cell, null);
                    hardHits.Remove(cell);
                }

                break;
            }
        }
    }

    void ExplodeAtCell(Vector3Int cell)
    {
NotifyExplosion(transform.position, 1);

        Vector3 pos = staticMap.GetCellCenterWorld(cell);
if (isServer)
{
    GameObject exp = Instantiate(explosionPrefab, pos, Quaternion.identity);
    NetworkServer.Spawn(exp);
}

    }

    //  PLAYER PATLAMA KONTROLÜ
    void CheckPlayerAtCell(Vector3Int cell)
    {
        if (player == null) return;

        if (player.CurrentCell == cell)
        {
            player.Die();
        }
    }
    public void RegisterObserver(IExplosionObserver observer)
{
    if (!observers.Contains(observer))
        observers.Add(observer);
}

public void NotifyExplosion(Vector2 position, int power)
{
    foreach (var observer in observers)
    {
        observer.OnExplosion(position, power);
    }
}

}
