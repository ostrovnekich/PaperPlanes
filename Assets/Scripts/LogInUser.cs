using System;
using Npgsql;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class LogInUser : MonoBehaviour
{
    public TMP_InputField username;    // Поле для ввода имени пользователя
    public TMP_InputField password;    // Поле для ввода пароля
    public TMP_Text errorField;        // Поле для отображения ошибок

    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    public void Login()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL-запрос для проверки логина и пароля
                string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username.text);
                    command.Parameters.AddWithValue("@password", password.text);

                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    if (userCount > 0) // Если пользователь найден
                    {
                        Debug.Log("Успешный вход!");
                        CurrentUser.Initialize(username.text);
                        if(CurrentUser.Instance.Username == "Admin")
                        SceneManager.LoadScene("AdminMainMenu");
                        else
                        SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        Debug.LogWarning("Неверное имя пользователя или пароль.");
                        errorField.text = "Invalid username or password.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при работе с базой данных: " + ex.Message);
            errorField.text = "Database connection error.";
        }
    }
}
