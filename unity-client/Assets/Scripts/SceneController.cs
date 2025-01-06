using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadCustomMatch() {
        SceneManager.LoadScene("Main");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Login");
    }
}
