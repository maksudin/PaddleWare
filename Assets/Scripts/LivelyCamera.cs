using UnityEngine;

namespace Assets.Scripts
{
    public class LivelyCamera : MonoBehaviour
    {
		[SerializeField, Min(0f)]
		float _springStrength = 100f,
			  _dampingStrength = 10f,
			  _jostleStrength = 40f,
			  _pushStrength = 1f,
			  _maxDeltaTime = 1f / 60f;

		Vector3 _anchorPosition, _velocity;

		void Awake() => _anchorPosition = transform.localPosition;

		public void JostleY() => _velocity.y += _jostleStrength;

		public void PushXZ(Vector2 impulse)
		{
			_velocity.x += _pushStrength * impulse.x;
			_velocity.z += _pushStrength * impulse.y;
		}

        private void LateUpdate()
        {
			float dt = Time.deltaTime;
			while (dt > _maxDeltaTime)
			{
				TimeStep(_maxDeltaTime);
				dt -= _maxDeltaTime;
			}
			TimeStep(dt);
		}

        void TimeStep(float dt)
		{
			Vector3 displacement = _anchorPosition - transform.localPosition;
			Vector3 acceleration = _springStrength * displacement - _dampingStrength * _velocity;
			_velocity += acceleration * dt;
			transform.localPosition += _velocity * dt;
		}
	}
}