using UnityEngine;
using TMPro;

public class MatchStatsController : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text values;
    public GameObject victory;

    public TitanController titanController;

    // Example variables
    private int percentage1 = 25;
    private int strikesLanded = 0;
    private int percentage3 = 56;
    private int percentage4 = 25;

    private int count1 = 353;
    private int count2 = 154;
    private int count3 = 5;

    private int overallPercentage = 80;
    private int subPercentage1 = 10;
    private int subPercentage2 = 90;
    private int subPercentage3 = 10;
    private int subPercentage4 = 5;

    private int subCount1 = 70;
    private int subCount2 = 110;
    private int subCount3 = 12;
    private int subCount4 = 8;

    private int overallValue = 90;
    private int finalValue1 = 754;
    private int finalValue2 = 340;

    void Start() {

        float damageDealt = 100f - titanController.opponentController.GetHealth();
        float offensiveEfficiency = Mathf.RoundToInt((titanController.GetStrikesLanded() * 100 / titanController.GetTotalStrikes()) * damageDealt / 100f);
        float strikesLandedPer = Mathf.RoundToInt(titanController.GetStrikesLanded() * 100 / titanController.GetTotalStrikes());
    
        float damageReceived = 100f - titanController.GetHealth();
        float defensiveEfficiency = Mathf.RoundToInt((titanController.opponentController.GetStrikesLanded() * 100 / titanController.opponentController.GetTotalStrikes()) * damageReceived / 100f);
        float strikesReceivedPer = Mathf.RoundToInt(titanController.opponentController.GetStrikesLanded() * 100 / titanController.opponentController.GetTotalStrikes());
    
        float energySpent = Mathf.RoundToInt(titanController.GetEnergySpent());
        float energyEfficiency = damageDealt/energySpent;
        float energyGenerated = Mathf.RoundToInt(titanController.GetEnergyGenerated());

        // Format the string
        string formattedText = string.Format(
            "{0}%\n\n{1}\n{2}% ({3})\n\n{4}%\n\n{5}\n{6}% ({7})\n\n{5}\n\n{6}\n{7}",

            offensiveEfficiency,
            damageDealt,
            strikesLandedPer, titanController.GetStrikesLanded(),

            defensiveEfficiency,
            damageReceived,
            strikesReceivedPer, titanController.opponentController.GetStrikesLanded(),

            energyEfficiency,
            energySpent,
            Mathf.RoundToInt(titanController.GetEnergyGenerated())
        );

        // Set the text field's value
        values.text = formattedText;
        title.text = titanController.GetName();
        victory.SetActive(titanController.GetHealth() > 0 && titanController.GetHealth() > titanController.opponentController.GetHealth());
    }
}