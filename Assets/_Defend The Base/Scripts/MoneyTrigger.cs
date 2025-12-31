using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public class MoneyTrigger : MonoBehaviour
{
    public List<GameObject> allMoney;
    public bool isTriggered;
    public int index = 0;
    public Collider unlockCollider1, unlockCollider2, unlockCollider3;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            if (isTriggered) return;
            isTriggered = true;
            StartCoroutine(CollectMoney());
        }
    }

    public void DropMoney()
    {
        GameController.instance.DropMoney();
        GetComponent<Collider>().enabled = true;
        // StartCoroutine(StartDropMoney());
    }
    IEnumerator StartDropMoney()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 32; i++)
        {
            yield return new WaitForSeconds(0.03f);
            allMoney[i].transform.parent = transform;
            allMoney[i].transform.DOScale(new Vector3(2f, 2.5f, 2f), 0.1f);
            allMoney[i].transform.localEulerAngles = Vector3.zero;
        }
    }

    IEnumerator CollectMoney()
    {
        for (int i = 0; i < 32; i++)
        {
            var temp = allMoney[i];
            temp.transform.parent = null;
            temp.transform.localScale = Vector3.zero;
            temp.transform.parent = PlayerController.instance.stackPoint;
            temp.transform.localEulerAngles = Vector3.zero;
            temp.transform.DOScale(new Vector3(2f, 2.5f, 2f), 0.8f).SetEase(Ease.OutBack);
            /*yield return new WaitForSeconds(0.025f);
            temp.transform.localEulerAngles = Vector3.zero;
            temp.transform.DOScale(new Vector3(1.5f, 2, 1.5f), 0.025f); */
            yield return new WaitForSeconds(0.015f);
        }

        if (index >= 1)
        {
            unlockCollider1.enabled = true;
            unlockCollider2.enabled = true;
        }
        if (index == 2)
        {
            GameController.instance.upgardeManager.moneyDropPoint.gameObject.SetActive(true);
        }
        if (index == 3)
        {
            unlockCollider3.enabled = true;
        }
        index++;
    }

    public void AddMoneyToList(GameObject money)
    {
        money.transform.DOScale(Vector3.one * 2, 0.5f);
        money.transform.DOJump(transform.position, 2, 1, 1f).OnComplete(() =>
        {
            money.transform.parent = transform;
            money.transform.eulerAngles = Vector3.zero;
            allMoney.Add(money);
        });

    }
}
