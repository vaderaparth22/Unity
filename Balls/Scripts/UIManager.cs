using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Animation imageFadeAnimation;

    GameObject mainMenuObject;

    public void Initialize()
    {
        mainMenuObject = GameObject.Find("MainMenu");
    }

    public void UIVisibility(bool toShow)
    {
        mainMenuObject.SetActive(toShow);
    }

    public void PlayFadeAnimation(bool toFade)
    {
        imageFadeAnimation.Play(toFade ? "fadeIn" : "");
    }

    void StopFadeAnimation()
    {
        imageFadeAnimation.Stop("fadeIn");
    }

    IEnumerator CheckForAnimationFinishStart()
    {
        PlayFadeAnimation(true);

        yield return new WaitUntil(() => !imageFadeAnimation.isPlaying);

        StopFadeAnimation();
        MainFlow.Instance.InitializeReferences();
    }

    public void Refresh()
    {
        CheckInputForStart();
    }

    void CheckInputForStart()
    {
        if (MainFlow.isGameStarted) return;

        if (Input.GetKeyDown(KeyCode.Space) && !MainFlow.isGameStarted)
            StartCoroutine(CheckForAnimationFinishStart());
    }
}
