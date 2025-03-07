using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Category");
    }

    public void ResetQuiz()
    {
        PlayerPrefs.DeleteAll();
    }


    public void RestartQuiz()
    {
        PlayerPrefs.DeleteAll();  // Reset quiz data
        PlayerPrefs.Save();        // Save changes immediately
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
        Debug.Log("Quiz restarted!");
    }


    public void Exit()
    {
        Application.Quit();
    }
}
