using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string sceneName2;


    public void StartGame()
    {
        StartCoroutine(DelaySceneLoad());
    }

    public void SettingsMenu()
    {
        SceneManager.LoadScene(sceneName2);
    }

    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
