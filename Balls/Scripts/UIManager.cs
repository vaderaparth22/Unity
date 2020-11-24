using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region SINGLETON
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public Animation imageFadeAnimation;
    public Text timeSurvivedText;
    public Text totalObjectHits;

    GameObject mainMenuObject;
    GameObject scoreTexts;
    GameObject quitPanel;

    public static int numOfHits;
    float startTime;

    public void Initialize()
    {
        mainMenuObject = GameObject.Find("MainMenu");
        scoreTexts = GameObject.Find("ScoreTexts");
        quitPanel = GameObject.Find("QuitPanel");

        ScoreVisibility(false);
        QuitPanelVisibility(false);

        ResetNumberOfHits();
    }

    public void MenuVisibility(bool toShow)
    {
        mainMenuObject.SetActive(toShow);
    }

    public void ScoreVisibility(bool toShow)
    {
        scoreTexts.SetActive(toShow);
    }

    public void QuitPanelVisibility(bool toShow)
    {
        quitPanel.SetActive(toShow);
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

        startTime = Time.time;
        ScoreVisibility(true);
        StopFadeAnimation();
        MainFlow.Instance.InitializeReferences();
    }

    public void Refresh()
    {
        CheckForQuitInput();
        CheckInputForStart();
        UpdateTimeSurvived();
    }

    void CheckInputForStart()
    {
        if (MainFlow.isGameStarted) return;

        if (Input.GetKeyDown(KeyCode.Space) && !MainFlow.isGameStarted)
            StartCoroutine(CheckForAnimationFinishStart());
    }

    void UpdateTimeSurvived()
    {
        if (!MainFlow.isGameStarted) return;

        timeSurvivedText.text = GetTimeSurvived();
    }

    string GetTimeSurvived()
    {
        float timePassed = Time.time - startTime;
        float minutes = timePassed / 60;
        float seconds = timePassed % 60;

        return Mathf.FloorToInt(minutes) + ":" + Mathf.FloorToInt(seconds);
    }

    public void UpdateTotalHits()
    {
        numOfHits++;
        totalObjectHits.text = GetTotalHitsString();
    }

    string GetTotalHitsString()
    {
        return "TOTAL OBJECT HIT : " + numOfHits.ToString();
    }

    void ResetNumberOfHits()
    {
        numOfHits = 0;
    }

    void CheckForQuitInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !MainFlow.isGameStarted)
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && MainFlow.isGameStarted)
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        MainFlow.Instance.SetTimeScale(0);
        MainFlow.Instance.SetCursorModeAndVisibility(CursorLockMode.None, true);
        QuitPanelVisibility(true);
    }

    void UnPauseGame()
    {
        MainFlow.Instance.SetTimeScale(1);
        MainFlow.Instance.SetCursorModeAndVisibility(CursorLockMode.None, false);
        QuitPanelVisibility(false);
    }

    public void QuitYes()
    {
        MainFlow.Instance.RestartFromUI();
    }

    public void QuitNo()
    {
        UnPauseGame();
    }
}
