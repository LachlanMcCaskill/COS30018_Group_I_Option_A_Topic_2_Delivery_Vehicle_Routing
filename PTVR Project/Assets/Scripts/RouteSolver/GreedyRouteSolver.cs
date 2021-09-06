using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
	public class GreedyRouteSolver : IRouteSolver
	{
		public List<Route> Solve(Vector3 start, List<Vector3> points, int vehicleCount)
		{
			List<Route> result = new List<Route>();

			for (int i=0; i<vehicleCount; i++) 
			{
				Route route = new Route();
				route.Destinations.Push(start);
				result.Add(route);
			}

			// while there are still points avaliable
			while (points.Count > 0)
			{
				// pick route with lowest total distance
				Route route = result.Aggregate((cur, next) => cur.TotalDistance <= next.TotalDistance ? cur : next);

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
			for (int i=0; i<vehicleCount; i++) 
			{
				// order from first to last
				Stack<Vector3> old = result[i].Destinations;
				result[i].Destinations = new Stack<Vector3>();
				while (old.Count > 0) result[i].Destinations.Push(old.Pop());

				// remove start
				result[i].Destinations.Pop();
			}

			return result;
		}
	}
}
