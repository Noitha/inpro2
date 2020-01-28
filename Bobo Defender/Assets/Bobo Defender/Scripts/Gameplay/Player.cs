using System.Globalization;
using TMPro;
using UnityEngine;

namespace Bobo_Defender.Scripts.Gameplay
{
    public class Player : MonoBehaviour
    {
        //Components
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidBody2D;
        private CircleCollider2D _circleCollider2D;
        private AudioSource _playerAudioSource;
        public AudioSource chargeAudioSource;
        
        public GameObject[] crystalBeingAttackIndicators;
        public Transform entireChargeContainer;
        public Transform activeChargeImageContainer;
        public GameObject inActiveChargeImageContainer;
        public SpriteRenderer[] chargeImages;
        public Vector2 releaseChargePosition;
        public Transform lastReleasePositionDisplay;
        public TextMeshProUGUI currentDistanceDisplay;
        
        private int _currentChargeLevel;
        private float _zRotation;
        private const int ReleaseForce = 230;
        private const int MaxVelocity = 40;
        private const float SlowRepeatTime = 0.15f;
        private bool _maxReached;
        private float _timer;
        
        public AudioClip releaseSound;
        public AudioClip collideWallSound;
        public AudioClip chargeSound;
        
        
        #region Unity Methods
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidBody2D = GetComponent<Rigidbody2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _playerAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _spriteRenderer.sortingLayerName = "Player";
            _spriteRenderer.sortingOrder = 0;
            
            _rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigidBody2D.gravityScale = 0;
            _rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidBody2D.freezeRotation = true;

            _circleCollider2D.radius = 1f;
            
            chargeImages = new SpriteRenderer[activeChargeImageContainer.childCount];
            
            for (var i = 0; i < activeChargeImageContainer.childCount; i++)
            {
                chargeImages[i] = activeChargeImageContainer.GetChild(i).GetComponent<SpriteRenderer>();
                chargeImages[i].gameObject.SetActive(false);
            }
            
            foreach (var indicator in crystalBeingAttackIndicators)
            {
                indicator.SetActive(false);
            }
            
            SetSkin();
            InvokeRepeating(nameof(PointDirection), 0, .05f);
            
            releaseChargePosition = transform.position;
            lastReleasePositionDisplay.position = releaseChargePosition;
            lastReleasePositionDisplay.gameObject.SetActive(true);
        }


        private void Update()
        {
            currentDistanceDisplay.text = Mathf.FloorToInt(Vector2.Distance(releaseChargePosition, transform.position)).ToString(new CultureInfo(""));
            
            if (Input.touchCount > 0)
            {
                _timer += Time.deltaTime;
                Charge(_timer);
                chargeAudioSource.clip = chargeSound;
                if (!chargeAudioSource.isPlaying && !_maxReached)
                {
                    chargeAudioSource.Play();
                }
                return;
            }
            
            if (!(_timer > 0))
            {
                return;
            }
            
            ReleaseCharge();
            _timer = 0;
            _maxReached = false;
            chargeAudioSource.Stop();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Enemy"))
            {
                return;
            }
            
            var enemy = other.gameObject.GetComponent<Enemy>();
            var distance = Vector2.Distance(releaseChargePosition, enemy.transform.position);
            
            if (distance >= enemy.enemyData.distanceKillRequirement)
            {
                enemy.Kill();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Collision"))
            {
                _playerAudioSource.PlayOneShot(collideWallSound);
            }
        }

        #endregion
        
        private void PointDirection()
        {
            _zRotation = Mathf.Atan2(Input.acceleration.y, Input.acceleration.x) * Mathf.Rad2Deg;
            entireChargeContainer.localEulerAngles = new Vector3(0, 0,  (int)_zRotation);
        }

        private void ReleaseCharge()
        {
            _playerAudioSource.PlayOneShot(releaseSound);
            inActiveChargeImageContainer.SetActive(true);

            CancelInvoke(nameof(BrakeSlowly));
            
            foreach (var chargeImage in chargeImages)
            {
                chargeImage.gameObject.SetActive(false);
            }
            
            var currentPlayerTransform = transform;
            
            releaseChargePosition = currentPlayerTransform.position;
            lastReleasePositionDisplay.position = releaseChargePosition;
            lastReleasePositionDisplay.gameObject.SetActive(true);
            
            _rigidBody2D.AddForce(entireChargeContainer.right * (ReleaseForce * _currentChargeLevel));
            
            InvokeRepeating(nameof(BrakeSlowly), 0, SlowRepeatTime);
        }

        private void Charge(float currentTime)
        {
            inActiveChargeImageContainer.SetActive(false);
            _currentChargeLevel = Mathf.Clamp(Mathf.FloorToInt(currentTime / 0.05f), 0, 10);

            for (var i = 0; i < _currentChargeLevel; i++)
            {
                chargeImages[i].gameObject.SetActive(true);
            }

            if (_currentChargeLevel >= chargeImages.Length)
            {
                _maxReached = true;
                chargeAudioSource.Stop();
            }
        }
        
        private void BrakeSlowly()
        {
            var currentVelocity = _rigidBody2D.velocity;

            if (Mathf.Abs(currentVelocity.x) > MaxVelocity)
            {
                if (currentVelocity.x > MaxVelocity)
                {
                    currentVelocity.x = MaxVelocity;
                }
                else
                {
                    currentVelocity.x = -MaxVelocity;
                }
            }
            
            if (Mathf.Abs(currentVelocity.y) > MaxVelocity)
            {
                if (currentVelocity.y > MaxVelocity)
                {
                    currentVelocity.y = MaxVelocity;
                }
                else
                {
                    currentVelocity.y = -MaxVelocity;
                }
            }
            
            if (currentVelocity.x - 0.5f >= 0)
            {
                currentVelocity.x -= .5f;
            }

            if (currentVelocity.x + 0.5f <= 0)
            {
                currentVelocity.x += .5f;
            }

            if (currentVelocity.y - 0.5f >= 0)
            {
                currentVelocity.y -= .5f;
            }

            if (currentVelocity.y + 0.5f <= 0)
            {
                currentVelocity.y += .5f;
            }
            
            _rigidBody2D.velocity = currentVelocity;
        }
        
        private void SetSkin()
        {
            var set = false;
            
            foreach (var skin in Resources.LoadAll<Sprite>("Sprites/Skins"))
            {
                if (GameState.userData != null && skin.name != GameState.userData.activeSkinId.ToString())
                {
                    continue;
                }
                
                _spriteRenderer.sprite = skin;
                set = true;
                break;
            }

            if (!set)
            {
                _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Skins/1");
            }
        }
    }
}