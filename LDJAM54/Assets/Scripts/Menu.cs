using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreUI;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        scoreUI.text = Player.killCount + " humans executed!";
    }

    public void StartGame()
    {
        Player.health = Player.PLAYER_START_HEALTH;
        Player.killCount = 0;
        SceneManager.LoadScene(1);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
