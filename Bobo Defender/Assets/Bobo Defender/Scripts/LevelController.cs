using Bobo_Defender.Scripts.Gameplay;
using Bobo_Defender.Scripts.Menus;
using UnityEngine;

namespace Bobo_Defender.Scripts
{
    public class LevelController : MonoBehaviour
    {
        public GameObject inGameMenu;
        public EndGameMenu endGameMenu;

        public Spawner[] spawners;
        public Crystal[] crystals;
        
        
        private void Start()
        {
            inGameMenu.SetActive(false);
            endGameMenu.Close();
        }
        
        public void CheckWinCondition()
        {
            var allCrystalsDestroyed = true;
            foreach (var crystal in crystals)
            {
                if (crystal.health > 0)
                {
                    allCrystalsDestroyed = false;
                    break;
                }
            }

            if (allCrystalsDestroyed)
            {
                EndLevel(true);
            }
            
            foreach (var spawner in spawners)
            {
                if (spawner.enemyAmount > 0)
                {
                    return;
                }
            }
            
            EndLevel(true);
        }
        
        private void EndLevel(bool won)
        {
            var unlockedSkin = 0;

            if (won)
            {
                var tryLimit = 5;
                while (tryLimit > 0)
                {
                    var randomNumber = Random.Range(0, 30);
                    
                    if (GameState.userData.unlockedSkins.Exists(id => id == randomNumber))
                    {
                        tryLimit--;
                        continue;
                    }

                    unlockedSkin = randomNumber;
                    break;
                }
            }

            endGameMenu.DisplayWinState(won, unlockedSkin, new []{crystals[0].health, crystals[1].health, crystals[2].health});
        }
    }
}