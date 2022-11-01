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

        UIManager.shareInstance.PlayAgainEvent += ResetResources;
    }

    int CurrentHealth
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
        UIManager.shareInstance.PlayAgainEvent += ResetResources;
    }

    private void OnDisable()
    {
        UIManager.shareInstance.PlayAgainEvent -= ResetResources;
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
    }

    void ResetResources()
    {
        CurrentHealth = maxHealth;
    }
}
