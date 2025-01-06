using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    public TMP_Text timerText;
public int startTime = 60; // Starting time in seconds
    private int currentTime;

    private bool isCountingDown = false;

    private MatchController matchController;

    void Start()
    {
        ResetTimer();
        matchController = gameObject.GetComponent<MatchController>();
    }

    public void StartCountdownFromButton()
    {
        if (!isCountingDown) // Prevent multiple countdowns from starting
        {
            StartCoroutine(StartCountdown());
        }
    }

    private System.Collections.IEnumerator StartCountdown()
    {
        isCountingDown = true;
        while (currentTime >= 0)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1); // Wait for 1 second
            currentTime--;
        }

        isCountingDown = false;
        matchController.TimeUp();

    }

    private void UpdateTimerUI()
    {
        timerText.text = currentTime.ToString(); // Update the UI
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }
}
