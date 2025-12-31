using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockObject : MonoBehaviour
{
    public int moneyIndex;
    public GameObject unlockObj;
    public MoneyTrigger moneyTrigger;
    public Storage storage;
    public Transform point1, point2, point3, moneyDropPoint;
    public bool isTriggered;
    public Image fillImage;
    public TextMeshProUGUI fillText;
    public bool isLast;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            if (isTriggered) return;
            isTriggered = true;

            if (isLast)
            {
                StartCoroutine(DropAllMoney());
            }
            else
            {
                StartCoroutine(DropMoney());
            }
        }
    }

    IEnumerator DropAllMoney()
    {
        DOTween.To(
            () => fillImage.fillAmount,
            x => fillImage.fillAmount = x,
            1f,
            2
        ).SetEase(Ease.Linear);

        for (int i = 0; i < 32; i++)
        {
            var temp = moneyTrigger.allMoney[i];
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 2, 1, 0.1f).OnComplete(() =>
            {
                temp.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.15f);
        moneyDropPoint.gameObject.SetActive(false);
        GameController.instance.fenceLevel1.SetActive(false);
        GameController.instance.fenceLevel2.transform.localScale = Vector3.zero;
        GameController.instance.fenceLevel2.SetActive(true);
        GameController.instance.fenceLevel2.transform.DOScale(Vector3.one, 0.01f);
        GameController.instance.allCameras[5].SetActive(true);
    }

    IEnumerator DropMoney()
    {
        UpdateUI();
        for (int i = 15; i >= 0; i--)
        {
            var temp = moneyTrigger.allMoney[i];
            moneyTrigger.allMoney.Remove(temp);
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 3, 1, 0.2f);
            yield return new WaitForSeconds(0.05f);
        }
        unlockObj.transform.localScale = Vector3.zero;
        unlockObj.SetActive(true);
        // unlockObj.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        unlockObj.transform.DOScale(Vector3.one * 1.5f, 0.1f).OnComplete(() => unlockObj.transform.DOScale(Vector3.one, 0.1f));

        StartCoroutine(StartMovingBullet());
        fillImage.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        if (moneyIndex == 16)
        {
            moneyTrigger.DropMoney();
            moneyTrigger.isTriggered = false;
            GameController.instance.upgardeManager.tower1.enabled = true;
            GameController.instance.upgardeManager.tower1.animator.enabled = true;
            GameController.instance.upgardeManager.tower1.bulletIcon.SetActive(false);
        }
        else
        {
            GameController.instance.upgardeManager.tower2.enabled = true;
            GameController.instance.upgardeManager.tower2.animator.enabled = true;
            GameController.instance.upgardeManager.tower2.bulletIcon.SetActive(false);
        }
    }

    IEnumerator StartMovingBullet()
    {
        while (storage.allBullets.Count > 0)
        {
            var temp = storage.allBullets[0];
            storage.allBullets.Remove(temp);
            MoveBullet(temp);
            yield return new WaitForSeconds(0.16f);
        }
    }

    public void MoveBullet(GameObject bullet)
    {
        bullet.GetComponent<Rigidbody>().isKinematic = true;
        bullet.transform.position = point1.position;
        bullet.transform.rotation = point1.rotation;
        bullet.transform.DOMove(point2.position, 4).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
        {
            bullet.transform.rotation = point2.rotation;
            bullet.transform.DOMove(point3.position, 4).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
                   {
                       Destroy(bullet.gameObject);
                   });
        });
    }

    public void UpdateUI()
    {
        // Reset initial values
        fillImage.fillAmount = 0f;
        fillText.text = "15";

        float duration = 0.8f; // adjust if needed

        // Fill image animation (0 -> 1)
        DOTween.To(
            () => fillImage.fillAmount,
            x => fillImage.fillAmount = x,
            1f,
            duration
        ).SetEase(Ease.Linear);

        // Text animation (30 -> 0)
        DOTween.To(
            () => 15,
            x => fillText.text = x.ToString(),
            0,
            duration
        ).SetEase(Ease.Linear);
    }
}
