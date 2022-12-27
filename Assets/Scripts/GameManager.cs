using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { inGame, gameOver }
public class GameManager : MonoBehaviour
{
    public delegate void StateChanged();
    public static event StateChanged OnInGame;
    public static event StateChanged OnGameOver;
    public PlayerResources deadPlayer;

    public static GameManager sharedInstance;

    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get => currentGameState;
        set
        {
            currentGameState = value;
            SetGameState(value);
        }
    }
    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        CurrentGameState = GameState.inGame;
    }

    void SetGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.inGame:      OnInGame?.Invoke();
                break;
            case GameState.gameOver:    OnGameOver?.Invoke();
                break;
            default:
                break;
        }
    }

}
