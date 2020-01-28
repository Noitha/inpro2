using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class EndGameMenu : Menu
    {
        public Button continueButton;

        public TextMeshProUGUI winText;
        public TextMeshProUGUI skinText;
        public Image newSkin;

        public Image crystalImageOne;
        public Image crystalImageTwo;
        public Image crystalImageThree;

        public TextMeshProUGUI crystalHealthOne;
        public TextMeshProUGUI crystalHealthTwo;
        public TextMeshProUGUI crystalHealthThree;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        }


        public void DisplayWinState(bool won, int id, int[] crystalHealth)
        {
            window.SetActive(true);

            skinText.gameObject.SetActive(won);
            newSkin.gameObject.SetActive(won);

            newSkin.sprite = won && id != 0
                ? Resources.Load<Sprite>("Sprites/Skins/" + id)
                : Resources.Load<Sprite>("Sprites/Skins/" + 12);

            winText.text = won ? "Well Done!" : "Next time will be better!";
            skinText.text = id != 0 ? "New skin unlocked" : "Sadly no new skin for you";

            crystalHealthOne.text = crystalHealth[0].ToString();
            crystalHealthTwo.text = crystalHealth[1].ToString();
            crystalHealthThree.text = crystalHealth[2].ToString();

            crystalImageOne.fillAmount = crystalHealth[0] / 100f;
            crystalImageTwo.fillAmount = crystalHealth[1] / 100f;
            crystalImageThree.fillAmount = crystalHealth[2] / 100f;

            StartCoroutine(SaveLevel(crystalHealth, id));
        }

        private IEnumerator SaveLevel(int[] crystalHealth, int skinId)
        {
            var crystalCount = crystalHealth.Count(health => health > 0);

            //Create object with login-properties.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", GameState.userData.username);
            jsonString.AddField("levelId", SceneManager.GetActiveScene().name);
            jsonString.AddField("newCrystalScore", crystalCount);
            jsonString.AddField("skinId", skinId);

            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("updateLevelScore", jsonString.ToString());

            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/updateLevelScore", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Something went wrong.");
            }
            else
            {
                if (webRequest.downloadHandler.text == "")
                {
                    Debug.Log("Error occured.");
                    yield break;
                }

                GameState.SetUserData(new JSONObject(webRequest.downloadHandler.text));
            }
        }
    }
}