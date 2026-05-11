using UnityEngine;

public class PowerUpBombCount : MonoBehaviour
{
    public int addBomb = 1;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            used = true;
            player.maxBombCount += addBomb;
            Destroy(gameObject);
        }
    }
}
