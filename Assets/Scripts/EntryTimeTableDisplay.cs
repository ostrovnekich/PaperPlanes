using System;
using Npgsql;
using UnityEngine;
using TMPro;

public class EntryTimeTableDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Контейнер для строк таблицы
    public Transform contentContainer;

    // Префаб строки
    public GameObject rowPrefab;

    // Поля для добавления новой записи времени входа
    public TMP_InputField addUsernameInput;
    public TMP_InputField addTimeInput;

    // Поле для удаления записи по ID
    public TMP_InputField deleteIdInput;

    void Start()
    {
        LoadEntryTimeTable();
    }

    public void LoadEntryTimeTable()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения всех записей из таблицы entry_time
                string query = "SELECT id, username, time FROM entry_time ORDER BY id";

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
                        DateTime time = reader.GetDateTime(2);

                        // Создаём новую строку из префаба
                        GameObject newRow = Instantiate(rowPrefab, contentContainer);

                        // Устанавливаем значения для текстовых полей в строке
                        TextMeshProUGUI[] rowTexts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                        if (rowTexts.Length >= 3)
                        {
                            rowTexts[0].text = id.ToString();
                            rowTexts[1].text = username;
                            rowTexts[2].text = time.ToString("yyyy-MM-dd");
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
            Debug.LogError("Ошибка при загрузке таблицы времени входа: " + ex.Message);
        }
    }

    public void AddEntryTime()
    {
        string username = addUsernameInput.text;
        string timeString = addTimeInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(timeString))
        {
            Debug.LogError("Имя пользователя и время не могут быть пустыми!");
            return;
        }

        // Проверяем правильность формата даты
        if (!DateTime.TryParse(timeString, out DateTime time))
        {
            Debug.LogError("Некорректный формат времени!");
            return;
        }

        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для добавления записи времени входа
                string query = "INSERT INTO entry_time (username, time) VALUES (@username, @time)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@time", time);

                    command.ExecuteNonQuery();
                    Debug.Log("Запись о времени входа успешно добавлена!");
                }
            }

            // Обновляем таблицу
            LoadEntryTimeTable();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при добавлении записи о времени входа: " + ex.Message);
        }
    }

    public void DeleteEntryTimeById()
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
                    string query = "DELETE FROM entry_time WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", recordId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.Log("Запись о времени входа успешно удалена!");
                        }
                        else
                        {
                            Debug.LogWarning("Запись с указанным ID не найдена.");
                        }
                    }
                }

                // Обновляем таблицу
                LoadEntryTimeTable();
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при удалении записи о времени входа: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Некорректный ID записи!");
        }
    }
}
