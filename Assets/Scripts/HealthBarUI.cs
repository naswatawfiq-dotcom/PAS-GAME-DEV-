using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthFill;     // bar health (depan)
    [SerializeField] private Image depletedFill;   // bar depleted (belakang, menyusul turun)

    [SerializeField] private float depletedDelay = 0.4f;   // jeda sebelum bar depleted turun
    [SerializeField] private float depletedSpeed = 0.5f;   // kecepatan turun (per detik)

    private float depletedTimer;

    void Awake()
    {
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    void Start()
    {
        if (playerHealth == null) return;

        float ratio = (float)playerHealth.Hp / playerHealth.MaxHp;
        if (healthFill != null) healthFill.fillAmount = ratio;
        if (depletedFill != null) depletedFill.fillAmount = ratio;
    }

    void HandleHealthChanged(int current, int max)
    {
        float ratio = (float)current / max;
        if (healthFill != null) healthFill.fillAmount = ratio;

        if (depletedFill == null) return;

        // HP naik (heal): bar depleted ikut langsung. HP turun: tunggu dulu.
        if (depletedFill.fillAmount < ratio)
            depletedFill.fillAmount = ratio;
        else
            depletedTimer = depletedDelay;
    }

    void Update()
    {
        if (healthFill == null || depletedFill == null) return;
        if (depletedFill.fillAmount <= healthFill.fillAmount) return;

        if (depletedTimer > 0f)
        {
            depletedTimer -= Time.deltaTime;
            return;
        }

        depletedFill.fillAmount = Mathf.MoveTowards(
            depletedFill.fillAmount,
            healthFill.fillAmount,
            depletedSpeed * Time.deltaTime
        );
    }
}