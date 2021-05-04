using System.Collections;
using System.Collections.Generic;
using TowerDefense.Level;
using UnityEngine;

public class StealMoney : MonoBehaviour
{
    private LevelManager levelManager;
    public int currencyToSteal = 10;

    void Start()
    {
        levelManager = LevelManager.instance;
        levelManager.currency.AddCurrency(-currencyToSteal);
    }
}
