using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private Paddle _bottomPaddle, _topPaddle;
        [SerializeField, Min(0f)] Vector2 _arenaExtents = new Vector2(10f, 10f);
        [SerializeField, Min(2)] int _pointsToWin = 3;
        [SerializeField] TextMeshProUGUI _countdownText;
        [SerializeField, Min(1f)] float _newGameDelay = 3f;
        [SerializeField] LivelyCamera _livelyCamera;
        float _countdownUntilNewGame;

        private void Awake() => _countdownUntilNewGame = _newGameDelay;

        void StartNewGame()
        {
            _ball.StartNewGame();
            _bottomPaddle.StartNewGame();
            _topPaddle.StartNewGame();
        }

        void EndGame()
        {
            _countdownUntilNewGame = _newGameDelay;
            _countdownText.SetText("GAME OVER");
            _countdownText.gameObject.SetActive(true);
            _ball.EndGame();
        }

        private void Update()
        {
            _bottomPaddle.Move(_ball.Position.x, _arenaExtents.x);
            _topPaddle.Move(_ball.Position.x, _arenaExtents.x);

            if (_countdownUntilNewGame <= 0f)
                UpdateGame();
            else
                UpdateCountdown();

        }

        void UpdateGame()
        {
            _ball.Move();
            BounceYIfNeeded();
            BounceXIfNeeded(_ball.Position.x);
            _ball.UpdateVisualization();
        }

        void UpdateCountdown()
        {
            _countdownUntilNewGame -= Time.deltaTime;
            if (_countdownUntilNewGame <= 0f)
            {
                _countdownText.gameObject.SetActive(false);
                StartNewGame();
            }
            else
            {
                float displayValue = Mathf.Ceil(_countdownUntilNewGame);
                if (displayValue < _newGameDelay)
                    _countdownText.SetText("{0}", displayValue);
            }
        }


        void BounceYIfNeeded()
        {
            float yExtents = _arenaExtents.y - _ball.Extents;
            if (_ball.Position.y < -yExtents)
                BounceY(-yExtents, defender: _bottomPaddle, attacker: _topPaddle);
            else if (_ball.Position.y > yExtents)
                BounceY(yExtents, defender: _topPaddle, attacker: _bottomPaddle);
        }

        void BounceXIfNeeded(float x)
        {
            float xExtents = _arenaExtents.x - _ball.Extents;
            if (x < -xExtents)
            {
                _livelyCamera.PushXZ(_ball.Velocity);
                _ball.BounceX(-xExtents);
            }
            else if (x > xExtents)
            {
                _livelyCamera.PushXZ(_ball.Velocity);
                _ball.BounceX(xExtents);
            }
        }

        void BounceY(float boundary, Paddle defender, Paddle attacker)
        {
            float durationAfterBounce = (_ball.Position.y - boundary) / _ball.Velocity.y;
            float bounceX = _ball.Position.x - _ball.Velocity.x * durationAfterBounce;
            _livelyCamera.PushXZ(_ball.Velocity);
            BounceXIfNeeded(bounceX);
            _ball.BounceY(boundary);

            if (defender.HitBall(bounceX, _ball.Extents, out float hitFactor))
                _ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
            else
            {
                _livelyCamera.JostleY();
                if (attacker.ScorePoint(_pointsToWin))
                    EndGame();
            }
        }
    }
}