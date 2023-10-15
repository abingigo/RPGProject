using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        Coroutine currentFader;

        public IEnumerator Fadeout(float time)
        {
            if (currentFader != null)
                StopCoroutine(currentFader);
            currentFader = StartCoroutine(FadeoutRoutine(time));
            yield return currentFader;
        }

        private IEnumerator FadeoutRoutine(float time)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator Fadein(float time)
        {
            if (currentFader != null)
                StopCoroutine(currentFader);
            currentFader = StartCoroutine(FadeinRoutine(time));
            yield return currentFader;
        }

        private IEnumerator FadeinRoutine(float time)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
