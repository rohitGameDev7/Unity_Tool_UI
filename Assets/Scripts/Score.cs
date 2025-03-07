using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Score : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreText;
    public const string ScoreKey = "Score";

    // called when script is first run
    void Start()
    {
       LoadScene();
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        SaveScore();
    }

    public void SubtractScore(int points)
    {
        score -= points;
        UpdateScoreText();
        SaveScore();
    }



    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save();
    }

    public void LoadScene()
    {
        if (PlayerPrefs.HasKey(ScoreKey))
        {
            score = PlayerPrefs.GetInt(ScoreKey);
        }
        else
        {
            score = 0;
        }
    }


    public void ResetScore()
    {
        PlayerPrefs.DeleteAll();
        score = 0;
        UpdateScoreText();
    }
}
