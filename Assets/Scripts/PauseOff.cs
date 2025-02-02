using UnityEngine;

public class PauseOff : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Debug.Log("Игра продолжена");
    }
}
