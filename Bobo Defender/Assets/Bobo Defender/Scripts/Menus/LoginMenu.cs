using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class LoginMenu : Menu
    {
        [Header("Input fields")]
        public TMP_InputField inputUsername;
        public TMP_InputField inputPassword;
        
        [Header("Buttons")]
        public Button buttonLogin;
        
        [Header("Messages")]
        public TextMeshProUGUI loginMessage;
        public TextMeshProUGUI textUsernameMessage;
        public TextMeshProUGUI textPasswordMessage;
        
        public MainMenu mainMenu;

        protected override void Awake()
        {
            base.Awake();
            buttonLogin.onClick.AddListener(delegate { StartCoroutine(SendLoginForm()); });
        }

        private void Start()
        {
            inputUsername.contentType = TMP_InputField.ContentType.Standard;
            inputPassword.contentType = TMP_InputField.ContentType.Password;
        }
        

        protected override void EnteredMenu()
        {
            loginMessage.text = "";
            textUsernameMessage.text = "";
            textPasswordMessage.text = "";

            inputUsername.text = "";
            inputPassword.text = "";
            
            loadIcon.SetActive(false);
        }

        private IEnumerator SendLoginForm()
        {
            audioSource.PlayOneShot(clickSound);
            loadIcon.SetActive(true);
            
            //Disable the submit-button while processing.
            buttonLogin.interactable = false;
            
            //Create object with login-properties.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", inputUsername.text);
            jsonString.AddField("password", inputPassword.text);
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("loginData", jsonString.ToString());

            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/login", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            //Check for any error.
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                loginMessage.text = "Login failed";
                loginMessage.color = notOkColor;
                
                //Re-enable the login-button.
                buttonLogin.interactable = true;
                
                loadIcon.SetActive(false);
            }
            else
            {
                var responseObject = new JSONObject(webRequest.downloadHandler.text);
                
                responseObject.GetField("username", delegate(JSONObject username) 
                {
                    username.GetField(out var usernameIsValid, "isValid", false);
                    username.GetField(out var usernameMessage, "message", "");

                    textUsernameMessage.text = usernameMessage;
                    textUsernameMessage.color = usernameIsValid ? okColor : notOkColor;
                });
                
                responseObject.GetField("password", delegate(JSONObject password) 
                {
                    password.GetField(out var passwordIsValid, "isValid", false);
                    password.GetField(out var passwordMessage, "message", "");
                    
                    textPasswordMessage.text = passwordMessage;
                    textPasswordMessage.color = passwordIsValid ? okColor : notOkColor;
                });
                
                responseObject.GetField(out var successful, "successful", false);

                var userData = new JSONObject();
                responseObject.GetField("userData", delegate(JSONObject userDataa) { userData = userDataa; }); 

                if (successful)
                {
                    loginMessage.text = "Login successful";
                    loginMessage.color = okColor;
                    PlayerPrefs.SetString("Username", inputUsername.text);
                    PlayerPrefs.Save();
                    GameState.SetUserData(userData);
                    Close();
                    mainMenu.Open();
                }
                else
                {
                    loginMessage.text = "Login failed";
                    loginMessage.color = notOkColor;
                }
            }
            
            //Re-enable the login-button.
            buttonLogin.interactable = true;
            
            loadIcon.SetActive(false);
        }
    }
}