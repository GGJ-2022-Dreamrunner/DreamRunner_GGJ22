using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    [SerializeField] private UnityEngine.UI.Image img;


    IEnumerator Fade()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        yield return new WaitForSeconds(1f);
        
        while (group.alpha != 0 || img.color.a != 0)
        {
            if (group != null)
                group.alpha -= Time.deltaTime;
            else if (img != null)
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    
    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(Fade());
    }

}
