using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    // Event zombie mati (didengar GameManager)
    public static event Action<Enemy> OnZombieMati;

    [SerializeField] private int hp = 100;
    public float ms = 2f;

    protected Transform player;

    [Header("State Machine")]
    [SerializeField] private float JarakDeteksi = 6f;
    [SerializeField] private float JarakSerang = 1.5f;
    [SerializeField] private float JedaSerang = 1f;

    [Header("Serangan")]
    [SerializeField] protected int damageSerang = 10;

    private StateZombie state = StateZombie.IDLE;
    private StateZombie stateSebelumnya = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player dengan tag 'Player' tidak ditemukan, zombie tidak akan mengejar.", this);
        }
    }

    void Update()
    {
        if (state == StateZombie.DEAD) return;

        PeriksaTransisi();

        if (state != stateSebelumnya)
        {
            MasukStateBaru(state);
            stateSebelumnya = state;
        }

        switch (state)
        {
            case StateZombie.CHASE:
                Kejar();
                break;
            case StateZombie.ATTACK:
                PerilakuAttack();
                break;
        }
    }

    // Implementasi IDamageable (sama seperti PlayerHealth)
    public void KenaDamage(int jumlah)
    {
        if (state == StateZombie.DEAD) return;

        hp -= jumlah;
        Debug.Log(gameObject.name + " kena damage: " + jumlah + " | Sisa HP: " + hp);

        if (hp <= 0)
        {
            Mati();
        }
    }

    // Dipertahankan supaya kode lama (GameManager) tetap jalan
    public void TakeDamage(int damageAmount)
    {
        KenaDamage(damageAmount);
    }

    private void Mati()
    {
        state = StateZombie.DEAD;
        Debug.Log(gameObject.name + " telah mati!");

        OnZombieMati?.Invoke(this);

        Destroy(gameObject);
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    void PeriksaTransisi()
    {
        float jarak = JarakKePlayer();

        if (jarak <= JarakSerang)
        {
            state = StateZombie.ATTACK;
        }
        else if (jarak <= JarakDeteksi)
        {
            state = StateZombie.CHASE;
        }
        else
        {
            state = StateZombie.PATROL;
        }
    }

    void MasukStateBaru(StateZombie stateBaru)
    {
        switch (stateBaru)
        {
            case StateZombie.IDLE:
                Debug.Log(name + " sedang IDLE");
                break;
            case StateZombie.PATROL:
                Debug.Log(name + " sedang PATROL");
                break;
            case StateZombie.CHASE:
                Debug.Log(name + " sedang CHASE");
                break;
            case StateZombie.ATTACK:
                Debug.Log(name + " sedang ATTACK");
                break;
        }
    }

    public void Kejar()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            ms * Time.deltaTime
        );
    }

    // Serangan dasar: kasih damage ke player. Zombie turunan bisa override,
    // lalu panggil base.Serang() supaya damage tetap diberikan.
    public virtual void Serang()
    {
        Debug.Log(name + " menyerang");

        if (player == null) return;

        IDamageable target = player.GetComponentInParent<IDamageable>();
        if (target != null) target.KenaDamage(damageSerang);
    }

    void PerilakuAttack()
    {
        if (Time.time >= waktuSerangTerakhir + JedaSerang)
        {
            Serang();
            waktuSerangTerakhir = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, JarakDeteksi);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, JarakSerang);
    }
}