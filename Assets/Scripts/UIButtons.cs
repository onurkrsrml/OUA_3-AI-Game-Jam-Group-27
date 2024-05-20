using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{

    public void nextLevel()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void continueGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void settingsMenu()
    {
        SceneManager.LoadScene("SettingMenu");
    }

    public void startGame()
    {
        SceneManager.LoadScene("FirstLevel");
    }

    public void finishMenu()
    {
        SceneManager.LoadScene("FinishMenu");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}