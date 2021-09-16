using System;
using System.Collections.Generic;

namespace MessageSystem
{
	public abstract class Message {}

	public static class MessageBoard
	{
		private static readonly Dictionary<Type,object> _actionTable = new Dictionary<Type,object>();

		public static void SendMessage<T>(T message) where T : Message
		{
			List<Action<T>> actions = GetActionsForType<T>();
			Log.Info($"{Log.Green(Log.StackTraceFileString(2))} sending message " + Log.Yellow($"<{typeof(T).Name}>") + $" to {Log.Cyan(actions.Count)} listeners.\n{Log.ObjectToString(message)}.");
			actions.ForEach(action => action(message));
		}

		public static void ListenForMessage<T>(Action<T> action) where T : Message
		{
			Log.Info($"{Log.Green(Log.StackTraceFileString(2))} is listening for " + Log.Yellow($"<{typeof(T).Name}>") + $" with {Log.Cyan(action.Method.Name)}.");
			GetActionsForType<T>().Add(action);
		}

		public static void StopListeningForMessage<T>(Action<T> action) where T : Message
		{
			Log.Info($"{Log.Green(Log.StackTraceFileString(2))} stopped listening for " + Log.Yellow($"<{typeof(T).Name}>") + $" with {Log.Cyan(action.Method.Name)}.");
			GetActionsForType<T>().RemoveAll(a => a == action);
		}

		private static List<Action<T>> GetActionsForType<T>() where T : Message
		{
			Type messageType = typeof(T);

			if (!_actionTable.ContainsKey(messageType))
			{
				_actionTable.Add(messageType, (object) new List<Action<T>>());
			}

			return (List<Action<T>>) _actionTable[messageType];
		}
	}
}
