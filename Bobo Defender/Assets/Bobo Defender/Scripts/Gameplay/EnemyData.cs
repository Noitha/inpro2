using UnityEngine;

namespace Bobo_Defender.Scripts.Gameplay
{
    [CreateAssetMenu(menuName = "Enemy/Create Enemy", fileName = "New Enemy")]
    public class EnemyData : ScriptableObject
    {
        public float speed;
        public int damage;
        public float distanceKillRequirement;
        public AudioClip deadSound;
    }
}