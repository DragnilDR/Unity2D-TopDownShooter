using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public bool pauseGame = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject screenLoading;
    [SerializeField] private Slider barLoad;
    [SerializeField] private Text percentLoad;
    [SerializeField] private GameObject deathScreen;

    [Header("Keybind")]
    public KeyCode pauseKey = KeyCode.Escape;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            //DontDestroyOnLoad(this); // название говорит само за себя
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (pauseGame == false)
            {
                Pause();
            }
            else Resume();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        pauseGame = false;
    }

    public void PlayGame()
    {
        screenLoading.SetActive(true);

        StartCoroutine(LoadAsync());
    }

    public void ReGame()
    {
        screenLoading.SetActive(true);

        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loc");

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            barLoad.value = asyncLoad.progress;
            percentLoad.text = barLoad.value * 100 + "%";

            if (asyncLoad.progress >= .9f && !asyncLoad.allowSceneActivation)
            {
                percentLoad.text = "Press Any Key";
                if (Input.anyKeyDown)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Pause()
    {
        if (deathScreen.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            pauseGame = true;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}