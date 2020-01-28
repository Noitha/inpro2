using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class LevelMenu : Menu
    {
        private int _levelCount;
        private int _currentIndex;

        public Button nextLevel;
        public Button previousLevel;
        public Button playButton;
        
        public TextMeshProUGUI levelIdText;

        public Image crystalOne;
        public Image crystalTwo;
        public Image crystalThree;
        
        public Color crystalClear = new Color(255,255,255,1);
        public Color crystalNotClear = new Color(60,60,60,1);

        protected override void Awake()
        {
            base.Awake();
            nextLevel.onClick.AddListener(NextLevel);
            previousLevel.onClick.AddListener(PreviousLevel);
            playButton.onClick.AddListener(PlayLevel);
        }

        private void Start()
        {
            _levelCount = SceneManager.sceneCountInBuildSettings - 1;
            _currentIndex = 1;
            levelIdText.text = _levelCount.ToString();
        }

        protected override void EnteredMenu()
        {
            StartCoroutine(GetLevelInfo());
            loadIcon.SetActive(false);
        }

        private void PlayLevel()
        {
            SceneManager.LoadScene(_currentIndex);
        }
        
        private void NextLevel()
        {
            _currentIndex++;

            if (_currentIndex > _levelCount)
            {
                _currentIndex = 1;
            }
            
            CheckCompletion();
        }
        private void PreviousLevel()
        {
            _currentIndex--;

            if (_currentIndex <= 0)
            {
                _currentIndex = _levelCount;
            }
            
            CheckCompletion();
        }

        private void CheckCompletion()
        {
            levelIdText.text = _currentIndex.ToString();
            
            var levelExists = GameState.userData.levels.Exists(id => id.id == _currentIndex);
            
            if (levelExists)
            {
                switch (GameState.userData.levels[_currentIndex - 1].stars)
                {
                    case 1:
                        crystalOne.color = crystalClear;
                        crystalTwo.color = crystalNotClear;
                        crystalThree.color = crystalNotClear;
                        break;
                    
                    case 2:
                        crystalOne.color = crystalClear;
                        crystalTwo.color = crystalClear;
                        crystalThree.color = crystalNotClear;
                        break;
                    
                    case 3:
                        crystalOne.color = crystalClear;
                        crystalTwo.color = crystalClear;
                        crystalThree.color = crystalClear;
                        break;
                }
            }
            else
            {
                crystalOne.color = crystalNotClear;
                crystalTwo.color = crystalNotClear;
                crystalThree.color = crystalNotClear;
            }
        }
        
        private IEnumerator GetLevelInfo()
        {
            loadIcon.SetActive(true);
            
            //Create JSON-object.
            var jsonString = new JSONObject(JSONObject.Type.OBJECT);
            jsonString.AddField("username", GameState.userData.username);
            
            //Create form and parse the object as json-string.
            var form = new WWWForm();
            form.AddField("levelInfo", jsonString.ToString());

            //Create a unity-web-request and pass the form.
            var webRequest = UnityWebRequest.Post(GameState.Url + "/getLevelInfo", form);
            
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
                
                GameState.SetNewLevelData(new JSONObject(webRequest.downloadHandler.text));
                CheckCompletion();
            }
        }
    }
}