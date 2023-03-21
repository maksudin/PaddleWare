using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField, Min(0f)] 
        private float _speed = 10f,
                      _minExtents = 4f,
                      _maxExtents = 4f,
                      _maxTargetingBias = 0.75f;
        [SerializeField] private bool _isAI;
        [SerializeField] TextMeshProUGUI _scoreText;

        int _score;
        float _targetingBias, _extents;

        static readonly int _emissionColorId = Shader.PropertyToID("_EmissionColor"),
                            _faceColorId = Shader.PropertyToID("_FaceColor"),
                            _timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");

        [SerializeField] MeshRenderer _goalRenderer;
        [SerializeField, ColorUsage(true, true)] Color _goalColor = Color.white;

        Material _goalMaterial, _paddleMaterial, _scoreMaterial;

        private void Awake()
        {
            _goalMaterial = _goalRenderer.material;
            _goalMaterial.SetColor(_emissionColorId, _goalColor);
            _paddleMaterial = GetComponent<MeshRenderer>().material;
            _scoreMaterial = _scoreText.fontMaterial;
            SetScore(0);
        }

        void ChangeTargetingBias() => _targetingBias = Random.Range(-_maxTargetingBias, _maxTargetingBias);

        void SetScore (int newScore, float pointsToWin = 1000f)
        {
            _score = newScore;
            _scoreText.SetText("{0}", newScore);
            _scoreMaterial.SetColor(_faceColorId, _goalColor * (newScore / pointsToWin));
            SetExtents(Mathf.Lerp(_maxExtents, _minExtents, newScore / (pointsToWin - 1f)));
        }

        public void StartNewGame()
        {
            SetScore(0);
            ChangeTargetingBias();
        }

        public bool ScorePoint(int pointsToWin)
        {
            _goalMaterial.SetFloat(_timeOfLastHitId, Time.time);
            SetScore(_score + 1, pointsToWin);
            return _score >= pointsToWin;
        }

        public void Move(float target, float arenaExtents)
        {
            Vector3 pos = transform.localPosition;
            pos.x = _isAI ? AdjustByAI(pos.x, target) : AdjustByPlayer(pos.x);
            float limit = arenaExtents - _extents;
            pos.x = Mathf.Clamp(pos.x, -limit, limit);
            transform.localPosition = pos;
        }

        private float AdjustByAI(float x, float target)
        {
            target += _targetingBias * _extents;
            if (x < target)
                return Mathf.Min(x + _speed * Time.deltaTime, target);

            return Mathf.Max(x - _speed * Time.deltaTime, target);
        }

        private float AdjustByPlayer(float x)
        {
            bool goRight = Input.GetKey(KeyCode.RightArrow);
            bool goLeft = Input.GetKey(KeyCode.LeftArrow);
            if (goRight && !goLeft)
                return x + _speed * Time.deltaTime;
            else if (goLeft && !goRight)
                return x - _speed * Time.deltaTime;

            return x;
        }

        public bool HitBall(float ballX, float ballExtents, out float hitFactor)
        {
            ChangeTargetingBias();
            hitFactor = (ballX - transform.localPosition.x) / (_extents + ballExtents);
            bool success = -1f <= hitFactor && hitFactor <= 1f;
            if (success)
                _paddleMaterial.SetFloat(_timeOfLastHitId, Time.time);

            return success;

        }

        void SetExtents(float newExtents)
        {
            _extents = newExtents;
            Vector3 scale = transform.localScale;
            scale.x = 2f * newExtents;
            transform.localScale = scale;
        }
    }
}