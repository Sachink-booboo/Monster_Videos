using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public List<GameObject> allBullets;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            CollectBullet(player);
        }
    }

    public void CollectBullet(PlayerController player)
    {
        for (int i = 0; i < 20; i++)
        {
            var temp = allBullets[0];
            allBullets.Remove(temp);
            temp.GetComponent<Rigidbody>().isKinematic = true;
            player.AddToStack(temp);
        }
        GetComponent<Collider>().enabled = false;
    }
}
