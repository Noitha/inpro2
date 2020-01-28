using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts.Gameplay
{
    public class EnemySpawnInformation : MonoBehaviour
    {
        public Image imageEnemyIcon;
        public TextMeshProUGUI textEnemyAmount;

        public void Initialize(Sprite icon, int amount)
        {
            imageEnemyIcon.sprite = icon;
            textEnemyAmount.text = amount.ToString();
        }

        public void UpdateInformation(int newAmount)
        {
            if (newAmount == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                textEnemyAmount.text = newAmount.ToString();
            }
        }
    }
}