using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable playerDamageable;
    // Start is called before the first frame update

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player is null)
        {
            Debug.LogError(">>>> MAKE SURE THERE IS A PLAYER TAG IN SCENE");
        }
        playerDamageable = player.GetComponent<Damageable>();

    }

    void Start()
    {
        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthBarText.text = "HP: " + playerDamageable.Health + "/" + playerDamageable.MaxHealth;
    }
    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }
    private void OnDisable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);

    }
    public void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP: " + newHealth + "/" + maxHealth;


    }

    private float CalculateSliderPercentage(float health, float maxHealth)
    {
        return health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
