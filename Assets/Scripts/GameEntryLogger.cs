using System;
using Npgsql;
using UnityEngine;

public class GameEntryLogger : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Имя пользователя, для которого записываем время входа
    private string username;

    void Start()
    {
        // Проверяем, было ли время входа уже записано в этой сессии
        username = CurrentUser.Instance.Username;
        if (!PlayerPrefs.HasKey("EntryLogged"))
        {
            LogEntryTime(username);
            // Устанавливаем флаг, чтобы запись больше не срабатывала
            PlayerPrefs.SetInt("EntryLogged", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Время входа уже записано. Повторной записи не требуется.");
        }
    }

    public void LogEntryTime(string username)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для вставки новой записи
                string query = "INSERT INTO entry_time (username, time) VALUES (@username, @time)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@time", DateTime.UtcNow);

                    // Выполняем запрос
                    command.ExecuteNonQuery();
                    Debug.Log($"Новая запись времени входа для пользователя '{username}' успешно создана.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при записи времени входа: " + ex.Message);
        }
    }
}
