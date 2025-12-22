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
    public Collider unlockCollider1, unlockCollider2;
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
        GetComponent<Collider>().enabled = true;
        StartCoroutine(StartDropMoney());
    }
    IEnumerator StartDropMoney()
    {
        for (int i = 0; i < 32; i++)
        {
            yield return new WaitForSeconds(0.03f);
            allMoney[i].transform.parent = transform;
            allMoney[i].transform.DOScale(new Vector3(1.5f, 2, 1.5f), 0.06f);
            allMoney[i].transform.localEulerAngles = Vector3.zero;
        }
    }

    IEnumerator CollectMoney()
    {
        for (int i = 0; i < 32; i++)
        {
            var temp = allMoney[i];
            temp.transform.parent = null;

            temp.transform.DOScale(Vector3.zero, 0.025f);
            yield return new WaitForSeconds(0.025f);
            temp.transform.parent = PlayerController.instance.stackPoint;
            temp.transform.localEulerAngles = Vector3.zero;
            temp.transform.DOScale(new Vector3(1.5f, 2, 1.5f), 0.025f);
        }

        if (index >= 1)
        {
            unlockCollider1.enabled = true;
            unlockCollider2.enabled = true;
        }
        index++;
    }
}
