using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archery : MonoBehaviour
{
    [Header("References")]
    // public Animator animator;
    public GameObject banditCharacter;
    public Transform arrowSpawnPoint;
    public GameObject arrowPrefab;
    public GameObject coinPrefab;

    [Header("Archery Settings")]
    public float detectionRadius = 8f;
    public float fireRate = 1f;
    public float arrowSpeed = 15f;

    [Header("Coin Stack")]
    public Transform[] coinStackPoints; // size = 4
    public float coinLayerHeight = 0.15f;

    private List<GameObject> storedCoins = new List<GameObject>();
    private int coinStackIndex = 0;

    private float lastFireTime;
    private ArcheryAnimStates currentState;

    void Start()
    {
        ChangeState(ArcheryAnimStates.BowIdle);

    }

    // Update is called once per frame
    void Update()
    {
        CheckAndShootMummy();
    }

    // -------------------------------------------------
    // CORE LOGIC
    // -------------------------------------------------
    void CheckAndShootMummy()
    {
        if (Time.time < lastFireTime + (1f / fireRate))
            return;

        Enemy nearest = FindNearestMummy();
        if (nearest == null) return;

        lastFireTime = Time.time;
        nearest.isTriggered = true;

        RotateTowards(nearest.transform.position);
        ChangeState(ArcheryAnimStates.Shoot);

        StartCoroutine(ShootRoutine(nearest));
    }

    Enemy FindNearestMummy()
    {
        Enemy[] mummies = FindObjectsOfType<Enemy>();
        float minDist = detectionRadius;
        Enemy nearest = null;

        foreach (var m in mummies)
        {
            if (m.isDead || m.isTriggered) continue;

            float dist = Vector3.Distance(transform.position, m.transform.position);
            if (dist <= minDist)
            {
                minDist = dist;
                nearest = m;
            }
        }

        return nearest;
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - banditCharacter.transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            banditCharacter.transform.rotation = Quaternion.LookRotation(dir);
    }

    // -------------------------------------------------
    // SHOOT
    // -------------------------------------------------
    IEnumerator ShootRoutine(Enemy enemy)
    {
        yield return new WaitForSeconds(0.2f);

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        arrow.transform.LookAt(enemy.transform);

        Vector3 targetPos = enemy.transform.position + Vector3.up * 1f;

        while (Vector3.Distance(arrow.transform.position, targetPos) > 0.2f)
        {
            arrow.transform.position = Vector3.MoveTowards(
                arrow.transform.position,
                targetPos,
                arrowSpeed * Time.deltaTime
            );
            yield return null;
        }

        Destroy(arrow);

        if (!enemy.isDead)
        {
            // SpawnCoinToStack(enemy.transform.position);
            enemy.Damage();
        }

        yield return new WaitForSeconds(0.3f);
        ChangeState(ArcheryAnimStates.BowIdle);
    }

    // -------------------------------------------------
    // COINS
    // -------------------------------------------------
    void SpawnCoinToStack(Vector3 startPos)
    {
        Transform targetPoint = GetNextCoinStackPoint();
        if (targetPoint == null) return;

        GameObject coin = Instantiate(coinPrefab, startPos, Quaternion.identity);
        storedCoins.Add(coin);

        Vector3 targetPos = targetPoint.position;
        StartCoroutine(MoveCoin(coin, targetPos));

        coinStackIndex++;
    }

    IEnumerator MoveCoin(GameObject coin, Vector3 target)
    {
        float t = 0;
        Vector3 start = coin.transform.position;

        while (t < 1)
        {
            t += Time.deltaTime / 0.6f;
            coin.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }

    Transform GetNextCoinStackPoint()
    {
        if (coinStackPoints.Length == 0) return null;

        int pointIndex = coinStackIndex % coinStackPoints.Length;
        int layer = coinStackIndex / coinStackPoints.Length;

        Transform basePoint = coinStackPoints[pointIndex];
        Vector3 pos = basePoint.position;
        pos.y += layer * coinLayerHeight;

        GameObject temp = new GameObject("CoinPoint");
        temp.transform.position = pos;
        return temp.transform;
    }

    // -------------------------------------------------
    // COLLECT STACK
    // -------------------------------------------------
    public void CollectStoredCoins(Transform player)
    {
        StartCoroutine(CollectRoutine(player));
    }

    IEnumerator CollectRoutine(Transform player)
    {
        foreach (var coin in storedCoins)
        {
            Vector3 start = coin.transform.position;
            Vector3 end = player.position + Vector3.up * 2;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / 0.4f;
                coin.transform.position =
                    Vector3.Lerp(start, end, t) +
                    Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.8f;
                yield return null;
            }

            Destroy(coin);
            // HUD.AddCoin(1);
        }

        storedCoins.Clear();
        coinStackIndex = 0;
    }

    // -------------------------------------------------
    // ANIMATION
    // -------------------------------------------------
    void ChangeState(ArcheryAnimStates state)
    {
        if (currentState == state) return;
        currentState = state;

        // animator.CrossFade(state.ToString().ToLower(), 0.25f);
    }
}
public enum ArcheryAnimStates
{
    AimIdle,
    BowIdle,
    Shoot
}