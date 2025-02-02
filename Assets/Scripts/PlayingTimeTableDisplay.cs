using System;
using Npgsql;
using UnityEngine;
using TMPro;

public class PlayingTimeTableDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Контейнер для строк таблицы
    public Transform contentContainer;

    // Префаб строки
    public GameObject rowPrefab;

    // Поля для добавления новой записи времени игры
    public TMP_InputField addUsernameInput;
    public TMP_InputField addPlayTimeInput;

    // Поле для удаления записи по ID
    public TMP_InputField deleteIdInput;

    void Start()
    {
        LoadPlayingTimeTable();
    }

    public void LoadPlayingTimeTable()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения всех записей из таблицы playing_time
                string query = "SELECT id, username, play_time FROM playing_time ORDER BY id";

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
                        int playTime = reader.GetInt32(2);

                        // Создаём новую строку из префаба
                        GameObject newRow = Instantiate(rowPrefab, contentContainer);

                        // Устанавливаем значения для текстовых полей в строке
                        TextMeshProUGUI[] rowTexts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                        if (rowTexts.Length >= 3)
                        {
                            rowTexts[0].text = id.ToString();
                            rowTexts[1].text = username;
                            rowTexts[2].text = playTime.ToString();
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
            Debug.LogError("Ошибка при загрузке таблицы времени игры: " + ex.Message);
        }
    }

    public void AddPlayingTime()
    {
        string username = addUsernameInput.text;
        int playTime = Convert.ToInt32(addPlayTimeInput.text);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(playTime.ToString()))
        {
            Debug.LogError("Имя пользователя и время игры не могут быть пустыми!");
            return;
        }

        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для добавления записи о времени игры
                string query = "INSERT INTO playing_time (username, play_time) VALUES (@username, @play_time)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@play_time", playTime);

                    command.ExecuteNonQuery();
                    Debug.Log("Запись о времени игры успешно добавлена!");
                }
            }

            // Обновляем таблицу
            LoadPlayingTimeTable();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при добавлении записи о времени игры: " + ex.Message);
        }
    }

    public void DeletePlayingTimeById()
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
                    string query = "DELETE FROM playing_time WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", recordId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.Log("Запись о времени игры успешно удалена!");
                        }
                        else
                        {
                            Debug.LogWarning("Запись с указанным ID не найдена.");
                        }
                    }
                }

                // Обновляем таблицу
                LoadPlayingTimeTable();
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при удалении записи о времени игры: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Некорректный ID записи!");
        }
    }
}
