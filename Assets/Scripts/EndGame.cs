using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{

    public Image fadeCanvas;
    public void onEndGame()
    {
        StartCoroutine(FadeAndTransition());
    }

    IEnumerator FadeAndTransition()
    {
        while (fadeCanvas.color.a <= 1f)
        {
            fadeCanvas.color = new Color(fadeCanvas.color.r, fadeCanvas.color.g, fadeCanvas.color.b, fadeCanvas.color.a + Time.deltaTime);
            yield return new WaitForEndOfFrame();
            print("in loop");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
