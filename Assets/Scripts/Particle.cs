using System;
using UnityEngine;

namespace PendulumWaves
{
	public abstract class Particle
	{
		public Particle(Vector3 position, float mass, float gravity)
		{
			this.position = position;
			this.mass = mass;
			this.gravity = gravity;

			force = Vector3.zero;
			acceleration = Vector3.zero;
		}

		public Vector3 position;
		public Vector3 force;
		public Vector3 acceleration;
		public float mass;
		public float gravity;

		public float inverseMass
		{
			get { return 1.0f / mass; }
		}
		
		public virtual Vector3 velocity
		{
			get { return Vector3.zero; }
		}

		public virtual void DoIntegration(float dt)
		{
		}

		public virtual void Move(Vector3 position)
		{
			this.position += position;
		}

		public float Distance(Particle other)
		{
			return Vector3.Distance(position, other.position);
		}

		public void AddForce(Vector3 force)
		{
			this.force += force;
			acceleration = this.force * inverseMass;
		}

		public void ClearForce()
		{
			force = Vector3.zero;
		}
	}

	public class StaticParticle : Particle
	{
		public StaticParticle(Vector3 position)
			: base (position, 0, 0)
		{
		}

		public override void Move(Vector3 position)
		{
		}
	}

	public class VerletParticle : Particle
	{
		public VerletParticle(Vector3 position, float mass, float gravity, float step)
			: base(position, mass, gravity)
		{
			_step = step;
			_previousPosition = position - Vector3.zero * _step;
		}

		private float _step;
		private Vector3 _previousPosition;

		public override Vector3 velocity
		{
			get { return (position - _previousPosition) * _step; }
		}

		public override void Move(Vector3 position)
		{
			this.position += position;
		}

		public override void DoIntegration(float dt)
		{
			_step = dt;
			Vector3 newPosition = (position * 2) - _previousPosition + acceleration * (_step * _step);
			_previousPosition = position;
			position = newPosition;
		}
	}
}
