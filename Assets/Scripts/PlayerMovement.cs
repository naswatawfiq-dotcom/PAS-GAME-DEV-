using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float kecepatan = 5f;
    public int skor = 0;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 arahGerak;
    private GameManager gameManager;

    // Arah hadap terakhir. Dipakai Blend Tree Idle/Walk (parameter MoveX dan MoveY).
    // Selalu salah satu dari (1,0) (-1,0) (0,1) (0,-1), tidak pernah (0,0),
    // karena (0,0) membuat Blend Tree mencampur SEMUA animasi sekaligus (sprite jadi gepeng).
    private Vector2 arahHadap = Vector2.down;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;      // game top-down, tidak jatuh
            rb.freezeRotation = true;
        }

        anim = GetComponentInChildren<Animator>();
        gameManager = FindFirstObjectByType<GameManager>();

        UpdateAnimasi();   // mulai dengan menghadap bawah
    }

    void OnMove(InputValue value)
    {
        arahGerak = value.Get<Vector2>();
    }

    void Update()
    {
        // Pakai Rigidbody2D supaya tidak tembus tembok. Kalau tidak ada, pakai transform.
        if (rb != null)
        {
            rb.linearVelocity = arahGerak * kecepatan;
        }
        else
        {
            transform.position += (Vector3)(arahGerak * kecepatan * Time.deltaTime);
        }

        UpdateAnimasi();
    }

    void UpdateAnimasi()
    {
        if (anim == null || anim.runtimeAnimatorController == null) return;

        bool berjalan = arahGerak.sqrMagnitude > 0.01f;

        // Ambil arah dominan (4 arah). Saat berhenti, arah hadap terakhir dipertahankan.
        if (berjalan)
        {
            if (Mathf.Abs(arahGerak.x) > Mathf.Abs(arahGerak.y))
                arahHadap = arahGerak.x > 0 ? Vector2.right : Vector2.left;
            else
                arahHadap = arahGerak.y > 0 ? Vector2.up : Vector2.down;
        }

        anim.SetBool(IsWalking, berjalan);
        anim.SetFloat(MoveX, arahHadap.x);
        anim.SetFloat(MoveY, arahHadap.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            skor++;
            Debug.Log("Skor saat ini: " + skor);

            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
        }
    }
}