using System;
using Npgsql;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseEditor : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // UI элементы
    public InputField usernameInput;
    public InputField passwordInput;
    public Button addUserButton;
    public Button updateUserButton;
    public Button deleteUserButton;
    public Text feedbackText;

    void Start()
    {
        // Привязка событий кнопок
        addUserButton.onClick.AddListener(() => AddUser(usernameInput.text, passwordInput.text));
        updateUserButton.onClick.AddListener(() => UpdateUser(usernameInput.text, passwordInput.text));
        deleteUserButton.onClick.AddListener(() => DeleteUser(usernameInput.text));
    }

    public void AddUser(string username, string password)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // Проверяем, существует ли пользователь с таким именем
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userCount == 0)
                    {
                        string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@username", username);
                            insertCommand.Parameters.AddWithValue("@password", password);
                            insertCommand.ExecuteNonQuery();
                            feedbackText.text = "Пользователь успешно добавлен!";
                        }
                    }
                    else
                    {
                        feedbackText.text = "Пользователь с таким именем уже существует.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Ошибка: " + ex.Message;
            Debug.LogError("Ошибка при работе с базой данных: " + ex.Message);
        }
    }

    public void UpdateUser(string username, string newPassword)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                string updateQuery = "UPDATE users SET password = @password WHERE username = @username";
                using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@username", username);
                    updateCommand.Parameters.AddWithValue("@password", newPassword);
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        feedbackText.text = "Пароль пользователя обновлен!";
                    }
                    else
                    {
                        feedbackText.text = "Пользователь с таким именем не найден.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Ошибка: " + ex.Message;
            Debug.LogError("Ошибка при работе с базой данных: " + ex.Message);
        }
    }

    public void DeleteUser(string username)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                string deleteQuery = "DELETE FROM users WHERE username = @username";
                using (var deleteCommand = new NpgsqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@username", username);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        feedbackText.text = "Пользователь успешно удален!";
                    }
                    else
                    {
                        feedbackText.text = "Пользователь с таким именем не найден.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Ошибка: " + ex.Message;
            Debug.LogError("Ошибка при работе с базой данных: " + ex.Message);
        }
    }
}
