using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public delegate void UIDelegate();
    public event UIDelegate PlayAgainEvent;
    public static UIManager shareInstance;

    [SerializeField] GameObject gameOverObject;
    [SerializeField] TMP_Text playerWinsText;
    [SerializeField] GameObject HUDObject;

    [SerializeField] Color playerOneWinsColor;
    [SerializeField] Color playerTwoWinsColor;

    private void Awake()
    {
        if(shareInstance == null)
        {
            shareInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PlayAgainEvent += ShowHUD;
        PlayAgainEvent += HideGameOver;
    }

    public void ShowGameOver(Player thePLayer)
    {
        gameOverObject.SetActive(true);
        if(thePLayer == Player.Player1)
        {
            playerWinsText.text = "PLAYER 2 WINS";
            playerWinsText.color = playerTwoWinsColor;
        }
        else if(thePLayer == Player.Player2)
        {
            playerWinsText.text = "PLAYER 1 WINS";
            playerWinsText.color = playerOneWinsColor;
        }
    }

    public void HideGameOver()
    {
        gameOverObject.SetActive(false);
    }

    public void HideHUD()
    {
        HUDObject.SetActive(false);
    }

    public void ShowHUD()
    {
        HUDObject.SetActive(true);
    }

    public void ExitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
    }

    public void PlayAgain()
    {
        PlayAgainEvent?.Invoke();
    }
}
