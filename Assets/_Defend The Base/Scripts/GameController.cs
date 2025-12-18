using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    public Animator craneAnimator;
    public List<GameObject> allCameras;
    public GameObject spawnManager, craneObject;
    public BaseController baseController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
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
        baseController.Init();
        allCameras[2].SetActive(true);
        PlayerController.instance.enabled = true;
        spawnManager.SetActive(true);
        /*  craneObject.transform.DORotate(new Vector3(0, -125, 0), 40, RotateMode.FastBeyond360).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
         {
             baseController.Init();
             allCameras[2].SetActive(true);
             PlayerController.instance.enabled = true;
             spawnManager.SetActive(true);
         }); */
    }
}
