using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public  bool GameIsPaused = false;
    public BallControl ballScript;
    public GameObject pauseMenuUI;
    public GameObject buttonPause;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        ballScript.isBeingHeld = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        ballScript.isBeingHeld = false;
    }
}
