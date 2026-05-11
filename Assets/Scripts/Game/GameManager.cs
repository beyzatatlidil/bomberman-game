using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI winnerText;

    [Header("Spawn Points")]
    public Transform spawnPointA;
    public Transform spawnPointB;

    [SyncVar(hook = nameof(OnRoundChanged))]
    private int currentRound = 1;

    private readonly List<PlayerController> alivePlayers = new();

    // Round state
    private bool roundInProgress = true;

    // ✅ Puan kırdırmayan mantık: gerçek maç var mı?
    // En az 2 oyuncu kayıtlıysa TRUE olur.
    private bool validMatch = false;

    void Awake()
    {
        Instance = this;
    }

   public override void OnStartServer()
{
    currentRound = 1;
    alivePlayers.Clear();
    roundInProgress = true;
    validMatch = false;

    RpcHideWinner(); // 🔒 SERVER BAŞLARKEN KAPAT
}

public override void OnStartClient()
{
    if (winnerText != null)
        winnerText.gameObject.SetActive(false); // 🔒 CLIENT BAŞLARKEN KAPAT
}


    // -------------------------
    // SERVER: Register
    // -------------------------
    [Server]
    public void RegisterPlayer(PlayerController player)
    {
        if (player == null) return;

        if (!alivePlayers.Contains(player))
            alivePlayers.Add(player);

        // ✅ En az 2 oyuncu varsa artık "geçerli maç"
        if (alivePlayers.Count >= 2)
            validMatch = true;
    }

    // -------------------------
    // SERVER: Player death
    // -------------------------
    [Server]
    public void PlayerDied(PlayerController player)
    {
        // Aynı round içinde birden fazla kez tetiklenmeyi engelle
        if (!roundInProgress) return;

        alivePlayers.Remove(player);
        roundInProgress = false;

        // 🔵 Geçerli maç yoksa (tek oyuncu test vb.) winner gösterme
        if (!validMatch)
        {
            StartCoroutine(NextRoundCoroutine(1.5f));
            return;
        }

        // 🟢 Geçerli maç varsa: son hayatta kalan winner
        if (alivePlayers.Count == 1)
        {
          RpcShowWinner(alivePlayers[0].playerDisplayName);

            StartCoroutine(NextRoundCoroutine(3f));
        }
        else
        {
            // Bu senaryo genelde "aynı anda ölüm" gibi durumlar.
            // Winner göstermeden round resetle.
            StartCoroutine(NextRoundCoroutine(1.5f));
        }
    }

    // -------------------------
    // SERVER: Round transition
    // -------------------------
    [Server]
    IEnumerator NextRoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextRound();
    }

    [Server]
    void NextRound()
    {
        // ✅ Yeni round başlangıcı
        currentRound++;
        roundInProgress = true;

        // ✅ Yeni round için match flag'i yeniden hesaplayacağız
        validMatch = false;

        RpcHideWinner();
        ResetPlayersServer();

        // Alive listeyi yeniden kur
        alivePlayers.Clear();
        foreach (var p in FindObjectsOfType<PlayerController>())
            RegisterPlayer(p);
    }

    // -------------------------
    // SERVER: Player reset
    // -------------------------
    [Server]
    void ResetPlayersServer()
    {
        var players = FindObjectsOfType<PlayerController>();

        for (int i = 0; i < players.Length; i++)
        {
            var p = players[i];
            if (p == null) continue;

            Vector3 spawn =
                (i == 0 && spawnPointA != null) ? spawnPointA.position :
                (i == 1 && spawnPointB != null) ? spawnPointB.position :
                p.transform.position;

            // Server authoritative position
            p.transform.position = spawn;

            // Client'lara uygulat (görsel sync garanti)
            p.RpcForcePosition(spawn);

            // Hareket/collider/sprite vb. geri aç
            p.ResetForNewRound();
        }
    }

    // -------------------------
    // UI
    // -------------------------
    void OnRoundChanged(int _, int newRound)
    {
        if (roundText != null)
            roundText.text = $"Round {newRound}";
    }

    [ClientRpc]
    void RpcShowWinner(string winner)
    {
        if (!winnerText) return;
        winnerText.gameObject.SetActive(true);
        winnerText.text = $"🏆 {winner} WINS!";
    }

    [ClientRpc]
    void RpcHideWinner()
    {
        if (winnerText)
            winnerText.gameObject.SetActive(false);
    }
}
