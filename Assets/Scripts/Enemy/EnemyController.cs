using UnityEngine;
using Mirror;

public enum EnemyType
{
    StaticMoving,
    Chasing
}

public class EnemyController : NetworkBehaviour
{
    public EnemyType enemyType = EnemyType.StaticMoving;
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Vector2 dir;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 🔥 Server-side init
    public override void OnStartServer()
{
    if (enemyType == EnemyType.StaticMoving)
    {
        PickNewDirection();
        InvokeRepeating(nameof(PickNewDirection), 0.5f, 1.0f);
    }
}


    void FixedUpdate()
    {
        if (!isServer) return;

        switch (enemyType)
        {
            case EnemyType.StaticMoving:
                rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
                break;

            case EnemyType.Chasing:
                ChasePlayer();
                break;
        }
    }

    // -------------------------
    // STATIC MOVING ENEMY
    // -------------------------
    void PickNewDirection()
    {
        int r = Random.Range(0, 4);
        dir = r switch
        {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            _ => Vector2.right
        };
    }

    // -------------------------
    // CHASING ENEMY (SIMPLE AI)
    // -------------------------
   void ChasePlayer()
{
    if (target == null)
        FindTarget();

    if (target == null)
    {
        rb.linearVelocity = Vector2.zero;
        return;
    }

    Vector2 diff = target.position - transform.position;

    Vector2 moveDir;

    // Önce X ekseninde yaklaş
    if (Mathf.Abs(diff.x) > 0.1f)
    {
        moveDir = new Vector2(Mathf.Sign(diff.x), 0);
    }
    // X hizalıysa Y ekseninde yaklaş
    else
    {
        moveDir = new Vector2(0, Mathf.Sign(diff.y));
    }

    rb.linearVelocity = moveDir * moveSpeed;
}

    void FindTarget()
{
    PlayerController[] players =
        FindObjectsOfType<PlayerController>();

    float minDist = float.MaxValue;
    target = null;

    foreach (var p in players)
    {
        if (p == null) continue;
        if (!p.isActiveAndEnabled) continue;

        float d = Vector2.Distance(transform.position, p.transform.position);
        if (d < minDist)
        {
            minDist = d;
            target = p.transform;
        }
    }
}


    // -------------------------
    // COLLISION
    // -------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;

        // Static enemy duvara çarpınca yön değiştirir
        if (enemyType == EnemyType.StaticMoving &&
            collision.gameObject.CompareTag("Wall"))
        {
            dir = -dir;
            return;
        }

        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.Die();
        }
    }
}
