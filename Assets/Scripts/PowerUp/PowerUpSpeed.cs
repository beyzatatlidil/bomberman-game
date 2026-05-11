using UnityEngine;
using System.Collections;

public class PowerUpSpeed : MonoBehaviour
{
    public float speedIncrease = 2f;
    public float duration = 5f;

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

            // ✅ ANINDA "ALINDI" HİSSİ
            if (sr != null) sr.enabled = false;
            if (col != null) col.enabled = false;

            StartCoroutine(ApplySpeed(player));
        }
    }

    private IEnumerator ApplySpeed(PlayerController player)
    {
        player.moveSpeed += speedIncrease;

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.moveSpeed -= speedIncrease;

        Destroy(gameObject);
    }
}
