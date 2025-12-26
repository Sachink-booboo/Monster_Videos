using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public List<GameObject> allBullets;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            StartCoroutine(CollectBullet(player));
        }
    }

    IEnumerator CollectBullet(PlayerController player)
    {
        for (int i = 0; i < 20; i++)
        {
            var temp = allBullets[0];
            allBullets.Remove(temp);
            temp.GetComponent<Rigidbody>().isKinematic = true;

            // temp.transform.localEulerAngles = Vector3.zero;
            temp.transform.DOJump(PlayerController.instance.stackPoint.position, 3, 1, 0.5f).OnComplete(() =>
            {
                player.AddToStack(temp);
            });
        }
        yield return new WaitForSeconds(0.02f);
        GetComponent<Collider>().enabled = false;
    }
}
