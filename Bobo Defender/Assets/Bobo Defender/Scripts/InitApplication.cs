using System.Collections;
using Bobo_Defender.Scripts.Menus;
using UnityEngine;
using UnityEngine.Networking;

namespace Bobo_Defender.Scripts
{
    public class InitApplication : MonoBehaviour
    {
        public LoginMenu loginMenu;
        public RegisterMenu registerMenu;
        public MainMenu mainMenu; 
        public SkinSelectionMenu skinSelectionMenu;
        public LevelMenu levelMenu;
        public GameObject loadWindow;
        
        private void Start()
        {
            //Close all the menus.
            loginMenu.Close();
            registerMenu.Close();
            mainMenu.Close();
            skinSelectionMenu.Close();
            levelMenu.Close();
            
            //Check if the username-entry exists.
            if(PlayerPrefs.HasKey("Username"))
            {
                StartCoroutine(ValidateToken());
            }
            else
            {
                loginMenu.Open();
            }
        }

        private IEnumerator ValidateToken()
        {
            loadWindow.SetActive(true);
            
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", PlayerPrefs.GetString("Username"));
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("username", jsonString.ToString());
            
            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/validateToken", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            loadWindow.SetActive(false);
            
            if (webRequest.downloadHandler.text == "")
            {
                loginMenu.Open();
            }
            else
            {
                GameState.SetUserData(new JSONObject(webRequest.downloadHandler.text));
                mainMenu.Open();
            }
        }
    }
}