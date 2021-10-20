using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RouteSolver
{
	public class GreedyRouteSolver : IRouteSolver
	{
		public List<RoutePlan> Solve(Vector3 start, List<Vector3> points,  List<TransportAgentIntroductionMessage> agentsWithCapacities, List<DestinationMessage> destinations)
		{
			List<RoutePlan> result = new List<RoutePlan>();

			for (int i=0; i<agentsWithCapacities.Count; i++) 
			{
				RoutePlan route = new RoutePlan();
				//  route.Destinations.Push(start); // the loop below handles this now
				result.Add(route);
			}

            int totalCapacity = 0;
            for (int i = 0; i < agentsWithCapacities.Count; i++)
            {
                totalCapacity += agentsWithCapacities[i].Capacity;
            }
            //  int trips = Mathf.CeilToInt((float)points.Count / (float)totalCapacity);

            // while there are still points avaliable
            while (points.Count > 0)
            {
                // pick route with lowest total distance
                RoutePlan route = result.Aggregate((cur, next) => cur.TotalDistance <= next.TotalDistance ? cur : next);

                //  every time capacity is reached, agent returns to start (use capacity + 1 because depot doesn't count against capacity)
                if (route.Destinations.Count % (agentsWithCapacities[0].Capacity + 1) == 0)
                {
                    route.Destinations.Push(start);
                }

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

            Debug.Log("Distance: " + TotalDistance(result));

            return result;
		}


        public float TotalDistance(List<RoutePlan> routes)
        {
            float distance = 0;


            for (int i = 0; i < routes.Count; i++)
            {
                Vector3[] routeArray = new Vector3[routes[i].Destinations.Count];
                routes[i].Destinations.CopyTo(routeArray, 0);

                Vector3 start = new Vector3(0, 0, 0);   // need to add the proper start coordinates to this later

                for (int j = 0; j < routeArray.Length; j++)
                {
                    Vector3 next = routeArray[j];
                    distance += Vector3.Distance(start, next);
                    start = next;
                }
                distance += Vector3.Distance(start, Vector3.zero);
            }

            return distance;

        }


        private Stack<T> Reverse<T>(Stack<T> stack)
		{
			Stack<T> reversed = new Stack<T>();
			while (stack.Count > 0) reversed.Push(stack.Pop());
			return reversed;
		}
	}
}
