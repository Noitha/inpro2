using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Menus
{
    public class InGameMenu : Menu
    {
        public Button resumeButton;
        
        public GameObject confirmRestartWindow;
        public Button restartButton;
        public Button confirmYesRestart;
        public Button confirmNoRestart;
        
        public GameObject confirmExitWindow;
        public Button exitButton;
        public Button confirmYesExit;
        public Button confirmNoExit;

        public GameObject gamePlayUi;
        
        protected override void Awake()
        {
            base.Awake();
            resumeButton.onClick.AddListener(Close);
            restartButton.onClick.AddListener(() => confirmRestartWindow.SetActive(true));
            confirmYesRestart.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            confirmNoRestart.onClick.AddListener(() => confirmRestartWindow.SetActive(false));
            exitButton.onClick.AddListener(() => confirmExitWindow.SetActive(true));
            confirmYesExit.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
            confirmNoExit.onClick.AddListener(() => confirmExitWindow.SetActive(false));
        }
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }
            
            window.SetActive(!window.activeInHierarchy);

            Time.timeScale = confirmExitWindow.activeInHierarchy ? 0 : 1;
            gamePlayUi.SetActive(!window.activeInHierarchy);
        }
    }
}