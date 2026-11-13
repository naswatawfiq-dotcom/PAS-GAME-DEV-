using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalKoin;
    private int koinTerkumpul = 0;
    private int jumlahZombieMati = 0;

    void Awake()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
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
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        Debug.Log("Koin diambil: " + koinTerkumpul + " / " + totalKoin);

        if (totalKoin > 0 && koinTerkumpul >= totalKoin)
        {
            Menang();
        }
    }

    void Menang()
    {
        Debug.Log("KAMU MENANG");

        Enemy[] semuaZombie = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (Enemy zombie in semuaZombie)
        {
            zombie.TakeDamage(9999);
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 22;
        GUI.Label(new Rect(16, 16, 480, 36), "Koin: " + koinTerkumpul + " / " + totalKoin);
        GUI.Label(new Rect(16, 52, 480, 36), "Zombie mati: " + jumlahZombieMati);
    }
}