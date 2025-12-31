using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Animator craneAnimator;
    public List<GameObject> allCameras;
    public GameObject craneObject, money;
    public BaseController baseController;
    public MoneyTrigger moneyTrigger;
    public SpawnManager spawnManager;
    public GameObject train, fenceLevel1, fenceLevel2, furnace;
    public UpgardeManager upgardeManager;
    public Material material;
    public List<GameObject> allMoneyObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {
        animateMaterial();
        train.transform.DOMoveX(-4, 1);
        PlayerController.instance.enabled = false;
        for (int i = 0; i < allCameras.Count; i++)
        {
            allCameras[i].SetActive(false);
        }
        allCameras[0].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        craneAnimator.SetTrigger("Collect");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ReturnToBase());
        /* craneObject.transform.DORotate(new Vector3(0, 70, 0), 20, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
        {
            StartCoroutine(ReturnToBase());
        }); */
    }

    IEnumerator ReturnToBase()
    {
        yield return new WaitForSeconds(2);
        allCameras[1].SetActive(true);
        yield return new WaitForSeconds(2.5f);
        allCameras[2].SetActive(true);
        PlayerController.instance.enabled = true;
        spawnManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        furnace.transform.DOScale(Vector3.one * 0.011f, 0.1f).SetLoops(6, LoopType.Yoyo);
        baseController.Init();
        /*  craneObject.transform.DORotate(new Vector3(0, -125, 0), 40, RotateMode.FastBeyond360).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
         {
             baseController.Init();
             allCameras[2].SetActive(true);
             PlayerController.instance.enabled = true;
             spawnManager.SetActive(true);
         }); */
    }

    public void RestartGame()
    {
        StartCoroutine(StartRestartGame());
    }

    IEnumerator StartRestartGame()
    {
        upgardeManager.tower1.enabled = false;
        upgardeManager.tower2.enabled = false;
        upgardeManager.tower1.animator.enabled = false;
        upgardeManager.tower2.animator.enabled = false;
        upgardeManager.tower1.bulletIcon.SetActive(true);
        upgardeManager.tower2.bulletIcon.SetActive(true);

        PlayerController.instance.enabled = false;

        allCameras[6].SetActive(false);
        allCameras[3].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        craneAnimator.SetTrigger("Collect");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(2);
        allCameras[4].SetActive(true);
        yield return new WaitForSeconds(2.5f);
        allCameras[3].SetActive(false);
        allCameras[4].SetActive(false);
        PlayerController.instance.enabled = true;
        moneyTrigger.DropMoney();
        moneyTrigger.isTriggered = false;
        yield return new WaitForSeconds(1f);
        furnace.transform.DOScale(Vector3.one * 0.011f, 0.1f).SetLoops(6, LoopType.Yoyo);
        yield return new WaitForSeconds(0.5f);
        baseController.Init2();
    }

    public void animateMaterial()
    {
        // Reset offset
        material.SetTextureOffset("_BaseMap", Vector2.zero);

        material
               .DOOffset(new Vector2(0f, -1f), "_BaseMap", 2f) // move texture on X
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Incremental); // infinite loop
    }

    public void DropMoney()
    {
        StartCoroutine(StartDropMoney());
        // for (int i = 0; i < 32; i++)
        // {
        //     var temp = allMoneyObjects[0];
        //     moneyTrigger.AddMoneyToList(temp);
        // }
    }

    IEnumerator StartDropMoney()
    {
        for (int i = 0; i < 32; i++)
        {
            var temp = allMoneyObjects[0];
            allMoneyObjects.RemoveAt(0);
            moneyTrigger.AddMoneyToList(temp);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
