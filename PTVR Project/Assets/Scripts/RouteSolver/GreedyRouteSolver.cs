using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
	public class GreedyRouteSolver : IRouteSolver
	{
		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points,  List<TransportAgentIntroductionMessage> agentsWithCapacities)
		{
			List<RoutePlan> result = new List<RoutePlan>();

			for (int i=0; i<agentsWithCapacities.Count; i++) 
			{
				RoutePlan route = new RoutePlan();
				route.Destinations.Push(start);
				result.Add(route);
			}

			// while there are still points avaliable
			while (points.Count > 0)
			{
				// pick route with lowest total distance
				RoutePlan route = result.Aggregate((cur, next) => cur.TotalDistance <= next.TotalDistance ? cur : next);

				// get the last point the route traveled to
				Vector3 currentPoint = route.Destinations.Peek();

				// find the nearest avaliable point to that point
				Vector3 nearestPoint = points
					.Select(p => Tuple.Create(p, Vector3.Distance(currentPoint, p)))
					.Aggregate((cur, next) => cur.Item2 <= next.Item2 ? cur : next)
					.Item1;

				// remove that point from the avaliable points list
				points.Remove(nearestPoint);

				// add that point to the route
				route.Destinations.Push(nearestPoint);
			}

			// final pass
			for (int i=0; i<agentsWithCapacities.Count; i++) 
			{
				// order from first to last
				result[i].Destinations = Reverse(result[i].Destinations);

				// remove start
				result[i].Destinations.Pop();
			}

			return result;
		}

		private Stack<T> Reverse<T>(Stack<T> stack)
		{
			Stack<T> reversed = new Stack<T>();
			while (stack.Count > 0) reversed.Push(stack.Pop());
			return reversed;
		}
	}
}
