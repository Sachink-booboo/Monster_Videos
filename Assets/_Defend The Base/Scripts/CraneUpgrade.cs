using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CraneUpgrade : MonoBehaviour
{
    public bool isTriggered;
    public MoneyTrigger moneyTrigger;
   
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
        for (int i = 32 - 1; i >= 0; i--)
        {
            var temp = moneyTrigger.allMoney[i];
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 1, 1, 0.05f);
            yield return new WaitForSeconds(0.03f);
        }
        GameController.instance.RestartGame();
       
    }
}
