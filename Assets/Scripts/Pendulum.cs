using UnityEngine;

namespace PendulumWaves
{
	public class Pendulum : MonoBehaviour
	{
		[SerializeField] private Transform _bob;
		[SerializeField] private LineRenderer _line;

		public Particle bob;
		public StaticParticle pivotPoint;
		public Vector3 equilibriumPosition;

		private IConstraint _constraint;

		public void Init(Particle bob, Vector3 equilibriumPosition)
		{
			gameObject.SetActive(true);

			this.bob = bob;
			this.pivotPoint = new StaticParticle(new Vector3(0, 0, bob.position.z));
			this.equilibriumPosition = equilibriumPosition;

			_constraint = new FixedDistanceConstraint(pivotPoint, bob);

			_line.positionCount = 2;
			_line.startWidth = 0.01f;
			_line.endWidth = 0.01f;
		}

		private void Start()
		{
			Render();
		}

		public void UpdateAndRender(float dt)
		{
			DoIntegration(dt);
			ApplyConstraints(1);
			Render();
		}

		private void ApplyConstraints(int iterations)
		{
			for (int j = 0; j < iterations; ++j)
			{
				_constraint.Apply();
			}
		}

		private void DoIntegration(float dt)
		{
			float theta = GetTheta(pivotPoint.position, bob.position, equilibriumPosition);

			float m = bob.mass;
			float g = -bob.gravity;
			float v = bob.velocity.magnitude;
			float r = pivotPoint.Distance(bob);

			Vector3 gravityDirection = -new Vector3(0, g, 0);
			Vector3 tensionDirection = (bob.position - pivotPoint.position);

			gravityDirection.Normalize();
			tensionDirection.Normalize();

			float Fg = m * g;
			float Fc = m * (v * v) / r;
			float Ft = m * g * Mathf.Cos(theta);

			bob.AddForce(gravityDirection * Fg);
			bob.AddForce(tensionDirection * Fc);
			bob.AddForce(tensionDirection * Ft);
			bob.DoIntegration(dt);
			bob.ClearForce();
		}

		private void Render()
		{
			float radius = bob.mass * 0.5f;
			Vector3 position = bob.position;
			_bob.transform.position = position;
			_bob.transform.localScale = new Vector3(radius, radius, radius);
			_line.SetPosition(0, pivotPoint.position);
			_line.SetPosition(1, position);
		}

		private float GetTheta(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 top = a - b;
			Vector3 bot = c - b;
			return Vector3.Angle(top, bot) * (Mathf.PI / 180f);
		}
	}

}

