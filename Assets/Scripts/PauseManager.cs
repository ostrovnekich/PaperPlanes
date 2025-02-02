using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // Ссылка на панель, которая отображается при паузе
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    // Метод для переключения паузы
    public void TogglePause()
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

    // Метод для установки паузы
    public void PauseGame()
    {
        Time.timeScale = 0; // Останавливаем время
        isPaused = true;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // Показываем панель
        }
        Debug.Log("Игра на паузе");
    }

    // Метод для снятия паузы
    public void ResumeGame()
    {
        Time.timeScale = 1; // Возвращаем время к нормальному ходу
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Скрываем панель
        }
        Debug.Log("Игра продолжена");
    }

    void Update()
    {
        // Нажатие клавиши Escape переключает паузу
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}
