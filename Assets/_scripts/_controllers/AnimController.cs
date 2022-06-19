using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class AnimController : MonoBehaviour
{
    public float startPanelFadeIn = 1f;
    public float startPanelFadeOut = 2f;

    public float restartPanelFadeIn = 1.5f;

    public float goHomeCameraFade = 1.5f;

    public delegate void OnAnimEnd(); // declare delegate type
    protected OnAnimEnd onAnimEnd;

    //Анимации при показе и исчезновении главного меню
    public void animateStartScreenIn(GameObject startPanel)
    {
        GameObject startButton = startPanel.transform.Find("StartButton").gameObject;
        GameObject leaderboardButton = TransformUtils.FindDeepChild(startPanel.transform, "LeaderboardButton").gameObject;
        GameObject settingsButton = TransformUtils.FindDeepChild(startPanel.transform, "SettingsButton").gameObject;

        StartCoroutine(LerpImageFillAmount01(startButton.GetComponent<Image>(), startPanelFadeIn, startPanelFadeIn));
        StartCoroutine(FadeImageIn(leaderboardButton.GetComponent<Image>(), startPanelFadeIn));
        StartCoroutine(FadeImageIn(settingsButton.GetComponent<Image>(), startPanelFadeIn));
    }

    //refactor rename startPanelFadeOut
    //после выбора класса
    public void animateOnGameStarting(GameObject selecClassPanel, OnAnimEnd callback) //Вызывается, когда пользователь начинает игру после выбора класса
    {
        //scaling panel
        iTween.ScaleTo(selecClassPanel, iTween.Hash("x", 1.2f, "y", 1.2f, "time", startPanelFadeOut - 0.1f));

        //fading class buttons
        GameObject content = selecClassPanel.transform.Find("Content").gameObject;
        foreach (var btnImg in content.GetComponentsInChildren<Image>())
        {
            StartCoroutine(FadeImageOut(btnImg, startPanelFadeOut));
        }

        //fading header
        GameObject header = selecClassPanel.transform.Find("Header").gameObject;
        StartCoroutine(FadeTextOut(header.GetComponent<Text>(), startPanelFadeOut));

        //changing background
        SpriteRenderer backgroundSpriteRend = TransformUtils.FindDeepChild(Camera.main.transform, "MenuBackground").
            GetComponent<SpriteRenderer>();
        StartCoroutine(FadeSpriteOut(backgroundSpriteRend, startPanelFadeOut));

        //assign callback
        onAnimEnd = callback;
        StartCoroutine(callbackCoroutine(startPanelFadeOut));
    }
    public void animateOnGameStartingUpdated(GameObject startPanel, OnAnimEnd callback) //Вызывается, когда пользователь начинает игру после выбора класса
    {
        //scaling panel
        iTween.ScaleTo(startPanel, iTween.Hash("x", 1.2f, "y", 1.2f, "time", startPanelFadeOut - 0.1f));

        //fading class buttons
        foreach (var btnImg in startPanel.GetComponentsInChildren<Image>())
        {
            StartCoroutine(FadeImageOut(btnImg, startPanelFadeOut));
        }

        //changing background
        SpriteRenderer backgroundSpriteRend = TransformUtils.FindDeepChild(Camera.main.transform, "MenuBackground").
            GetComponent<SpriteRenderer>();
        StartCoroutine(FadeSpriteOut(backgroundSpriteRend, startPanelFadeOut));

        //assign callback
        onAnimEnd = callback;
        StartCoroutine(callbackCoroutine(startPanelFadeOut));
    }
    //old
    //public void animateStartScreenOut(GameObject startPanel, OnAnimEnd callback) //Вызывается, когда пользователь начинает игру
    //{

    //    GameObject startButton = startPanel.transform.Find("StartButton").gameObject;
    //    GameObject leaderboardButton = TransformUtils.FindChildByRecursion(startPanel.transform, "LeaderboardButton").gameObject;
    //    GameObject settingsButton = TransformUtils.FindChildByRecursion(startPanel.transform, "SettingsButton").gameObject;

    //    //fading and scaling images
    //    iTween.ScaleTo(startPanel, iTween.Hash("x", 1.5f, "y", 1.5f, "time", startPanelFadeOut - 0.1f));
    //    StartCoroutine(FadeImageOut(startButton.GetComponent<Image>(), startPanelFadeOut));
    //    StartCoroutine(FadeImageOut(leaderboardButton.GetComponent<Image>(), startPanelFadeOut));
    //    StartCoroutine(FadeImageOut(settingsButton.GetComponent<Image>(), startPanelFadeOut));

    //    //changing background
    //    SpriteRenderer backgroundSpriteRend = TransformUtils.FindChildByRecursion(Camera.main.transform, "MenuBackground").
    //        GetComponent<SpriteRenderer>();
    //    StartCoroutine(FadeSpriteOut(backgroundSpriteRend, startPanelFadeOut));

    //    onAnimEnd = callback;
    //    StartCoroutine(callbackCoroutine(startPanelFadeOut));
    //}
    public void animateGoHome(OnAnimEnd callback)
    {
        FadeCameraToBlack(goHomeCameraFade);

        onAnimEnd = callback;
        StartCoroutine(callbackCoroutine(goHomeCameraFade));
    }
    public void animateOnHomeReturned()
    {
        FadeCameraFromBlack(goHomeCameraFade);
    }

    public void animateRestartScreen(GameObject restartPanel, OnAnimEnd callback)
    {
        GameObject scorePanel = TransformUtils.FindDeepChild(restartPanel.transform, "ScorePanel").gameObject;
        Text[] textComponents = scorePanel.GetComponentsInChildren<Text>();

        GameObject homeButton = TransformUtils.FindDeepChild(restartPanel.transform, "HomeButton").gameObject;
        GameObject restartButton = TransformUtils.FindDeepChild(restartPanel.transform, "RestartButton").gameObject;
        GameObject highscoreImg = TransformUtils.FindDeepChild(restartPanel.transform, "NewHighscoreImg").gameObject;


        StartCoroutine(FadeImageIn(scorePanel.GetComponent<Image>(), restartPanelFadeIn));
        StartCoroutine(FadeTextComponents(textComponents, restartPanelFadeIn));

        StartCoroutine(FadeImageIn(homeButton.GetComponent<Image>(), restartPanelFadeIn));
        StartCoroutine(FadeImageIn(restartButton.GetComponent<Image>(), restartPanelFadeIn));
        StartCoroutine(FadeImageIn(highscoreImg.GetComponent<Image>(), restartPanelFadeIn));

        onAnimEnd = callback;
        StartCoroutine(callbackCoroutine(restartPanelFadeIn));
    }
    public void animateTimePoints(GameObject restartPanel, GameObject gamePanel, float points, float playedTime)
    {
        Text scoreTxt = TransformUtils.FindDeepChild(restartPanel.transform, "ScoreValue").
            GetComponent<Text>();

        GameObject timerPanel = TransformUtils.FindDeepChild(restartPanel.transform, "TimerCopyPanel").gameObject;
        //Text timerTxt = TransformUtils.FindChildByRecursion(gamePanel.transform, "TimerValue").
        //    GetComponent<Text>();

        scoreTxt.text = (points + playedTime).ToString();
        timerPanel.SetActive(false);
        //edit докончить
    }

    private IEnumerator callbackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        onAnimEnd();
    }


    // ---------------- Custom animations --------------------//
    public IEnumerator FadeIn(Graphic graphic, float duration, Action onCompleted = null)
    {
        Color startColor = graphic.color;
        startColor.a = 0f;

        Color endColor = startColor;
        endColor.a = 1f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            graphic.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);

        onCompleted?.Invoke();
    }
    public IEnumerator FadeOut(Graphic graphic, float duration, Action onCompleted = null)
    {
        Color startColor = graphic.color;
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            graphic.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);

        onCompleted?.Invoke();
    }
    public IEnumerator PulsateAndHide(GameObject go, Action onCompleted = null)
    {
        if (go == null)
        {
            Debug.LogWarning("Pulsate object animation declined. Object is null");
            yield break;
        }

        int pulseCount = 2;
        int pulsed = 0;
        do
        {
            go.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            go.SetActive(false);
            yield return new WaitForSeconds(0.7f);

            pulsed++;

        } while (pulsed < pulseCount);

        go.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        go.SetActive(false);

        onCompleted?.Invoke();
    }
    public IEnumerator FadeImageIn(Image img, float duration)
    {
        Color startColor = img.color;
        startColor.a = 0f;

        Color endColor = startColor;
        endColor.a = 1f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            img.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }
    public IEnumerator FadeImageOut(Image img, float duration)
    {
        Color startColor = img.color;
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            img.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }

    // ---------------- Custom animations --------------------//
    public IEnumerator FadeTextOut(Text txt, float duration)
    {
        Color startColor = txt.color;
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            txt.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }

    public IEnumerator FadeSpriteOut(SpriteRenderer spriteRend, float duration)
    {
        Color startColor = spriteRend.color;
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            spriteRend.color = Color.Lerp(startColor, endColor, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }

    public IEnumerator FadeTextComponents(Text[] textArr, float duration)
    {
        Color startColor = textArr[0].color;
        startColor.a = 0f;

        Color endColor = startColor;
        endColor.a = 1f;

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
            foreach (var txt in textArr)
            {
                txt.color = Color.Lerp(startColor, endColor, normalisedTime);
            }
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }

    public IEnumerator LerpImageFillAmount01(Image img, float duration, float delay)
    {
        img.fillAmount = 0;
        if(delay > 0)
            yield return new WaitForSeconds(delay);

        // remember the start
        float start = Time.time;
        float elapsed = 0;
        do
        {  // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp01(elapsed / duration);
            img.fillAmount = Mathf.Lerp(0, 1, normalisedTime);
            // wait for the next frame
            yield return null;
        }
        while (elapsed < duration);
    }

    

    Image blackScreen;
    public void FadeCameraToBlack(float time)
    {
        if (blackScreen == null)
            blackScreen = TransformUtils.FindDeepChild(GameObject.Find("Canvas").transform, "BlackScreen")
                .GetComponent<Image>();

        blackScreen.color = Color.black;
        blackScreen.canvasRenderer.SetAlpha(0.0f);
        blackScreen.CrossFadeAlpha(1.0f, time, false);
    }

    public void FadeCameraFromBlack(float time)
    {
        if (blackScreen == null)
            blackScreen = TransformUtils.FindDeepChild(GameObject.Find("Canvas").transform, "BlackScreen")
                .GetComponent<Image>();

        blackScreen.color = Color.black;
        blackScreen.canvasRenderer.SetAlpha(1.0f);
        blackScreen.CrossFadeAlpha(0.0f, time, false);
    }

    public static float CAMERA_SHAKE_DURATION = 4f;
}
