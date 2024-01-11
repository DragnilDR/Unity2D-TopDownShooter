using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyToSpawn
{
    public GameObject enemyPref;
    public int count;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private enum WhoToSpawn { Player, Enemy }
    [SerializeField] private WhoToSpawn whoToSpawn;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private List<EnemyToSpawn> enemies = new List<EnemyToSpawn>();
    [SerializeField] private List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform playerTransform;

    [SerializeField] private float spawnDetectionRange;
    [SerializeField] private float startTimeBtwSpawn;
    [SerializeField] private float timeBtwSpawn;

    private void Awake()
    {
        switch (whoToSpawn)
        {
            case WhoToSpawn.Player:
                SpawnPlayer();
                break;
        }
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        timeBtwSpawn = startTimeBtwSpawn;
    }

    private void Update()
    {
        if (playerTransform.gameObject.activeSelf && whoToSpawn == WhoToSpawn.Enemy)
        {
            float playerDist = Vector2.Distance(transform.position, playerTransform.position);

            if (timeBtwSpawn <= 0)
            {
                if (spawnDetectionRange <= playerDist)
                {
                    GenerateEnemies();
                }
                else if (spawnDetectionRange >= playerDist)
                {
                    SpawnEnemy();
                    timeBtwSpawn = startTimeBtwSpawn;
                }
            }
            else if (timeBtwSpawn >= 0 && spawnDetectionRange <= playerDist)
            {
                timeBtwSpawn -= Time.deltaTime;
            }
        }
    }

    public void SpawnPlayer()
    {
        GameObject playerToSpawn = ObjectPool.Instance.GetPooledObject(playerPrefab);

        if (playerToSpawn != null)
        {
            playerToSpawn.transform.position = transform.position;
            playerToSpawn.transform.rotation = transform.rotation;
            playerToSpawn.SetActive(true);
            playerToSpawn.name = "Player";
        }
    }

    private void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();

        foreach (var enemy in enemies)
        {
            for (int i = 0; i < enemy.count; i++)
            {
                generatedEnemies.Add(enemy.enemyPref);
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    private void SpawnEnemy()
    {
        foreach (var enemyPrefab in enemiesToSpawn)
        {
            Vector3 randomPos = new Vector3(Random.Range(-2, 3), Random.Range(-2, 3), 0);

            GameObject enemy = ObjectPool.Instance.GetPooledObject(enemyPrefab);

            if (enemy != null)
            {
                enemy.transform.position = transform.position + randomPos;
                enemy.transform.rotation = Quaternion.identity;
                enemy.SetActive(true);
            }
        }

        enemiesToSpawn.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnDetectionRange);
    }
}

