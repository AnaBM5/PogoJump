using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Components")] 
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject instructionScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    

    private void Start()
    {
        gameOverScreen.SetActive(false);
        scoreText.gameObject.SetActive(false);
        
        overlay.SetActive(true);
        instructionScreen.SetActive(true);

        scoreText.text = "0";
        gameOverScoreText.text = "0";
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void HideInstructions()
    {
        instructionScreen.SetActive(false);
        overlay.SetActive(false);
        scoreText.gameObject.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        gameOverScoreText.text = scoreText.text;
        
        scoreText.gameObject.SetActive(false);
        overlay.SetActive(true);
        gameOverScreen.SetActive(true);
    }

    public void ChangeScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

}
