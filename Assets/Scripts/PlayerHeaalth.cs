using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private bool tampilkanTeksHp = true;   // matikan kalau sudah pakai HealthBarUI

    private int hp;

    public int Hp => hp;
    public int MaxHp => maxHp;

    // (hp sekarang, hp maksimal) - didengar oleh HealthBarUI
    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        // Awake (bukan Start) supaya hp sudah terisi sebelum UI membacanya
        hp = maxHp;
    }

    public void KenaDamage(int jumlah)
    {
        if (hp <= 0) return;   // sudah mati, abaikan

        hp = Mathf.Max(hp - jumlah, 0);
        Debug.Log($"Player kena damage {jumlah}, HP sisa: {hp}");

        OnHealthChanged?.Invoke(hp, maxHp);

        if (hp <= 0) Mati();
    }

    void Mati()
    {
        Debug.Log("Player mati!");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        PlayerMovement gerak = GetComponent<PlayerMovement>();
        if (gerak != null) gerak.enabled = false;
    }

    void OnGUI()
    {
        if (!tampilkanTeksHp) return;
        GUI.skin.label.fontSize = 22;
        GUI.Label(new Rect(16, 88, 480, 36), "HP: " + hp + " / " + maxHp);
    }
}