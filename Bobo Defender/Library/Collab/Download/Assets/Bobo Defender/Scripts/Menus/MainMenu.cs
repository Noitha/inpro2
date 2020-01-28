using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class MainMenu : Menu
    {
        public TextMeshProUGUI welcomeText;

        public LoginMenu loginMenu;
        public Button logoutButton;

        protected override void Awake()
        {
            base.Awake();
            logoutButton.onClick.AddListener(() => StartCoroutine(Logout()));
        }

        protected override void EnteredMenu()
        {
            welcomeText.text = "Welcome " + GameState.userData.username;
            loadIcon.SetActive(false);
        }

        private IEnumerator Logout()
        {
            audioSource.PlayOneShot(clickSound);
            loadIcon.SetActive(true);
            
            //Create object with username-field.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", GameState.userData.username);
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("logout", jsonString.ToString());
            
            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/logout", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            //Check for any error.
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                if (!bool.Parse(webRequest.downloadHandler.text)) yield break;
                
                Close();
                loginMenu.Open();
                GameState.ClearUserData();
            }
            
            loadIcon.SetActive(false);
        }
    }
}