using System;
using Npgsql;
using UnityEngine;
using TMPro;

public class UsersTableDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Контейнер для строк таблицы
    public Transform contentContainer;

    // Префаб строки
    public GameObject rowPrefab;

    // Поля для добавления нового пользователя
    public TMP_InputField addUsernameInput;
    public TMP_InputField addPasswordInput;

    // Поле для удаления пользователя по ID
    public TMP_InputField deleteIdInput;

    void Start()
    {
        LoadUsersTable();
    }

    public void LoadUsersTable()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения всех записей из таблицы users
                string query = "SELECT id, username, password FROM users ORDER BY id";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    // Удаляем старые строки, если они есть
                    foreach (Transform child in contentContainer)
                    {
                        Destroy(child.gameObject);
                    }

                    // Создаём строки для каждой записи
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        string password = reader.GetString(2);

                        // Создаём новую строку из префаба
                        GameObject newRow = Instantiate(rowPrefab, contentContainer);

                        // Устанавливаем значения для текстовых полей в строке
                        TextMeshProUGUI[] rowTexts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                        if (rowTexts.Length >= 3)
                        {
                            rowTexts[0].text = id.ToString();
                            rowTexts[1].text = username;
                            rowTexts[2].text = password;
                        }
                        else
                        {
                            Debug.LogError("rowPrefab должен содержать минимум три TextMeshProUGUI компонента!");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при загрузке таблицы пользователей: " + ex.Message);
        }
    }

    public void AddUser()
    {
        string username = addUsernameInput.text;
        string password = addPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Имя пользователя и пароль не могут быть пустыми!");
            return;
        }

        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для добавления нового пользователя
                string query = "INSERT INTO users (username, password) VALUES (@username, @password)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    command.ExecuteNonQuery();
                    Debug.Log("Пользователь успешно добавлен!");
                }
            }

            // Обновляем таблицу
            LoadUsersTable();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при добавлении пользователя: " + ex.Message);
        }
    }

    public void DeleteUserById()
    {
        if (int.TryParse(deleteIdInput.text, out int userId))
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Debug.Log("Подключение к базе данных успешно!");

                    // SQL запрос для удаления пользователя по ID
                    string query = "DELETE FROM users WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.Log("Пользователь успешно удалён!");
                        }
                        else
                        {
                            Debug.LogWarning("Пользователь с указанным ID не найден.");
                        }
                    }
                }

                // Обновляем таблицу
                LoadUsersTable();
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при удалении пользователя: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Некорректный ID пользователя!");
        }
    }
}
