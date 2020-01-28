using System.Collections;
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
        public Image crystalBeingAttackedIndicator;
        
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
            
            crystalBeingAttackedIndicator.gameObject.SetActive(true);
            
            var diff = crystalBeingAttackedIndicator.transform.position - transform.position;
            diff.Normalize();
            
            StopCoroutine(DisableIndicator());
            crystalBeingAttackedIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 180);
            StartCoroutine(DisableIndicator());
            
            if (health <= 0)
            {
                Destroy(gameObject);
            }

            healthSlider.value = health;
            textDisplay.text = health + "/100";
            
            Destroy(enemy.gameObject);
        }

        private IEnumerator DisableIndicator()
        {
            yield return new WaitForSeconds(3);
            crystalBeingAttackedIndicator.gameObject.SetActive(false);
        }
    }
}