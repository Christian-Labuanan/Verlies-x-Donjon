using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startgame : MonoBehaviour
{
    public int gameStartScreen;

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScreen);
    }
}
