using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class RegisterMenu : Menu
    {
        [Header("Input fields")]
        public TMP_InputField inputEmailField;
        public TMP_InputField inputUsernameField;
        public TMP_InputField inputPasswordField;
        public TMP_InputField inputPasswordSecondField;
        
        [Header("Buttons")]
        public Button buttonRegister;

        [Header("Messages")]
        public TextMeshProUGUI textRegisterMessage;
        public TextMeshProUGUI textUsernameMessage;
        public TextMeshProUGUI textEmailMessage;
        public TextMeshProUGUI textPasswordMessage;
        public TextMeshProUGUI textPasswordRetypeMessage;

        protected override void Awake()
        {
            base.Awake();
            buttonRegister.onClick.AddListener(delegate { StartCoroutine(SendRegisterForm()); });
        }
        private void Start()
        {
            inputUsernameField.contentType = TMP_InputField.ContentType.Alphanumeric;
            inputEmailField.contentType = TMP_InputField.ContentType.EmailAddress;
            inputPasswordField.contentType = TMP_InputField.ContentType.Password;
            inputPasswordSecondField.contentType = TMP_InputField.ContentType.Password;
            ResetForm();
        }

        protected override void EnteredMenu()
        {
            textRegisterMessage.text = "";
            textUsernameMessage.text = "";
            textEmailMessage.text = "";
            textPasswordMessage.text = "";
            textPasswordRetypeMessage.text = "";

            inputEmailField.text = "";
            inputUsernameField.text = "";
            inputPasswordField.text = "";
            inputPasswordSecondField.text = "";
            
            loadIcon.SetActive(false);
        }

        private IEnumerator SendRegisterForm()
        {
            audioSource.PlayOneShot(clickSound);
            loadIcon.SetActive(true);
            
            //Disable the submit-button while processing.
            buttonRegister.interactable = false;
            
            //Create object with register-properties.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", inputUsernameField.text);
            jsonString.AddField("email", inputEmailField.text);
            jsonString.AddField("password", inputPasswordField.text);
            jsonString.AddField("passwordRetype", inputPasswordSecondField.text);
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("registerData", jsonString.ToString());

            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/register", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            //Check for any error.
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                textRegisterMessage.text = "Registration failed";
                textRegisterMessage.color = notOkColor;
                
                //Re-enable the register-button.
                buttonRegister.interactable = true;
                
                loadIcon.SetActive(false);
            }
            else
            {
                var responseObject = new JSONObject(webRequest.downloadHandler.text);
                
                responseObject.GetField("email", delegate(JSONObject email) 
                {
                    email.GetField(out var emailIsValid, "isValid",false);
                    email.GetField(out var emailMessage, "message", "");
                    
                    textEmailMessage.text = emailMessage;
                    textEmailMessage.color = emailIsValid ? okColor : notOkColor;
                });
                
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
                
                responseObject.GetField("passwordRetype", delegate(JSONObject passwordRetype) 
                {
                    passwordRetype.GetField(out var passwordRetypeIsValid, "isValid", false);
                    passwordRetype.GetField(out var passwordRetypeMessage, "message", "");
                    
                    textPasswordRetypeMessage.text = passwordRetypeMessage;
                    textPasswordRetypeMessage.color = passwordRetypeIsValid ? okColor : notOkColor;
                });
                
                responseObject.GetField(out var successful, "successful", false);

                if (successful)
                {
                    textRegisterMessage.text = "Registration successful";
                    textRegisterMessage.color = okColor;
                    
                    PlayerPrefs.SetString("Username", inputUsernameField.text);
                    PlayerPrefs.Save();
                    
                    ResetForm();
                }
                else
                {
                    textRegisterMessage.text = "Registration failed";
                    textRegisterMessage.color = notOkColor;
                }
            }
            
            //Re-enable the register-button.
            buttonRegister.interactable = true;
            
            loadIcon.SetActive(false);
        }

        private void ResetForm()
        {
            inputUsernameField.text = "";
            inputEmailField.text = "";
            inputPasswordField.text = "";
            inputPasswordSecondField.text = "";
            
            textUsernameMessage.text = "";
            textEmailMessage.text = "";
            textPasswordMessage.text = "";
            textPasswordRetypeMessage.text = "";
        }
    }
}