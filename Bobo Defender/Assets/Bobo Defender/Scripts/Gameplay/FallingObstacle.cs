using UnityEngine;
using Random = UnityEngine.Random;

namespace Bobo_Defender.Scripts.Gameplay
{
    public class FallingObstacle : MonoBehaviour
    {
        public GameObject obstacle;
        public float retryTrigger;
        public float timer;
        
        private void Start()
        {
            timer = retryTrigger;
        }

        private void Update()
        {
            if (retryTrigger > 0)
            {
                retryTrigger -= Time.deltaTime;
                return;
            }

            if (Random.Range(0, 5) > 2)
            {
                Instantiate(obstacle,transform.position, Quaternion.identity);
            }

            retryTrigger = timer;
        }
    }
}