using System.Globalization;
using Pathfinding;
using TMPro;
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
        public EnemyData enemyData;
        public Spawner originSpawner;

        public TextMeshProUGUI distanceRequirementDisplay;
        public TextMeshProUGUI damageDisplay;
        
        private float _nextWayPointDistance = 2;
        private int _currentWayPoint;
        private bool _reachedEndOfPath;
        private bool _ready;

        private void Awake()
        {
            tag = "Enemy";

            _seeker = GetComponent<Seeker>();
        }

        private void Start()
        {
            distanceRequirementDisplay.text = enemyData.distanceKillRequirement.ToString(new CultureInfo(""));
            damageDisplay.text = enemyData.damage.ToString();
        }

        public void SetData(EnemyData ed, Crystal crystal, Spawner os)
        {
            _ready = false;
            enemyData = ed;
            originSpawner = os;
            _targetPosition = crystal;
            _seeker.StartPath(transform.position, _targetPosition.transform.position, OnPathComplete);
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