using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints, targetPoints;
    // [SerializeField] private int maxEnemy = 200;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("Spawn Area")]
    [SerializeField] private float halfSize = 30f;

    [Header("References")]
    public List<GameObject> enemyPrefab;

    public List<Enemy> enemyList = new List<Enemy>();

    private float spawnTimer;

    // -------------------------------------------------
    // UNITY
    // -------------------------------------------------
    void Start()
    {
        // Spawn initial mummies (same as TS version)
        for (int i = 0; i < 50; i++)
            SpawnEnemy();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }

        /*  // Update mummies + cleanup
         for (int i = enemyList.Count - 1; i >= 0; i--)
         {
             Enemy m = enemyList[i];
             if (m == null)
             {
                 enemyList.RemoveAt(i);
                 continue;
             }

             m.Tick(Time.deltaTime);

             // remove if dead & destroyed
             if (m.isDead && m.transform == null)
                 enemyList.RemoveAt(i);
         } */
    }

    // -------------------------------------------------
    // SPAWN
    // -------------------------------------------------
    void SpawnEnemy()
    {
        // if (enemyList.Count >= maxEnemy)
        //     return;

        var ind = Random.Range(0, spawnPoints.Length);

        Transform spawnPos = spawnPoints[ind];


        GameObject enemyObj = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count)], spawnPos.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        enemy.Init(targetPoints[ind]);

        enemyList.Add(enemy);
    }


    public Transform GetNearestEnemy(Vector3 fromPos)
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemyList)
        {
            if (enemy == null || enemy.isDead)
                continue;

            float dist = Vector3.Distance(fromPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    // -------------------------------------------------
    // SPAWN POSITION
    // -------------------------------------------------
    Vector3 GetRandomSquareBorderPoint()
    {
        float h = halfSize;
        int side = Random.Range(0, 4);
        float t = Random.Range(-h, h);

        switch (side)
        {
            case 0: return new Vector3(h, 0, t);   // +X
            case 1: return new Vector3(-h, 0, t);  // -X
            case 2: return new Vector3(t, 0, h);   // +Z
            default: return new Vector3(t, 0, -h); // -Z
        }
    }
}
