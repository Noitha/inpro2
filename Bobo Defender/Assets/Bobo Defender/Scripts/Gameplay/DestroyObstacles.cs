using UnityEngine;

namespace Bobo_Defender.Scripts.Gameplay
{
    public class DestroyObstacles : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Obstacle"))
            {
                return;
            }
            Destroy(other.gameObject,1);
        }
    }
}