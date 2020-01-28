using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bobo_Defender.Scripts
{
    public class Spawner : MonoBehaviour
    {
        public List<SpawnData> spawnData = new List<SpawnData>();
        private readonly Queue<SpawnData> _queue = new Queue<SpawnData>();
        private float _currentTime;
        private SpawnData _currentSpawnData;
        private bool _empty = true;

        public EnemySpawnInformation enemySpawnInformationPrefab;
        public Transform enemySpawnInformationContainer;

        public Enemy enemyPrefab;
        
        public Transform spawnPosition;
        public Transform enemyContainer;
        
        [Serializable]
        public class SpawnData
        {
            public EnemyData enemyData;
            public int amount;
            public float delay;
            public Crystal target;
            public EnemySpawnInformation enemySpawnInformation;

            public SpawnData(EnemyData enemyData, int amount, float delay, Crystal target)
            {
                this.enemyData = enemyData;
                this.amount = amount;
                this.delay = delay;
                this.target = target;
            }
        }
        
        private void Start()
        {
            _empty = false;
            _currentTime = 0;
            
            foreach (var data in spawnData)
            {
               /* var enemySpawnInformation = Instantiate(enemySpawnInformationPrefab, enemySpawnInformationContainer);
                enemySpawnInformation.Initialize(data.enemyData.icon, data.amount);
                
                data.enemySpawnInformation = enemySpawnInformation;*/
                
                _queue.Enqueue(data);
            }

            _currentSpawnData = null;
        }

        private void Update()
        {
            if (_empty)
            {
                return;
            }
            
            //Increase time.
            _currentTime += Time.deltaTime;

            //Get new data from queue.
            if (_currentSpawnData == null)
            {
                _empty = !NextDataInQueue(out _currentSpawnData);
            }
            //Get new data from queue.
            else if (_currentSpawnData.amount == 0)
            {
                _empty = !NextDataInQueue(out _currentSpawnData);
            }
            //Get current data from queue.
            else
            {
                if (!(_currentTime >= _currentSpawnData.delay))
                {
                    return;
                }
                
                var enemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity, enemyContainer);
                enemy.ready = false;
                enemy.SetData(_currentSpawnData.enemyData, _currentSpawnData.target);
                
                _currentTime = 0;
                _currentSpawnData.amount--;
                
                //_currentSpawnData.enemySpawnInformation.UpdateInformation(_currentSpawnData.amount);
            }
        }
        
        private bool NextDataInQueue(out SpawnData data)
        {
            if(_queue.Count == 0)
            {
                data = new SpawnData(null, 0, 0, null);
                return false;
            }

            data = _queue.Dequeue();
            return true;
        }
    }
}