using UnityEngine;
using UnityEngine.SceneManagement;

public class BattlePause : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenu;
    public string sceneName;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (GamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        GetComponent<AudioSource>().Pause();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        GamePaused = false;
        GetComponent<AudioSource>().Play();
    }
    public void BackToMain()
    {
        SceneManager.LoadScene(sceneName);
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        GamePaused = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
