using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraneUpgrade : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public GameObject level1, level2;
    public bool isTriggered;
    public MoneyTrigger moneyTrigger;
    public Image fillImage;
    public TextMeshProUGUI fillText;

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
        UpdateUI();
        for (int i = 32 - 1; i >= 0; i--)
        {
            var temp = moneyTrigger.allMoney[i];
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 2, 1, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        GameController.instance.allCameras[3].SetActive(true);
        level2.SetActive(true);
        level1.SetActive(false);
        particleSystem.Play();
        GameController.instance.craneAnimator.SetTrigger("Upgrade");
        yield return new WaitForSeconds(1.5f);
        GameController.instance.RestartGame();
        fillImage.transform.parent.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        // Reset initial values
        fillImage.fillAmount = 0f;
        fillText.text = "30";

        float duration = 2f; // adjust if needed

        // Fill image animation (0 -> 1)
        DOTween.To(
            () => fillImage.fillAmount,
            x => fillImage.fillAmount = x,
            1f,
            duration
        ).SetEase(Ease.Linear);

        // Text animation (30 -> 0)
        DOTween.To(
            () => 30,
            x => fillText.text = x.ToString(),
            0,
            duration
        ).SetEase(Ease.Linear);
    }
}
