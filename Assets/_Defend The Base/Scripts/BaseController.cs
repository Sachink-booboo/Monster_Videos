using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public GameObject bulletPrefab;
    public Transform spawnPoint, endPoint;
    public Transform storageParent;
    public Storage storage;
    public void Init()
    {
        // InvokeRepeating(nameof(StartProduction), 0.1f, 0.1f);
        StartCoroutine(CallStartProduction());
    }

    public void Init2()
    {
        // InvokeRepeating(nameof(StartProduction), 0.1f, 0.1f);
        StartCoroutine(CallStartProduction2());
    }

    IEnumerator CallStartProduction()
    {
        for (int i = 0; i < 50; i++)
        {
            StartProduction();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator CallStartProduction2()
    {
        for (int i = 0; i < 350; i++)
        {
            StartProduction();
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void StartProduction()
    {
        // Spawn bullet
        particleSystem.Play();
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
