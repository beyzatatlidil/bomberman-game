using UnityEngine;
using Mirror;

public class Explosion : NetworkBehaviour
{
    public float radius = 0.6f;

    public override void OnStartServer()
    {
        KillPlayers();
    }

    [Server]
void KillPlayers()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        radius
    );

    foreach (var hit in hits)
    {
        PlayerController player =
            hit.GetComponent<PlayerController>();

        if (player != null)
        {
            player.Die();
        }
    }
}

}
