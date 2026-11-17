using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Harus sama dengan Threshold di Blend Tree Move dan Idle (kalau pakai Animator)
    private const float BlendDown = 0f;
    private const float BlendLeft = 1f / 3f;
    private const float BlendRight = 2f / 3f;
    private const float BlendUp = 1f;

    public float kecepatan = 5f;
    public int skor = 0;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 arahGerak;
    private GameManager gameManager;

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
        anim.SetBool("IsWalking", berjalan);

        if (!berjalan) return;

        float blend;
        if (Mathf.Abs(arahGerak.x) > Mathf.Abs(arahGerak.y))
            blend = arahGerak.x > 0 ? BlendRight : BlendLeft;
        else
            blend = arahGerak.y > 0 ? BlendUp : BlendDown;

        anim.SetFloat("Blend", blend);
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