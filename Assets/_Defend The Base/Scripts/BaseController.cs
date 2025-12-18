using DG.Tweening;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint, endPoint;
    public Transform storageParent;
    public Storage storage;

    public void Init()
    {
        InvokeRepeating(nameof(StartProduction), 0.1f, 0.1f);
    }

    public void StartProduction()
    {
        // Spawn bullet
        GameObject temp = Instantiate(
            bulletPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Random Y rotation (correct way)
        float randomY = Random.Range(0f, 180f);
        temp.transform.rotation = Quaternion.Euler(
            spawnPoint.eulerAngles.x,
            randomY,
            spawnPoint.eulerAngles.z
        );

        // Move bullet
        temp.transform.DOMove(endPoint.position, 1f)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                temp.transform.parent = storageParent;
                storage.allBullets.Add(temp);
            });
    }
}
