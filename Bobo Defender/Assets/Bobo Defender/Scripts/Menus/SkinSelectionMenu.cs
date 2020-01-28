using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class SkinSelectionMenu : Menu
    {
        public List<Sprite> skins = new List<Sprite>();
        private int _currentIndex;
        
        public Image image;
        public Button previousSkin;
        public Button nextSkin;
        public Button selectSkin;
        public Image selectSkinImage;

        public TextMeshProUGUI selectButtonText;

        public Color unLockedColor = new Color(217,38,38,1);
        public Color lockedOrActiveColor = new Color(79,72,72,1);
        
        protected override void Awake()
        {
            base.Awake();
            previousSkin.onClick.AddListener(PreviousSkin);
            nextSkin.onClick.AddListener(NextSkin);
            selectSkin.onClick.AddListener(() => StartCoroutine(SelectSkin()));
        }
        
        private void Start()
        {
            _currentIndex = 1;
            image.sprite = skins[0];
        }

        protected override void EnteredMenu()
        {
            _currentIndex = GameState.userData.activeSkinId;
            image.sprite = skins[GameState.userData.activeSkinId - 1];
            CheckAvailability();
            loadIcon.SetActive(false);
        }
        
        
        private void NextSkin()
        {
            _currentIndex++;

            if (_currentIndex == skins.Count + 1)
            {
                _currentIndex = 1;
            }
            
            image.sprite = skins[_currentIndex - 1];
            CheckAvailability();
            audioSource.PlayOneShot(clickSound);
        }

        private void PreviousSkin()
        {
            _currentIndex--;

            if (_currentIndex == 0)
            {
                _currentIndex = skins.Count;
            }
            
            image.sprite = skins[_currentIndex - 1];
            CheckAvailability();
            audioSource.PlayOneShot(clickSound);
        }


        private void CheckAvailability()
        {
            if (_currentIndex == GameState.userData.activeSkinId)
            {
                selectSkin.interactable = false;
                selectSkinImage.color = lockedOrActiveColor;
                selectButtonText.text = "Active";
                return;
            }
            
            if (GameState.userData.unlockedSkins.Exists(unlockedSkin => unlockedSkin == _currentIndex))
            {
                selectSkin.interactable = true;
                selectSkinImage.color = unLockedColor;
                selectButtonText.text = "Select";
            }
            else
            {
                selectSkin.interactable = false;
                selectSkinImage.color = lockedOrActiveColor;
                selectButtonText.text = "Locked";
            }
        }
        
        
        private IEnumerator SelectSkin()
        {
            loadIcon.SetActive(true);
            
            //Create JSON-object.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", GameState.userData.username);
            jsonString.AddField("activeSkinId", _currentIndex);
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("changeSkin", jsonString.ToString());
            
            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/changeSkin", form);
            
            //Wait for the response.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.downloadHandler.text == "")
            {
                Debug.Log("Something went wrong.");
            }
            else
            {
                if (!bool.Parse(webRequest.downloadHandler.text))
                {
                    yield break;
                }

                GameState.SetNewActiveSkin(_currentIndex);
                CheckAvailability();
            }
            
            loadIcon.SetActive(false);
        }
    }
}