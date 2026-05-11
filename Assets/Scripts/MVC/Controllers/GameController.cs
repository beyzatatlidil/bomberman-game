using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerView playerPrefab;

    private void Awake()
    {
        Debug.Log("✅ GameController Awake çalıştı");

        if (playerPrefab == null)
        {
            Debug.LogError("❌ Player Prefab atanmadı!");
            return;
        }

        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }
}
