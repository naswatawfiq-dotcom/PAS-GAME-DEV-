using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;   // isi dengan prefab ConeZombie, BucketHeadZombie, dst.
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 6f);
    [SerializeField] private float minDistanceFromPlayer = 3f;

    private Transform player;
    private float timer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("EnemySpawner: Enemy Prefabs masih kosong.", this);
        }
    }

    void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        int hidup = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (hidup >= maxEnemies) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null) return;

        Instantiate(prefab, TitikSpawnAcak(), Quaternion.identity);
    }

    Vector3 TitikSpawnAcak()
    {
        Vector2 titik = transform.position;

        for (int i = 0; i < 10; i++)
        {
            titik = (Vector2)transform.position + new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f));

            if (player == null || Vector2.Distance(titik, player.position) >= minDistanceFromPlayer)
                break;
        }
        return titik;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}