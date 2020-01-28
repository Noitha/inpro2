using UnityEngine;

namespace Bobo_Defender.Scripts.Gameplay
{
   public class TeleportPlayer : MonoBehaviour
   {
      private void OnTriggerEnter2D(Collider2D other)
      {
         if (!other.CompareTag("Player"))
         {
            return;
         }
         
         other.transform.position = new Vector2(Random.Range(-18,18), Random.Range(-10,10));
      }
   }
}
