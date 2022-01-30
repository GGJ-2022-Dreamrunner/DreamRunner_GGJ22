using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class DreamManager : MonoBehaviour
{
    public bool dreamLeavable = false, dreamComplete = false;

    public Dream dr;

    public Image fadeCanvas;

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

    public void ExitDream()
    {
        StartCoroutine(FadeAndTransition());

    }

    IEnumerator FadeAndTransition()
    {
        yield return new WaitForSeconds(3);
        while (fadeCanvas.color.a <= 1f)
        {
            fadeCanvas.color = new Color(fadeCanvas.color.r, fadeCanvas.color.g, fadeCanvas.color.b, fadeCanvas.color.a + .025f);
            yield return new WaitForEndOfFrame();
            print("in loop");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
