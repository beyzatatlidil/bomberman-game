using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        // Basit hareket: sola doğru
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player'a değerse öldür
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}
