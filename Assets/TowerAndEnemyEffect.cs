using ActionGameFramework.Health;
using Core.Health;
using System.Collections.Generic;
using TowerDefense.Affectors;
using TowerDefense.Level;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using UnityEngine;

public class TowerAndEnemyEffect : MonoBehaviour
{
    [SerializeField] private SerializableIAlignmentProvider player;
    [SerializeField] private SerializableIAlignmentProvider enemy;
    [SerializeField] private SerializableIAlignmentProvider support;

    private WaveManager waveManager;
    private Affector[] affectors;
    private Tower towerData;

    private SlowAffector slowAffector;
    private FireRateAffector fireRateAffector;
    private DamageAffector damageAffector;
    private HPAffector hpAffector;

    private int choosedValue = 0;
    private int currentLevel = 0;

    private void Awake()
    {
        waveManager = LevelManager.instance.GetComponent<WaveManager>();
        towerData = GetComponent<Tower>();
    }

    private void Start()
    {
        waveManager.waveChanged += ApplyNewEffect;
        towerData.towerUpgrade += UpdateCurrentLevel;
    }

    private void LateUpdate()
    {
        if (hpAffector == null)
        {
            FindAffectors();
        }
    }

    private void OnDestroy()
    {
        waveManager.waveChanged -= ApplyNewEffect;
        towerData.towerUpgrade -= UpdateCurrentLevel;
    }

    private void FindAffectors()
    {
        affectors = GameObject.FindWithTag("Affector").transform.parent.gameObject.GetComponent<TowerLevel>().Affectors;

        foreach (Affector affector in affectors)
        {
            switch (affector.GetType().Name)
            {
                case "SlowAffector":
                    slowAffector = affector as SlowAffector;
                    break;
                case "DamageAffector":
                    damageAffector = affector as DamageAffector;
                    break;
                case "HPAffector":
                    hpAffector = affector as HPAffector;
                    break;
                case "FireRateAffector":
                    fireRateAffector = affector as FireRateAffector;
                    break;
            }
        }
    }

    private void UpdateCurrentLevel()
    {
        currentLevel = towerData.currentLevel;
        ResetPowerUP();
        ActivateRandomPowerUP();
    }

    private void ActivateAllAffectors()
    {
        towerData.configuration.alignment = support;
        foreach (Affector affector in affectors)
        {
            affector.isActive = true;
        }
    }

    private void DeactivateTowers()
    {
        // Change alignment
    }

    private void ApplyNewEffect()
    {
        // Get random value.
        choosedValue = Random.Range(0, 100);

        if (choosedValue >= 95)
        {
            ActivateAllAffectors();
        }
        else if (choosedValue <= 5)
        {
            DeactivateTowers();
        }
        else
        {
            ActivateRandomPowerUP();
        }
    }

    private void ActivateRandomPowerUP()
    {
        ResetPowerUP();

        damageAffector.isActive = true;
        damageAffector.damageFactor *= (1 + GetPercentage(true));

        /*

        // Get value for positive or negative option to get.
        choosedValue = Random.Range(0, 100);

        // Positive effect.
        if (choosedValue <= GoodEffectChance())
        {
            // Get value for power up to choose.
            choosedValue = Random.Range(0, 3);

            if (choosedValue == 0)
            {
                // +% DMG
                damageAffector.isActive = true;
                damageAffector.damageFactor *= (1+GetPercentage(true));
            }
            else if (choosedValue == 1)
            {
                // +% Fire Rate
                fireRateAffector.isActive = true;
                fireRateAffector.fireRateFactor *= (1+GetPercentage(true));
            }
            else if (choosedValue == 2)
            {
                // -% EnemyHP
                hpAffector.isActive = true;
                hpAffector.hpFactor *= (1-GetPercentage(true));
            }
            else
            {
                // -% EnemySpeed
                slowAffector.isActive = true;
                slowAffector.slowFactor *= (1-GetPercentage(true));
            }
        }

        // Negative effect.
        else
        {
            // Get value for power up to choose.
            choosedValue = Random.Range(0, 3);

            if (choosedValue == 0)
            {
                // -% DMG
                damageAffector.isActive = true;
                damageAffector.damageFactor *= (1-GetPercentage(false));
            }
            else if (choosedValue == 1)
            {
                // -% Fire Rate
                fireRateAffector.isActive = true;
                fireRateAffector.fireRateFactor *= (1-GetPercentage(false));
            }
            else if (choosedValue == 2)
            {
                // +% EnemyHP
                hpAffector.isActive = true;
                hpAffector.hpFactor *= (1+GetPercentage(false));
            }
            else
            {
                // +% EnemySpeed
                slowAffector.isActive = true;
                slowAffector.slowFactor *= (1+GetPercentage(false));
            }
        }
        */
    }

    private void ResetPowerUP()
    {
        foreach (Affector affector in affectors)
        {
            affector.isActive = false;

            switch (affector.GetType().Name)
            {
                case "SlowAffector":
                    slowAffector.slowFactor = 1;
                    break;

                case "DamageAffector":
                    damageAffector.damageFactor = 1;
                    break;

                case "HPAffector":
                    hpAffector.hpFactor = 1;
                    break;

                case "FireRateAffector":
                    fireRateAffector.fireRateFactor = 1;
                    break;
            }
        }
    }

    private float GetPercentage(bool goodEffect)
    {
        if (!goodEffect) return 0.05f;
        else
        {
            if (currentLevel == 0)
            {
                return 0.05f;
            }
            else if (currentLevel == 1)
            {
                return 0.08f;
            }
            else
            {
                return 0.125f;
            }
        }
    }

    private float GoodEffectChance()
    {
        if (currentLevel == 0)
        {
            return 50;
        }
        else if (currentLevel == 1)
        {
            return 61.5f;
        }
        else
        {
            return 71.428f;
        }
    }
}
