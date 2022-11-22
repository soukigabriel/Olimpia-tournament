using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] int currentHealth;
    Player thisPlayer;
    GameObject myHealthBar;
    Slider myHealthBarSlider;

    

    private void Awake()
    {
        thisPlayer = gameObject.GetComponent<PlayerController>().thisPlayer;
        if(thisPlayer == Player.Player1)
        {
            myHealthBar = GameObject.Find("Player1Bar");
        }
        else if(thisPlayer == Player.Player2)
        {
            myHealthBar = GameObject.Find("Player2Bar");
        }
        myHealthBarSlider = myHealthBar.GetComponent<Slider>();

        GameManager.OnInGame += ResetResources;
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            myHealthBarSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnInGame += ResetResources;
    }

    private void OnDisable()
    {
        GameManager.OnInGame -= ResetResources;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        myHealthBarSlider.value = currentHealth;
    }

    public void SetHealth(int health)
    {
        CurrentHealth += health;
    }

    void Die()
    {
        UIManager.shareInstance.ShowGameOver(thisPlayer);
        gameObject.GetComponentInChildren<Animator>().SetInteger("RandomDeath", Random.Range(1, 5));
        gameObject.GetComponentInChildren<Animator>().SetTrigger("Death");
        GameManager.sharedInstance.CurrentGameState = GameState.gameOver;
    }

    void ResetResources()
    {
        CurrentHealth = maxHealth;
    }
}
