using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    PlayerController myController;

    public Image healthImage;
    public Image meleeImage;
    public Image rangedImage;
    public Image specialImage;

    public float maxHealth = 3;
    public int maxMeleeStacks = 3;
    public int maxRangedStacks = 3;
    public int maxSpecialMeter = 9;

    float health;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value <= 0)
            {
                healthImage.gameObject.SetActive(false);
                Die();
            }
            else if (value >= maxHealth)
            {
                health = maxHealth;
            }
            else
            {
                health = value;
            }
            healthImage.fillAmount = health / maxHealth;
        }
    }

    int meleeStacks;
    public int MeleeStacks
    {
        get
        {
            return meleeStacks;
        }
        set
        {
            if (value <= 0)
            {
                meleeStacks = 0;
            }
            else if (value >= maxMeleeStacks)
            {
                meleeStacks = maxMeleeStacks;
            }
            else
            {
                meleeStacks = value;
            }
            meleeImage.fillAmount = 1.0f * meleeStacks / maxMeleeStacks;
        }
    }

    int rangedStacks;
    public int RangedStacks
    {
        get
        {
            return rangedStacks;
        }
        set
        {
            if (value <= 0)
            {
                rangedStacks = 0;
            }
            else if (value >= maxRangedStacks)
            {
                rangedStacks = maxRangedStacks;
            }
            else
            {
                rangedStacks = value;
            }
            rangedImage.fillAmount = 1.0f * rangedStacks / maxRangedStacks;
        }
    }

    float specialMeter;
    public float SpecialMeter
    {
        get
        {
            return specialMeter;
        }
        set
        {
            if (value <= 0)
            {
                specialMeter = 0;
            }
            else if (value >= maxSpecialMeter)
            {
                specialMeter = maxSpecialMeter;
            }
            else
            {
                specialMeter = value;
            }
            specialImage.fillAmount = specialMeter / maxSpecialMeter;
        }
    }

    private void Start()
    {
        myController = GetComponent<PlayerController>();

        Health = maxHealth;
        MeleeStacks = 0;
        RangedStacks = 0;
        SpecialMeter = 0;
    }

    void Die()
    {
        SceneManager.LoadScene(0);
    }
}
