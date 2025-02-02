using System;
using Npgsql;
using UnityEngine;
using TMPro;

public class HighScoresTableDisplay : MonoBehaviour
{
    // Строка подключения к базе данных
    private string connectionString = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";

    // Контейнер для строк таблицы
    public Transform contentContainer;

    // Префаб строки
    public GameObject rowPrefab;

    // Поля для добавления нового рекорда
    public TMP_InputField addUsernameInput;
    public TMP_InputField addHighScoreInput;

    // Поле для удаления рекорда по ID
    public TMP_InputField deleteIdInput;

    void Start()
    {
        LoadHighScoresTable();
    }

    public void LoadHighScoresTable()
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Подключение к базе данных успешно!");

                // SQL запрос для получения всех записей из таблицы hightscores
                string query = "SELECT id, username, higtscore FROM hightscores ORDER BY higtscore DESC";

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
                        int highscore = reader.GetInt32(2);

                        // Создаём новую строку из префаба
                        GameObject newRow = Instantiate(rowPrefab, contentContainer);

                        // Устанавливаем значения для текстовых полей в строке
                        TextMeshProUGUI[] rowTexts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                        if (rowTexts.Length >= 3)
                        {
                            rowTexts[0].text = id.ToString();
                            rowTexts[1].text = username;
                            rowTexts[2].text = highscore.ToString();
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
            Debug.LogError("Ошибка при загрузке таблицы рекордов: " + ex.Message);
        }
    }

    public void AddHighScore()
    {
        string username = addUsernameInput.text;
        if (!int.TryParse(addHighScoreInput.text, out int highscore))
        {
            Debug.LogError("Некорректный рекорд! Он должен быть числом.");
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

                // SQL запрос для добавления нового рекорда
                string query = "INSERT INTO hightscores (username, higtscore) VALUES (@username, @highscore)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@highscore", highscore);

                    command.ExecuteNonQuery();
                    Debug.Log("Рекорд успешно добавлен!");
                }
            }

            // Обновляем таблицу
            LoadHighScoresTable();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка при добавлении рекорда: " + ex.Message);
        }
    }

    public void DeleteHighScoreById()
    {
        if (int.TryParse(deleteIdInput.text, out int recordId))
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Debug.Log("Подключение к базе данных успешно!");

                    // SQL запрос для удаления рекорда по ID
                    string query = "DELETE FROM hightscores WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", recordId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.Log("Рекорд успешно удалён!");
                        }
                        else
                        {
                            Debug.LogWarning("Рекорд с указанным ID не найден.");
                        }
                    }
                }

                // Обновляем таблицу
                LoadHighScoresTable();
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при удалении рекорда: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Некорректный ID записи!");
        }
    }
}
