using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Archery : MonoBehaviour
{
    [Header("References")]
    // public Animator animator;
    public Animator animator;
    public GameObject banditCharacter;
    public Transform arrowSpawnPoint, arrowSpawnPoint2;
    public GameObject arrowPrefab;
    public ParticleSystem effect, effect2;

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
    public bool isSecondTower;
    public MoneyTrigger moneyTrigger;

    public GameObject gun1, gun2, bulletIcon;
    public List<Enemy> triggeredEnemy;
    public bool isArchery, isMultiShot;

    void Start()
    {
        ChangeState(ArcheryAnimStates.BowIdle);
        if (isArchery) banditCharacter.transform.DORotate(new Vector3(0, 230, 0), 2).SetLoops(-1, LoopType.Yoyo);

        if (isMultiShot)
        {
            animator.Play("MultyShot");
        }
        else
        {
            animator.Play("Shoot");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndShootMummy();
    }

    void OnTriggerEnter(Collider other)
    {
        if (enabled) return;
        if (other.TryGetComponent(out PlayerController player))
        {
            if (player.allBullets.Count <= 0) return;

            StartCoroutine(DropBullets());
        }
    }

    IEnumerator DropBullets()
    {
        var player = PlayerController.instance;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            var obj = player.allBullets[player.allBullets.Count - 1];
            player.allBullets.Remove(obj);
            obj.transform.DOJump(banditCharacter.transform.position, 2, 1, 0.3f).OnComplete(() => Destroy(obj.gameObject));
        }
        yield return new WaitForSeconds(0.5f);
        this.enabled = true;
        bulletIcon.SetActive(false);
        if (isSecondTower)
        {
            moneyTrigger.DropMoney();
        }
    }

    // -------------------------------------------------
    // CORE LOGIC
    // -------------------------------------------------
    void CheckAndShootMummy()
    {
        if (Time.time < lastFireTime + (1f / fireRate))
            return;

        if (isMultiShot)
        {
            for (int i = 0; i < 3; i++)
            {
                Enemy nearest;
                if (isArchery)
                {
                    nearest = FindNearestTriggeredEnemyInForward();
                }
                else
                {
                    nearest = FindNearestMummy();
                }
                if (nearest == null) return;

                lastFireTime = Time.time;
                nearest.isTriggered = true;

                // RotateTowards(nearest.transform.position);
                ChangeState(ArcheryAnimStates.Shoot);

                StartCoroutine(ShootRoutine(nearest));
            }
        }
        else
        {
            Enemy nearest;
            if (isArchery)
            {
                nearest = FindNearestTriggeredEnemyInForward();
            }
            else
            {
                nearest = FindNearestMummy();
            }
            if (nearest == null) return;

            lastFireTime = Time.time;
            nearest.isTriggered = true;

            // RotateTowards(nearest.transform.position);
            ChangeState(ArcheryAnimStates.Shoot);

            StartCoroutine(ShootRoutine(nearest));
        }
    }

    Enemy FindNearestTriggeredEnemyInForward()
    {
        if (triggeredEnemy == null || triggeredEnemy.Count == 0)
            return null;

        float minDist = detectionRadius;
        Enemy nearest = null;

        Vector3 origin = banditCharacter.transform.position;
        Vector3 forward = banditCharacter.transform.forward;

        // iterate backwards so we can safely remove
        for (int i = triggeredEnemy.Count - 1; i >= 0; i--)
        {
            Enemy enemy = triggeredEnemy[i];

            if (enemy == null || enemy.isDead)
            {
                triggeredEnemy.RemoveAt(i);
                continue;
            }

            if (enemy.isTriggered)
                continue;

            Vector3 dir = enemy.transform.position - origin;
            float distance = dir.magnitude;

            if (distance > detectionRadius)
                continue;

            dir.Normalize();

            // forward direction check
            float dot = Vector3.Dot(forward, dir);
            if (dot < 0.1f) // shooting cone
                continue;

            if (distance < minDist)
            {
                minDist = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    Enemy FindNearestMummyInForward()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        float minDist = detectionRadius;
        Enemy nearest = null;

        Vector3 origin = banditCharacter.transform.position;
        Vector3 forward = banditCharacter.transform.forward;

        foreach (var enemy in enemies)
        {
            if (enemy.isDead || enemy.isTriggered)
                continue;

            Vector3 dir = enemy.transform.position - origin;
            float distance = dir.magnitude;

            if (distance > detectionRadius)
                continue;

            dir.Normalize();

            // Forward direction check
            float dot = Vector3.Dot(forward, dir);

            // Adjust this value to control shoot angle
            if (dot < 0.1f)
                continue;

            if (distance < minDist)
            {
                minDist = distance;
                nearest = enemy;
            }
        }

        return nearest;
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

        GameObject arrow;
        if (isMultiShot)
        {
            arrow = Instantiate(arrowPrefab, arrowSpawnPoint2.position, arrowSpawnPoint2.rotation);
        }
        else
        {
            arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        }
        arrow.transform.LookAt(enemy.transform);

        if (isMultiShot) arrow.GetComponent<Bullet>().impactRange = 2;

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
    // ANIMATION
    // -------------------------------------------------
    void ChangeState(ArcheryAnimStates state)
    {
        if (currentState == state) return;
        currentState = state;

        // animator.CrossFade(state.ToString().ToLower(), 0.25f);
    }

    public void ChangeGun()
    {
        gun1.SetActive(false);
        gun2.SetActive(true);
        // arrowPrefab = ObjectPooling.Instance.poolPrefabs[6].prefab.gameObject;
        fireRate = 7;
        animator.Play("MultyShot");
        isMultiShot = true;
    }
}
public enum ArcheryAnimStates
{
    AimIdle,
    BowIdle,
    Shoot
}