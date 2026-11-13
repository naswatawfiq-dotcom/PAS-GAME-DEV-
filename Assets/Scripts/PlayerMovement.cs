using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float kecepatan = 5f;
    private Vector2 arahGerak;
    public int skor = 0;

    // Variabel baru untuk menampung referensi GameManager
    private GameManager gameManager;

    void Start()
    {
        // Mencari objek GameManager yang ada di scene secara otomatis saat game dimulai
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    void OnMove(InputValue value)
    {
        arahGerak = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 arah = new Vector3(arahGerak.x, arahGerak.y, 0);
        transform.position += arah * kecepatan * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            skor++;
            Debug.Log("Skor saat ini: " + skor);

            // MEMANGGIL FUNGSI DI GAMEMANAGER
            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
        }
    }
}