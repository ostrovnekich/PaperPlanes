using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Npgsql;

public class Leaderboard : MonoBehaviour
{
    private const string CONNECTION_STRING = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";
    public TMP_Text leaderboardText;

public async void LoadLeaderboard()
{
    Debug.Log("Метод LoadLeaderboard вызван!");
    try
    {
        using (var connection = new NpgsqlConnection(CONNECTION_STRING))
        {
            await connection.OpenAsync();
            Debug.Log("Подключение к базе данных успешно!");

            string query = "SELECT username, higtscore FROM hightscores ORDER BY higtscore DESC LIMIT 10";
            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<string> leaderboardEntries = new List<string>();
                    int rank = 1;

                    while (await reader.ReadAsync())
                    {
                        string username = reader.GetString(0);
                        int higtscore = reader.GetInt32(1);
                        leaderboardEntries.Add($"{rank}. {username} - {higtscore}");
                        rank++;
                    }

                    Debug.Log("Список лидеров успешно загружен!");
                    leaderboardText.text = string.Join("\n", leaderboardEntries);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Ошибка загрузки списка лидеров: {ex.Message}");
        leaderboardText.text = "Loading error.";
    }
}
}
