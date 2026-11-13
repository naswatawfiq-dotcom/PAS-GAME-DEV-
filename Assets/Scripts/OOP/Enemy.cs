using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    // DEKLARASI EVENT ZOMBIE MATI (Harus ada agar GameManager tidak Error CS0117)
    public static event Action<Enemy> OnZombieMati;

    [SerializeField] private int hp = 100;
    public float ms = 2f;

    protected Transform player;

    [Header("State Machine")]
    [SerializeField] private float JarakDeteksi = 6f;
    [SerializeField] private float JarakSerang = 1.5f;
    [SerializeField] private float JedaSerang = 1f;

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

    // FUNGSI UNTUK MENERIMA DAMAGE DARI PLAYER
    public void TakeDamage(int damageAmount)
    {
        if (state == StateZombie.DEAD) return;

        hp -= damageAmount;
        Debug.Log(gameObject.name + " kena damage: " + damageAmount + " | Sisa HP: " + hp);

        if (hp <= 0)
        {
            Mati();
        }
    }

    // FUNGSI PEMANGGIL EVENT SAAT ZOMBIE MATI
    private void Mati()
    {
        state = StateZombie.DEAD;
        Debug.Log(gameObject.name + " telah mati!");

        // Mengirimkan sinyal ke GameManager
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
                Debug.Log("Enemy sedang IDLE");
                break;
            case StateZombie.PATROL:
                Debug.Log("Enemy sedang PATROL");
                break;
            case StateZombie.CHASE:
                Debug.Log("Enemy sedang CHASE");
                break;
            case StateZombie.ATTACK:
                Debug.Log("Enemy sedang ATTACK");
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

    public virtual void Serang()
    {
        Debug.Log("Enemy Serang");
    }

    void PerilakuAttack()
    {
        if (Time.time >= waktuSerangTerakhir + JedaSerang)
        {
            Serang();
            waktuSerangTerakhir = Time.time;
        }
    }
}