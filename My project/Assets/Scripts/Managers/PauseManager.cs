using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public Button logoutButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        logoutButton.onClick.AddListener(HandleLogout);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void ResumeGame() 
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f; 
        isPaused = false;
    }

    private void HandleLogout()
    {
        SceneManager.LoadScene("LoginScene");
    }
}