using System;
using Npgsql;
using UnityEngine;

public class PlayingTimeTracker : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Имя пользователя
    private string username;
    private static float sessionStartTime; // Статическая переменная для отслеживания времени
    private static float totalPlayTime; // Статическая переменная для хранения общего времени игры

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        username = CurrentUser.Instance.Username; // Предполагается, что имя пользователя уже определено

        // Если это первая сцена или первый запуск, инициализируем время
        if (sessionStartTime == 0)
        {
            sessionStartTime = Time.time;
        }

        // Загрузка общего времени игры, если оно есть
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
    }

    void OnApplicationQuit()
    {
        // Рассчитываем время игры в текущей сессии в минутах
        int playTimeInMinutes = Mathf.RoundToInt((Time.time - sessionStartTime) / 60f);
        totalPlayTime += playTimeInMinutes; // Добавляем к общему времени

        // Сохраняем общее время в PlayerPrefs
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.Save();

        // Записываем время в базу данных
        UpdatePlayingTime(username, playTimeInMinutes);
    }

    public void UpdatePlayingTime(string username, int playTime)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // Проверяем, существует ли запись для пользователя
                string checkQuery = "SELECT play_time FROM playing_time WHERE username = @username";
                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    var result = checkCommand.ExecuteScalar();

                    if (result != null)
                    {
                        // Если запись существует, обновляем play_time
                        int currentPlayTime = Convert.ToInt32(result);
                        string updateQuery = "UPDATE playing_time SET play_time = @newPlayTime WHERE username = @username";
                        using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@newPlayTime", currentPlayTime + playTime);
                            updateCommand.Parameters.AddWithValue("@username", username);

                            updateCommand.ExecuteNonQuery();
                            Debug.Log($"Время игры для пользователя '{username}' обновлено. Общее время: {currentPlayTime + playTime} минут.");
                        }
                    }
                    else
                    {
                        // Если записи нет, создаём её
                        string insertQuery = "INSERT INTO playing_time (username, play_time) VALUES (@username, @playTime)";
                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@username", username);
                            insertCommand.Parameters.AddWithValue("@playTime", playTime);

                            insertCommand.ExecuteNonQuery();
                            Debug.Log($"Запись времени игры для пользователя '{username}' создана. Общее время: {playTime} минут.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при обновлении времени игры: " + ex.Message);
        }
    }
}
