using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bobo_Defender.Scripts
{
    public class Crystal : MonoBehaviour
    {
        public int health;
        public Slider healthSlider;
        public TextMeshProUGUI textDisplay;


        private void Start()
        {
            health = 100;
            
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100;
            healthSlider.value = 100;

            textDisplay.text = "100";
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("Enemy"))
            {
                return;
            }

            var enemy = other.GetComponent<Enemy>();

            health -= enemy.enemyData.damage;
            
            if (health <= 0)
            {
                Destroy(gameObject);
            }

            healthSlider.value = health;
            textDisplay.text = health + "/100";
            
            Destroy(enemy.gameObject);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}