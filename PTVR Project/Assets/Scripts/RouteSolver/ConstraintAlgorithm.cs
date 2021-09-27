using System.Collections.Generic;
using UnityEngine;

namespace RouteSolver
{
	public class ConstraintAlgorithm : IRouteSolver
    {
        public List<RoutePlan> Solve(Vector3 start, List<Vector3> points, int vehicleCount)
        {
            /*foreach(Vector3 p in points)
            {
                Debug.Log(p.ToString());
            }*/

            //Debug.Log(start.ToString());

            List<RoutePlan> result = new List<RoutePlan>();
            List<Vector3> pointsToSet = new List<Vector3>(points);

			for (int i=0; i<vehicleCount; i++) 
			{
				RoutePlan route = new RoutePlan();
				route.Destinations.Push(start);
                //Debug.Log("Added: " + start.ToString());
				result.Add(route);
			}

            float shortestDistance = Mathf.Infinity;
            float currentDistance = 0;
            Vector3 pointToSet = Vector3.zero;

            foreach(RoutePlan r in result)
            {
                //get the most recent destination and compare distance with every other destination in the scene
                for(int i = 0; i<3; i++) //don't know the capacity of each agent, so assume it is three for each one
                {
                    foreach(Vector3 p in pointsToSet)
                    {
                        //calculate the distance beteween the top item of the stack and all the points
                        currentDistance = Vector3.Distance(r.Destinations.Peek(), p);
                        if(currentDistance < shortestDistance)
                        {
                            pointToSet = p;
                            shortestDistance = currentDistance;
                        }
                    }

                    if(pointToSet != Vector3.zero && pointsToSet != null && pointsToSet.Count != 0)
                    {
                        r.Destinations.Push(pointToSet);
                        //Debug.Log("Added: " + pointsToSet.ToString());
                        pointsToSet.Remove(pointToSet);
                        shortestDistance = Mathf.Infinity;
                        currentDistance = 0;
                    }
                }
            }

            /*foreach(RoutePlan r in result)
            {
                foreach(Vector3 p in r.Destinations)
                {
                    if(!points.Contains(p))
                    {
                        Debug.Log("ERROR: One or more of the points in result is not on the map.");
                        Debug.Log("Point: " + p.ToString() + " is not in the world.");
                    }
                }
            }*/

            // final pass
			for (int i=0; i<vehicleCount; i++) 
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