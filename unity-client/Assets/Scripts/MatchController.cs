using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using SimpleFileBrowser;
using System.Collections;
using UnityEngine.UI;

public class MatchController : MonoBehaviour {

    // public TMP_InputField codeEditor;
    public TMP_Text countdownText;
    public TitanController redTitan;
    public TitanController blueTitan;

    public GameObject popup;

    private TimerController timerController;

    public GameObject summary;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popup.SetActive(true);
        timerController = gameObject.GetComponent<TimerController>();
    }

    bool IsReady() {
        return redTitan.IsReady() && blueTitan.IsReady();
    }

    // Update is called once per frame
    void Update()
    {
        popup.SetActive(!IsReady());
    }

    // public void StartMatch() {
        // Load player code from UI
        // string redPlayerCode = codeEditor.text;
        // redTitan.LoadSourceCode(redPlayerCode);

        // Load titan code from file
        // TextAsset sourceBlue=(TextAsset)Resources.Load("Blue.lua");
        // blueTitan.LoadSourceCode(sourceBlue.text);

        // TextAsset sourceRed=(TextAsset)Resources.Load("Red.lua");
        // redTitan.LoadSourceCode(sourceRed.text);

        // Run
        // if (IsReady()) {
        //     redTitan.PowerUp();
        //     blueTitan.PowerUp();
        // }
    //     StartCoroutine(StartMatchWithCountdown());
    // }

    private IEnumerator StartMatchWithCountdown()
    {
        redTitan.ActivatePassive();
        blueTitan.ActivatePassive();
        // Display countdown
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }

        // Display "Fight!" or clear the text
        countdownText.text = "Fight!";
        yield return new WaitForSeconds(1.5f); // Show "Fight!" for 1 second
        countdownText.text = ""; // Clear the text

        // Load player code from UI (if any)

        // Load titan code from file (if needed)

        // Run
        // if (IsReady())
        // {
            timerController.StartCountdownFromButton();
            redTitan.PowerUp();
            blueTitan.PowerUp();
            Debug.Log("start??!?!!?!??!?!");
        // }
    }

    public void TimeUp() {
        Debug.Log("Time Up");
        if (redTitan.GetHealth() > blueTitan.GetHealth()) {
            // Win red with health
            redTitan.DeclareWinner();
            blueTitan.PowerDown();
        } else if (redTitan.GetHealth() < blueTitan.GetHealth()) {
            // Win blue with health
            blueTitan.DeclareWinner();
            redTitan.PowerDown();
        } else if (redTitan.GetEnergy() > blueTitan.GetEnergy()) {
            // Win red with energy
            redTitan.DeclareWinner();
            blueTitan.PowerDown();
        } else {
            // Win blue with energy
            blueTitan.DeclareWinner();
            redTitan.PowerDown();
        }
        // Show match summary
        Invoke("ShowSummary", 2f);
    }

    public void ShowSummary() {
        summary.SetActive(true);
    }

    public void StopMatch() {
        // Run
        redTitan.PowerDown();
        blueTitan.PowerDown();
    }

    public void Reload() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // public void Fight() {
    //     StartMatch();
    // }

    public void LoadBlueScript() {
        LoadFile(blueTitan);
        
    }

    public void LoadRedScript() {
        LoadFile(redTitan);
    }
    // public void LoadFile(TitanController target) {
    //     string path = EditorUtility.OpenFilePanel($"Load .lua script for {target.gameObject.tag}", "", "lua");
    //     if (path.Length != 0)
    //     {  
    //         var fileContent = File.ReadAllBytes(path);
    //         string content = Encoding.UTF8.GetString(fileContent);
    //         // texture.LoadImage(fileContent);
    //         target.LoadSourceCode(content);
    //     }
    // }

     public void LoadFile(TitanController target)
    {
        // Configure the file browser filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Lua Files", ".lua"), new FileBrowser.Filter("All Files", "*"));
        FileBrowser.SetDefaultFilter(".lua");

        // Show the file browser
        FileBrowser.ShowLoadDialog(
            (paths) => OnFileSelected(paths, target),    // Success callback
            () => Debug.Log("File selection cancelled"), // Cancel callback
            FileBrowser.PickMode.Files);

        // Note: You cannot set a title explicitly, but you can set context for the user.
        Debug.Log("File dialog opened. Please select a Lua file.");

        // If both are uploaded start automatically
    }

    private void OnFileSelected(string[] paths, TitanController target)
    {
        if (paths.Length > 0)
        {
            string path = paths[0]; // Get the selected file path

            // Read file content
            var fileContent = File.ReadAllBytes(path);
            string content = Encoding.UTF8.GetString(fileContent);

            // Pass the content to your target controller
            target.LoadSourceCode(content);
        }

        if (IsReady()) {
            StartCoroutine(StartMatchWithCountdown());
        }
    }
}
