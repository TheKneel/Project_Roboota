using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : MonoBehaviour
{
    [SerializeField] GameObject PauseGamePanel;
    [SerializeField] GameObject GameOverPanel;

    private void Start()
    {
        Time.timeScale = 1.0f;
        PauseGamePanel.SetActive(false);
        GameOverPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("GameStartScene");
    }
}
