using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth;
    public float chipSpeed;

    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;
    public GameObject GameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthText.text = health + "/" + maxHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        updateHealthUI();

        if (health <= 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            GameOverPanel.SetActive(true);
        }
    }

    public void updateHealthUI() 
    {
        //Debug.Log(health);
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = health/ maxHealth;
        if (fillBack > hFraction) 
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }

        if (fillFront < hFraction) 
        {
            backHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/ chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, hFraction, percentComplete);
        }
    }

    public void takeDamage(float damage) 
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        healthText.text = (health + "/" + maxHealth).ToString();
        lerpTimer = 0f;
    }

    public void ragainHealth(float healthAmount) 
    {
        health += healthAmount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        healthText.text = (health + "/" + maxHealth).ToString();
        lerpTimer = 0;
    }
}
