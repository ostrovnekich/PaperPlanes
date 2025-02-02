using System;
using System.Threading.Tasks;
using Npgsql;
using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Текстовое поле Unity UI для отображения статистики
    public TMP_Text statsText;

    void Start()
    {
        // Загружаем статистику при запуске
    }

    public async void LoadPlayerStatsAsync()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Асинхронное открытие соединения
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения статистики игрока
                string query = @"
                    SELECT 
                        COALESCE((SELECT play_time FROM playing_time WHERE username = @username), 0) AS total_play_time,
                        COALESCE((SELECT count FROM games_count WHERE username = @username), 0) AS games_played
                ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", CurrentUser.Instance.Username);

                    using (var reader = await command.ExecuteReaderAsync()) // Асинхронное выполнение команды
                    {
                        if (await reader.ReadAsync()) // Асинхронное чтение результатов
                        {
                            // Получаем данные из базы
                            int totalPlayTime = reader.GetInt32(0); // Общее время игры (в минутах)
                            int gamesPlayed = reader.GetInt32(1);  // Количество игр

                            // Формируем текст для отображения
                            statsText.text = $"{CurrentUser.Instance.Username}\n" +
                                             $"Game time: {totalPlayTime} minutes\n" +
                                             $"Games count: {gamesPlayed}";
                        }
                        else
                        {
                            statsText.text = $"Статистика для пользователя '{CurrentUser.Instance.Username}' не найдена.";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при загрузке статистики: " + ex.Message);
            statsText.text = "Ошибка при загрузке статистики.";
        }
    }
}
