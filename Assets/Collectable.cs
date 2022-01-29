using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] CollectableManager manager;
    [SerializeField] ThirdPersonPlayer.NetscapeFPSController player;

    private void Awake()
    {
        player = FindObjectOfType<ThirdPersonPlayer.NetscapeFPSController>();
        manager = FindObjectOfType<CollectableManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        print(other);
        if (other.gameObject == player.gameObject)
        {
            manager.collect();
            Destroy(this.gameObject);
        } 
    }
}
