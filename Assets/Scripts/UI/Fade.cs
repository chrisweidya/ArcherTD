using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour {

    public delegate void FadeOut(float time, string nextScene);
    private static event FadeOut FadeOutEvent;
    public static void FadeAndChangeScene(float time, string nextScene) {
        if(FadeOutEvent != null) {
            FadeOutEvent(time, nextScene);
        }
    }

    enum FadeType { In, Out};

    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private float time = 1f;
    [SerializeField]
    private bool fadeIn = false;
    private bool _fading = false;

    private void OnEnable() {
        FadeOutEvent += StartFadeOut;
    }

    private void OnDisable() {
        FadeOutEvent -= StartFadeOut;
    }

    void Start () {
		if(fadeIn) {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
            StartCoroutine(StartFade(0, FadeType.In, time, null));
        }
	}

    public void StartFadeIn(float time) {
        StopAllCoroutines();
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        StartCoroutine(StartFade(0, FadeType.In, time, null));
    }

    public void StartFadeOut(float time, string nextScene) {
        StopAllCoroutines();
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        StartCoroutine(StartFade(1, FadeType.Out, time, nextScene));
    }
	
    private IEnumerator StartFade(float targetAlpha, FadeType type, float time, string nextScene) {
        float currAlpha = fadeImage.color.a;
        yield return new WaitForSeconds(1);

        if (type == FadeType.In) {
            for(float t = time; t >= 0; t-= Time.deltaTime) {
                currAlpha = Mathf.Clamp01(t / time);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currAlpha);
                yield return null;
            }
        }
        else if(type == FadeType.Out){
            for (float t = 0; t < time; t += Time.deltaTime) {
                currAlpha = Mathf.Clamp01(t / time);
                print(currAlpha);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currAlpha);
                yield return null;
            }
            if(nextScene != null)
                SceneManager.LoadScene(nextScene);
        }
        _fading = false;
        yield return null;
    }
}
