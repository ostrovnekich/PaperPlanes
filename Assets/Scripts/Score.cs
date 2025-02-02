using UnityEngine;
using UnityEngine.UI;
using Npgsql;
using System;
using System.Threading.Tasks;

public class Score : MonoBehaviour
{
    private const string CONNECTION_STRING = "Host=autorack.proxy.rlwy.net;Port=54427;Username=postgres;Password=ooIdicRpqOLCrdRfXRLqHrEaHkUOmMnQ;Database=railway";
    public RandomSpawner pointRandomSpawner;
    public Text scoreText;
    private float timeSincePointTrigger = 0f;
    private int targetScore = 0;
    private float currentScore = 0f;
    public float scoreLerpSpeed = 5f;
    private bool scoreSyncedToDatabase = true;

    void Start()
    {
        UpdateScoreText();
    }

    void Update()
    {
        timeSincePointTrigger += Time.deltaTime;

        if (Mathf.RoundToInt(currentScore) != targetScore)
        {
            currentScore = Mathf.Lerp(currentScore, targetScore, Time.deltaTime * scoreLerpSpeed);
            UpdateScoreText();

            scoreSyncedToDatabase = false;
        }

        if (!scoreSyncedToDatabase && Mathf.RoundToInt(currentScore) == targetScore)
        {
            _ = UpdateHighScoreAsync();
            scoreSyncedToDatabase = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point"))
        {
            pointRandomSpawner.RemoveAllSpawnedObjects();
            pointRandomSpawner.SpawnObjectsInZones();

            targetScore += 100000 / Mathf.RoundToInt(timeSincePointTrigger);
            timeSincePointTrigger = 0f;

            Debug.Log("Триггер Point активирован. Счётчик времени сброшен.");
        }
    }

    private async Task UpdateHighScoreAsync()
    {
        using (var dbConnection = new NpgsqlConnection(CONNECTION_STRING))
        {
            await dbConnection.OpenAsync();
            Debug.Log("Подключение к базе данных открыто!");

            string selectQuery = "SELECT higtscore FROM hightscores WHERE username = @username";
            using (var selectCommand = new NpgsqlCommand(selectQuery, dbConnection))
            {
                selectCommand.Parameters.AddWithValue("@username", CurrentUser.Instance.Username);

                object result = await selectCommand.ExecuteScalarAsync();
                int currentHighScore = result != null ? Convert.ToInt32(result) : 0;

                if (Mathf.RoundToInt(currentScore) > currentHighScore)
                {
                    string updateQuery = "UPDATE hightscores SET higtscore = @higtscore WHERE username = @username";
                    using (var updateCommand = new NpgsqlCommand(updateQuery, dbConnection))
                    {
                        updateCommand.Parameters.AddWithValue("@higtscore", Mathf.RoundToInt(currentScore));
                        updateCommand.Parameters.AddWithValue("@username", CurrentUser.Instance.Username);

                        await updateCommand.ExecuteNonQueryAsync();
                        Debug.Log($"Рекорд обновлен! Новый рекорд: {Mathf.RoundToInt(currentScore)}");
                    }
                }
                else
                {
                    Debug.Log("Текущий счет меньше рекорда. Рекорд не обновлен.");
                }
            }

            Debug.Log("Подключение к базе данных закрыто.");
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.RoundToInt(currentScore);
        }
    }
}
