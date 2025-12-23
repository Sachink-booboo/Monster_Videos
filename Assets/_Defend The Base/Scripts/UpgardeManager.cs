using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UpgardeManager : MonoBehaviour
{
    public ParticleSystem effect;
    public Animator animator1, animator2;
    public Transform point1, point2, point3, point4;
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
        for (int i = 0; i < 32; i++)
        {
            var temp = moneyTrigger.allMoney[i];
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 3, 1, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        // GameController.instance.spawnManager.isStop = true;
        effect.Play();
        MoveToTower1();
        MoveToTower2();
    }

    public void MoveToTower1()
    {
        animator1.gameObject.SetActive(true);
        animator1.transform.DOLookAt(point1.position, 0.1f, AxisConstraint.Y);
        animator1.transform.DOMove(point1.position, 2).SetSpeedBased();
    }

    public void MoveToTower2()
    {
        animator2.gameObject.SetActive(true);
        animator2.transform.DOLookAt(point2.position, 0.1f, AxisConstraint.Y);
        animator2.transform.DOMove(point2.position, 10).SetSpeedBased();
    }
}
