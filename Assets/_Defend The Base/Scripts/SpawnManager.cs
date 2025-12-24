using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints, targetPoints;
    // [SerializeField] private int maxEnemy = 200;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("References")]
    public List<GameObject> enemyPrefab;

    public List<Enemy> enemyList = new List<Enemy>();

    private float spawnTimer;
    public bool isStop;

    void Start()
    {
        for (int i = 0; i < 50; i++)
            SpawnEnemy();
    }

    public void SpawnMoreEnemy()
    {
        for (int i = 0; i < 50; i++)
            SpawnEnemy();
    }

    void Update()
    {
        if (isStop) return;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var ind = Random.Range(0, spawnPoints.Length);

        Transform spawnPos = spawnPoints[ind];


        GameObject enemyObj = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count)], spawnPos.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        enemy.Init(targetPoints[ind]);

        enemyList.Add(enemy);
    }
}
