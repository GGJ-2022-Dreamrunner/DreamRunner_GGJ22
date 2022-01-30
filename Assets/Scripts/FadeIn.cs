using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup group;


    IEnumerator Fade()
    {
        yield return new WaitForSeconds(3f);
        while (group.alpha != 0)
        {
            group.alpha -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(group.gameObject);
        yield return null;
    }
    
    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(Fade());
    }

}
