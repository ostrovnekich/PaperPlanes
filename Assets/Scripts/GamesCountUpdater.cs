using System;
using Npgsql;
using UnityEngine;

public class GamesCountUpdater : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Имя пользователя, для которого обновляем количество игр
    private string username;

    void Start()
    {
        username = CurrentUser.Instance.Username;
        // Увеличиваем значение count при старте
        IncrementGameCount(username);
    }

    public void IncrementGameCount(string username)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // Проверяем, существует ли запись для пользователя
                string checkQuery = "SELECT count FROM games_count WHERE username = @username";
                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);

                    var result = checkCommand.ExecuteScalar();
                    if (result != null)
                    {
                        // Если запись существует, увеличиваем count
                        int currentCount = Convert.ToInt32(result);
                        string updateQuery = "UPDATE games_count SET count = @newCount WHERE username = @username";
                        using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@newCount", currentCount + 1);
                            updateCommand.Parameters.AddWithValue("@username", username);

                            updateCommand.ExecuteNonQuery();
                            Debug.Log($"Счётчик игр для пользователя '{username}' увеличен до {currentCount + 1}.");
                        }
                    }
                    else
                    {
                        // Если записи нет, создаём её
                        string insertQuery = "INSERT INTO games_count (username, count) VALUES (@username, 1)";
                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@username", username);
                            insertCommand.ExecuteNonQuery();
                            Debug.Log($"Счётчик игр для пользователя '{username}' создан с начальным значением 1.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при обновлении игрового счётчика: " + ex.Message);
        }
    }
}
