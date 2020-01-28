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
        private Crystal _targetPosition;
        private Seeker _seeker;
        private Path _path;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidBody2D;
        public EnemyData enemyData;
        
        private float _nextWayPointDistance;
        private int _currentWayPoint;
        private bool _reachedEndOfPath;
        private bool _ready;

        private void Awake()
        {
            tag = "Enemy";
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = 0;
            _spriteRenderer.sortingLayerName = "Enemy";
            
            _seeker = GetComponent<Seeker>();
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _ready = false;
        }

        public void SetData(EnemyData enemyData, Crystal crystal)
        {
            this.enemyData = enemyData;
            _targetPosition = crystal;
            _seeker.StartPath(transform.position, _targetPosition.transform.position, OnPathComplete);
            _spriteRenderer.sprite = enemyData.icon;
            _ready = true;
        }

        public void Update()
        {
            if (!_ready || _path == null)
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