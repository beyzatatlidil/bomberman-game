using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class PlayerController : NetworkBehaviour, IExplosionObserver
{
    public PlayerModel model;

    [Header("Bomb")]
    public int maxBombCount = 1;
    private int currentBombCount = 0;

    [Header("Stats")]
    public int bombPower = 1;
    public float moveSpeed = 6f;

    [Header("Refs")]
    public bool canMove = true;
    public GameObject bombPrefab;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Tilemap groundTilemap;

    public Vector3Int CurrentCell { get; private set; }

    // STATE 
    private IPlayerState currentState;

    // ölüm spamını engelle
    [SyncVar] private bool isDead = false;

    // winner için düzgün isim
    [SyncVar] public string playerDisplayName;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
        if (groundTilemap == null)
            groundTilemap = FindFirstObjectByType<Tilemap>();

        if (groundTilemap == null)
            Debug.LogError("❌ Ground Tilemap bulunamadı!");
    }

    public override void OnStartServer()
    {
        playerDisplayName = $"P{netId}";
        model = new PlayerModel(playerDisplayName);

        GameManager.Instance.RegisterPlayer(this);

        // başlangıç state
        SetState(new AliveState());
    }

    // STATE CORE 
    public void SetState(IPlayerState newState)
    {
        currentState = newState;
        currentState.Enter(this);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        //  davranışı belirler
        currentState.Update(this);

        if (groundTilemap != null)
            CurrentCell = groundTilemap.WorldToCell(transform.position);

        // bomba input'u burada bırakıldı (güvenli)
        if (Input.GetKeyDown(KeyCode.Space))
            PlaceBomb();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    // MOVEMENT
    public void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        movement = new Vector2(x, y).normalized;
    }

    //  BOMB 
    void PlaceBomb()
    {
        if (isDead) return;
        if (currentBombCount >= maxBombCount) return;
        if (!bombPrefab || groundTilemap == null) return;

        Vector3Int cell = groundTilemap.WorldToCell(transform.position);
        Vector3 pos = groundTilemap.GetCellCenterWorld(cell);

        Collider2D hit = Physics2D.OverlapCircle(pos, 0.15f);
        if (hit != null && hit.GetComponent<Bomb>() != null)
            return;

        CmdPlaceBomb(pos);
        currentBombCount++;
    }

    [Command]
    void CmdPlaceBomb(Vector3 pos)
    {
        GameObject b = Instantiate(bombPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(b);

        Bomb bomb = b.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.power = bombPower;
            bomb.owner = this;
            bomb.RegisterObserver(this);
        }
    }

    // DEATH
    [Server]
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied(this);

        RpcDisablePlayer();

        // DEAD STATE
        SetState(new DeadState());
    }

    // OBSERVER 
    public void OnBombExploded()
    {
        currentBombCount = Mathf.Max(0, currentBombCount - 1);
    }

    public void OnExplosion(Vector2 explosionPosition, int power)
    {
        TakeDamage(explosionPosition, power);
    }

    public void TakeDamage(Vector2 explosionPosition, int power)
    {
        if (isDead) return;

        float distance = Vector2.Distance(transform.position, explosionPosition);
        if (distance < 0.6f)
        {
            if (isServer)
                Die();
        }
    }

    public void OnExplosion(Vector2 position)
    {
        // boş
    }

    [ClientRpc]
    void RpcDisablePlayer()
    {
        canMove = false;

        if (TryGetComponent(out Collider2D col))
            col.enabled = false;

        if (TryGetComponent(out SpriteRenderer sr))
            sr.enabled = false;

        if (TryGetComponent(out Rigidbody2D r))
            r.linearVelocity = Vector2.zero;
    }

    //  ROUND RESET
    public void ResetForNewRound()
    {
        isDead = false;
        canMove = true;
        currentBombCount = 0;

        if (TryGetComponent(out Collider2D col)) col.enabled = true;
        if (TryGetComponent(out SpriteRenderer sr)) sr.enabled = true;

        if (TryGetComponent(out Rigidbody2D r))
            r.linearVelocity = Vector2.zero;

        // ALIVE STATE
        SetState(new AliveState());
    }

    [ClientRpc]
    public void RpcForcePosition(Vector3 pos)
    {
        transform.position = pos;

        if (TryGetComponent(out Rigidbody2D r))
            r.linearVelocity = Vector2.zero;
    }
}
