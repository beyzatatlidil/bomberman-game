using UnityEngine;

public class PlayerBombController : MonoBehaviour
{
    public GameObject bombPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bombPrefab == null)
            {
                Debug.LogError("Bomb Prefab atanmadı!");
                return;
            }

            Instantiate(bombPrefab, transform.position, Quaternion.identity);
            Debug.Log("Bomba bırakıldı");
        }
    }
}
