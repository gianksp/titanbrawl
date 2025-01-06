using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InfoBar : MonoBehaviour
{

    public TitanController controller;

    public TMP_Text val;
    public TMP_Text regen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetName() != null) {
            val.text= controller.GetName();
        }

        regen.text = $"+{controller.GetEnergyRegen()}/s";
    }
}
