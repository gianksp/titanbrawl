using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public TitanController controller;
    public bool isEnergy = false;
    public bool isHealth = false;
    public Image fill;
    public TMP_Text val;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float amount = isHealth ? controller.GetHealth() : controller.GetEnergy();
        fill.fillAmount = amount/100f;
        val.text= amount.ToString("f0");
    }
}
