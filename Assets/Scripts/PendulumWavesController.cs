using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace PendulumWaves
{
	public class PendulumWavesController : MonoBehaviour
	{
		[SerializeField] private Slider _initialAngleSlider;
		[SerializeField] private Text _initialAngleValueText;
		[SerializeField] private Text _elapsedTimeText;
		[SerializeField] private Button _playButton;
		[SerializeField] private Button _resetButton;
		[SerializeField] private Pendulum _pendulumPrefab;

		private const int NUM_PENDULUMS = 12;
		private const int MIN_OSCILLATIONS = 23;
		private const float TIME_PERIOD = 60f;
		private const float GRAVITY = 9.807f;
		private const float MASS = 0.5f;

		private bool _doUpdate;
		private float _initialAngle;
		private float _currentTime = 0f;
		private float _accumulator = 0f;
		private float _deltaTime = 1f/600f;

		private Stopwatch _stopWatch = new Stopwatch();
		private List<Pendulum> _pendulums = new List<Pendulum>();

		private float _elapsedTime
		{
			set { _elapsedTimeText.text = string.Format("Elapsed Time: {0:0.##}s", value); }
		}

		private void Awake()
		{
			_elapsedTime = 0f;
			_playButton.onClick.AddListener(Play);
			_resetButton.onClick.AddListener(Reset);
			_initialAngleSlider.onValueChanged.AddListener(OnInitialAngleChanged);
			_initialAngleSlider.minValue = -90f;
			_initialAngleSlider.maxValue = 90f;
			_initialAngleSlider.value = -25;
			Application.runInBackground = true;
			Init();
		}

		private void OnDestroy()
		{
			_playButton.onClick.RemoveListener(Play);
			_resetButton.onClick.RemoveListener(Reset);
			_initialAngleSlider.onValueChanged.RemoveListener(OnInitialAngleChanged);
		}

		private void Update()
		{
			float newTime = Time.time;
			float frameTime = newTime - _currentTime;

			_currentTime = newTime;
			_accumulator += frameTime;

			while (_accumulator >= _deltaTime)
			{
				if (_doUpdate)
				{
					InternalUpdate(_deltaTime);
				}

				_accumulator -= _deltaTime;
			}
		}

		private void Init()
		{
			foreach (var pendulum in _pendulums)
			{
				Destroy(pendulum.gameObject);
			}
			_pendulums.Clear();

			for (int i = 0; i < NUM_PENDULUMS; ++i)
			{
				int oscillations = MIN_OSCILLATIONS + i;
				float frequency = oscillations / TIME_PERIOD;
				float length = -(GRAVITY / (4 * (Mathf.PI * Mathf.PI) * (frequency * frequency)));

				float x = Mathf.Sin(_initialAngle) * length;
				float y = Mathf.Cos(_initialAngle) * length;
				float z = i * MASS;

				Vector3 position = new Vector3(x, y, z);
				Vector3 equilibriumPosition = new Vector3(0f, length, 0f);
				VerletParticle bob = new VerletParticle(position, MASS, GRAVITY, _deltaTime);
				Pendulum pendulum = Instantiate(_pendulumPrefab, _pendulumPrefab.transform.parent);
				pendulum.Init(bob, equilibriumPosition);
				_pendulums.Add(pendulum);
			}
		}

		private void InternalUpdate(float deltaTime)
		{
			double elapsedSeconds = _stopWatch.Elapsed.TotalSeconds;

			if (elapsedSeconds == 0)
			{
				_stopWatch.Start();
			}
			else if (elapsedSeconds >= TIME_PERIOD)
			{
				Reset();
			}
			else
			{
				foreach (var pendulum in _pendulums)
				{
					pendulum.UpdateAndRender(_deltaTime);
				}
				_elapsedTime = (float)elapsedSeconds;
			}
		}

		private void Play()
		{
			_playButton.interactable = false;
			_resetButton.interactable = true;
			_doUpdate = true;
			_stopWatch.Start();
		}

		private void Reset()
		{
			_playButton.interactable = true;
			_resetButton.interactable = false;
			_doUpdate = false;
			_stopWatch.Stop();
			_stopWatch.Reset();
			_elapsedTime = 0f;
			_currentTime = 0.0f;
			_accumulator = 0.0f;
			Init();
		}

		private void OnInitialAngleChanged(float value)
		{
			_initialAngle = Mathf.PI / 180f * -value;
			_initialAngleValueText.text = string.Format("{0:0.##}", value);
			Reset();
		}
	}
}


