using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	public class AlternativeGreedyRouteSolver : IRouteSolver
    {
        public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, List<TransportAgentIntroductionMessage> agentsWithCapacities)
        {
            List<RoutePlan> result = new List<RoutePlan>();
            //copy points to another list so we can keep track of the points we've added to routes
            List<Vector3> pointsToSet = new List<Vector3>(points);

            //variables for finding the shortest distance
            float shortestDistance = Mathf.Infinity;
            float currentDistance = 0;
            Vector3 pointToSet = Vector3.zero;

            int totalCapacity = 0;
            for (int i = 0; i < agentsWithCapacities.Count; i++)
            {
                totalCapacity += agentsWithCapacities[i].Capacity;
            }
            int trips = Mathf.CeilToInt((float)points.Count / (float)totalCapacity);

            //loop for each agent that has sent a message, using it's capacity to generate a route.
            foreach (TransportAgentIntroductionMessage t in agentsWithCapacities)
            {
                //create the route and add the first point to the results
                RoutePlan route = new RoutePlan();
                route.Destinations.Push(start);
                for (int r = 0; r < trips; r++)
                {
                    if (r != 0)
                    {
                        route.Destinations.Push(start);  // add depot because agent has to go back between each trip 
                    }
                    for (int i = 0; i < t.Capacity; i++)
                    {
                        foreach (Vector3 p in pointsToSet)
                        {
                            //calculate the distance beteween the top item of the stack and all the points
                            currentDistance = Vector3.Distance(route.Destinations.Peek(), p);
                            if (currentDistance < shortestDistance)
                            {
                                pointToSet = p;
                                shortestDistance = currentDistance;
                            }
                        }

                        //Push the new point to our route if it is the shortest, remove it from the list of points so we can't accidentally add it again
                        if (pointToSet != Vector3.zero && pointsToSet != null && pointsToSet.Count != 0)
                        {
                            route.Destinations.Push(pointToSet);
                            //Debug.Log("Added: " + pointsToSet.ToString());
                            pointsToSet.Remove(pointToSet);
                            shortestDistance = Mathf.Infinity;
                            currentDistance = 0;
                        }
                    }
                }
                result.Add(route);
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