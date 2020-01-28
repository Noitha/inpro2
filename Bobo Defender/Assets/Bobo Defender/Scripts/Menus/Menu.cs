using System.Collections.Generic;
using UnityEngine;

namespace Bobo_Defender.Scripts.Menus
{
    public abstract class Menu : MonoBehaviour
    {
        [Header("Window")]
        public GameObject window;
        
        [Header("Links to other menus")]
        public List<MenuNavigation> menuNavigation = new List<MenuNavigation>();

        public GameObject loadIcon;
        public AudioSource audioSource;
        public AudioClip clickSound;
        
        protected Color okColor = new Color(0.3270834f, 0.8867924f, 0.1631364f, 1);
        protected Color notOkColor = new Color(0.01882343f, 0.02799544f, 0.08490568f, 1);
        
        protected virtual void Awake()
        {
            okColor = new Color(0.3270834f, 0.8867924f, 0.1631364f, 1);
            notOkColor = new Color(0.8490566f, 0.1481844f, 0.1481844f, 1);
            
            foreach (var mn in menuNavigation)
            {
                mn.navigationButton.onClick.AddListener(delegate { GoToMenu(mn.menuToEnable); });
            }
        }

        private void GoToMenu(Menu menuToEnable)
        {
            menuToEnable.window.SetActive(true);
            menuToEnable.EnteredMenu();
            window.SetActive(false);
            audioSource.PlayOneShot(clickSound);
        }

        public void Close()
        {
            window.SetActive(false);
        }
        
        public void Open()
        {
            window.SetActive(true);
            EnteredMenu();
        }

        protected virtual void EnteredMenu() { }
    }
}