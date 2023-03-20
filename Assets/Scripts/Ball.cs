using UnityEngine;

namespace Assets.Scripts
{
    public class Ball : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float _maxXSpeed = 20f,
                      _startXSpeed = 8f,
                      _maxStartXSpeed = 2f,
                      _constantYSpeed = 10f,
                      _extents = 0.5f;

        [SerializeField] ParticleSystem _bounceParticleSystem,
                                        _trailParticleSystem,
                                        _startParticleSystem;

        [SerializeField] int _bounceParticleEmission = 20, 
                             _startParticleEmission = 100;

        private Vector2 _position, _velocity;

        public float Extents => _extents;
        public Vector2 Position => _position;
        public Vector2 Velocity => _velocity;

        private void Awake() => gameObject.SetActive(false);

        public void StartNewGame()
        {
            _position = Vector2.zero;
            UpdateVisualization();
            _velocity.x = Random.Range(-_maxStartXSpeed, _maxStartXSpeed);
            _velocity.y = -_constantYSpeed;
            gameObject.SetActive(true);
            _startParticleSystem.Emit(_startParticleEmission);
            SetTrailEmission(true);
            _trailParticleSystem.Play();
        }

        public void EndGame()
        {
            _position.x = 0f;
            gameObject.SetActive(false);
            SetTrailEmission(false);
        }

        void SetTrailEmission(bool enabled)
        {
            ParticleSystem.EmissionModule emission = _trailParticleSystem.emission;
            emission.enabled = enabled;
        }

        public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
        {
            _velocity.x = _maxXSpeed * speedFactor;
            _position.x = start + _velocity.x * deltaTime;
        }

        public void UpdateVisualization() => _trailParticleSystem.transform.localPosition =
            transform.localPosition = new Vector3(_position.x, 0f, _position.y);

        public void Move() => _position += _velocity * Time.deltaTime;

        public void BounceX (float boundary)
        {
            _position.x = 2f * boundary - _position.x;
            _velocity.x = -_velocity.x;
            EmitBounceParticles(boundary < 0f ? 90f : 270f);
        }

        public void BounceY(float boundary)
        {
            _position.y = 2f * boundary - _position.y;
            _velocity.y = -_velocity.y;
            EmitBounceParticles(boundary < 0f ? 0f : 180f);
        }

        void EmitBounceParticles(float rotation)
        {
            ParticleSystem.ShapeModule shape = _bounceParticleSystem.shape;
            shape.position = new Vector3(0f, 0f, 0f);
            shape.rotation = new Vector3(0f, rotation, 0f);
            _bounceParticleSystem.Emit(_bounceParticleEmission);
        }
    }
}