using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class Log
{
	public static void Info(string message) => Write(message, LogLevel.Info);
	public static void Warning(string message) => Write(message, LogLevel.Warning);
	public static void Error(string message) => Write(message, LogLevel.Error);

	private static void Write(string message, LogLevel level)
	{
		Action<object> Log = level switch
		{
			LogLevel.Error   => Debug.LogError,
			LogLevel.Warning => Debug.LogWarning,
			_                => Debug.Log,
		};

		string timeString = Grey($"{DateTime.Now:HH:mm:ss} ({Time.unscaledTime * 1000}ms)");
		string fileString = Grey(StackTraceFileString(3));
		Log($"{timeString} {fileString}\n{message}");
	}

	private enum LogLevel
	{
		Info,
		Warning,
		Error,
	}

	public static string ObjectToString<T>(T obj)
	{
		Type type = obj.GetType();
		FieldInfo[] fields = type.GetFields();
		PropertyInfo[] properties = type.GetProperties();

		Dictionary<string, object> values = new Dictionary<string, object>();
		Array.ForEach(fields, (field) =>
		{
			values.Add(field.Name, field.GetValue(obj) ?? Log.Red("null"));
		});
		Array.ForEach(properties, (property) =>
		{
			if (property.CanRead) values.Add(property.Name, property.GetValue(obj, null) ?? Log.Red("null"));
		});

		string str = "{\n";
		foreach (var val in values)
		{
			str += $"\t{val.Key} = {Log.Cyan(val.Value)},\n";
		}
		str += "}";

		return str;
	}

#if UNITY_EDITOR
		public static string Blue(object obj) => $"<color=#2196F3>{obj}</color>";
		public static string Cyan(object obj) => $"<color=#00BCD4>{obj}</color>";
		public static string Green(object obj) => $"<color=#4CAF50>{obj}</color>";
		public static string Grey(object obj) => $"<color=#666666>{obj}</color>";
		public static string Lime(object obj) => $"<color=#8BC34A>{obj}</color>";
		public static string Orange(object obj) => $"<color=#FF9800>{obj}</color>";
		public static string Pink(object obj) => $"<color=#E91E63>{obj}</color>";
		public static string Purple(object obj) => $"<color=#9C27B0>{obj}</color>";
		public static string Red(object obj) => $"<color=#F44336>{obj}</color>";
		public static string White(object obj) => $"<color=#FFFFFF>{obj}</color>";
		public static string Yellow(object obj) => $"<color=#FFEB3B>{obj}</color>";

		public static string StackTraceFileString(int stackFrame)
		{
			char seperator = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
			var stackTrace = new System.Diagnostics.StackTrace(true).GetFrame(stackFrame);
			string fileString = $"{stackTrace.GetFileName().Split(seperator).Last()}:{stackTrace.GetFileLineNumber()}";
			return fileString;
		}
#else
		public static string Blue(object obj) => obj.ToString();
		public static string Cyan(object obj) => obj.ToString();
		public static string Green(object obj) => obj.ToString();
		public static string Grey(object obj) => obj.ToString();
		public static string Lime(object obj) => obj.ToString();
		public static string Orange(object obj) => obj.ToString();
		public static string Pink(object obj) => obj.ToString();
		public static string Purple(object obj) => obj.ToString();
		public static string Red(object obj) => obj.ToString();
		public static string White(object obj) => obj.ToString();
		public static string Yellow(object obj) => obj.ToString();

		public static string StackTraceFileString(int stackFrame) => "---DEBUG INFORMATION DISABLED---";
#endif
}
