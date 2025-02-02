using System;
using Npgsql;
using UnityEngine;

public class DatabaseConnection : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    void Start()
    {
        // Пример добавления нового пользователя
        AddUser("newuser", "newpassword");
    }

    public void AddUser(string username, string password)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // Шаг 1: Проверяем, существует ли пользователь с таким именем
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    // Шаг 2: Если пользователя нет, добавляем его
                    if (userCount == 0)
                    {
                        string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@username", username);
                            insertCommand.Parameters.AddWithValue("@password", password);

                            // Выполняем вставку
                            insertCommand.ExecuteNonQuery();
                            Debug.Log("Пользователь успешно добавлен!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Пользователь с таким именем уже существует.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при работе с базой данных: " + ex.Message);
        }
    }
}
