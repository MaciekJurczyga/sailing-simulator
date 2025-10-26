using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class LoginManager : MonoBehaviour
{
    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI feedbackText; 
    
    public string mainSceneName = "Sailboat"; 

    void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
    }

    private void HandleLogin()
    {
        String passwordHash = "fn9rzfGELzwC0NJ+3UWL031j0F+8B4A3od5TEI0Xt98=";
        String loginHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=";
        string login = loginInput.text;
        string password = passwordInput.text;
        
        if (Sha256(login) == loginHash && Sha256(password) == passwordHash)
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Logowanie pomyślne";
                feedbackText.color = Color.green;
            }
            
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            passwordInput.text = "";
            loginInput.text = "";
            if (feedbackText != null)
            {
                feedbackText.text = "Niepoprawny login lub hasło";
                feedbackText.color = Color.red;
            }
        }
    }
    private static string Sha256(string s)
    {
        using (var sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
    
}