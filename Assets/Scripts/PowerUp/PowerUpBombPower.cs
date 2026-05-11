using UnityEngine;

public class PowerUpBombPower : MonoBehaviour
{
    public int addPower = 1;
    private bool used = false;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            used = true;

            if (sr != null) sr.enabled = false;
            if (col != null) col.enabled = false;

            player.bombPower += addPower;

            Destroy(gameObject);
        }
    }
}
