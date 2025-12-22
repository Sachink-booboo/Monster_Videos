using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UnlockObject : MonoBehaviour
{
    public int moneyIndex;
    public GameObject unlockObj;
    public MoneyTrigger moneyTrigger;
    public Storage storage;
    public Transform point1, point2, point3;
    public bool isTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            if (isTriggered) return;
            isTriggered = true;
            StartCoroutine(DropMoney());
        }
    }

    IEnumerator DropMoney()
    {
        for (int i = moneyIndex; i < moneyIndex + 16; i++)
        {
            var temp = moneyTrigger.allMoney[i];
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 1, 1, 0.05f);
            yield return new WaitForSeconds(0.03f);
        }
        unlockObj.SetActive(true);
        StartCoroutine(StartMovingBullet());
    }

    IEnumerator StartMovingBullet()
    {
        while (storage.allBullets.Count > 0)
        {
            var temp = storage.allBullets[0];
            storage.allBullets.Remove(temp);
            MoveBullet(temp);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void MoveBullet(GameObject bullet)
    {
        bullet.transform.position = point1.position;
        bullet.transform.rotation = point1.rotation;
        bullet.transform.DOMove(point2.position, 10).SetSpeedBased().OnComplete(() =>
        {
            bullet.transform.rotation = point2.rotation;
            bullet.transform.DOMove(point3.position, 10).SetSpeedBased().OnComplete(() =>
                   {
                       Destroy(bullet.gameObject);
                   });
        });
    }
}
