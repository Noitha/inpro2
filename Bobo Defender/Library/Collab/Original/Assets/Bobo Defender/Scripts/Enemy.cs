using Pathfinding;
using UnityEngine;

namespace Bobo_Defender.Scripts
{
    [RequireComponent
    (
        typeof(Rigidbody2D), 
        typeof(BoxCollider2D), 
        typeof(Seeker)
    )]
    public class Enemy : MonoBehaviour
    {
        private Seeker _seeker;
        private Path _path;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidBody2D;
        public EnemyData enemyData;
        
        private float _nextWayPointDistance = 3f;
        private int _currentWayPoint;
        private bool _reachedEndOfPath;
        public bool ready;

        private void Awake()
        {
            tag = "Enemy";
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = 0;
            _spriteRenderer.sortingLayerName = "Enemy";

            _seeker = GetComponent<Seeker>();
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public void SetData(EnemyData enemyData, Crystal crystal)
        {
            this.enemyData = enemyData;
            _seeker.StartPath(transform.position, crystal.GetPosition(), OnPathComplete);
            _spriteRenderer.sprite = enemyData.icon;
            ready = true;
        }

        public void Update()
        {
            if (!ready || _path == null)
            {
                return;
            }

            _reachedEndOfPath = false;
            
            float distanceToWayPoint;
            while (true)
            {
                distanceToWayPoint = Vector3.Distance(transform.position, _path.vectorPath[_currentWayPoint]);
                if (distanceToWayPoint < _nextWayPointDistance)
                {
                    if (_currentWayPoint + 1 < _path.vectorPath.Count)
                    {
                        _currentWayPoint++;
                    }
                    else
                    {
                        _reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            
            var speedFactor = _reachedEndOfPath ? Mathf.Sqrt(distanceToWayPoint / _nextWayPointDistance) : 1f;
            var dir = (_path.vectorPath[_currentWayPoint] - transform.position).normalized;
            var velocity = speedFactor * enemyData.speed * dir;
            
            transform.Translate(velocity * Time.deltaTime);
        }
        private void OnPathComplete(Path path)
        {
            if (path.error)
            {
                return;
            }
            
            _path = path;

            _currentWayPoint = 0;
        }
    }
}