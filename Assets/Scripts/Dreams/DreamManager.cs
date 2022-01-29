using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DreamManager : MonoBehaviour
{
    public bool dreamLeavable = false, dreamComplete = false;

    public Dream dr;

    public void DeliverItem(item item)
    {
        foreach (item i in dr.items)
        {
            if (item == i)
            {
                i.delivered = true;
                dreamLeavable = true;
                return;
            }
        }
    }

    private void Update()
    {
        bool f = true;
        foreach (item d in dr.items)
        {
            f &= d.delivered;
        }

        f = dreamComplete;
    }

}
