using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("PowerUp Prefabs")]
    public GameObject[] powerUpPrefabs;

    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    // 🔹 REPOSITORY (class İÇİNDE olmalı)
    private IPowerUpRepository repository;

    void Awake()
    {
        repository = new InMemoryPowerUpRepository();
    }

    public void TrySpawn(Vector3 position)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ PowerUp prefab array boş!");
            return;
        }

        // Drop şansı
        if (Random.value > dropChance)
            return;

        // 🔹 REPOSITORY KULLANIMI
        PowerUpType type = repository.GetRandomPowerUp();

        // 🔹 FACTORY KULLANIMI
        PowerUpFactory.Create(
            type,
            position,
            powerUpPrefabs
        );
    }
}
