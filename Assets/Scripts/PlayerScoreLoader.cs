using System;
using UnityEngine;
using TMPro;
using Npgsql;

public class PlayerScoreLoader : MonoBehaviour
{
    private const string CONNECTION_STRING = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";
    public TMP_Text playerScoreText; // Текстовый объект для отображения счета игрока

    private int targetScore = 0; // Итоговый счет, полученный из базы данных
    private float displayedScore = 0; // Текущий отображаемый счет
    public float lerpSpeed = 5f; // Скорость плавного изменения счета

    private void Start()
    {
        if (string.IsNullOrEmpty(CurrentUser.Instance.Username))
        {
            Debug.LogError("Имя пользователя не задано!");
            playerScoreText.text = "Ошибка: имя пользователя не задано.";
            return;
        }

        // Загружаем счет игрока из базы данных
        LoadPlayerScoreFromDatabase();
    }

    private void Update()
    {
        // Плавное увеличение отображаемого счета
        if (Mathf.RoundToInt(displayedScore) < targetScore)
        {
            displayedScore = Mathf.Lerp(displayedScore, targetScore, Time.deltaTime * lerpSpeed);
            UpdateScoreText();
        }
    }

    private void LoadPlayerScoreFromDatabase()
    {
        using (var connection = new NpgsqlConnection(CONNECTION_STRING))
        {
            try
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                string query = "SELECT higtscore FROM hightscores WHERE username = @username";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", CurrentUser.Instance.Username);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        targetScore = Convert.ToInt32(result);
                        Debug.Log($"Счёт игрока {CurrentUser.Instance.Username}: {targetScore}");
                    }
                    else
                    {
                        Debug.LogWarning("Игрок не найден в таблице hightscores.");
                        targetScore = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке счета: {ex.Message}");
                playerScoreText.text = $"Score loading error";
                targetScore = 0;
            }
        }
    }

    private void UpdateScoreText()
    {
        // Обновление текста с отображаемым счетом
        playerScoreText.text = $"Your score: {Mathf.RoundToInt(displayedScore)}";
    }
}
