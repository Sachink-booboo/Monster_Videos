using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class UpgardeManager : MonoBehaviour
{
    public ParticleSystem effect;
    public Animator animator1, animator2;
    public Transform point1, point2, point3, point4;
    public bool isTriggered;
    public MoneyTrigger moneyTrigger;
    public Archery tower1, tower2;
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
        transform.DOScale(Vector3.one * 0.57f, 0.1f).OnComplete(() => transform.DOScale(Vector3.one * 0.52f, 0.1f));
        // GameController.instance.spawnManager.isStop = true;
        effect.Play();
        MoveToTower1();
        MoveToTower2();
        GameController.instance.spawnManager.SpawnMoreEnemy();
        moneyTrigger.DropMoney();
        moneyTrigger.isTriggered = false;
    }

    public void MoveToTower1()
    {
        animator1.gameObject.SetActive(true);
        animator1.transform.DOLookAt(point1.position, 0.1f, AxisConstraint.Y);
        animator1.Play("Running");
        animator1.transform.DOMove(point1.position, 3).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            animator1.Play("Idle");
            animator1.transform.DOJump(tower1.banditCharacter.transform.position, 2, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                animator1.gameObject.SetActive(false);
                tower1.ChangeGun();
            });
            tower1.effect.Play();
        });
    }

    public void MoveToTower2()
    {
        animator2.gameObject.SetActive(true);
        animator2.transform.DOLookAt(point2.position, 0.1f, AxisConstraint.Y);
        animator2.Play("Running");
        animator2.transform.DOMove(point2.position, 4).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            animator2.Play("Idle");
            animator2.transform.DOJump(point3.position, 1, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                animator2.Play("Running");
                animator2.transform.DOLookAt(point4.position, 0.1f, AxisConstraint.Y);
                animator2.transform.DOMove(point4.position, 4).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
                {
                    animator2.Play("Idle");
                    animator2.transform.DOJump(tower2.banditCharacter.transform.position, 2, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        animator2.gameObject.SetActive(false);
                        tower2.ChangeGun();
                    });
                    tower2.effect.Play();
                });
            });

        });
    }
}
