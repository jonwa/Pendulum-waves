using UnityEngine;

namespace PendulumWaves
{
	public interface IConstraint
	{
		void Apply();
	}

	public class FixedDistanceConstraint : IConstraint
	{
		public FixedDistanceConstraint(Particle pointA, Particle pointB)
		{
			_pointA = pointA;
			_pointB = pointB;
			_distance = pointB.Distance(pointA);
		}

		private float _distance;
		private Particle _pointA;
		private Particle _pointB;

		public void Apply()
		{
			Vector3 direction = _pointB.position - _pointA.position;
			float distance = direction.magnitude;
			float diff = (distance - _distance) / distance;
			Vector3 move = direction * 0.5f * diff;
			_pointA.Move(move);
			_pointB.Move(-move);
		}
	}
}