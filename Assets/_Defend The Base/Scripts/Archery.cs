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
    public Transform arrowSpawnPoint;
    public GameObject arrowPrefab;
    public ParticleSystem effect;

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

    public GameObject gun1, gun2;

    void Start()
    {
        ChangeState(ArcheryAnimStates.BowIdle);

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
            this.enabled = true;
            animator.Play("Shoot");
            if (isSecondTower)
            {
                moneyTrigger.DropMoney();
            }
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

        // RotateTowards(nearest.transform.position);
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
        arrowPrefab = ObjectPooling.Instance.poolPrefabs[6].prefab.gameObject;
        fireRate = 5;
        animator.Play("RpgShoot");
    }
}
public enum ArcheryAnimStates
{
    AimIdle,
    BowIdle,
    Shoot
}