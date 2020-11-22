using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainFlow : MonoBehaviour
{
    #region SINGLETON
    private static MainFlow instance = null;
    public static MainFlow Instance
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

    public float worldHeight;
    public float worldWidth;
    public float restartAfter;

    public GameObject gameWorld;
    public GameObject gameBackgroundParticles;
    public GameObject menuBackgroundParticles;

    SpawnManager spawnManager;
    UIManager uiManager;
    public SoundManager soundManager;
    
    public Ball playerBall { get; private set; }

    public static bool isDead;
    public static bool isGameStarted;

    GameObject explosionPrefab;
    
    private void Start()
    {
        LoadResources();
    }

    public void InitializeReferences()
    {
        SetGameWorld(true);
        EnableOrDisableParticles(true, false);

        uiManager.UIVisibility(false);
        SetCursorMode(CursorLockMode.None, false);

        spawnManager.SpawnEnemiesOnStart();
        playerBall = GameObject.FindObjectOfType<Ball>();
        playerBall.Initialize();

        isDead = false;
        isGameStarted = true;
    }

    public void LoadResources()
    {
        explosionPrefab = Resources.Load<GameObject>("Prefabs/ExplosionEffect");

        spawnManager = GetComponent<SpawnManager>();
        spawnManager.LoadEnemies();
        CameraShaker.Instance.Initialize();
        soundManager.Initialize();
        uiManager = GetComponent<UIManager>();
        uiManager.Initialize();

        EnableOrDisableParticles(false, true);
    }

    void EnableOrDisableParticles(bool gameParticles, bool menuParticles)
    {
        SetGameBackgroundParticles(gameParticles);
        SetMenuBackgroundParticles(menuParticles);
    }

    void SetGameWorld(bool toShow)
    {
        gameWorld.SetActive(toShow);
    }

    void Update()
    {
        uiManager.Refresh();
        CameraShaker.Instance.Refresh();

        if (!isGameStarted) return;

        playerBall.Refresh();
        spawnManager.RefreshAll();
    }

    public void SlowMotion(bool activate)
    {
        if (isDead) return;

        if(activate)
        {
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    public void PlayerDied()
    {
        isDead = true;
        isGameStarted = false;
        soundManager.PlayExplosionSound();
        SetTimeScale(1);
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(restartAfter);
        SceneManager.LoadScene(0);
    }

    public void SetTimeScale(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
    }

    void SetCursorMode(CursorLockMode lockMode, bool isvisible)
    {
        Cursor.lockState = lockMode;
        Cursor.visible = isvisible;
    }

    public void PlayExplosionAt(Vector2 pos)
    {
        GameObject effect = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(effect, 2f);
    }

    public void SetGameBackgroundParticles(bool toShow)
    {
        gameBackgroundParticles.SetActive(toShow);
    }

    public void SetMenuBackgroundParticles(bool toShow)
    {
        menuBackgroundParticles.SetActive(toShow);
    }
}
