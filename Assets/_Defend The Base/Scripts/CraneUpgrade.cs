using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraneUpgrade : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public GameObject level11, level12, level21, level22;
    public bool isTriggered;
    public MoneyTrigger moneyTrigger;
    public Image fillImage;
    public TextMeshProUGUI fillText;
    public GameObject crack1, crack2, crack3, crack4;
    public ParticleSystem effect1, effect2, effect3, effect4;
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
        GameController.instance.allCameras[6].SetActive(true);
        for (int i = 32 - 1; i >= 0; i--)
        {
            var temp = moneyTrigger.allMoney[i];
            moneyTrigger.allMoney.Remove(temp);
            temp.transform.parent = null;
            temp.transform.DOJump(transform.position, 3, 1, 0.2f);
            yield return new WaitForSeconds(0.05f);
        }
        // yield return new WaitForSeconds(1.5f);
        level21.SetActive(true);
        level22.SetActive(true);
        level11.SetActive(false);
        level12.SetActive(false);
        particleSystem.Play();
        GameController.instance.craneAnimator.SetTrigger("Upgrade");
        StartCoroutine(StartUpgrade());
        yield return new WaitForSeconds(2.4f);
        GameController.instance.RestartGame();
        fillImage.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator StartUpgrade()
    {
        yield return new WaitForSeconds(1.4f);
        effect1.Play();
        yield return new WaitForSeconds(0.2f);
        crack1.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        effect2.Play();
        yield return new WaitForSeconds(0.2f);
        crack2.SetActive(true);
        effect3.Play();
        yield return new WaitForSeconds(0.1f);
        crack3.SetActive(true);
        effect4.Play();
        yield return new WaitForSeconds(0.1f);
        crack4.SetActive(true);
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
