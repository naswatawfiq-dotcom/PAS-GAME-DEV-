using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalKoin;

    [Header("UI (opsional, kalau kosong pakai teks OnGUI)")]
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text zombieText;
    [SerializeField] private GameObject winPanel;

    private int koinTerkumpul = 0;
    private int jumlahZombieMati = 0;
    private bool sudahMenang = false;

    void Awake()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        UpdateTeks();
    }

    void OnEnable()
    {
        Enemy.OnZombieMati += SaatZombieMati;
    }

    void OnDisable()
    {
        Enemy.OnZombieMati -= SaatZombieMati;
    }

    void SaatZombieMati(Enemy zombie)
    {
        jumlahZombieMati++;
        Debug.Log("GameManager dengar event. Zombie mati: " + jumlahZombieMati + " (" + zombie.name + ")");
        UpdateTeks();
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        Debug.Log("Koin diambil: " + koinTerkumpul + " / " + totalKoin);
        UpdateTeks();

        if (totalKoin > 0 && koinTerkumpul >= totalKoin)
        {
            Menang();
        }
    }

    void UpdateTeks()
    {
        if (coinsText != null)
            coinsText.text = sudahMenang ? "Kamu menang!" : "Koin: " + koinTerkumpul + " / " + totalKoin;

        if (zombieText != null)
            zombieText.text = "Zombie mati: " + jumlahZombieMati;
    }

    void Menang()
    {
        if (sudahMenang) return;
        sudahMenang = true;

        Debug.Log("KAMU MENANG");
        if (winPanel != null) winPanel.SetActive(true);
        UpdateTeks();

        // Berhenti memunculkan zombie baru
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null) spawner.enabled = false;

        // Semua zombie yang ada dimatikan
        Enemy[] semuaZombie = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Enemy zombie in semuaZombie)
        {
            zombie.TakeDamage(9999);
        }
    }

    void OnGUI()
    {
        if (coinsText != null) return;   // UI TMP sudah dipakai

        GUI.skin.label.fontSize = 22;
        GUI.Label(new Rect(16, 16, 480, 36), "Koin: " + koinTerkumpul + " / " + totalKoin);
        GUI.Label(new Rect(16, 52, 480, 36), "Zombie mati: " + jumlahZombieMati);
    }
}