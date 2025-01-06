using System;
using System.Collections; // Required for IEnumerator
using System.Linq; // For LINQ operations
using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest
using UnityEngine.UI; 
using TMPro;

public class LoginManager : MonoBehaviour
{

    public string serverUrl = "https://titanbrawl.vercel.app";
    public string clientId = "Iv23liwymIEMU8MEwYub";

    public Image avatar;
    public TMP_Text username;
    public GameObject loginButton;
    public GameObject logoutButton;
    public GameObject userContainer;

    public GameObject options;

    private bool isLoggedIn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Authenticate() {
        string uniqueSessionId = Guid.NewGuid().ToString(); // Generate a unique session ID
        // Save the session ID locally in Unity to track it
        PlayerPrefs.SetString("session_id", uniqueSessionId);
        string redirectUri = $"{serverUrl}/callback";
        string url = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=read:user&state={uniqueSessionId}";
        Application.OpenURL(url);
    }

    public void OnLogoutUser()
    {
        Debug.Log("User logging out...");
        StartCoroutine(Logout());
    }

    private IEnumerator Logout()
    {
        string sessionId = PlayerPrefs.GetString("session_id", null);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("No session ID found. Cannot log out.");
            yield break;
        }

        string url = $"{serverUrl}/logout";
        WWWForm form = new WWWForm();
        form.AddField("sessionId", sessionId);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Logout successful!");
            
            ClearUserData();
        }
        else
        {
            Debug.LogError($"Logout failed: {request.error}");
            UpdateButtonVisibility();
        }
    }
    
        private void UpdateButtonVisibility()
    {
        loginButton.gameObject.SetActive(!isLoggedIn);
        logoutButton.gameObject.SetActive(isLoggedIn);
        // avatar.gameObject.transform.parent.gameObject.SetActive(isLoggedIn);
        // username.gameObject.SetActive(isLoggedIn);
        userContainer.SetActive(isLoggedIn);
        options.SetActive(isLoggedIn);
    }

    void ClearUserData() {
                    // Clear session ID and update UI
            PlayerPrefs.DeleteKey("session_id");
            isLoggedIn = false;
            UpdateButtonVisibility();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("Has Focus");
            // Check if the session ID is saved (user completed OAuth flow)
            string sessionId = PlayerPrefs.GetString("session_id", null);
            Debug.Log(sessionId);
            if (!string.IsNullOrEmpty(sessionId))
            {
                // Trigger FetchUserInfo automatically
                StartCoroutine(QueryAuthServer(sessionId));
            }
            else
            {
                Debug.Log("Session ID not found. Ensure authentication flow was started.");
                ClearUserData();
            }
        }
    }

    public void FetchUserInfo()
    {
        // Retrieve the session ID saved earlier
        string sessionId = PlayerPrefs.GetString("session_id", null);

        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("Session ID not found. Did you start authentication?");
            return;
        }

        // Start the coroutine to query the backend
        StartCoroutine(QueryAuthServer(sessionId));
    }

    IEnumerator QueryAuthServer(string sessionId)
    {
        string url = $"{serverUrl}/getUserInfo?sessionId={sessionId}";
        Debug.Log($"querying {url}");
        // Create a UnityWebRequest to send the GET request
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Wait for the request to complete
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the JSON response from the backend
            string response = request.downloadHandler.text;
            Debug.Log("User Info: " + response);

            var userData = JsonUtility.FromJson<UserResponse>(response);
            Debug.Log($"Welcome, {userData.user.login}!");
            LoadImageFromUrl(userData.user.avatar_url);
            if (userData.user.login.Length > 11)
            {
                username.text = userData.user.login.Substring(0, 11) + "...";
            }
            else
            {
                username.text = userData.user.login;
            }
            isLoggedIn = userData.user.login != null ? true : false;
            UpdateButtonVisibility();
        }
        else
        {
            // Log an error if the request fails
            Debug.LogError("Failed to fetch user info: " + request.error);
            UpdateButtonVisibility();
        }
    }

    // Define a class to deserialize the backend's JSON response
    [System.Serializable]
    public class UserResponse
    {
        public User user;

        [System.Serializable]
        public class User
        {
            public string login;
            public string name;
            public string avatar_url;
        }
    }

    public void LoadImageFromUrl(string url)
        {
            StartCoroutine(LoadImage(url));
        }

        private IEnumerator LoadImage(string url)
        {
            // Use UnityWebRequest to fetch the texture from the URL
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Convert the downloaded texture into a Sprite
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = TextureToSprite(texture);

                // Assign the Sprite to the Image component
                avatar.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Failed to load texture from URL: {request.error}");
            }
        }


    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
}
