using System;
using Npgsql;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AddUser : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField password2;
    public TMP_Text errorField;
    
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    public void Add()
{
    try
    {
        if (password.text != password2.text)
        {
            errorField.text = "Passwords don't match";
            return;
        }

        if (username.text == "" || username.text.Length > 20)
        {
            errorField.text = "Incorrect username";
            return;
        }

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            Debug.Log("Подключение к базе данных успешно!");

            // Проверяем, существует ли пользователь с таким именем
            string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@username", username.text);
                int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (userCount == 0) // Если пользователь не найден
                {
                    // Добавляем пользователя в таблицу users
                    string insertUserQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    using (var insertUserCommand = new NpgsqlCommand(insertUserQuery, connection))
                    {
                        insertUserCommand.Parameters.AddWithValue("@username", username.text);
                        insertUserCommand.Parameters.AddWithValue("@password", password.text);

                        insertUserCommand.ExecuteNonQuery();
                        Debug.Log("Пользователь успешно добавлен!");
                    }

                    // Добавляем пользователя в таблицу hightscores с начальным рекордом 0
                    string insertHighScoreQuery = "INSERT INTO hightscores (username, higtscore) VALUES (@username, @higtscore)";
                    using (var insertHighScoreCommand = new NpgsqlCommand(insertHighScoreQuery, connection))
                    {
                        insertHighScoreCommand.Parameters.AddWithValue("@username", username.text);
                        insertHighScoreCommand.Parameters.AddWithValue("@higtscore", 0);

                        insertHighScoreCommand.ExecuteNonQuery();
                        Debug.Log("Пользователь успешно добавлен в таблицу hightscores!");
                    }

                    // Инициализируем текущего пользователя и загружаем главную сцену
                    CurrentUser.Initialize(username.text);
                    if(CurrentUser.Instance.Username == "Admin")
                    SceneManager.LoadScene("AdminMainMenu");
                    else
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    errorField.text = "Username already exists";
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Обрабатываем другие возможные ошибки
        Debug.LogError("An error occurred: " + ex.Message);
        errorField.text = "Database connection error";
    }
}

}
