using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;     // Panel chứa Play, Options, Credits, Exit
    public GameObject creditsPanel;      // Panel Credit

    public void PlayGame()
    {
        SceneManager.LoadScene("HeroScene");
    }

    public void OpenOptions()
    {
        Debug.Log("Open Options (chưa làm UI)");
    }

    public void OpenCredits()
    {
        // Tắt panel menu chính
        mainMenuPanel.SetActive(false);

        // Bật panel credits
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        // Tắt panel credit
        creditsPanel.SetActive(false);

        // Bật lại panel menu chính
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
