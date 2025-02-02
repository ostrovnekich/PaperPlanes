using System;
using Npgsql;
using UnityEngine;
using TMPro;

public class GamesCountTableDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Контейнер для строк таблицы
    public Transform contentContainer;

    // Префаб строки
    public GameObject rowPrefab;

    // Поля для добавления или обновления записи
    public TMP_InputField addUsernameInput;
    public TMP_InputField addCountInput;

    // Поле для удаления записи по ID
    public TMP_InputField deleteIdInput;

    void Start()
    {
        LoadGamesCountTable();
    }

    public void LoadGamesCountTable()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения всех записей из таблицы games_count
                string query = "SELECT id, username, count FROM games_count ORDER BY id";

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
                        int count = reader.GetInt32(2);

                        // Создаём новую строку из префаба
                        GameObject newRow = Instantiate(rowPrefab, contentContainer);

                        // Устанавливаем значения для текстовых полей в строке
                        TextMeshProUGUI[] rowTexts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                        if (rowTexts.Length >= 3)
                        {
                            rowTexts[0].text = id.ToString();
                            rowTexts[1].text = username;
                            rowTexts[2].text = count.ToString();
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
            Debug.LogError("Ошибка при загрузке таблицы игр: " + ex.Message);
        }
    }

public void AddGamesCount()
{
    string username = addUsernameInput.text;
    if (!int.TryParse(addCountInput.text, out int count))
    {
        Debug.LogError("Некорректное значение для поля count! Оно должно быть числом.");
        return;
    }

    if (string.IsNullOrEmpty(username))
    {
        Debug.LogError("Имя пользователя не может быть пустым!");
        return;
    }

    try
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            Debug.Log("Подключение к базе данных успешно!");

            // SQL запрос для добавления новой записи
            string query = "INSERT INTO games_count (username, count) VALUES (@username, @count)";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@count", count);

                command.ExecuteNonQuery();
                Debug.Log("Запись успешно добавлена!");
            }
        }

        // Обновляем таблицу
        LoadGamesCountTable();
    }
    catch (Exception ex)
    {
        Debug.LogError("Ошибка при добавлении записи: " + ex.Message);
    }
}

    public void DeleteGamesCountById()
    {
        if (int.TryParse(deleteIdInput.text, out int recordId))
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Debug.Log("Подключение к базе данных успешно!");

                    // SQL запрос для удаления записи по ID
                    string query = "DELETE FROM games_count WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", recordId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.Log("Запись успешно удалена!");
                        }
                        else
                        {
                            Debug.LogWarning("Запись с указанным ID не найдена.");
                        }
                    }
                }

                // Обновляем таблицу
                LoadGamesCountTable();
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при удалении записи: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Некорректный ID записи!");
        }
    }
}
