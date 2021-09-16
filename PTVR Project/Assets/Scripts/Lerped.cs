using System;
using UnityEngine;

public class Lerped<T>
{
	private Func<T, T, float, T> _easingMethod;
	private float _lastTimeSeconds;
	private bool _unscaledTime;
	private T _current;
	private T _target;

	private float CurrentTime => _unscaledTime ? Time.unscaledTime : Time.time;

	/// <param name="initial">
	/// The value the lerped value should start at.
	/// </param>
	/// <param name="durationSeconds">
	/// How long the lerp should last for in seconds.
	/// </param>
	/// <param name="easingMethod">
	/// The easing method to apply during interpolation, this should be one
	/// of the methods in Scripts.Utilities.Easing
	/// </param>
	/// <param name="unscaledTime">
	/// Whether or not the lerp should ignore time scale.
	/// </param>
	public Lerped(T initial, float durationSeconds, Func<T, T, float, T> easingMethod, bool unscaledTime = false)
	{
		_current = initial;
		_target = initial;
		DurationSeconds = durationSeconds;
		_easingMethod = easingMethod;
		_unscaledTime = unscaledTime;
	}

	/// <summary>
	/// get returns the value lerped between the previous and current
	/// values over _durationSeconds.
	///
	/// set will change the target of the lerp and reset the internal timer
	/// so that the lerp starts again.
	/// </summary>
	public T Value
	{
		get => _easingMethod(_current, _target, Interpolation);
		set
		{
			_current = Value;
			_target = value;
			_lastTimeSeconds = CurrentTime;
		}
	}

	/// <summary>
	/// The duration of the interpolation in seconds.
	/// </summary>
	public float DurationSeconds { get; set; }

	/// <summary>
	/// The current interpolation value used by Value between 0.0f and 1.0f.
	/// </summary>
	public float Interpolation => Mathf.Min((CurrentTime - _lastTimeSeconds) / DurationSeconds, 1.0f);

	/// <summary>
	/// Whether the current value has finished interpolating.
	/// </summary>
	public bool InterpolationComplete => Interpolation == 1.0f;
}

public static class Easing
{
	public static Vector2 Linear(Vector2 start, Vector2 end, float interpolation) => Vector2.LerpUnclamped(start, end, Linear(interpolation));
	public static Vector2 EaseIn(Vector2 start, Vector2 end, float interpolation) => Vector2.LerpUnclamped(start, end, EaseIn(interpolation));
	public static Vector2 EaseOut(Vector2 start, Vector2 end, float interpolation) => Vector2.LerpUnclamped(start, end, EaseOut(interpolation));
	public static Vector2 EaseInOut(Vector2 start, Vector2 end, float interpolation) => Vector2.LerpUnclamped(start, end, EaseInOut(interpolation));
	
	public static Vector3 Linear(Vector3 start, Vector3 end, float interpolation) => Vector3.LerpUnclamped(start, end, Linear(interpolation));
	public static Vector3 EaseIn(Vector3 start, Vector3 end, float interpolation) => Vector3.LerpUnclamped(start, end, EaseIn(interpolation));
	public static Vector3 EaseOut(Vector3 start, Vector3 end, float interpolation) => Vector3.LerpUnclamped(start, end, EaseOut(interpolation));
	public static Vector3 EaseInOut(Vector3 start, Vector3 end, float interpolation) => Vector3.LerpUnclamped(start, end, EaseInOut(interpolation));
	
	public static Color Linear(Color start, Color end, float interpolation) => Color.LerpUnclamped(start, end, Linear(interpolation));
	public static Color EaseIn(Color start, Color end, float interpolation) => Color.LerpUnclamped(start, end, EaseIn(interpolation));
	public static Color EaseOut(Color start, Color end, float interpolation) => Color.LerpUnclamped(start, end, EaseOut(interpolation));
	public static Color EaseInOut(Color start, Color end, float interpolation) => Color.LerpUnclamped(start, end, EaseInOut(interpolation));

	public static float Linear(float n) => n;
	
	// https://www.desmos.com/calculator/pbquteuou4
	public static float EaseIn(float n) => Mathf.Pow(n, 2);

	// https://www.desmos.com/calculator/ugnz3nncuq
	public static float EaseOut(float n) => Mathf.Sqrt(n);
	
	// https://www.desmos.com/calculator/za8sugou91
	private static float EaseInOut(float n) => n <= 0.5
		? +2 * Mathf.Pow(n, 2)
		: -2 * Mathf.Pow(n - 1, 2) + 1;
}
